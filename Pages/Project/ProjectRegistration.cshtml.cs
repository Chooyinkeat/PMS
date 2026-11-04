using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PMS.Pages.Project
{
    public class ProjectRegistrationModel : PageModel
    {
        public IActionResult OnGet() => RedirectToPage("/Project/List");
    }
}
