using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Models;
using PMS.Models.Common;
using PMS.Services;
using System.ComponentModel.DataAnnotations;

namespace PMS.Pages.Project
{
    public class DetailModel : BasePmsPageModel
    {
        private readonly IPdfService _pdfService;

        public DetailModel(ApplicationDbContext context, IPermissionService permissionService, IPdfService pdfService)
            : base(context, permissionService)
        {
            _pdfService = pdfService;
        }

        public ProjectDetailViewModel Project { get; set; } = null!;
        public List<User> AvailableUsers { get; set; } = new();

        [BindProperty]
        public AssignMemberInputModel MemberInput { get; set; } = new();

        [BindProperty]
        public ProjectPaymentInputModel PaymentInput { get; set; } = new();

        public class ProjectDetailViewModel
        {
            public int ProjectId { get; set; }
            public string ProjectCode { get; set; } = string.Empty;
            public string ProjectName { get; set; } = string.Empty;
            public int CustomerId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerCode { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public DateOnly? StartDate { get; set; }
            public DateOnly? DueDate { get; set; }
            public string? Description { get; set; }
            public string? Remark { get; set; }
            public string Status { get; set; } = "Pending";
            public DateTime CreatedDate { get; set; }
            public decimal TotalPaid { get; set; }
            public decimal RemainingBalance => Math.Max(0, Price - TotalPaid);
            public int PaymentPercent => Price > 0 ? (int)Math.Min(100, Math.Round((TotalPaid / Price) * 100)) : 0;
            public List<ProjectMemberItemViewModel> Members { get; set; } = new();
            public List<ProjectPaymentItemViewModel> Payments { get; set; } = new();
        }

        public class ProjectMemberItemViewModel
        {
            public int ProjectMemberId { get; set; }
            public int UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string MemberRole { get; set; } = "Member";
            public DateTime AssignedDate { get; set; }
            public bool Status { get; set; }
            public string? Remark { get; set; }
        }

        public class ProjectPaymentItemViewModel
        {
            public int PaymentId { get; set; }
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

        public class AssignMemberInputModel
        {
            public int ProjectId { get; set; }

            [Required(ErrorMessage = "Please select a user to assign.")]
            public int UserId { get; set; }

            [Required(ErrorMessage = "Please specify a team role.")]
            [StringLength(50)]
            public string MemberRole { get; set; } = "Member";

            [StringLength(500)]
            public string? Remark { get; set; }
        }

        public class ProjectPaymentInputModel
        {
            public int ProjectId { get; set; }
            public int? PaymentId { get; set; }

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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!await CheckPermissionAsync(AppModules.Project, PermissionAction.View))
                return RedirectToAccessDenied(AppModules.Project, PermissionAction.View);

            var ok = await LoadProjectAsync(id);
            if (!ok)
            {
                SetErrorMessage("Project not found.");
                return RedirectToPage("./List");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAssignMemberAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Project, PermissionAction.Edit))
                return RedirectToAccessDenied(AppModules.Project, PermissionAction.Edit);

            var project = await Context.Projects.FindAsync(MemberInput.ProjectId);
            if (project == null)
            {
                SetErrorMessage("Project not found.");
                return RedirectToPage("./List");
            }

            // Check if already assigned
            if (await Context.Projectmembers.AnyAsync(pm => pm.ProjectId == MemberInput.ProjectId && pm.UserId == MemberInput.UserId))
            {
                SetErrorMessage("This user is already a member of this project team.");
                return RedirectToPage(new { id = MemberInput.ProjectId });
            }

            var member = new Projectmember
            {
                ProjectId = MemberInput.ProjectId,
                UserId = MemberInput.UserId,
                MemberRole = MemberInput.MemberRole.Trim(),
                Remark = MemberInput.Remark?.Trim(),
                AssignedDate = DateTime.Now,
                Status = true,
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUserId > 0 ? CurrentUserId : null
            };

            Context.Projectmembers.Add(member);
            await Context.SaveChangesAsync();

            SetSuccessMessage("Team member assigned successfully.");
            return RedirectToPage(new { id = MemberInput.ProjectId });
        }

        public async Task<IActionResult> OnPostRemoveMemberAsync(int memberId, int projectId)
        {
            if (!await CheckPermissionAsync(AppModules.Project, PermissionAction.Edit))
                return RedirectToAccessDenied(AppModules.Project, PermissionAction.Edit);

            var member = await Context.Projectmembers.FindAsync(memberId);
            if (member != null && member.ProjectId == projectId)
            {
                Context.Projectmembers.Remove(member);
                await Context.SaveChangesAsync();
                SetSuccessMessage("Team member removed from project.");
            }

            return RedirectToPage(new { id = projectId });
        }

