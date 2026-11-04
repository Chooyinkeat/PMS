using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMS.Models;

[Table("payment")]
[Index("CreatedBy", Name = "FK_Payment_CreatedBy")]
[Index("EditedBy", Name = "FK_Payment_EditedBy")]
[Index("ProjectId", Name = "IX_Payment_ProjectID")]
[Index("PaymentStatus", Name = "IX_Payment_Status")]
[Index("PaymentReference", Name = "UK_Payment_Reference", IsUnique = true)]
public partial class Payment
{
    [Key]
    [Column("PaymentID", TypeName = "int(11)")]
    public int PaymentId { get; set; }

    [Column("ProjectID", TypeName = "int(11)")]
    public int ProjectId { get; set; }

    [StringLength(100)]
    public string? PaymentReference { get; set; }

    [StringLength(500)]
    public string? PaymentDescription { get; set; }

    [Precision(15, 2)]
    public decimal Amount { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public DateOnly? DueDate { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [StringLength(30)]
    public string PaymentStatus { get; set; } = null!;

    [StringLength(500)]
    public string? Remark { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "int(11)")]
    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EditedDate { get; set; }

    [Column(TypeName = "int(11)")]
    public int? EditedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("PaymentCreatedByNavigations")]
    public virtual User? CreatedByNavigation { get; set; }

    [ForeignKey("EditedBy")]
    [InverseProperty("PaymentEditedByNavigations")]
    public virtual User? EditedByNavigation { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("Payments")]
    public virtual Project Project { get; set; } = null!;
}
