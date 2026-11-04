using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using PMS.Services;
using System.ComponentModel.DataAnnotations;

namespace PMS.Pages.Project
{
    public class PaymentModel : BasePmsPageModel
    {
        private readonly IPdfService _pdfService;

        public PaymentModel(ApplicationDbContext context, IPermissionService permissionService, IPdfService pdfService)
            : base(context, permissionService)
        {
            _pdfService = pdfService;
        }

        public List<PaymentViewModel> Payments { get; set; } = new();
        public List<Models.Project> Projects { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ProjectFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty]
        public PaymentInputModel Input { get; set; } = new();

        public class PaymentViewModel
        {
            public int PaymentId { get; set; }
            public int ProjectId { get; set; }
            public string ProjectCode { get; set; } = string.Empty;
            public string ProjectName { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string? PaymentReference { get; set; }
            public string? PaymentDescription { get; set; }
            public decimal Amount { get; set; }
            public DateOnly? PaymentDate { get; set; }
            public DateOnly? DueDate { get; set; }
            public string? PaymentMethod { get; set; }
            public string PaymentStatus { get; set; } = "Pending";
            public string? Remark { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public class PaymentInputModel
        {
            public int? PaymentId { get; set; }

            [Required(ErrorMessage = "Please select a project.")]
            public int ProjectId { get; set; }

            [StringLength(100)]
            public string? PaymentReference { get; set; }

            [Required(ErrorMessage = "Description is required.")]
            [StringLength(500)]
            public string PaymentDescription { get; set; } = string.Empty;

            [Required(ErrorMessage = "Amount is required.")]
            [Range(0.01, 999999999999.99, ErrorMessage = "Amount must be greater than zero.")]
            public decimal Amount { get; set; }

            public DateOnly? PaymentDate { get; set; }
            public DateOnly? DueDate { get; set; }

            [StringLength(50)]
            public string? PaymentMethod { get; set; } = "Bank Transfer";

            [Required]
            [StringLength(30)]
            public string PaymentStatus { get; set; } = "Pending";

            [StringLength(500)]
            public string? Remark { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Payment, PermissionAction.View))
                return RedirectToAccessDenied(AppModules.Payment, PermissionAction.View);

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Payment, PermissionAction.Add))
                return RedirectToAccessDenied(AppModules.Payment, PermissionAction.Add);

            var project = await Context.Projects.FindAsync(Input.ProjectId);
            if (project == null)
            {
                ModelState.AddModelError("Input.ProjectId", "Selected project not found.");
            }