        public async Task<IActionResult> OnPostAddPaymentAsync()
        {
            if (!await CheckPermissionAsync(AppModules.Payment, PermissionAction.Add))
                return RedirectToAccessDenied(AppModules.Payment, PermissionAction.Add);

            var project = await Context.Projects.FindAsync(PaymentInput.ProjectId);
            if (project == null)
            {
                SetErrorMessage("Project not found.");
                return RedirectToPage("./List");
            }

            if (!string.IsNullOrWhiteSpace(PaymentInput.PaymentReference))
            {
                var refCode = PaymentInput.PaymentReference.Trim();
                if (await Context.Payments.AnyAsync(p => p.PaymentReference != null && p.PaymentReference.ToLower() == refCode.ToLower()))
                {
                    SetErrorMessage($"Payment reference '{refCode}' is already registered.");
                    return RedirectToPage(new { id = PaymentInput.ProjectId });
                }
            }
            else
            {
                // Auto-generate reference
                PaymentInput.PaymentReference = $"INV-{project.ProjectCode}-{DateTime.Now:yyyyMMdd}-{DateTime.Now.Millisecond:D3}";
            }

            var payment = new Payment
            {
                ProjectId = PaymentInput.ProjectId,
                PaymentReference = PaymentInput.PaymentReference,
                PaymentDescription = PaymentInput.PaymentDescription.Trim(),
                Amount = PaymentInput.Amount,
                PaymentDate = PaymentInput.PaymentDate,
                DueDate = PaymentInput.DueDate,
                PaymentMethod = PaymentInput.PaymentMethod,
                PaymentStatus = PaymentInput.PaymentStatus,
                Remark = PaymentInput.Remark?.Trim(),
                CreatedDate = DateTime.Now,
                CreatedBy = CurrentUserId > 0 ? CurrentUserId : null
            };

            Context.Payments.Add(payment);
            await Context.SaveChangesAsync();

            SetSuccessMessage($"Payment record '{payment.PaymentReference}' recorded successfully.");
            return RedirectToPage(new { id = PaymentInput.ProjectId });
        }

        public async Task<IActionResult> OnPostUpdatePaymentStatusAsync(int paymentId, int projectId, string newStatus)
        {
            if (!await CheckPermissionAsync(AppModules.Payment, PermissionAction.Edit))
                return RedirectToAccessDenied(AppModules.Payment, PermissionAction.Edit);

            var payment = await Context.Payments.FindAsync(paymentId);
            if (payment != null && payment.ProjectId == projectId)
            {
                payment.PaymentStatus = newStatus;
                if (newStatus == "Paid" && !payment.PaymentDate.HasValue)
                {
                    payment.PaymentDate = DateOnly.FromDateTime(DateTime.Today);
                }
                payment.EditedDate = DateTime.Now;
                payment.EditedBy = CurrentUserId > 0 ? CurrentUserId : null;
                await Context.SaveChangesAsync();

                SetSuccessMessage($"Payment status updated to '{newStatus}'.");
            }

            return RedirectToPage(new { id = projectId });
        }

