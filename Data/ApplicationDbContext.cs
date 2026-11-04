using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PMS.Models;

namespace PMS.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Personincharge> Personincharges { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Projectmember> Projectmembers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Usergroup> Usergroups { get; set; }

    public virtual DbSet<Usergroupaccess> Usergroupaccesses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PRIMARY");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.Status).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerCreatedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Customer_CreatedBy");

            entity.HasOne(d => d.EditedByNavigation).WithMany(p => p.CustomerEditedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Customer_EditedBy");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PRIMARY");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.Status).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.LocationCreatedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Location_CreatedBy");

            entity.HasOne(d => d.EditedByNavigation).WithMany(p => p.LocationEditedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Location_EditedBy");

            entity.HasOne(d => d.Pic).WithMany(p => p.Locations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Location_PIC");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.PaymentStatus).HasDefaultValueSql("'Pending'");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PaymentCreatedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Payment_CreatedBy");

            entity.HasOne(d => d.EditedByNavigation).WithMany(p => p.PaymentEditedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Payment_EditedBy");

            entity.HasOne(d => d.Project).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Project");
        });

        modelBuilder.Entity<Personincharge>(entity =>
        {
            entity.HasKey(e => e.Picid).HasName("PRIMARY");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.Status).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PersoninchargeCreatedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PIC_CreatedBy");

            entity.HasOne(d => d.Customer).WithMany(p => p.Personincharges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PIC_Customer");

            entity.HasOne(d => d.EditedByNavigation).WithMany(p => p.PersoninchargeEditedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PIC_EditedBy");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PRIMARY");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.Status).HasDefaultValueSql("'Pending'");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectCreatedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Project_CreatedBy");

            entity.HasOne(d => d.Customer).WithMany(p => p.Projects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Project_Customer");

            entity.HasOne(d => d.EditedByNavigation).WithMany(p => p.ProjectEditedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Project_EditedBy");
        });

        modelBuilder.Entity<Projectmember>(entity =>
        {
            entity.HasKey(e => e.ProjectMemberId).HasName("PRIMARY");

            entity.Property(e => e.AssignedDate).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.MemberRole).HasDefaultValueSql("'Member'");
            entity.Property(e => e.Status).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectmemberCreatedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ProjectMember_CreatedBy");

            entity.HasOne(d => d.EditedByNavigation).WithMany(p => p.ProjectmemberEditedByNavigations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ProjectMember_EditedBy");

            entity.HasOne(d => d.Project).WithMany(p => p.Projectmembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectMember_Project");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectmemberUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectMember_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.Status).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_User_CreatedBy");

            entity.HasOne(d => d.EditedByNavigation).WithMany(p => p.InverseEditedByNavigation)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_User_EditedBy");
        });

        modelBuilder.Entity<Usergroup>(entity =>
        {
            entity.HasKey(e => e.UserGroupId).HasName("PRIMARY");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("current_timestamp()");
            entity.Property(e => e.Status).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<Usergroupaccess>(entity =>
        {
            entity.HasKey(e => e.Ugaid).HasName("PRIMARY");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.Ugagroup).WithMany(p => p.Usergroupaccesses).HasConstraintName("FK_UGA_UserGroup");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
