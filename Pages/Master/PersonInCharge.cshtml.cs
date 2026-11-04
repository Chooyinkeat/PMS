using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using PMS.Services;
using System.ComponentModel.DataAnnotations;

namespace PMS.Pages.Master
{
    public class PersonInChargeModel : BasePmsPageModel
    {
        private readonly IPdfService _pdfService;

        public PersonInChargeModel(ApplicationDbContext context, IPermissionService permissionService, IPdfService pdfService)
            : base(context, permissionService)
        {
            _pdfService = pdfService;
        }

        public List<PicViewModel> Pics { get; set; } = new();
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
        public PicInputModel Input { get; set; } = new();

        public class PicViewModel
        {
            public int PicId { get; set; }
            public string PicCode { get; set; } = string.Empty;
            public string PicName { get; set; } = string.Empty;
            public string? PicRole { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string? Remark { get; set; }
            public bool Status { get; set; }
            public int LocationCount { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public class PicInputModel
        {
            public int? PicId { get; set; }

            [Required(ErrorMessage = "PIC code is required.")]
            [StringLength(50, ErrorMessage = "PIC code cannot exceed 50 characters.")]
            public string PicCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Name is required.")]
            [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
            public string PicName { get; set; } = string.Empty;

            [StringLength(100, ErrorMessage = "Role cannot exceed 100 characters.")]
            public string? PicRole { get; set; }

            [Required(ErrorMessage = "Customer is required.")]
            public int CustomerId { get; set; }

            [StringLength(500)]
            public string? Remark { get; set; }

            public bool Status { get; set; } = true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await CheckPermissionAsync(AppModules.PersonInCharge, PermissionAction.View))
                return RedirectToAccessDenied(AppModules.PersonInCharge, PermissionAction.View);

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!await CheckPermissionAsync(AppModules.PersonInCharge, PermissionAction.Add))
                return RedirectToAccessDenied(AppModules.PersonInCharge, PermissionAction.Add);

            if (await Context.Personincharges.AnyAsync(p => p.Piccode.ToLower() == Input.PicCode.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.PicCode", "A Person In Charge with this code already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            var pic = new Personincharge
            {
                Piccode = Input.PicCode.Trim().ToUpper(),
                Picname = Input.PicName.Trim(),
                Picrole = Input.PicRole?.Trim(),
                CustomerId = Input.CustomerId,
                Remark = Input.Remark?.Trim(),
                Status = Input.Status,
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUserId > 0 ? CurrentUserId : null
            };

            Context.Personincharges.Add(pic);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"Person In Charge '{pic.Picname}' created successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!await CheckPermissionAsync(AppModules.PersonInCharge, PermissionAction.Edit))
                return RedirectToAccessDenied(AppModules.PersonInCharge, PermissionAction.Edit);

            if (!Input.PicId.HasValue)
            {
                SetErrorMessage("Invalid PIC ID.");
                return RedirectToPage();
            }

            var pic = await Context.Personincharges.FindAsync(Input.PicId.Value);
            if (pic == null)
            {
                SetErrorMessage("Person In Charge not found.");
                return RedirectToPage();
            }

            if (await Context.Personincharges.AnyAsync(p => p.Picid != pic.Picid && p.Piccode.ToLower() == Input.PicCode.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.PicCode", "A Person In Charge with this code already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            pic.Piccode = Input.PicCode.Trim().ToUpper();
            pic.Picname = Input.PicName.Trim();
            pic.Picrole = Input.PicRole?.Trim();
            pic.CustomerId = Input.CustomerId;
            pic.Remark = Input.Remark?.Trim();
            pic.Status = Input.Status;
            pic.EditedDate = DateTime.Now;
            pic.EditedBy = CurrentUserId > 0 ? CurrentUserId : null;

            await Context.SaveChangesAsync();
            SetSuccessMessage($"Person In Charge '{pic.Picname}' updated successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!await CheckPermissionAsync(AppModules.PersonInCharge, PermissionAction.Delete))
                return RedirectToAccessDenied(AppModules.PersonInCharge, PermissionAction.Delete);

            var pic = await Context.Personincharges
                .Include(p => p.Locations)
                .FirstOrDefaultAsync(p => p.Picid == id);

            if (pic == null)
            {
                SetErrorMessage("Person In Charge not found.");
                return RedirectToPage();
            }

            if (pic.Locations.Any())
            {
                SetErrorMessage($"Cannot delete PIC '{pic.Picname}' because {pic.Locations.Count} location(s) are assigned to them.");
                return RedirectToPage();
            }

            Context.Personincharges.Remove(pic);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"Person In Charge '{pic.Picname}' deleted successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            if (!await CheckPermissionAsync(AppModules.PersonInCharge, PermissionAction.Print))
                return RedirectToAccessDenied(AppModules.PersonInCharge, PermissionAction.Print);

            var query = Context.Personincharges
                .Include(p => p.Customer)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(p => p.Piccode.ToLower().Contains(term) ||
                                         p.Picname.ToLower().Contains(term) ||
                                         (p.Picrole != null && p.Picrole.ToLower().Contains(term)));
            }

            if (CustomerFilter.HasValue && CustomerFilter > 0)
                query = query.Where(p => p.CustomerId == CustomerFilter.Value);

            if (StatusFilter == "active") query = query.Where(p => p.Status == true);
            else if (StatusFilter == "inactive") query = query.Where(p => p.Status == false);

            var picList = await query
                .OrderBy(p => p.Picname)
                .Select(p => new
                {
                    p.Piccode,
                    p.Picname,
                    Role = p.Picrole ?? "Representative",
                    Customer = p.Customer.CustomerName,
                    Status = p.Status == true ? "Active" : "Inactive",
                    CreatedDate = p.CreatedDate.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            var columns = new List<PdfColumn<dynamic>>
            {
                new() { Header = "PIC Code", ValueSelector = p => p.Piccode, Width = 80f },
                new() { Header = "Full Name", ValueSelector = p => p.Picname, Width = 130f },
                new() { Header = "Designation / Role", ValueSelector = p => p.Role, Width = 110f },
                new() { Header = "Representing Customer", ValueSelector = p => p.Customer, Width = 140f },
                new() { Header = "Status", ValueSelector = p => p.Status, Width = 55f, Alignment = TextAlignment.Center }
            };

            var summary = new Dictionary<string, string>
            {
                { "Total Contacts", picList.Count.ToString() },
                { "Active Contacts", picList.Count(p => p.Status == "Active").ToString() },
                { "Represented Clients", picList.Select(p => (string)p.Customer).Distinct().Count().ToString() }
            };

            var pdfBytes = _pdfService.GenerateListPdf(
                title: "Person In Charge (PIC) Directory",
                subtitle: $"Exported {DateTime.Now:yyyy-MM-dd HH:mm}",
                columns: columns,
                data: picList,
                generatedBy: CurrentUserName,
                summaryItems: summary);

            return File(pdfBytes, "application/pdf", $"PIC_Directory_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private async Task LoadDataAsync()
        {
            Customers = await Context.Customers
                .AsNoTracking()
                .Where(c => c.Status == true)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();

            var query = Context.Personincharges
                .Include(p => p.Customer)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(p => p.Piccode.ToLower().Contains(term) ||
                                         p.Picname.ToLower().Contains(term) ||
                                         (p.Picrole != null && p.Picrole.ToLower().Contains(term)));
            }

            if (CustomerFilter.HasValue && CustomerFilter > 0)
                query = query.Where(p => p.CustomerId == CustomerFilter.Value);

            if (StatusFilter == "active") query = query.Where(p => p.Status == true);
            else if (StatusFilter == "inactive") query = query.Where(p => p.Status == false);

            var totalCount = await query.CountAsync();

            Pagination = new PaginationDto
            {
                CurrentPage = CurrentPage < 1 ? 1 : CurrentPage,
                PageSize = PageSize < 1 ? 10 : PageSize,
                TotalItems = totalCount
            };

            var entities = await query
                .OrderBy(p => p.Picname)
                .Skip((Pagination.CurrentPage - 1) * Pagination.PageSize)
                .Take(Pagination.PageSize)
                .ToListAsync();

            var picIds = entities.Select(p => p.Picid).ToList();
            var locCounts = await Context.Locations
                .Where(l => l.Picid != null && picIds.Contains(l.Picid.Value))
                .GroupBy(l => l.Picid!.Value)
                .Select(g => new { PicId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PicId, x => x.Count);

            Pics = entities.Select(p => new PicViewModel
            {
                PicId = p.Picid,
                PicCode = p.Piccode,
                PicName = p.Picname,
                PicRole = p.Picrole,
                CustomerId = p.CustomerId,
                CustomerName = p.Customer?.CustomerName ?? "Unknown",
                Remark = p.Remark,
                Status = p.Status == true,
                LocationCount = locCounts.ContainsKey(p.Picid) ? locCounts[p.Picid] : 0,
                CreatedDate = p.CreatedDate
            }).ToList();
        }
    }
}
