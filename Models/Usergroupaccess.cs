using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMS.Models;

[Table("usergroupaccess")]
[Index("UgagroupId", "Ugamodule", Name = "UX_UGA_Group_Module", IsUnique = true)]
public partial class Usergroupaccess
{
    [Key]
    [Column("UGAID", TypeName = "int(11)")]
    public int Ugaid { get; set; }

    [Column("UGAGroupID", TypeName = "int(11)")]
    public int UgagroupId { get; set; }

    [Column("UGAModule")]
    [StringLength(100)]
    public string Ugamodule { get; set; } = null!;

    [Column("UGAView")]
    public bool Ugaview { get; set; }

    [Column("UGAAdd")]
    public bool Ugaadd { get; set; }

    [Column("UGAEdit")]
    public bool Ugaedit { get; set; }

    [Column("UGADelete")]
    public bool Ugadelete { get; set; }

    [Column("UGAPrint")]
    public bool Ugaprint { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EditedDate { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? EditedBy { get; set; }

    [ForeignKey("UgagroupId")]
    [InverseProperty("Usergroupaccesses")]
    public virtual Usergroup Ugagroup { get; set; } = null!;
}
