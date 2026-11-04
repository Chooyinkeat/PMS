using PMS.Models.Common;
using System.Security.Claims;

namespace PMS.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(ClaimsPrincipal user, string module, PermissionAction action);
        Task<bool> HasPermissionAsync(int userId, string module, PermissionAction action);
        Task<ModulePermissionDto> GetPermissionAsync(ClaimsPrincipal user, string module);
        Task<ModulePermissionDto> GetPermissionAsync(int userId, string module);
        Task<Dictionary<string, ModulePermissionDto>> GetAllPermissionsAsync(ClaimsPrincipal user);
        Task<Dictionary<string, ModulePermissionDto>> GetAllPermissionsAsync(int userId);
        Task EnsureDefaultPermissionsAsync();
        int? GetUserId(ClaimsPrincipal user);
    }
}
