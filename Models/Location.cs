using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMS.Models;

[Table("location")]
[Index("CreatedBy", Name = "FK_Location_CreatedBy")]
[Index("EditedBy", Name = "FK_Location_EditedBy")]
[Index("Picid", Name = "IX_Location_PICID")]
[Index("LocationCode", Name = "UK_Location_Code", IsUnique = true)]
public partial class Location
{
    [Key]
    [Column("LocationID", TypeName = "int(11)")]
    public int LocationId { get; set; }

    [StringLength(50)]
    public string LocationCode { get; set; } = null!;

    [StringLength(200)]
    public string LocationName { get; set; } = null!;

    [StringLength(500)]
    public string? FullAddress { get; set; }

    [Column("PICID", TypeName = "int(11)")]
    public int? Picid { get; set; }

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
    [InverseProperty("LocationCreatedByNavigations")]
    public virtual User? CreatedByNavigation { get; set; }

    [ForeignKey("EditedBy")]
    [InverseProperty("LocationEditedByNavigations")]
    public virtual User? EditedByNavigation { get; set; }

    [ForeignKey("Picid")]
    [InverseProperty("Locations")]
    public virtual Personincharge? Pic { get; set; }
}
