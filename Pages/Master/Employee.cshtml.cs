using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PMS.Pages.Master
{
    public class EmployeeModel : PageModel
    {
        public IActionResult OnGet() => RedirectToPage("/Setup/UserMaintenance");
    }
}