        public async Task<IActionResult> OnGetExportDossierPdfAsync(int id)
        {
            if (!await CheckPermissionAsync(AppModules.Project, PermissionAction.Print))
                return RedirectToAccessDenied(AppModules.Project, PermissionAction.Print);

            var ok = await LoadProjectAsync(id);
            if (!ok)
            {
                SetErrorMessage("Project not found.");
                return RedirectToPage("./List");
            }

            // Assemble field groups for PDF Dossier
            var generalInfo = new PdfFieldGroup
            {
                Title = "Project Overview",
                Fields = new List<KeyValuePair<string, string>>
                {
                    new("Project Code", Project.ProjectCode),
                    new("Project Title", Project.ProjectName),
                    new("Status", Project.Status),
                    new("Registration Date", Project.CreatedDate.ToString("yyyy-MM-dd HH:mm"))
                }
            };

            var customerInfo = new PdfFieldGroup
            {
                Title = "Customer / Client Details",
                Fields = new List<KeyValuePair<string, string>>
                {
                    new("Customer Name", Project.CustomerName),
                    new("Customer Code", Project.CustomerCode),
                    new("Start Date", Project.StartDate.HasValue ? Project.StartDate.Value.ToString("yyyy-MM-dd") : "Not set"),
                    new("Target Due Date", Project.DueDate.HasValue ? Project.DueDate.Value.ToString("yyyy-MM-dd") : "Not set")
                }
            };

            var financialInfo = new PdfFieldGroup
            {
                Title = "Financial Summary",
                Fields = new List<KeyValuePair<string, string>>
                {
                    new("Contract Value", Project.Price.ToString("C2")),
                    new("Payments Received", Project.TotalPaid.ToString("C2")),
                    new("Balance Outstanding", Project.RemainingBalance.ToString("C2")),
                    new("Collection Rate", $"{Project.PaymentPercent}%")
                }
            };

            if (!string.IsNullOrWhiteSpace(Project.Description))
            {
                generalInfo.Fields.Add(new("Scope / Deliverables", Project.Description));
            }
            if (!string.IsNullOrWhiteSpace(Project.Remark))
            {
                generalInfo.Fields.Add(new("Internal Notes", Project.Remark));
            }

            // Tables: Team Members & Payments
            var tables = new List<PdfDetailTable>();

            // Team Table
            var memberRows = Project.Members.Select(m => new List<string>
            {
                m.UserName,
                m.Email,
                m.MemberRole,
                m.AssignedDate.ToString("yyyy-MM-dd"),
                m.Status ? "Active" : "Inactive"
            }).ToList();

            tables.Add(new PdfDetailTable
            {
                Title = "Assigned Team Members",
                Headers = new List<string> { "Member Name", "Email", "Role / Designation", "Assigned Date", "Status" },
                ColumnWidths = new List<float> { 120f, 150f, 110f, 80f, 60f },
                Alignments = new List<TextAlignment> { TextAlignment.Left, TextAlignment.Left, TextAlignment.Left, TextAlignment.Center, TextAlignment.Center },
                Rows = memberRows
            });

            // Payments Table
            var paymentRows = Project.Payments.Select(p => new List<string>
            {
                p.PaymentReference ?? "N/A",
                p.PaymentDescription ?? "N/A",
                p.Amount.ToString("C2"),
                p.PaymentMethod ?? "N/A",
                p.DueDate.HasValue ? p.DueDate.Value.ToString("yyyy-MM-dd") : "-",
                p.PaymentStatus
            }).ToList();

            tables.Add(new PdfDetailTable
            {
                Title = "Project Invoicing & Payment History",
                Headers = new List<string> { "Reference #", "Description", "Amount", "Method", "Due Date", "Status" },
                ColumnWidths = new List<float> { 90f, 160f, 80f, 80f, 65f, 65f },
                Alignments = new List<TextAlignment> { TextAlignment.Left, TextAlignment.Left, TextAlignment.Right, TextAlignment.Left, TextAlignment.Center, TextAlignment.Center },
                Rows = paymentRows
            });

            var pdfBytes = _pdfService.GenerateRecordDetailsPdf(
                title: "Official Project Dossier",
                recordCode: Project.ProjectCode,
                recordName: Project.ProjectName,
                fieldGroups: new List<PdfFieldGroup> { generalInfo, customerInfo, financialInfo },
                tables: tables,
                generatedBy: CurrentUserName);

            return File(pdfBytes, "application/pdf", $"Project_Dossier_{Project.ProjectCode}_{DateTime.Now:yyyyMMdd}.pdf");
        }

        private async Task<bool> LoadProjectAsync(int id)
        {
            var p = await Context.Projects
                .Include(p => p.Customer)
                .Include(p => p.Projectmembers)
                    .ThenInclude(pm => pm.User)
                .Include(p => p.Payments)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (p == null) return false;

            var paid = p.Payments.Where(pm => pm.PaymentStatus == "Paid").Sum(pm => pm.Amount);

            Project = new ProjectDetailViewModel
            {
                ProjectId = p.ProjectId,
                ProjectCode = p.ProjectCode,
                ProjectName = p.ProjectName,
                CustomerId = p.CustomerId,
                CustomerName = p.Customer?.CustomerName ?? "Unknown",
                CustomerCode = p.Customer?.CustomerCode ?? "Unknown",
                Price = p.Price,
                StartDate = p.StartDate,
                DueDate = p.DueDate,
                Description = p.Description,
                Remark = p.Remark,
                Status = p.Status,
                CreatedDate = p.CreatedDate,
                TotalPaid = paid,
                Members = p.Projectmembers.OrderBy(m => m.MemberRole).Select(m => new ProjectMemberItemViewModel
                {
                    ProjectMemberId = m.ProjectMemberId,
                    UserId = m.UserId,
                    UserName = m.User?.UserName ?? "Unknown",
                    Email = m.User?.Email ?? "",
                    MemberRole = m.MemberRole,
                    AssignedDate = m.AssignedDate,
                    Status = m.Status == true,
                    Remark = m.Remark
                }).ToList(),
                Payments = p.Payments.OrderByDescending(pm => pm.CreatedDate).Select(pm => new ProjectPaymentItemViewModel
                {
                    PaymentId = pm.PaymentId,
                    PaymentReference = pm.PaymentReference,
                    PaymentDescription = pm.PaymentDescription,
                    Amount = pm.Amount,
                    PaymentDate = pm.PaymentDate,
                    DueDate = pm.DueDate,
                    PaymentMethod = pm.PaymentMethod,
                    PaymentStatus = pm.PaymentStatus,
                    Remark = pm.Remark,
                    CreatedDate = pm.CreatedDate
                }).ToList()
            };

            var assignedUserIds = Project.Members.Select(m => m.UserId).ToList();
            AvailableUsers = await Context.Users
                .AsNoTracking()
                .Where(u => u.Status == true && !assignedUserIds.Contains(u.UserId))
                .OrderBy(u => u.UserName)
                .ToListAsync();

            return true;
        }
    }
}
