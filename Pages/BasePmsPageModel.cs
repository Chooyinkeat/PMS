using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMS.Data;
using PMS.Models.Common;
using PMS.Services;
using System.Security.Claims;

namespace PMS.Pages
{
    public abstract class BasePmsPageModel : PageModel
    {
        protected readonly ApplicationDbContext Context;
        protected readonly IPermissionService PermissionService;

        public ModulePermissionDto PagePermission { get; set; } = ModulePermissionDto.NoAccess("");

        protected BasePmsPageModel(ApplicationDbContext context, IPermissionService permissionService)
        {
            Context = context;
            PermissionService = permissionService;
        }

        public int CurrentUserId
        {
            get
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idClaim, out var id)) return id;
                var sessionUserId = HttpContext.Session.GetInt32("UserId");
                return sessionUserId ?? 0;
            }
        }

        public string CurrentUserName
        {
            get
            {
                return User.Identity?.Name ?? HttpContext.Session.GetString("UserName") ?? "System";
            }
        }

        protected async Task<bool> CheckPermissionAsync(string module, PermissionAction action)
        {
            PagePermission = await PermissionService.GetPermissionAsync(User, module);

            var allowed = action switch
            {
                PermissionAction.View => PagePermission.CanView,
                PermissionAction.Add => PagePermission.CanAdd,
                PermissionAction.Edit => PagePermission.CanEdit,
                PermissionAction.Delete => PagePermission.CanDelete,
                PermissionAction.Print => PagePermission.CanPrint,
                _ => false
            };

            return allowed;
        }

        protected IActionResult RedirectToAccessDenied(string module, PermissionAction action)
        {
            return RedirectToPage("/AccessDenied", new { module, action = action.ToString() });
        }

        protected void SetSuccessMessage(string message)
        {
            TempData["SuccessMessage"] = message;
        }

        protected void SetErrorMessage(string message)
        {
            TempData["ErrorMessage"] = message;
        }
    }
}
