using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using PMS.Services;
using System.ComponentModel.DataAnnotations;

namespace PMS.Pages.Project
{
    public class ListModel : BasePmsPageModel
    {
        private readonly IPdfService _pdfService;

        public ListModel(ApplicationDbContext context, IPermissionService permissionService, IPdfService pdfService)
            : base(context, permissionService)
        {
            _pdfService = pdfService;
        }

        public List<ProjectListItemViewModel> Projects { get; set; } = new();
        public List<Customer> Customers { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CustomerFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty]
        public ProjectInputModel Input { get; set; } = new();

        public class ProjectListItemViewModel
        {
            public int ProjectId { get; set; }
            public string ProjectCode { get; set; } = string.Empty;
            public string ProjectName { get; set; } = string.Empty;
            public int CustomerId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public DateOnly? StartDate { get; set; }
            public DateOnly? DueDate { get; set; }
            public string Status { get; set; } = "Pending";
            public int MemberCount { get; set; }
            public decimal TotalPaid { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public class ProjectInputModel
        {
            public int? ProjectId { get; set; }

            [Required(ErrorMessage = "Project code is required.")]
            [StringLength(50, ErrorMessage = "Project code cannot exceed 50 characters.")]
            public string ProjectCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Project name is required.")]
            [StringLength(200, ErrorMessage = "Project name cannot exceed 200 characters.")]
            public string ProjectName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Customer is required.")]
            public int CustomerId { get; set; }

            [Required(ErrorMessage = "Contract price is required.")]
            [Range(0, 999999999999.99, ErrorMessage = "Price must be a valid positive amount.")]
            public decimal Price { get; set; }

            public DateOnly? StartDate { get; set; }

            public DateOnly? DueDate { get; set; }

            public string? Description { get; set; }

            [StringLength(500)]
            public string? Remark { get; set; }

            [Required]
            [StringLength(30)]
            public string Status { get; set; } = "Pending";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Project, PermissionAction.View))
                return RedirectToAccessDenied(AppModules.Project, PermissionAction.View);

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Project, PermissionAction.Add))
                return RedirectToAccessDenied(AppModules.Project, PermissionAction.Add);

