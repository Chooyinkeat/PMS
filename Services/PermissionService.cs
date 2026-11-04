using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using System.Security.Claims;

namespace PMS.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;

        public PermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int? GetUserId(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return null;

            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out var userId))
                return userId;

            return null;
        }

        public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string module, PermissionAction action)
        {
            var userId = GetUserId(user);
            if (!userId.HasValue)
                return false;

            return await HasPermissionAsync(userId.Value, module, action);
        }

        public async Task<bool> HasPermissionAsync(int userId, string module, PermissionAction action)
        {
            var perm = await GetPermissionAsync(userId, module);
            return action switch
            {
                PermissionAction.View => perm.CanView,
                PermissionAction.Add => perm.CanAdd,
                PermissionAction.Edit => perm.CanEdit,
                PermissionAction.Delete => perm.CanDelete,
                PermissionAction.Print => perm.CanPrint,
                _ => false
            };
        }

        public async Task<ModulePermissionDto> GetPermissionAsync(ClaimsPrincipal user, string module)
        {
            var userId = GetUserId(user);
            if (!userId.HasValue)
                return ModulePermissionDto.NoAccess(module);

            return await GetPermissionAsync(userId.Value, module);
        }

        public async Task<ModulePermissionDto> GetPermissionAsync(int userId, string module)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.Status != true || !user.UserGroupId.HasValue)
            {
                return ModulePermissionDto.NoAccess(module);
            }

            var group = await _context.Usergroups
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.UserGroupId == user.UserGroupId.Value);

            if (group == null || group.Status != true)
            {
                return ModulePermissionDto.NoAccess(module);
            }

            // If group is explicitly named "Administrator" or "Admin" or "Superadmin"
            var isNamedAdmin = group.UserGroupName.Contains("Admin", StringComparison.OrdinalIgnoreCase);

            var access = await _context.Usergroupaccesses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.UgagroupId == user.UserGroupId.Value &&
                                          a.Ugamodule.ToLower() == module.ToLower());

            if (access == null)
            {
                // If named Admin, grant full access by default even if rows haven't been seeded yet
                if (isNamedAdmin)
                {
                    return ModulePermissionDto.FullAccess(module);
                }
                return ModulePermissionDto.NoAccess(module);
            }

            return new ModulePermissionDto
            {
                Module = module,
                CanView = access.Ugaview,
                CanAdd = access.Ugaadd,
                CanEdit = access.Ugaedit,
                CanDelete = access.Ugadelete,
                CanPrint = access.Ugaprint
            };
        }

        public async Task<Dictionary<string, ModulePermissionDto>> GetAllPermissionsAsync(ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            if (!userId.HasValue)
            {
                return AppModules.AllModules.ToDictionary(m => m.Code, m => ModulePermissionDto.NoAccess(m.Code));
            }

            return await GetAllPermissionsAsync(userId.Value);
        }

        public async Task<Dictionary<string, ModulePermissionDto>> GetAllPermissionsAsync(int userId)
        {
            var result = new Dictionary<string, ModulePermissionDto>(StringComparer.OrdinalIgnoreCase);

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.Status != true || !user.UserGroupId.HasValue)
            {
                foreach (var mod in AppModules.AllModules)
                {
                    result[mod.Code] = ModulePermissionDto.NoAccess(mod.Code);
                }
                return result;
            }

            var group = await _context.Usergroups
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.UserGroupId == user.UserGroupId.Value);

            var isNamedAdmin = group != null && group.UserGroupName.Contains("Admin", StringComparison.OrdinalIgnoreCase);

            var accessList = await _context.Usergroupaccesses
                .AsNoTracking()
                .Where(a => a.UgagroupId == user.UserGroupId.Value)
                .ToListAsync();

            foreach (var mod in AppModules.AllModules)
            {
                var access = accessList.FirstOrDefault(a => a.Ugamodule.Equals(mod.Code, StringComparison.OrdinalIgnoreCase));
                if (access != null)
                {
                    result[mod.Code] = new ModulePermissionDto
                    {
                        Module = mod.Code,
                        CanView = access.Ugaview,
                        CanAdd = access.Ugaadd,
                        CanEdit = access.Ugaedit,
                        CanDelete = access.Ugadelete,
                        CanPrint = access.Ugaprint
                    };
                }
                else if (isNamedAdmin)
                {
                    result[mod.Code] = ModulePermissionDto.FullAccess(mod.Code);
                }
                else
                {
                    result[mod.Code] = ModulePermissionDto.NoAccess(mod.Code);
                }
            }

            return result;
        }

        public async Task EnsureDefaultPermissionsAsync()
        {
            try
            {
                var groups = await _context.Usergroups.ToListAsync();
                if (!groups.Any())
                {
                    // Create default Administrator group if none exists
                    var adminGroup = new Usergroup
                    {
                        UserGroupName = "Administrator",
                        UserGroupDescription = "System Administrators with full access to all modules.",
                        Status = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "System"
                    };
                    _context.Usergroups.Add(adminGroup);
                    await _context.SaveChangesAsync();
                    groups.Add(adminGroup);
                }

                foreach (var group in groups)
                {
                    var existingAccess = await _context.Usergroupaccesses
                        .Where(a => a.UgagroupId == group.UserGroupId)
                        .ToListAsync();

                    var isNamedAdmin = group.UserGroupName.Contains("Admin", StringComparison.OrdinalIgnoreCase);

                    foreach (var mod in AppModules.AllModules)
                    {
                        var entry = existingAccess.FirstOrDefault(a => a.Ugamodule.Equals(mod.Code, StringComparison.OrdinalIgnoreCase));
                        if (entry == null)
                        {
                            _context.Usergroupaccesses.Add(new Usergroupaccess
                            {
                                UgagroupId = group.UserGroupId,
                                Ugamodule = mod.Code,
                                Ugaview = true,
                                Ugaadd = isNamedAdmin,
                                Ugaedit = isNamedAdmin,
                                Ugadelete = isNamedAdmin,
                                Ugaprint = true,
                                CreatedDate = DateTime.Now,
                                CreatedBy = "System"
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch
            {
                // Soft fail if DB is temporarily busy
            }
        }
    }
}
