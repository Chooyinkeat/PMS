using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMS.Models;

[Table("personincharge")]
[Index("CreatedBy", Name = "FK_PIC_CreatedBy")]
[Index("EditedBy", Name = "FK_PIC_EditedBy")]
[Index("CustomerId", Name = "IX_PIC_CustomerID")]
[Index("Piccode", Name = "UK_PIC_Code", IsUnique = true)]
public partial class Personincharge
{
    [Key]
    [Column("PICID", TypeName = "int(11)")]
    public int Picid { get; set; }

    [Column("PICCode")]
    [StringLength(50)]
    public string Piccode { get; set; } = null!;

    [Column("PICName")]
    [StringLength(150)]
    public string Picname { get; set; } = null!;

    [Column("PICRole")]
    [StringLength(100)]
    public string? Picrole { get; set; }

    [Column("CustomerID", TypeName = "int(11)")]
    public int CustomerId { get; set; }

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
    [InverseProperty("PersoninchargeCreatedByNavigations")]
    public virtual User? CreatedByNavigation { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Personincharges")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("EditedBy")]
    [InverseProperty("PersoninchargeEditedByNavigations")]
    public virtual User? EditedByNavigation { get; set; }

    [InverseProperty("Pic")]
    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}
