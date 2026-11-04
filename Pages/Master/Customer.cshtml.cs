using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using PMS.Services;
using System.ComponentModel.DataAnnotations;

namespace PMS.Pages.Master
{
    public class CustomerModel : BasePmsPageModel
    {
        private readonly IPdfService _pdfService;

        public CustomerModel(ApplicationDbContext context, IPermissionService permissionService, IPdfService pdfService)
            : base(context, permissionService)
        {
            _pdfService = pdfService;
        }

        public List<CustomerViewModel> Customers { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty]
        public CustomerInputModel Input { get; set; } = new();

        public class CustomerViewModel
        {
            public int CustomerId { get; set; }
            public string CustomerCode { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string? Remark { get; set; }
            public bool Status { get; set; }
            public int PicCount { get; set; }
            public int ProjectCount { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public class CustomerInputModel
        {
            public int? CustomerId { get; set; }

            [Required(ErrorMessage = "Customer code is required.")]
            [StringLength(50, ErrorMessage = "Customer code cannot exceed 50 characters.")]
            public string CustomerCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Customer name is required.")]
            [StringLength(200, ErrorMessage = "Customer name cannot exceed 200 characters.")]
            public string CustomerName { get; set; } = string.Empty;

            [StringLength(500)]
            public string? Remark { get; set; }

            public bool Status { get; set; } = true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Customer, PermissionAction.View))
                return RedirectToAccessDenied(AppModules.Customer, PermissionAction.View);

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Customer, PermissionAction.Add))
                return RedirectToAccessDenied(AppModules.Customer, PermissionAction.Add);

