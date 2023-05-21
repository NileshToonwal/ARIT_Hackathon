using System;
using System.Collections.Generic;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities;

public partial class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions<RepositoryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<category_master> category_master { get; set; }

    public virtual DbSet<cfg_codevalue> cfg_codevalue { get; set; }

    public virtual DbSet<issue_detail> issue_detail { get; set; }

    public virtual DbSet<issue_notes_detail> issue_notes_detail { get; set; }

    public virtual DbSet<user_details> user_details { get; set; }

    public virtual DbSet<user_login> user_login { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<category_master>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<cfg_codevalue>(entity =>
        {
            entity.HasKey(e => e.codevalueid).HasName("cfg_codevalue_pkey");

            entity.Property(e => e.codevalueid).ValueGeneratedNever();
            entity.Property(e => e.additionalinfo).HasColumnType("json");
            entity.Property(e => e.codetype).HasMaxLength(15);
            entity.Property(e => e.codevalue).HasMaxLength(30);
            entity.Property(e => e.codevaluedescription).HasMaxLength(150);
            entity.Property(e => e.createdby).HasMaxLength(20);
            entity.Property(e => e.createddate).HasDefaultValueSql("now()");
            entity.Property(e => e.isactive)
                .IsRequired()
                .HasDefaultValueSql("true");
            entity.Property(e => e.isenable)
                .IsRequired()
                .HasDefaultValueSql("true");
            entity.Property(e => e.modifiedby).HasMaxLength(20);
        });

        modelBuilder.Entity<issue_detail>(entity =>
        {
            entity.HasKey(e => e.issue_id).HasName("issue_detail_pkey");

            entity.Property(e => e.issue_id).UseIdentityAlwaysColumn();
            entity.Property(e => e.category).HasMaxLength(200);
            entity.Property(e => e.details).HasMaxLength(1500);
            entity.Property(e => e.exchange).HasMaxLength(50);
            entity.Property(e => e.filename).HasMaxLength(200);
            entity.Property(e => e.fullname).HasMaxLength(30);
            entity.Property(e => e.isactive)
                .IsRequired()
                .HasDefaultValueSql("true");
            entity.Property(e => e.issue_by).HasMaxLength(50);
            entity.Property(e => e.issue_created_dt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.mode).HasMaxLength(150);
            entity.Property(e => e.modified_by).HasMaxLength(50);
            entity.Property(e => e.modified_dt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.pan).HasMaxLength(10);
            entity.Property(e => e.segment).HasMaxLength(50);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.subcategory).HasMaxLength(200);
            entity.Property(e => e.summary).HasMaxLength(150);
            entity.Property(e => e.targate_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ucc).HasMaxLength(10);
        });

        modelBuilder.Entity<issue_notes_detail>(entity =>
        {
            entity.HasKey(e => e.trans_id).HasName("issue_notes_detail_pkey");

            entity.Property(e => e.trans_id).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_by).HasMaxLength(50);
            entity.Property(e => e.created_dt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.filename).HasMaxLength(200);
            entity.Property(e => e.isactive)
                .IsRequired()
                .HasDefaultValueSql("true");
            entity.Property(e => e.modified_by).HasMaxLength(50);
            entity.Property(e => e.modified_dt).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<user_details>(entity =>
        {
            entity.HasKey(e => e.transid).HasName("user_details_pkey");

            entity.HasIndex(e => e.pan, "user_details_pan_isactive")
                .IsUnique()
                .HasFilter("(isactive IS TRUE)");

            entity.Property(e => e.transid).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .HasDefaultValueSql("'000000'::character varying");
            entity.Property(e => e.created_dt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.dob).HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(65);
            entity.Property(e => e.fullname).HasMaxLength(30);
            entity.Property(e => e.isactive)
                .IsRequired()
                .HasDefaultValueSql("true");
            entity.Property(e => e.mobile).HasMaxLength(14);
            entity.Property(e => e.modified_by).HasMaxLength(20);
            entity.Property(e => e.modified_dt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.pan).HasMaxLength(10);
            entity.Property(e => e.roletype).HasMaxLength(20);
            entity.Property(e => e.ucc).HasMaxLength(10);
        });

        modelBuilder.Entity<user_login>(entity =>
        {
            entity.HasKey(e => e.transid).HasName("user_login_pkey");

            entity.HasIndex(e => new { e.pan, e.device_name }, "unique_pan_device_name").IsUnique();

            entity.Property(e => e.transid).UseIdentityAlwaysColumn();
            entity.Property(e => e.created_by)
                .HasMaxLength(20)
                .HasDefaultValueSql("'000000'::character varying");
            entity.Property(e => e.created_dt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.device_name).HasMaxLength(100);
            entity.Property(e => e.expiry_dt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ip_address).HasMaxLength(136);
            entity.Property(e => e.modified_by).HasMaxLength(20);
            entity.Property(e => e.modified_dt).HasColumnType("timestamp without time zone");
            entity.Property(e => e.otp).HasMaxLength(6);
            entity.Property(e => e.pan).HasMaxLength(10);
            entity.Property(e => e.roletype).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
