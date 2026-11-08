using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using PMS.Services;

namespace PMS.Pages.Setup
{
    public class DocMaintenanceModel : BasePmsPageModel
    {
        private readonly IPdfService _pdfService;

        public DocMaintenanceModel(ApplicationDbContext context, IPermissionService permissionService, IPdfService pdfService)
            : base(context, permissionService)
        {
            _pdfService = pdfService;
        }

        public List<Customer> Customers { get; set; } = new();
        public List<Models.Project> Projects { get; set; } = new();

        public class ProjectReportRow
        {
            public string ProjectCode { get; set; } = "";
            public string ProjectName { get; set; } = "";
            public string Customer { get; set; } = "";
            public string Price { get; set; } = "";
            public string Status { get; set; } = "";
            public string DueDate { get; set; } = "";
        }

        public class PaymentReportRow
        {
            public string Ref { get; set; } = "";
            public string Project { get; set; } = "";
            public string Customer { get; set; } = "";
            public string Amount { get; set; } = "";
            public string Method { get; set; } = "";
            public string DueDate { get; set; } = "";
            public string Status { get; set; } = "";
        }

        public class CustomerReportRow
        {
            public string CustomerCode { get; set; } = "";
            public string CustomerName { get; set; } = "";
            public int Projects { get; set; }
            public int Pics { get; set; }
            public string Status { get; set; } = "";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Document, PermissionAction.View))
                return RedirectToAccessDenied(AppModules.Document, PermissionAction.View);

            Customers = await Context.Customers.AsNoTracking().Where(c => c.Status == true).OrderBy(c => c.CustomerName).ToListAsync();
            Projects = await Context.Projects.AsNoTracking().OrderBy(p => p.ProjectName).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetExportProjectsReportAsync(string? status, int? customerId)
        {
            if (!await CheckPermissionAsync(AppModules.Document, PermissionAction.Print))
                return RedirectToAccessDenied(AppModules.Document, PermissionAction.Print);

            var query = Context.Projects
                .Include(p => p.Customer)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => p.Status == status);

            if (customerId.HasValue && customerId > 0)
                query = query.Where(p => p.CustomerId == customerId.Value);

            var list = await query
                .OrderBy(p => p.ProjectName)
                .Select(p => new ProjectReportRow
                {
                    ProjectCode = p.ProjectCode,
                    ProjectName = p.ProjectName,
                    Customer = p.Customer != null ? p.Customer.CustomerName : "Unknown",
                    Price = p.Price.ToString("C2"),
                    Status = p.Status,
                    DueDate = p.DueDate.HasValue ? p.DueDate.Value.ToString("yyyy-MM-dd") : "-"
                })
                .ToListAsync();

            var columns = new List<PdfColumn<ProjectReportRow>>
            {
                new() { Header = "Project Code", ValueSelector = p => p.ProjectCode, Width = 80f },
                new() { Header = "Project Title", ValueSelector = p => p.ProjectName, Width = 150f },
                new() { Header = "Customer", ValueSelector = p => p.Customer, Width = 120f },
                new() { Header = "Contract Value", ValueSelector = p => p.Price, Width = 85f, Alignment = TextAlignment.Right },
                new() { Header = "Target Due", ValueSelector = p => p.DueDate, Width = 70f, Alignment = TextAlignment.Center },
                new() { Header = "Status", ValueSelector = p => p.Status, Width = 65f, Alignment = TextAlignment.Center }
            };

            var summary = new Dictionary<string, string>
            {
                { "Total Projects", list.Count.ToString() },
                { "Active / In Progress", list.Count(p => p.Status == "In Progress").ToString() },
                { "Completed", list.Count(p => p.Status == "Completed").ToString() }
            };

            var pdf = _pdfService.GenerateListPdf(
                title: "Official Project Portfolio Audit",
                subtitle: $"Comprehensive Project Summary - Generated {DateTime.Now:yyyy-MM-dd HH:mm}",
                columns: columns,
                data: list,
                generatedBy: CurrentUserName,
                summaryItems: summary,
                landscape: true);

