using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using PMS.Services;

namespace PMS.Pages
{
    public class IndexModel : BasePmsPageModel
    {
        public IndexModel(ApplicationDbContext context, IPermissionService permissionService)
            : base(context, permissionService)
        {
        }

        public DashboardStatsViewModel Stats { get; set; } = new();
        public List<RecentProjectViewModel> RecentProjects { get; set; } = new();
        public List<RecentPaymentViewModel> RecentPayments { get; set; } = new();

        public class DashboardStatsViewModel
        {
            public int TotalProjects { get; set; }
            public int InProgressProjects { get; set; }
            public int CompletedProjects { get; set; }
            public int PendingProjects { get; set; }
            public int TotalCustomers { get; set; }
            public decimal TotalContractSum { get; set; }
            public decimal TotalCollected { get; set; }
            public decimal TotalOutstanding => Math.Max(0, TotalContractSum - TotalCollected);
            public int CollectionRate => TotalContractSum > 0 ? (int)Math.Min(100, Math.Round((TotalCollected / TotalContractSum) * 100)) : 0;
            public int TotalUsers { get; set; }
        }

        public class RecentProjectViewModel
        {
            public int ProjectId { get; set; }
            public string ProjectCode { get; set; } = string.Empty;
            public string ProjectName { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string Status { get; set; } = "Pending";
            public DateOnly? DueDate { get; set; }
        }

        public class RecentPaymentViewModel
        {
            public int PaymentId { get; set; }
            public string? PaymentReference { get; set; }
            public string ProjectCode { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Status { get; set; } = "Pending";
            public DateOnly? DueDate { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Ensure default admin/group permissions exist
            await PermissionService.EnsureDefaultPermissionsAsync();

            // 1. Projects Statistics
            var allProjects = await Context.Projects.AsNoTracking().ToListAsync();
            var allCustomers = await Context.Customers.AsNoTracking().ToListAsync();
            var allPayments = await Context.Payments.AsNoTracking().ToListAsync();
            var allUsers = await Context.Users.AsNoTracking().ToListAsync();

            Stats = new DashboardStatsViewModel
            {
                TotalProjects = allProjects.Count,
                InProgressProjects = allProjects.Count(p => p.Status == "In Progress"),
                CompletedProjects = allProjects.Count(p => p.Status == "Completed"),
                PendingProjects = allProjects.Count(p => p.Status == "Pending"),
                TotalCustomers = allCustomers.Count(c => c.Status == true),
                TotalContractSum = allProjects.Sum(p => p.Price),
                TotalCollected = allPayments.Where(pm => pm.PaymentStatus == "Paid").Sum(pm => pm.Amount),
                TotalUsers = allUsers.Count(u => u.Status == true)
            };

            // 2. Recent Projects
            RecentProjects = await Context.Projects
                .Include(p => p.Customer)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedDate)
                .Take(5)
                .Select(p => new RecentProjectViewModel
                {
                    ProjectId = p.ProjectId,
                    ProjectCode = p.ProjectCode,
                    ProjectName = p.ProjectName,
                    CustomerName = p.Customer.CustomerName,
                    Price = p.Price,
                    Status = p.Status,
                    DueDate = p.DueDate
                })
                .ToListAsync();

            // 3. Recent Payments
            RecentPayments = await Context.Payments
                .Include(pm => pm.Project)
                .AsNoTracking()
                .OrderByDescending(pm => pm.CreatedDate)
                .Take(5)
                .Select(pm => new RecentPaymentViewModel
                {
                    PaymentId = pm.PaymentId,
                    PaymentReference = pm.PaymentReference,
                    ProjectCode = pm.Project.ProjectCode,
                    Amount = pm.Amount,
                    Status = pm.PaymentStatus,
                    DueDate = pm.DueDate
                })
                .ToListAsync();

            return Page();
        }
    }
}
