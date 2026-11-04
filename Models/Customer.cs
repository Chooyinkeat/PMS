using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMS.Models;

[Table("customer")]
[Index("CreatedBy", Name = "FK_Customer_CreatedBy")]
[Index("EditedBy", Name = "FK_Customer_EditedBy")]
[Index("CustomerCode", Name = "UK_Customer_Code", IsUnique = true)]
public partial class Customer
{
    [Key]
    [Column("CustomerID", TypeName = "int(11)")]
    public int CustomerId { get; set; }

    [StringLength(50)]
    public string CustomerCode { get; set; } = null!;

    [StringLength(200)]
    public string CustomerName { get; set; } = null!;

    [StringLength(500)]
    public string? Remark { get; set; }

    [Required]
    public bool? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "int(11)")]
    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EditedDate { get; set; }

    [Column(TypeName = "int(11)")]
    public int? EditedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("CustomerCreatedByNavigations")]
    public virtual User? CreatedByNavigation { get; set; }

    [ForeignKey("EditedBy")]
    [InverseProperty("CustomerEditedByNavigations")]
    public virtual User? EditedByNavigation { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Personincharge> Personincharges { get; set; } = new List<Personincharge>();

    [InverseProperty("Customer")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
