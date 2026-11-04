using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using PMS.Services;
using System.ComponentModel.DataAnnotations;

namespace PMS.Pages.Master
{
    public class LocationModel : BasePmsPageModel
    {
        private readonly IPdfService _pdfService;

        public LocationModel(ApplicationDbContext context, IPermissionService permissionService, IPdfService pdfService)
            : base(context, permissionService)
        {
            _pdfService = pdfService;
        }

        public List<LocationViewModel> Locations { get; set; } = new();
        public List<Personincharge> Pics { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PicFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty]
        public LocationInputModel Input { get; set; } = new();

        public class LocationViewModel
        {
            public int LocationId { get; set; }
            public string LocationCode { get; set; } = string.Empty;
            public string LocationName { get; set; } = string.Empty;
            public string? FullAddress { get; set; }
            public int? PicId { get; set; }
            public string PicName { get; set; } = "None";
            public string CustomerName { get; set; } = "None";
            public string? Remark { get; set; }
            public bool Status { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public class LocationInputModel
        {
            public int? LocationId { get; set; }

            [Required(ErrorMessage = "Location code is required.")]
            [StringLength(50, ErrorMessage = "Location code cannot exceed 50 characters.")]
            public string LocationCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Location name is required.")]
            [StringLength(200, ErrorMessage = "Location name cannot exceed 200 characters.")]
            public string LocationName { get; set; } = string.Empty;

            [StringLength(500)]
            public string? FullAddress { get; set; }

            public int? PicId { get; set; }

            [StringLength(500)]
            public string? Remark { get; set; }

            public bool Status { get; set; } = true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Location, PermissionAction.View))
                return RedirectToAccessDenied(AppModules.Location, PermissionAction.View);

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Location, PermissionAction.Add))
                return RedirectToAccessDenied(AppModules.Location, PermissionAction.Add);

