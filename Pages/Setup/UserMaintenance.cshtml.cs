using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using PMS.Services;
using System.ComponentModel.DataAnnotations;

namespace PMS.Pages.Setup
{
    public class UserMaintenanceModel : BasePmsPageModel
    {
        private readonly IPdfService _pdfService;

        public UserMaintenanceModel(ApplicationDbContext context, IPermissionService permissionService, IPdfService pdfService)
            : base(context, permissionService)
        {
            _pdfService = pdfService;
        }

        public List<UserViewModel> Users { get; set; } = new();
        public List<Usergroup> UserGroups { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? GroupFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        // Input Model for Create / Edit
        [BindProperty]
        public UserInputModel Input { get; set; } = new();

        public class UserViewModel
        {
            public int UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public int? UserGroupId { get; set; }
            public string UserGroupName { get; set; } = "None";
            public bool Status { get; set; }
            public string? Remark { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? EditedDate { get; set; }
        }

        public class UserInputModel
        {
            public int? UserId { get; set; }

            [Required(ErrorMessage = "User name is required.")]
            [StringLength(100, ErrorMessage = "User name cannot exceed 100 characters.")]
            public string UserName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
            [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
            public string Email { get; set; } = string.Empty;

            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
            public string? Password { get; set; }

            public int? UserGroupId { get; set; }

            [StringLength(500)]
            public string? Remark { get; set; }

            public bool Status { get; set; } = true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await CheckPermissionAsync(AppModules.User, PermissionAction.View))
                return RedirectToAccessDenied(AppModules.User, PermissionAction.View);

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!await CheckPermissionAsync(AppModules.User, PermissionAction.Add))
                return RedirectToAccessDenied(AppModules.User, PermissionAction.Add);

            if (string.IsNullOrWhiteSpace(Input.Password))
            {
                ModelState.AddModelError("Input.Password", "Password is required for new users.");
            }

            // Check unique username
            if (await Context.Users.AnyAsync(u => u.UserName.ToLower() == Input.UserName.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.UserName", "A user with this username already exists.");
            }

            // Check unique email
            if (await Context.Users.AnyAsync(u => u.Email.ToLower() == Input.Email.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.Email", "A user with this email address already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            var hasher = new PasswordHasher<User>();
            var newUser = new User
            {
                UserName = Input.UserName.Trim(),
                Email = Input.Email.Trim(),
                UserGroupId = Input.UserGroupId,
                Status = Input.Status,
                Remark = Input.Remark?.Trim(),
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUserId > 0 ? CurrentUserId : null
            };

            newUser.Password = hasher.HashPassword(newUser, Input.Password!);

            Context.Users.Add(newUser);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"User '{newUser.UserName}' created successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!await CheckPermissionAsync(AppModules.User, PermissionAction.Edit))
                return RedirectToAccessDenied(AppModules.User, PermissionAction.Edit);

            if (!Input.UserId.HasValue)
            {
                SetErrorMessage("Invalid user ID.");
                return RedirectToPage();
            }

            var user = await Context.Users.FindAsync(Input.UserId.Value);
            if (user == null)
            {
                SetErrorMessage("User not found.");
                return RedirectToPage();
            }

            // Check unique username
            if (await Context.Users.AnyAsync(u => u.UserId != user.UserId && u.UserName.ToLower() == Input.UserName.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.UserName", "A user with this username already exists.");
            }

            // Check unique email
            if (await Context.Users.AnyAsync(u => u.UserId != user.UserId && u.Email.ToLower() == Input.Email.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.Email", "A user with this email address already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            user.UserName = Input.UserName.Trim();
            user.Email = Input.Email.Trim();
            user.UserGroupId = Input.UserGroupId;
            user.Status = Input.Status;
            user.Remark = Input.Remark?.Trim();
            user.EditedDate = DateTime.Now;
            user.EditedBy = CurrentUserId > 0 ? CurrentUserId : null;

            if (!string.IsNullOrWhiteSpace(Input.Password))
            {
                var hasher = new PasswordHasher<User>();
                user.Password = hasher.HashPassword(user, Input.Password);
            }

            await Context.SaveChangesAsync();
            SetSuccessMessage($"User '{user.UserName}' updated successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!await CheckPermissionAsync(AppModules.User, PermissionAction.Delete))
                return RedirectToAccessDenied(AppModules.User, PermissionAction.Delete);

            if (id == CurrentUserId)
            {
                SetErrorMessage("You cannot delete your own user account.");
                return RedirectToPage();
            }

            var user = await Context.Users.FindAsync(id);
            if (user == null)
            {
                SetErrorMessage("User not found.");
                return RedirectToPage();
            }

            // Check if user has associated project members
            var hasProjects = await Context.Projectmembers.AnyAsync(pm => pm.UserId == id);
            if (hasProjects)
            {
                // Soft-delete by setting status = false to protect relational data integrity
                user.Status = false;
                user.EditedDate = DateTime.Now;
                user.EditedBy = CurrentUserId > 0 ? CurrentUserId : null;
                await Context.SaveChangesAsync();
                SetSuccessMessage($"User '{user.UserName}' is assigned to project teams and has been deactivated instead of deleted.");
                return RedirectToPage();
            }

            Context.Users.Remove(user);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"User '{user.UserName}' deleted successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            if (!await CheckPermissionAsync(AppModules.User, PermissionAction.Print))
                return RedirectToAccessDenied(AppModules.User, PermissionAction.Print);

            var query = Context.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(u => u.UserName.ToLower().Contains(term) || u.Email.ToLower().Contains(term));
            }

            if (StatusFilter == "active") query = query.Where(u => u.Status == true);
            else if (StatusFilter == "inactive") query = query.Where(u => u.Status == false);

            if (GroupFilter.HasValue && GroupFilter > 0)
                query = query.Where(u => u.UserGroupId == GroupFilter.Value);

            var userList = await query
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.UserName,
                    u.Email,
                    GroupName = u.UserGroupId != null ? Context.Usergroups.Where(g => g.UserGroupId == u.UserGroupId).Select(g => g.UserGroupName).FirstOrDefault() ?? "None" : "None",
                    Status = u.Status == true ? "Active" : "Inactive",
                    CreatedDate = u.CreatedDate.ToString("yyyy-MM-dd HH:mm")
                })
                .ToListAsync();

            var columns = new List<PdfColumn<dynamic>>
            {
                new() { Header = "Username", ValueSelector = u => u.UserName, Width = 110f },
                new() { Header = "Email", ValueSelector = u => u.Email, Width = 150f },
                new() { Header = "User Group", ValueSelector = u => u.GroupName, Width = 110f },
                new() { Header = "Status", ValueSelector = u => u.Status, Width = 60f, Alignment = TextAlignment.Center },
                new() { Header = "Created Date", ValueSelector = u => u.CreatedDate, Width = 90f, Alignment = TextAlignment.Center }
            };

            var summary = new Dictionary<string, string>
            {
                { "Total Users", userList.Count.ToString() },
                { "Active Users", userList.Count(u => u.Status == "Active").ToString() },
                { "Inactive Users", userList.Count(u => u.Status == "Inactive").ToString() }
            };

            var pdfBytes = _pdfService.GenerateListPdf(
                title: "User Accounts Report",
                subtitle: $"Exported {DateTime.Now:yyyy-MM-dd HH:mm}",
                columns: columns,
                data: userList,
                generatedBy: CurrentUserName,
                summaryItems: summary);

            return File(pdfBytes, "application/pdf", $"User_Maintenance_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private async Task LoadDataAsync()
        {
            UserGroups = await Context.Usergroups
                .AsNoTracking()
                .OrderBy(g => g.UserGroupName)
                .ToListAsync();

            var query = Context.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(u => u.UserName.ToLower().Contains(term) || u.Email.ToLower().Contains(term));
            }

            if (StatusFilter == "active") query = query.Where(u => u.Status == true);
            else if (StatusFilter == "inactive") query = query.Where(u => u.Status == false);

            if (GroupFilter.HasValue && GroupFilter > 0)
                query = query.Where(u => u.UserGroupId == GroupFilter.Value);

            var totalCount = await query.CountAsync();

            Pagination = new PaginationDto
            {
                CurrentPage = CurrentPage < 1 ? 1 : CurrentPage,
                PageSize = PageSize < 1 ? 10 : PageSize,
                TotalItems = totalCount
            };

            var groupsDict = UserGroups.ToDictionary(g => g.UserGroupId, g => g.UserGroupName);

            var userEntities = await query
                .OrderByDescending(u => u.CreatedDate)
                .Skip((Pagination.CurrentPage - 1) * Pagination.PageSize)
                .Take(Pagination.PageSize)
                .ToListAsync();

            Users = userEntities.Select(u => new UserViewModel
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                UserGroupId = u.UserGroupId,
                UserGroupName = u.UserGroupId.HasValue && groupsDict.ContainsKey(u.UserGroupId.Value)
                    ? groupsDict[u.UserGroupId.Value]
                    : "None",
                Status = u.Status == true,
                Remark = u.Remark,
                CreatedDate = u.CreatedDate,
                EditedDate = u.EditedDate
            }).ToList();
        }
    }
}
