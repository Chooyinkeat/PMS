using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using PMS.Services;
using System.ComponentModel.DataAnnotations;

namespace PMS.Pages.Setup
{
    public class UserGroupMaintenanceModel : BasePmsPageModel
    {
        private readonly IPdfService _pdfService;

        public UserGroupMaintenanceModel(ApplicationDbContext context, IPermissionService permissionService, IPdfService pdfService)
            : base(context, permissionService)
        {
            _pdfService = pdfService;
        }

        public List<GroupViewModel> Groups { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty]
        public GroupInputModel Input { get; set; } = new();

        [BindProperty]
        public PermissionMatrixInputModel MatrixInput { get; set; } = new();

        public class GroupViewModel
        {
            public int UserGroupId { get; set; }
            public string UserGroupName { get; set; } = string.Empty;
            public string? UserGroupDescription { get; set; }
            public bool Status { get; set; }
            public int UserCount { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public class GroupInputModel
        {
            public int? UserGroupId { get; set; }

            [Required(ErrorMessage = "Group name is required.")]
            [StringLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
            public string UserGroupName { get; set; } = string.Empty;

            [StringLength(255)]
            public string? UserGroupDescription { get; set; }

            public bool Status { get; set; } = true;
        }

        public class PermissionMatrixInputModel
        {
            public int UserGroupId { get; set; }
            public string UserGroupName { get; set; } = string.Empty;
            public List<ModulePermissionRow> Rows { get; set; } = new();
        }

        public class ModulePermissionRow
        {
            public string ModuleCode { get; set; } = string.Empty;
            public string ModuleName { get; set; } = string.Empty;
            public bool CanView { get; set; }
            public bool CanAdd { get; set; }
            public bool CanEdit { get; set; }
            public bool CanDelete { get; set; }
            public bool CanPrint { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await CheckPermissionAsync(AppModules.UserGroup, PermissionAction.View))
                return RedirectToAccessDenied(AppModules.UserGroup, PermissionAction.View);

            // Ensure access records exist for consistency
            await PermissionService.EnsureDefaultPermissionsAsync();

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateGroupAsync()
        {
            if (!await CheckPermissionAsync(AppModules.UserGroup, PermissionAction.Add))
                return RedirectToAccessDenied(AppModules.UserGroup, PermissionAction.Add);

            if (await Context.Usergroups.AnyAsync(g => g.UserGroupName.ToLower() == Input.UserGroupName.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.UserGroupName", "A user group with this name already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            var group = new Usergroup
            {
                UserGroupName = Input.UserGroupName.Trim(),
                UserGroupDescription = Input.UserGroupDescription?.Trim(),
                Status = Input.Status,
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUserName
            };

            Context.Usergroups.Add(group);
            await Context.SaveChangesAsync();

            // Initialize default access records for this new group
            foreach (var mod in AppModules.AllModules)
            {
                Context.Usergroupaccesses.Add(new Usergroupaccess
                {
                    UgagroupId = group.UserGroupId,
                    Ugamodule = mod.Code,
                    Ugaview = true,
                    Ugaadd = false,
                    Ugaedit = false,
                    Ugadelete = false,
                    Ugaprint = false,
                    CreatedDate = DateTime.Now,
                    CreatedBy = CurrentUserName
                });
            }

            await Context.SaveChangesAsync();
            SetSuccessMessage($"User Group '{group.UserGroupName}' created successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditGroupAsync()
        {
            if (!await CheckPermissionAsync(AppModules.UserGroup, PermissionAction.Edit))
                return RedirectToAccessDenied(AppModules.UserGroup, PermissionAction.Edit);

            if (!Input.UserGroupId.HasValue)
            {
                SetErrorMessage("Invalid group ID.");
                return RedirectToPage();
            }

            var group = await Context.Usergroups.FindAsync(Input.UserGroupId.Value);
            if (group == null)
            {
                SetErrorMessage("Group not found.");
                return RedirectToPage();
            }

            if (await Context.Usergroups.AnyAsync(g => g.UserGroupId != group.UserGroupId && g.UserGroupName.ToLower() == Input.UserGroupName.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.UserGroupName", "A user group with this name already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            group.UserGroupName = Input.UserGroupName.Trim();
            group.UserGroupDescription = Input.UserGroupDescription?.Trim();
            group.Status = Input.Status;
            group.EditedDate = DateTime.Now;
            group.EditedBy = CurrentUserName;

            await Context.SaveChangesAsync();
            SetSuccessMessage($"User Group '{group.UserGroupName}' updated successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetPermissionsAsync(int id)
        {
            if (!await CheckPermissionAsync(AppModules.UserGroup, PermissionAction.View))
                return new JsonResult(new { success = false, message = "Access denied." });

            var group = await Context.Usergroups.FindAsync(id);
            if (group == null)
                return new JsonResult(new { success = false, message = "Group not found." });

            var existingAccess = await Context.Usergroupaccesses
                .Where(a => a.UgagroupId == id)
                .ToListAsync();

            var rows = AppModules.AllModules.Select(m =>
            {
                var acc = existingAccess.FirstOrDefault(a => a.Ugamodule.Equals(m.Code, StringComparison.OrdinalIgnoreCase));
                return new ModulePermissionRow
                {
                    ModuleCode = m.Code,
                    ModuleName = m.Name,
                    CanView = acc?.Ugaview ?? false,
                    CanAdd = acc?.Ugaadd ?? false,
                    CanEdit = acc?.Ugaedit ?? false,
                    CanDelete = acc?.Ugadelete ?? false,
                    CanPrint = acc?.Ugaprint ?? false
                };
            }).ToList();

            return new JsonResult(new
            {
                success = true,
                groupId = group.UserGroupId,
                groupName = group.UserGroupName,
                rows = rows
            });
        }

        public async Task<IActionResult> OnPostSavePermissionsAsync([FromBody] PermissionMatrixInputModel model)
        {
            if (!await CheckPermissionAsync(AppModules.UserGroup, PermissionAction.Edit))
                return new JsonResult(new { success = false, message = "Access denied." });

            if (model.UserGroupId <= 0)
                return new JsonResult(new { success = false, message = "Invalid group ID." });

            var existingAccess = await Context.Usergroupaccesses
                .Where(a => a.UgagroupId == model.UserGroupId)
                .ToListAsync();

            foreach (var row in model.Rows)
            {
                var acc = existingAccess.FirstOrDefault(a => a.Ugamodule.Equals(row.ModuleCode, StringComparison.OrdinalIgnoreCase));
                if (acc == null)
                {
                    acc = new Usergroupaccess
                    {
                        UgagroupId = model.UserGroupId,
                        Ugamodule = row.ModuleCode,
                        CreatedDate = DateTime.Now,
                        CreatedBy = CurrentUserName
                    };
                    Context.Usergroupaccesses.Add(acc);
                }

                acc.Ugaview = row.CanView;
                acc.Ugaadd = row.CanAdd;
                acc.Ugaedit = row.CanEdit;
                acc.Ugadelete = row.CanDelete;
                acc.Ugaprint = row.CanPrint;
                acc.EditedDate = DateTime.Now;
                acc.EditedBy = CurrentUserName;
            }

            await Context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Permissions updated successfully." });
        }

        public async Task<IActionResult> OnPostDeleteGroupAsync(int id)
        {
            if (!await CheckPermissionAsync(AppModules.UserGroup, PermissionAction.Delete))
                return RedirectToAccessDenied(AppModules.UserGroup, PermissionAction.Delete);

            var group = await Context.Usergroups.FindAsync(id);
            if (group == null)
            {
                SetErrorMessage("Group not found.");
                return RedirectToPage();
            }

            // Check if any users belong to this group
            var userCount = await Context.Users.CountAsync(u => u.UserGroupId == id);
            if (userCount > 0)
            {
                SetErrorMessage($"Cannot delete group '{group.UserGroupName}' because {userCount} user account(s) are assigned to it.");
                return RedirectToPage();
            }

            // Delete associated usergroupaccess records first
            var accesses = await Context.Usergroupaccesses.Where(a => a.UgagroupId == id).ToListAsync();
            Context.Usergroupaccesses.RemoveRange(accesses);

            Context.Usergroups.Remove(group);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"User Group '{group.UserGroupName}' deleted successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            if (!await CheckPermissionAsync(AppModules.UserGroup, PermissionAction.Print))
                return RedirectToAccessDenied(AppModules.UserGroup, PermissionAction.Print);

            var query = Context.Usergroups.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(g => g.UserGroupName.ToLower().Contains(term) || (g.UserGroupDescription != null && g.UserGroupDescription.ToLower().Contains(term)));
            }

            var groupList = await query
                .OrderBy(g => g.UserGroupName)
                .Select(g => new
                {
                    g.UserGroupName,
                    Description = g.UserGroupDescription ?? "None",
                    Status = g.Status == true ? "Active" : "Inactive",
                    UserCount = Context.Users.Count(u => u.UserGroupId == g.UserGroupId),
                    CreatedDate = g.CreatedDate.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            var columns = new List<PdfColumn<dynamic>>
            {
                new() { Header = "Group Name", ValueSelector = g => g.UserGroupName, Width = 130f },
                new() { Header = "Description", ValueSelector = g => g.Description, Width = 180f },
                new() { Header = "Members", ValueSelector = g => g.UserCount.ToString(), Width = 60f, Alignment = TextAlignment.Center },
                new() { Header = "Status", ValueSelector = g => g.Status, Width = 60f, Alignment = TextAlignment.Center },
                new() { Header = "Created Date", ValueSelector = g => g.CreatedDate, Width = 90f, Alignment = TextAlignment.Center }
            };

            var summary = new Dictionary<string, string>
            {
                { "Total Groups", groupList.Count.ToString() },
                { "Active Groups", groupList.Count(g => g.Status == "Active").ToString() },
                { "Total Members", groupList.Sum(g => (int)g.UserCount).ToString() }
            };

            var pdfBytes = _pdfService.GenerateListPdf(
                title: "User Groups & Roles Report",
                subtitle: $"Exported {DateTime.Now:yyyy-MM-dd HH:mm}",
                columns: columns,
                data: groupList,
                generatedBy: CurrentUserName,
                summaryItems: summary);

            return File(pdfBytes, "application/pdf", $"User_Groups_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private async Task LoadDataAsync()
        {
            var query = Context.Usergroups.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(g => g.UserGroupName.ToLower().Contains(term) || (g.UserGroupDescription != null && g.UserGroupDescription.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();

            Pagination = new PaginationDto
            {
                CurrentPage = CurrentPage < 1 ? 1 : CurrentPage,
                PageSize = PageSize < 1 ? 10 : PageSize,
                TotalItems = totalCount
            };

            var groupEntities = await query
                .OrderBy(g => g.UserGroupName)
                .Skip((Pagination.CurrentPage - 1) * Pagination.PageSize)
                .Take(Pagination.PageSize)
                .ToListAsync();

            var groupIds = groupEntities.Select(g => g.UserGroupId).ToList();
            var userCounts = await Context.Users
                .Where(u => u.UserGroupId != null && groupIds.Contains(u.UserGroupId.Value))
                .GroupBy(u => u.UserGroupId!.Value)
                .Select(g => new { GroupId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.GroupId, x => x.Count);

            Groups = groupEntities.Select(g => new GroupViewModel
            {
                UserGroupId = g.UserGroupId,
                UserGroupName = g.UserGroupName,
                UserGroupDescription = g.UserGroupDescription,
                Status = g.Status == true,
                UserCount = userCounts.ContainsKey(g.UserGroupId) ? userCounts[g.UserGroupId] : 0,
                CreatedDate = g.CreatedDate
            }).ToList();
        }
    }
}
