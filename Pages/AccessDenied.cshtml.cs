using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PMS.Pages
{
    public class AccessDeniedModel : PageModel
    {
        public string? ModuleName { get; set; }
        public string? ActionName { get; set; }

        public void OnGet(string? module, string? action)
        {
            ModuleName = module;
            ActionName = action;
        }
    }
}
