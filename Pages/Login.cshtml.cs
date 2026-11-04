using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using System.Security.Claims;
using System.Diagnostics;

namespace PMS.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            //var hasher = new PasswordHasher<object>();
            //string hash = hasher.HashPassword(null, "globaladmin");
            //Debug.WriteLine(hash);
            if (user == null || !VerifyPassword(user, Password))
            {
                ErrorMessage = "Invalid email/username or password.";
                return Page();   
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));
            HttpContext.Session.SetInt32("UserId", user.UserId);     
            HttpContext.Session.SetString("UserName", user.UserName);
            return RedirectToPage("/Index");
        }

        public void OnGet()
        {
        }

        private bool VerifyPassword(User user, string password)
        {
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