            if (await Context.Projects.AnyAsync(p => p.ProjectCode.ToLower() == Input.ProjectCode.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.ProjectCode", "A project with this project code already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            var project = new Models.Project
            {
                ProjectCode = Input.ProjectCode.Trim().ToUpper(),
                ProjectName = Input.ProjectName.Trim(),
                CustomerId = Input.CustomerId,
                Price = Input.Price,
                StartDate = Input.StartDate,
                DueDate = Input.DueDate,
                Description = Input.Description?.Trim(),
                Remark = Input.Remark?.Trim(),
                Status = Input.Status,
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUserId > 0 ? CurrentUserId : null
            };

            Context.Projects.Add(project);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"Project '{project.ProjectName}' created successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Project, PermissionAction.Edit))
                return RedirectToAccessDenied(AppModules.Project, PermissionAction.Edit);

            if (!Input.ProjectId.HasValue)
            {
                SetErrorMessage("Invalid project ID.");
                return RedirectToPage();
            }

            var project = await Context.Projects.FindAsync(Input.ProjectId.Value);
            if (project == null)
            {
                SetErrorMessage("Project not found.");
                return RedirectToPage();
            }

            if (await Context.Projects.AnyAsync(p => p.ProjectId != project.ProjectId && p.ProjectCode.ToLower() == Input.ProjectCode.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.ProjectCode", "A project with this project code already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            project.ProjectCode = Input.ProjectCode.Trim().ToUpper();
            project.ProjectName = Input.ProjectName.Trim();
            project.CustomerId = Input.CustomerId;
            project.Price = Input.Price;
            project.StartDate = Input.StartDate;
            project.DueDate = Input.DueDate;
            project.Description = Input.Description?.Trim();
            project.Remark = Input.Remark?.Trim();
            project.Status = Input.Status;
            project.EditedDate = DateTime.Now;
            project.EditedBy = CurrentUserId > 0 ? CurrentUserId : null;

            await Context.SaveChangesAsync();
            SetSuccessMessage($"Project '{project.ProjectName}' updated successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!await CheckPermissionAsync(AppModules.Project, PermissionAction.Delete))
                return RedirectToAccessDenied(AppModules.Project, PermissionAction.Delete);

            var project = await Context.Projects
                .Include(p => p.Payments)
                .Include(p => p.Projectmembers)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
            {
                SetErrorMessage("Project not found.");
                return RedirectToPage();
            }

            // Remove associated payments and members
            if (project.Payments.Any())
            {
                Context.Payments.RemoveRange(project.Payments);
            }

            if (project.Projectmembers.Any())
            {
                Context.Projectmembers.RemoveRange(project.Projectmembers);
            }

            Context.Projects.Remove(project);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"Project '{project.ProjectName}' and associated records deleted successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Project, PermissionAction.Print))
                return RedirectToAccessDenied(AppModules.Project, PermissionAction.Print);

            var query = Context.Projects
                .Include(p => p.Customer)
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

            if (!string.IsNullOrWhiteSpace(StatusFilter))
                query = query.Where(p => p.Status == StatusFilter);

            var projectList = await query
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new
                {
                    p.ProjectCode,
                    p.ProjectName,
                    Customer = p.Customer.CustomerName,
                    Price = p.Price.ToString("C2"),
                    Status = p.Status,
                    StartDate = p.StartDate.HasValue ? p.StartDate.Value.ToString("yyyy-MM-dd") : "-",
                    DueDate = p.DueDate.HasValue ? p.DueDate.Value.ToString("yyyy-MM-dd") : "-"
                })
                .ToListAsync();

            var columns = new List<PdfColumn<dynamic>>
            {
                new() { Header = "Project Code", ValueSelector = p => p.ProjectCode, Width = 80f },
                new() { Header = "Project Title", ValueSelector = p => p.ProjectName, Width = 150f },
                new() { Header = "Client", ValueSelector = p => p.Customer, Width = 120f },
                new() { Header = "Contract Value", ValueSelector = p => p.Price, Width = 80f, Alignment = TextAlignment.Right },
                new() { Header = "Due Date", ValueSelector = p => p.DueDate, Width = 70f, Alignment = TextAlignment.Center },
                new() { Header = "Status", ValueSelector = p => p.Status, Width = 65f, Alignment = TextAlignment.Center }
            };

            var summary = new Dictionary<string, string>
            {
                { "Total Projects", projectList.Count.ToString() },
                { "In Progress", projectList.Count(p => p.Status == "In Progress").ToString() },
                { "Completed", projectList.Count(p => p.Status == "Completed").ToString() }
            };

            var pdfBytes = _pdfService.GenerateListPdf(
                title: "Master Project Register Report",
                subtitle: $"Exported {DateTime.Now:yyyy-MM-dd HH:mm}",
                columns: columns,
                data: projectList,
                generatedBy: CurrentUserName,
                summaryItems: summary,
                landscape: true);

            return File(pdfBytes, "application/pdf", $"Project_Register_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private async Task LoadDataAsync()
        {
            Customers = await Context.Customers
                .AsNoTracking()
                .Where(c => c.Status == true)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();

            var query = Context.Projects
                .Include(p => p.Customer)
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

            if (!string.IsNullOrWhiteSpace(StatusFilter))
                query = query.Where(p => p.Status == StatusFilter);

            var totalCount = await query.CountAsync();

            Pagination = new PaginationDto
            {
                CurrentPage = CurrentPage < 1 ? 1 : CurrentPage,
                PageSize = PageSize < 1 ? 10 : PageSize,
                TotalItems = totalCount
            };

            var entities = await query
                .OrderByDescending(p => p.CreatedDate)
                .Skip((Pagination.CurrentPage - 1) * Pagination.PageSize)
                .Take(Pagination.PageSize)
                .ToListAsync();

            var projectIds = entities.Select(p => p.ProjectId).ToList();

            var memberCounts = await Context.Projectmembers
                .Where(pm => projectIds.Contains(pm.ProjectId))
                .GroupBy(pm => pm.ProjectId)
                .Select(g => new { ProjectId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ProjectId, x => x.Count);

            var paymentsSum = await Context.Payments
                .Where(pm => projectIds.Contains(pm.ProjectId) && pm.PaymentStatus == "Paid")
                .GroupBy(pm => pm.ProjectId)
                .Select(g => new { ProjectId = g.Key, Total = g.Sum(p => p.Amount) })
                .ToDictionaryAsync(x => x.ProjectId, x => x.Total);

            Projects = entities.Select(p => new ProjectListItemViewModel
            {
                ProjectId = p.ProjectId,
                ProjectCode = p.ProjectCode,
                ProjectName = p.ProjectName,
                CustomerId = p.CustomerId,
                CustomerName = p.Customer?.CustomerName ?? "Unknown",
                Price = p.Price,
                StartDate = p.StartDate,
                DueDate = p.DueDate,
                Status = p.Status,
                MemberCount = memberCounts.ContainsKey(p.ProjectId) ? memberCounts[p.ProjectId] : 0,
                TotalPaid = paymentsSum.ContainsKey(p.ProjectId) ? paymentsSum[p.ProjectId] : 0,
                CreatedDate = p.CreatedDate
            }).ToList();
        }
    }
}
