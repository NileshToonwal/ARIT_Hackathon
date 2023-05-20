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

    public virtual DbSet<user_login> user_login { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            entity.Property(e => e.expirey_dt).HasColumnType("timestamp without time zone");
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
