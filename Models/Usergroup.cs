using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMS.Models;

[Table("usergroup")]
public partial class Usergroup
{
    [Key]
    [Column("UserGroupID", TypeName = "int(11)")]
    public int UserGroupId { get; set; }

    [StringLength(100)]
    public string UserGroupName { get; set; } = null!;

    [StringLength(255)]
    public string? UserGroupDescription { get; set; }

    [Required]
    public bool? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EditedDate { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? EditedBy { get; set; }

    [InverseProperty("Ugagroup")]
    public virtual ICollection<Usergroupaccess> Usergroupaccesses { get; set; } = new List<Usergroupaccess>();
}
