using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMS.Models;

[Table("project")]
[Index("CreatedBy", Name = "FK_Project_CreatedBy")]
[Index("EditedBy", Name = "FK_Project_EditedBy")]
[Index("CustomerId", Name = "IX_Project_CustomerID")]
[Index("Status", Name = "IX_Project_Status")]
[Index("ProjectCode", Name = "UK_Project_Code", IsUnique = true)]
public partial class Project
{
    [Key]
    [Column("ProjectID", TypeName = "int(11)")]
    public int ProjectId { get; set; }

    [StringLength(50)]
    public string ProjectCode { get; set; } = null!;

    [StringLength(200)]
    public string ProjectName { get; set; } = null!;

    [Column("CustomerID", TypeName = "int(11)")]
    public int CustomerId { get; set; }

    [Precision(15, 2)]
    public decimal Price { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? DueDate { get; set; }

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? Remark { get; set; }

    [StringLength(30)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "int(11)")]
    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EditedDate { get; set; }

    [Column(TypeName = "int(11)")]
    public int? EditedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("ProjectCreatedByNavigations")]
    public virtual User? CreatedByNavigation { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Projects")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("EditedBy")]
    [InverseProperty("ProjectEditedByNavigations")]
    public virtual User? EditedByNavigation { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("Project")]
    public virtual ICollection<Projectmember> Projectmembers { get; set; } = new List<Projectmember>();
}
