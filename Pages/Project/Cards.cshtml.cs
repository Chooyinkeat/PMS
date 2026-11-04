using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using PMS.Services;

namespace PMS.Pages.Project
{
    public class CardsModel : BasePmsPageModel
    {
        public CardsModel(ApplicationDbContext context, IPermissionService permissionService)
            : base(context, permissionService)
        {
        }

        public List<ProjectCardViewModel> Projects { get; set; } = new();
        public List<Customer> Customers { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CustomerFilter { get; set; }

        public class ProjectCardViewModel
        {
            public int ProjectId { get; set; }
            public string ProjectCode { get; set; } = string.Empty;
            public string ProjectName { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal TotalPaid { get; set; }
            public int PaymentPercent => Price > 0 ? (int)Math.Min(100, Math.Round((TotalPaid / Price) * 100)) : 0;
            public DateOnly? StartDate { get; set; }
            public DateOnly? DueDate { get; set; }
            public string Status { get; set; } = "Pending";
            public List<string> MemberInitials { get; set; } = new();
            public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateOnly.FromDateTime(DateTime.Today) && Status != "Completed" && Status != "Cancelled";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Project, PermissionAction.View))
                return RedirectToAccessDenied(AppModules.Project, PermissionAction.View);

            Customers = await Context.Customers
                .AsNoTracking()
                .Where(c => c.Status == true)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();

            var query = Context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Projectmembers)
                    .ThenInclude(pm => pm.User)
                .Include(p => p.Payments)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(p => p.ProjectCode.ToLower().Contains(term) ||
                                         p.ProjectName.ToLower().Contains(term) ||
                                         (p.Description != null && p.Description.ToLower().Contains(term)));
            }

            if (CustomerFilter.HasValue && CustomerFilter > 0)
                query = query.Where(p => p.CustomerId == CustomerFilter.Value);

            var projectEntities = await query
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            Projects = projectEntities.Select(p =>
            {
                var paid = p.Payments.Where(pm => pm.PaymentStatus == "Paid").Sum(pm => pm.Amount);
                var initials = p.Projectmembers
                    .Select(pm => pm.User.UserName)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Take(4)
                    .Select(n => n.Length >= 2 ? n.Substring(0, 2).ToUpper() : n.ToUpper())
                    .ToList();

                return new ProjectCardViewModel
                {
                    ProjectId = p.ProjectId,
                    ProjectCode = p.ProjectCode,
                    ProjectName = p.ProjectName,
                    CustomerName = p.Customer?.CustomerName ?? "Unknown",
                    Price = p.Price,
                    TotalPaid = paid,
                    StartDate = p.StartDate,
                    DueDate = p.DueDate,
                    Status = p.Status,
                    MemberInitials = initials
                };
            }).ToList();

            return Page();
        }
    }
}