            return File(pdf, "application/pdf", $"Project_Portfolio_Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        public async Task<IActionResult> OnGetExportPaymentsReportAsync(string? status, int? projectId)
        {
            if (!await CheckPermissionAsync(AppModules.Document, PermissionAction.Print))
                return RedirectToAccessDenied(AppModules.Document, PermissionAction.Print);

            var query = Context.Payments
                .Include(p => p.Project)
                .ThenInclude(pr => pr.Customer)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => p.PaymentStatus == status);

            if (projectId.HasValue && projectId > 0)
                query = query.Where(p => p.ProjectId == projectId.Value);

            var list = await query
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new PaymentReportRow
                {
                    Ref = p.PaymentReference ?? "N/A",
                    Project = p.Project != null ? p.Project.ProjectCode : "N/A",
                    Customer = p.Project != null && p.Project.Customer != null ? p.Project.Customer.CustomerName : "N/A",
                    Amount = p.Amount.ToString("C2"),
                    Status = p.PaymentStatus,
                    Method = p.PaymentMethod ?? "Transfer",
                    DueDate = p.DueDate.HasValue ? p.DueDate.Value.ToString("yyyy-MM-dd") : "-"
                })
                .ToListAsync();

            var columns = new List<PdfColumn<PaymentReportRow>>
            {
                new() { Header = "Invoice Ref #", ValueSelector = p => p.Ref, Width = 95f },
                new() { Header = "Project", ValueSelector = p => p.Project, Width = 75f },
                new() { Header = "Customer", ValueSelector = p => p.Customer, Width = 110f },
                new() { Header = "Amount", ValueSelector = p => p.Amount, Width = 80f, Alignment = TextAlignment.Right },
                new() { Header = "Method", ValueSelector = p => p.Method, Width = 75f },
                new() { Header = "Due Date", ValueSelector = p => p.DueDate, Width = 65f, Alignment = TextAlignment.Center },
                new() { Header = "Status", ValueSelector = p => p.Status, Width = 65f, Alignment = TextAlignment.Center }
            };

            var summary = new Dictionary<string, string>
            {
                { "Total Billings", list.Count.ToString() },
                { "Paid", list.Count(p => p.Status == "Paid").ToString() },
                { "Pending", list.Count(p => p.Status == "Pending").ToString() }
            };

            var pdf = _pdfService.GenerateListPdf(
                title: "Financial Billing & Cashflow Statement",
                subtitle: $"Revenue Audit - Generated {DateTime.Now:yyyy-MM-dd HH:mm}",
                columns: columns,
                data: list,
                generatedBy: CurrentUserName,
                summaryItems: summary,
                landscape: true);

            return File(pdf, "application/pdf", $"Financial_Billing_Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        public async Task<IActionResult> OnGetExportCustomersReportAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Document, PermissionAction.Print))
                return RedirectToAccessDenied(AppModules.Document, PermissionAction.Print);

            var list = await Context.Customers
                .AsNoTracking()
                .OrderBy(c => c.CustomerName)
                .Select(c => new CustomerReportRow
                {
                    CustomerCode = c.CustomerCode,
                    CustomerName = c.CustomerName,
                    Projects = Context.Projects.Count(p => p.CustomerId == c.CustomerId),
                    Pics = Context.Personincharges.Count(p => p.CustomerId == c.CustomerId),
                    Status = c.Status == true ? "Active" : "Inactive"
                })
                .ToListAsync();

            var columns = new List<PdfColumn<CustomerReportRow>>
            {
                new() { Header = "Customer Code", ValueSelector = c => c.CustomerCode, Width = 90f },
                new() { Header = "Customer / Client Name", ValueSelector = c => c.CustomerName, Width = 180f },
                new() { Header = "Projects Count", ValueSelector = c => c.Projects.ToString(), Width = 70f, Alignment = TextAlignment.Center },
                new() { Header = "PICs Count", ValueSelector = c => c.Pics.ToString(), Width = 65f, Alignment = TextAlignment.Center },
                new() { Header = "Status", ValueSelector = c => c.Status, Width = 60f, Alignment = TextAlignment.Center }
            };

            var summary = new Dictionary<string, string>
            {
                { "Total Customers", list.Count.ToString() },
                { "Active Clients", list.Count(c => c.Status == "Active").ToString() },
                { "Total Projects", list.Sum(c => c.Projects).ToString() }
            };

            var pdf = _pdfService.GenerateListPdf(
                title: "Corporate Customer Directory & Portfolio",
                subtitle: $"Client Register - Generated {DateTime.Now:yyyy-MM-dd HH:mm}",
                columns: columns,
                data: list,
                generatedBy: CurrentUserName,
                summaryItems: summary);

            return File(pdf, "application/pdf", $"Customer_Portfolio_Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }
    }
}