            if (!string.IsNullOrWhiteSpace(Input.PaymentReference))
            {
                var refCode = Input.PaymentReference.Trim();
                if (await Context.Payments.AnyAsync(p => p.PaymentReference != null && p.PaymentReference.ToLower() == refCode.ToLower()))
                {
                    ModelState.AddModelError("Input.PaymentReference", "A payment with this reference already exists.");
                }
            }
            else if (project != null)
            {
                Input.PaymentReference = $"INV-{project.ProjectCode}-{DateTime.Now:yyyyMMdd}-{DateTime.Now.Millisecond:D3}";
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            var payment = new Payment
            {
                ProjectId = Input.ProjectId,
                PaymentReference = Input.PaymentReference,
                PaymentDescription = Input.PaymentDescription.Trim(),
                Amount = Input.Amount,
                PaymentDate = Input.PaymentDate,
                DueDate = Input.DueDate,
                PaymentMethod = Input.PaymentMethod,
                PaymentStatus = Input.PaymentStatus,
                Remark = Input.Remark?.Trim(),
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUserId > 0 ? CurrentUserId : null
            };

            Context.Payments.Add(payment);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"Payment '{payment.PaymentReference}' created successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Payment, PermissionAction.Edit))
                return RedirectToAccessDenied(AppModules.Payment, PermissionAction.Edit);

            if (!Input.PaymentId.HasValue)
            {
                SetErrorMessage("Invalid payment ID.");
                return RedirectToPage();
            }

            var payment = await Context.Payments.FindAsync(Input.PaymentId.Value);
            if (payment == null)
            {
                SetErrorMessage("Payment not found.");
                return RedirectToPage();
            }

            if (!string.IsNullOrWhiteSpace(Input.PaymentReference))
            {
                var refCode = Input.PaymentReference.Trim();
                if (await Context.Payments.AnyAsync(p => p.PaymentId != payment.PaymentId && p.PaymentReference != null && p.PaymentReference.ToLower() == refCode.ToLower()))
                {
                    ModelState.AddModelError("Input.PaymentReference", "A payment with this reference already exists.");
                }
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            payment.ProjectId = Input.ProjectId;
            payment.PaymentReference = Input.PaymentReference;
            payment.PaymentDescription = Input.PaymentDescription.Trim();
            payment.Amount = Input.Amount;
            payment.PaymentDate = Input.PaymentDate;
            payment.DueDate = Input.DueDate;
            payment.PaymentMethod = Input.PaymentMethod;
            payment.PaymentStatus = Input.PaymentStatus;
            payment.Remark = Input.Remark?.Trim();
            payment.EditedDate = DateTime.Now;
            payment.EditedBy = CurrentUserId > 0 ? CurrentUserId : null;

            await Context.SaveChangesAsync();
            SetSuccessMessage($"Payment '{payment.PaymentReference}' updated successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!await CheckPermissionAsync(AppModules.Payment, PermissionAction.Delete))
                return RedirectToAccessDenied(AppModules.Payment, PermissionAction.Delete);

            var payment = await Context.Payments.FindAsync(id);
            if (payment == null)
            {
                SetErrorMessage("Payment not found.");
                return RedirectToPage();
            }

            Context.Payments.Remove(payment);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"Payment '{payment.PaymentReference}' deleted successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostMarkPaidAsync(int id)
        {
            if (!await CheckPermissionAsync(AppModules.Payment, PermissionAction.Edit))
                return RedirectToAccessDenied(AppModules.Payment, PermissionAction.Edit);

            var payment = await Context.Payments.FindAsync(id);
            if (payment != null)
            {
                payment.PaymentStatus = "Paid";
                if (!payment.PaymentDate.HasValue)
                {
                    payment.PaymentDate = DateOnly.FromDateTime(DateTime.Today);
                }
                payment.EditedDate = DateTime.Now;
                payment.EditedBy = CurrentUserId > 0 ? CurrentUserId : null;
                await Context.SaveChangesAsync();
                SetSuccessMessage($"Payment '{payment.PaymentReference}' marked as Paid.");
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Payment, PermissionAction.Print))
                return RedirectToAccessDenied(AppModules.Payment, PermissionAction.Print);

            var query = Context.Payments
                .Include(p => p.Project)
                .ThenInclude(pr => pr.Customer)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(p => (p.PaymentReference != null && p.PaymentReference.ToLower().Contains(term)) ||
                                         (p.PaymentDescription != null && p.PaymentDescription.ToLower().Contains(term)) ||
                                         p.Project.ProjectCode.ToLower().Contains(term) ||
                                         p.Project.ProjectName.ToLower().Contains(term));
            }

            if (ProjectFilter.HasValue && ProjectFilter > 0)
                query = query.Where(p => p.ProjectId == ProjectFilter.Value);

            if (!string.IsNullOrWhiteSpace(StatusFilter))
                query = query.Where(p => p.PaymentStatus == StatusFilter);

            var paymentList = await query
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new
                {
                    Reference = p.PaymentReference ?? "N/A",
                    p.Project.ProjectCode,
                    Customer = p.Project.Customer.CustomerName,
                    Amount = p.Amount.ToString("C2"),
                    Status = p.PaymentStatus,
                    Method = p.PaymentMethod ?? "Transfer",
                    DueDate = p.DueDate.HasValue ? p.DueDate.Value.ToString("yyyy-MM-dd") : "-",
                    PaidDate = p.PaymentDate.HasValue ? p.PaymentDate.Value.ToString("yyyy-MM-dd") : "-"
                })
                .ToListAsync();

            var columns = new List<PdfColumn<dynamic>>
            {
                new() { Header = "Ref #", ValueSelector = p => p.Reference, Width = 90f },
                new() { Header = "Project", ValueSelector = p => p.ProjectCode, Width = 75f },
                new() { Header = "Client", ValueSelector = p => p.Customer, Width = 110f },
                new() { Header = "Amount", ValueSelector = p => p.Amount, Width = 80f, Alignment = TextAlignment.Right },
                new() { Header = "Method", ValueSelector = p => p.Method, Width = 75f },
                new() { Header = "Due Date", ValueSelector = p => p.DueDate, Width = 65f, Alignment = TextAlignment.Center },
                new() { Header = "Status", ValueSelector = p => p.Status, Width = 65f, Alignment = TextAlignment.Center }
            };

            var summary = new Dictionary<string, string>
            {
                { "Total Invoices", paymentList.Count.ToString() },
                { "Paid", paymentList.Count(p => p.Status == "Paid").ToString() },
                { "Pending", paymentList.Count(p => p.Status == "Pending").ToString() }
            };

            var pdfBytes = _pdfService.GenerateListPdf(
                title: "Financial Payments & Billings Report",
                subtitle: $"Exported {DateTime.Now:yyyy-MM-dd HH:mm}",
                columns: columns,
                data: paymentList,
                generatedBy: CurrentUserName,
                summaryItems: summary,
                landscape: true);

            return File(pdfBytes, "application/pdf", $"Payments_Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private async Task LoadDataAsync()
        {
            Projects = await Context.Projects
                .AsNoTracking()
                .OrderBy(p => p.ProjectName)
                .ToListAsync();

            var query = Context.Payments
                .Include(p => p.Project)
                .ThenInclude(pr => pr.Customer)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(p => (p.PaymentReference != null && p.PaymentReference.ToLower().Contains(term)) ||
                                         (p.PaymentDescription != null && p.PaymentDescription.ToLower().Contains(term)) ||
                                         p.Project.ProjectCode.ToLower().Contains(term) ||
                                         p.Project.ProjectName.ToLower().Contains(term));
            }

            if (ProjectFilter.HasValue && ProjectFilter > 0)
                query = query.Where(p => p.ProjectId == ProjectFilter.Value);

            if (!string.IsNullOrWhiteSpace(StatusFilter))
                query = query.Where(p => p.PaymentStatus == StatusFilter);

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

            Payments = entities.Select(p => new PaymentViewModel
            {
                PaymentId = p.PaymentId,
                ProjectId = p.ProjectId,
                ProjectCode = p.Project?.ProjectCode ?? "Unknown",
                ProjectName = p.Project?.ProjectName ?? "Unknown",
                CustomerName = p.Project?.Customer?.CustomerName ?? "Unknown",
                PaymentReference = p.PaymentReference,
                PaymentDescription = p.PaymentDescription,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                DueDate = p.DueDate,
                PaymentMethod = p.PaymentMethod,
                PaymentStatus = p.PaymentStatus,
                Remark = p.Remark,
                CreatedDate = p.CreatedDate
            }).ToList();
        }
    }
}
