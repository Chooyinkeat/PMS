using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PMS.Pages.Setup
{
    public class CustomerModel : PageModel
    {
        public IActionResult OnGet() => RedirectToPage("/Master/Customer");
    }
}