            if (await Context.Locations.AnyAsync(l => l.LocationCode.ToLower() == Input.LocationCode.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.LocationCode", "A location with this code already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            var loc = new Location
            {
                LocationCode = Input.LocationCode.Trim().ToUpper(),
                LocationName = Input.LocationName.Trim(),
                FullAddress = Input.FullAddress?.Trim(),
                Picid = Input.PicId,
                Remark = Input.Remark?.Trim(),
                Status = Input.Status,
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUserId > 0 ? CurrentUserId : null
            };

            Context.Locations.Add(loc);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"Location '{loc.LocationName}' created successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Location, PermissionAction.Edit))
                return RedirectToAccessDenied(AppModules.Location, PermissionAction.Edit);

            if (!Input.LocationId.HasValue)
            {
                SetErrorMessage("Invalid location ID.");
                return RedirectToPage();
            }

            var loc = await Context.Locations.FindAsync(Input.LocationId.Value);
            if (loc == null)
            {
                SetErrorMessage("Location not found.");
                return RedirectToPage();
            }

            if (await Context.Locations.AnyAsync(l => l.LocationId != loc.LocationId && l.LocationCode.ToLower() == Input.LocationCode.Trim().ToLower()))
            {
                ModelState.AddModelError("Input.LocationCode", "A location with this code already exists.");
            }

            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please correct the errors in the form.");
                await LoadDataAsync();
                return Page();
            }

            loc.LocationCode = Input.LocationCode.Trim().ToUpper();
            loc.LocationName = Input.LocationName.Trim();
            loc.FullAddress = Input.FullAddress?.Trim();
            loc.Picid = Input.PicId;
            loc.Remark = Input.Remark?.Trim();
            loc.Status = Input.Status;
            loc.EditedDate = DateTime.Now;
            loc.EditedBy = CurrentUserId > 0 ? CurrentUserId : null;

            await Context.SaveChangesAsync();
            SetSuccessMessage($"Location '{loc.LocationName}' updated successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!await CheckPermissionAsync(AppModules.Location, PermissionAction.Delete))
                return RedirectToAccessDenied(AppModules.Location, PermissionAction.Delete);

            var loc = await Context.Locations.FindAsync(id);
            if (loc == null)
            {
                SetErrorMessage("Location not found.");
                return RedirectToPage();
            }

            Context.Locations.Remove(loc);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"Location '{loc.LocationName}' deleted successfully.");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Location, PermissionAction.Print))
                return RedirectToAccessDenied(AppModules.Location, PermissionAction.Print);

            var query = Context.Locations
                .Include(l => l.Pic)
                .ThenInclude(p => p!.Customer)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(l => l.LocationCode.ToLower().Contains(term) ||
                                         l.LocationName.ToLower().Contains(term) ||
                                         (l.FullAddress != null && l.FullAddress.ToLower().Contains(term)));
            }

            if (PicFilter.HasValue && PicFilter > 0)
                query = query.Where(l => l.Picid == PicFilter.Value);

            if (StatusFilter == "active") query = query.Where(l => l.Status == true);
            else if (StatusFilter == "inactive") query = query.Where(l => l.Status == false);

            var locList = await query
                .OrderBy(l => l.LocationName)
                .Select(l => new
                {
                    l.LocationCode,
                    l.LocationName,
                    Address = l.FullAddress ?? "Not specified",
                    Pic = l.Pic != null ? l.Pic.Picname : "Unassigned",
                    Status = l.Status == true ? "Active" : "Inactive",
                    CreatedDate = l.CreatedDate.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            var columns = new List<PdfColumn<dynamic>>
            {
                new() { Header = "Code", ValueSelector = l => l.LocationCode, Width = 70f },
                new() { Header = "Location / Site", ValueSelector = l => l.LocationName, Width = 130f },
                new() { Header = "Address", ValueSelector = l => l.Address, Width = 160f },
                new() { Header = "PIC Contact", ValueSelector = l => l.Pic, Width = 100f },
                new() { Header = "Status", ValueSelector = l => l.Status, Width = 55f, Alignment = TextAlignment.Center }
            };

            var summary = new Dictionary<string, string>
            {
                { "Total Locations", locList.Count.ToString() },
                { "Active Sites", locList.Count(l => l.Status == "Active").ToString() },
                { "Inactive Sites", locList.Count(l => l.Status == "Inactive").ToString() }
            };

            var pdfBytes = _pdfService.GenerateListPdf(
                title: "Physical Locations & Sites Report",
                subtitle: $"Exported {DateTime.Now:yyyy-MM-dd HH:mm}",
                columns: columns,
                data: locList,
                generatedBy: CurrentUserName,
                summaryItems: summary);

            return File(pdfBytes, "application/pdf", $"Locations_Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private async Task LoadDataAsync()
        {
            Pics = await Context.Personincharges
                .Include(p => p.Customer)
                .AsNoTracking()
                .Where(p => p.Status == true)
                .OrderBy(p => p.Picname)
                .ToListAsync();

            var query = Context.Locations
                .Include(l => l.Pic)
                .ThenInclude(p => p!.Customer)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(l => l.LocationCode.ToLower().Contains(term) ||
                                         l.LocationName.ToLower().Contains(term) ||
                                         (l.FullAddress != null && l.FullAddress.ToLower().Contains(term)));
            }

            if (PicFilter.HasValue && PicFilter > 0)
                query = query.Where(l => l.Picid == PicFilter.Value);

            if (StatusFilter == "active") query = query.Where(l => l.Status == true);
            else if (StatusFilter == "inactive") query = query.Where(l => l.Status == false);

            var totalCount = await query.CountAsync();

            Pagination = new PaginationDto
            {
                CurrentPage = CurrentPage < 1 ? 1 : CurrentPage,
                PageSize = PageSize < 1 ? 10 : PageSize,
                TotalItems = totalCount
            };

            var entities = await query
                .OrderBy(l => l.LocationName)
                .Skip((Pagination.CurrentPage - 1) * Pagination.PageSize)
                .Take(Pagination.PageSize)
                .ToListAsync();

            Locations = entities.Select(l => new LocationViewModel
            {
                LocationId = l.LocationId,
                LocationCode = l.LocationCode,
                LocationName = l.LocationName,
                FullAddress = l.FullAddress,
                PicId = l.Picid,
                PicName = l.Pic?.Picname ?? "Unassigned",
                CustomerName = l.Pic?.Customer?.CustomerName ?? "None",
                Remark = l.Remark,
                Status = l.Status == true,
                CreatedDate = l.CreatedDate
            }).ToList();
        }
    }
}
