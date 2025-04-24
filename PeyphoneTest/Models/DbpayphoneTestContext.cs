using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PeyphoneTest.Models;

public partial class DbpayphoneTestContext : DbContext
{
    public DbpayphoneTestContext()
    {
    }

    public DbpayphoneTestContext(DbContextOptions<DbpayphoneTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<HistoryMovement> HistoryMovements { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HistoryMovement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HistoryM__3213E83F9388DC75");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.WalletId).HasColumnName("walletId");

            entity.HasOne(d => d.Wallet).WithMany(p => p.HistoryMovements)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HistoryMo__walle__3F466844");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__Users__B7C92638551EE60E");

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wallets__3213E83F81CB38B4");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Balance)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("balance");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.DocumentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("documentId");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
