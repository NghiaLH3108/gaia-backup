using System;
using System.Collections.Generic;
using GaiaWeb.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace GaiaWeb.DAL.Data;

public partial class GaiaDbContext : DbContext
{
    public GaiaDbContext()
    {
    }

    public GaiaDbContext(DbContextOptions<GaiaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MaterialBatch> MaterialBatches { get; set; }

    public virtual DbSet<MaterialImage> MaterialImages { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductTimeline> ProductTimelines { get; set; }

    public virtual DbSet<Story> Stories { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<TransportationHistory> TransportationHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        modelBuilder.Entity<MaterialBatch>(entity =>
        {
            entity.HasKey(e => e.BatchId).HasName("PK__Material__5D55CE5867B47333");

            entity.ToTable("MaterialBatch");

            entity.HasIndex(e => e.SupplierId, "IDX_Batch_SupplierId");

            entity.HasIndex(e => e.BatchCode, "UQ_Batch_Code").IsUnique();

            entity.Property(e => e.ApprovedTime).HasColumnType("datetime");
            entity.Property(e => e.BatchCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CollectionAddress).HasMaxLength(255);
            entity.Property(e => e.CollectionTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");
            entity.Property(e => e.WeightKg).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Supplier).WithMany(p => p.MaterialBatches)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Batch_Supplier");
        });

        modelBuilder.Entity<MaterialImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Material__7516F70CD4FA9B9F");

            entity.ToTable("MaterialImage");

            entity.HasIndex(e => e.BatchId, "IDX_Image_BatchId");

            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.UploadTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Batch).WithMany(p => p.MaterialImages)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Image_Batch");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6CD9CA69FA8");

            entity.ToTable("Product");

            entity.HasIndex(e => e.BatchId, "IDX_Product_BatchId");

            entity.HasIndex(e => e.Qrtoken, "UQ_Product_QRToken").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentStatus)
                .HasMaxLength(30)
                .HasDefaultValue("Available");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.Qrtoken)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("QRToken");

            entity.HasOne(d => d.Batch).WithMany(p => p.Products)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Batch");
        });

        modelBuilder.Entity<ProductTimeline>(entity =>
        {
            entity.HasKey(e => e.TimelineId).HasName("PK__ProductT__1DE4F0854C95343F");

            entity.ToTable("ProductTimeline");

            entity.HasIndex(e => e.ProductId, "IDX_Timeline_ProductId");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.TimelineTime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.VideoUrl).HasMaxLength(255);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductTimelines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Timeline_Product");
        });

        modelBuilder.Entity<Story>(entity =>
        {
            entity.HasKey(e => e.StoryId).HasName("PK__Story__3E82C04850518A5B");

            entity.ToTable("Story");

            entity.HasIndex(e => e.ProductId, "IDX_Story_ProductId");

            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Product).WithMany(p => p.Stories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Story_Product");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B40D0BB74F");

            entity.ToTable("Supplier");

            entity.HasIndex(e => e.UserId, "UQ_Supplier_UserId").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.WarehouseName).HasMaxLength(200);

            entity.HasOne(d => d.User).WithOne(p => p.Supplier)
                .HasForeignKey<Supplier>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Supplier_Users");
        });

        modelBuilder.Entity<TransportationHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__Transpor__4D7B4ABD4C423231");

            entity.ToTable("TransportationHistory");

            entity.HasIndex(e => e.BatchId, "IDX_Transport_BatchId");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Batch).WithMany(p => p.TransportationHistories)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transport_Batch");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C2B0D3F5F");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ_Users_Phone").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
