using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMS.Models;

[Table("projectmember")]
[Index("CreatedBy", Name = "FK_ProjectMember_CreatedBy")]
[Index("EditedBy", Name = "FK_ProjectMember_EditedBy")]
[Index("UserId", Name = "IX_ProjectMember_UserID")]
[Index("ProjectId", "UserId", Name = "UK_ProjectMember_Project_User", IsUnique = true)]
public partial class Projectmember
{
    [Key]
    [Column("ProjectMemberID", TypeName = "int(11)")]
    public int ProjectMemberId { get; set; }

    [Column("ProjectID", TypeName = "int(11)")]
    public int ProjectId { get; set; }

    [Column("UserID", TypeName = "int(11)")]
    public int UserId { get; set; }

    [StringLength(50)]
    public string MemberRole { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime AssignedDate { get; set; }

    [Required]
    public bool? Status { get; set; }

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
    [InverseProperty("ProjectmemberCreatedByNavigations")]
    public virtual User? CreatedByNavigation { get; set; }

    [ForeignKey("EditedBy")]
    [InverseProperty("ProjectmemberEditedByNavigations")]
    public virtual User? EditedByNavigation { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("Projectmembers")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ProjectmemberUsers")]
    public virtual User User { get; set; } = null!;
}
