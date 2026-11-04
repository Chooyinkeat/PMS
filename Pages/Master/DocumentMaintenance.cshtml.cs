using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PMS.Pages.Master
{
    public class DocumentMaintenanceModel : PageModel
    {
        public IActionResult OnGet() => RedirectToPage("/Setup/DocMaintenance");
    }
}