            if (await Context.Customers.AnyAsync(c => c.CustomerCode.ToLower() == Input.CustomerCode.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.CustomerCode", "A customer with this code already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            var customer = new Customer
            {
                CustomerCode = Input.CustomerCode.Trim().ToUpper(),
                CustomerName = Input.CustomerName.Trim(),
                Remark = Input.Remark?.Trim(),
                Status = Input.Status,
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUserId > 0 ? CurrentUserId : null
            };

            Context.Customers.Add(customer);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"Customer '{customer.CustomerName}' created successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Customer, PermissionAction.Edit))
                return RedirectToAccessDenied(AppModules.Customer, PermissionAction.Edit);

            if (!Input.CustomerId.HasValue)
            {
                SetErrorMessage("Invalid customer ID.");
                return RedirectToPage();
            }

            var customer = await Context.Customers.FindAsync(Input.CustomerId.Value);
            if (customer == null)
            {
                SetErrorMessage("Customer not found.");
                return RedirectToPage();
            }

            if (await Context.Customers.AnyAsync(c => c.CustomerId != customer.CustomerId && c.CustomerCode.ToLower() == Input.CustomerCode.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.CustomerCode", "A customer with this code already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            customer.CustomerCode = Input.CustomerCode.Trim().ToUpper();
            customer.CustomerName = Input.CustomerName.Trim();
            customer.Remark = Input.Remark?.Trim();
            customer.Status = Input.Status;
            customer.EditedDate = DateTime.Now;
            customer.EditedBy = CurrentUserId > 0 ? CurrentUserId : null;

            await Context.SaveChangesAsync();
            SetSuccessMessage($"Customer '{customer.CustomerName}' updated successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!await CheckPermissionAsync(AppModules.Customer, PermissionAction.Delete))
                return RedirectToAccessDenied(AppModules.Customer, PermissionAction.Delete);

            var customer = await Context.Customers
                .Include(c => c.Projects)
                .Include(c => c.Personincharges)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                SetErrorMessage("Customer not found.");
                return RedirectToPage();
            }

            if (customer.Projects.Any())
            {
                SetErrorMessage($"Cannot delete customer '{customer.CustomerName}' because {customer.Projects.Count} project(s) are associated with it.");
                return RedirectToPage();
            }

            if (customer.Personincharges.Any())
            {
                SetErrorMessage($"Cannot delete customer '{customer.CustomerName}' because {customer.Personincharges.Count} person(s) in charge are linked to it.");
                return RedirectToPage();
            }

            Context.Customers.Remove(customer);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"Customer '{customer.CustomerName}' deleted successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Customer, PermissionAction.Print))
                return RedirectToAccessDenied(AppModules.Customer, PermissionAction.Print);

            var query = Context.Customers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(c => c.CustomerCode.ToLower().Contains(term) || c.CustomerName.ToLower().Contains(term));
            }

            if (StatusFilter == "active") query = query.Where(c => c.Status == true);
            else if (StatusFilter == "inactive") query = query.Where(c => c.Status == false);

            var customerList = await query
                .OrderBy(c => c.CustomerName)
                .Select(c => new
                {
                    c.CustomerCode,
                    c.CustomerName,
                    Remark = c.Remark ?? "None",
                    Status = c.Status == true ? "Active" : "Inactive",
                    Projects = Context.Projects.Count(p => p.CustomerId == c.CustomerId),
                    CreatedDate = c.CreatedDate.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            var columns = new List<PdfColumn<dynamic>>
            {
                new() { Header = "Customer Code", ValueSelector = c => c.CustomerCode, Width = 90f },
                new() { Header = "Customer Name", ValueSelector = c => c.CustomerName, Width = 180f },
                new() { Header = "Projects", ValueSelector = c => c.Projects.ToString(), Width = 55f, Alignment = TextAlignment.Center },
                new() { Header = "Status", ValueSelector = c => c.Status, Width = 60f, Alignment = TextAlignment.Center },
                new() { Header = "Created Date", ValueSelector = c => c.CreatedDate, Width = 85f, Alignment = TextAlignment.Center }
            };

            var summary = new Dictionary<string, string>
            {
                { "Total Customers", customerList.Count.ToString() },
                { "Active Customers", customerList.Count(c => c.Status == "Active").ToString() },
                { "Inactive Customers", customerList.Count(c => c.Status == "Inactive").ToString() }
            };

            var pdfBytes = _pdfService.GenerateListPdf(
                title: "Customers Directory Report",
                subtitle: $"Exported {DateTime.Now:yyyy-MM-dd HH:mm}",
                columns: columns,
                data: customerList,
                generatedBy: CurrentUserName,
                summaryItems: summary);

            return File(pdfBytes, "application/pdf", $"Customer_List_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private async Task LoadDataAsync()
        {
            var query = Context.Customers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(c => c.CustomerCode.ToLower().Contains(term) || c.CustomerName.ToLower().Contains(term));
            }

            if (StatusFilter == "active") query = query.Where(c => c.Status == true);
            else if (StatusFilter == "inactive") query = query.Where(c => c.Status == false);

            var totalCount = await query.CountAsync();

            Pagination = new PaginationDto
            {
                CurrentPage = CurrentPage < 1 ? 1 : CurrentPage,
                PageSize = PageSize < 1 ? 10 : PageSize,
                TotalItems = totalCount
            };

            var entities = await query
                .OrderBy(c => c.CustomerName)
                .Skip((Pagination.CurrentPage - 1) * Pagination.PageSize)
                .Take(Pagination.PageSize)
                .ToListAsync();

            var customerIds = entities.Select(c => c.CustomerId).ToList();

            var picCounts = await Context.Personincharges
                .Where(p => customerIds.Contains(p.CustomerId))
                .GroupBy(p => p.CustomerId)
                .Select(g => new { CustomerId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CustomerId, x => x.Count);

            var projectCounts = await Context.Projects
                .Where(p => customerIds.Contains(p.CustomerId))
                .GroupBy(p => p.CustomerId)
                .Select(g => new { CustomerId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CustomerId, x => x.Count);

            Customers = entities.Select(c => new CustomerViewModel
            {
                CustomerId = c.CustomerId,
                CustomerCode = c.CustomerCode,
                CustomerName = c.CustomerName,
                Remark = c.Remark,
                Status = c.Status == true,
                PicCount = picCounts.ContainsKey(c.CustomerId) ? picCounts[c.CustomerId] : 0,
                ProjectCount = projectCounts.ContainsKey(c.CustomerId) ? projectCounts[c.CustomerId] : 0,
                CreatedDate = c.CreatedDate
            }).ToList();
        }
    }
}
