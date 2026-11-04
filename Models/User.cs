using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PMS.Models;

[Table("user")]
[Index("CreatedBy", Name = "FK_User_CreatedBy")]
[Index("EditedBy", Name = "FK_User_EditedBy")]
[Index("Email", Name = "UK_User_Email", IsUnique = true)]
[Index("UserName", Name = "UK_User_UserName", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID", TypeName = "int(11)")]
    public int UserId { get; set; }

    [StringLength(100)]
    public string UserName { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [Column("UserGroupID", TypeName = "int(11)")]
    public int? UserGroupId { get; set; }

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
    [InverseProperty("InverseCreatedByNavigation")]
    public virtual User? CreatedByNavigation { get; set; }

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Customer> CustomerCreatedByNavigations { get; set; } = new List<Customer>();

    [InverseProperty("EditedByNavigation")]
    public virtual ICollection<Customer> CustomerEditedByNavigations { get; set; } = new List<Customer>();

    [ForeignKey("EditedBy")]
    [InverseProperty("InverseEditedByNavigation")]
    public virtual User? EditedByNavigation { get; set; }

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<User> InverseCreatedByNavigation { get; set; } = new List<User>();

    [InverseProperty("EditedByNavigation")]
    public virtual ICollection<User> InverseEditedByNavigation { get; set; } = new List<User>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Location> LocationCreatedByNavigations { get; set; } = new List<Location>();

    [InverseProperty("EditedByNavigation")]
    public virtual ICollection<Location> LocationEditedByNavigations { get; set; } = new List<Location>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Payment> PaymentCreatedByNavigations { get; set; } = new List<Payment>();

    [InverseProperty("EditedByNavigation")]
    public virtual ICollection<Payment> PaymentEditedByNavigations { get; set; } = new List<Payment>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Personincharge> PersoninchargeCreatedByNavigations { get; set; } = new List<Personincharge>();

    [InverseProperty("EditedByNavigation")]
    public virtual ICollection<Personincharge> PersoninchargeEditedByNavigations { get; set; } = new List<Personincharge>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Project> ProjectCreatedByNavigations { get; set; } = new List<Project>();

    [InverseProperty("EditedByNavigation")]
    public virtual ICollection<Project> ProjectEditedByNavigations { get; set; } = new List<Project>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Projectmember> ProjectmemberCreatedByNavigations { get; set; } = new List<Projectmember>();

    [InverseProperty("EditedByNavigation")]
    public virtual ICollection<Projectmember> ProjectmemberEditedByNavigations { get; set; } = new List<Projectmember>();

    [InverseProperty("User")]
    public virtual ICollection<Projectmember> ProjectmemberUsers { get; set; } = new List<Projectmember>();
}
