using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PMS.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Clear all session values
            HttpContext.Session.Clear();

            // 2. Remove the login (auth) cookie
            await HttpContext.SignOutAsync("Cookies");

            // 3. Remove the session cookie from the browser
            Response.Cookies.Delete(".AspNetCore.Session");

            // 4. Stop the browser from showing cached pages with the Back button
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";

            // 5. Go to the login page
            return RedirectToPage("/Login");
        }

        // If someone opens /Logout directly in the browser
        public IActionResult OnGet() => RedirectToPage("/Index");
    }
}