using System;
using System.Collections.Generic;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities;

public partial class RepositoryContext : DbContext
{
    public RepositoryContext()
    {
    }

    public RepositoryContext(DbContextOptions<RepositoryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseNpgsql("Host=localhost;Database=grievance_preprod;Username=postgres;Password=1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.Transid).HasName("user_login_pkey");

            entity.ToTable("user_login");

            entity.HasIndex(e => new { e.Pan, e.DeviceName }, "unique_pan_device_name").IsUnique();

            entity.Property(e => e.Transid)
                .UseIdentityAlwaysColumn()
                .HasColumnName("transid");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .HasDefaultValueSql("'000000'::character varying")
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_dt");
            entity.Property(e => e.DeviceName)
                .HasMaxLength(100)
                .HasColumnName("device_name");
            entity.Property(e => e.ExpireyDt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expirey_dt");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(136)
                .HasColumnName("ip_address");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(20)
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_dt");
            entity.Property(e => e.Otp)
                .HasMaxLength(6)
                .HasColumnName("otp");
            entity.Property(e => e.Pan)
                .HasMaxLength(10)
                .HasColumnName("pan");
            entity.Property(e => e.Roletype)
                .HasMaxLength(20)
                .HasColumnName("roletype");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
