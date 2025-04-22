using System;
using System.Collections.Generic;
using GasServiceUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GasServiceUA.Data;

public partial class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<MeterReading> MeterReadings { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Tariff> Tariffs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION_STRING");
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserLogin<int>>().HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId });
        modelBuilder.Entity<IdentityUserRole<int>>().HasKey(r => new { r.UserId, r.RoleId });
        modelBuilder.Entity<IdentityUserToken<int>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillsId).HasName("PK__Bills__CE7F6864F1E665F9");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.MeterReadings).WithMany(p => p.Bills)
                .HasForeignKey(d => d.MeterReadingsId)
                .HasConstraintName("bills_meterreadingsid_foreign");

            entity.HasOne(d => d.Users).WithMany(p => p.Bills)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bills_usersid_foreign");
        });

        modelBuilder.Entity<MeterReading>(entity =>
        {
            entity.HasKey(e => e.MeterReadingsId).HasName("PK__MeterRea__1D5EBCAADAA07D6C");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Users).WithMany(p => p.MeterReadings)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("meterreadings_usersid_foreign");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentsId).HasName("PK__Payments__FD75744AD43A323D");

            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.Users).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_usersid_foreign");
        });

        modelBuilder.Entity<Tariff>(entity =>
        {
            entity.HasKey(e => e.TariffsId).HasName("PK__Tariffs__69CF08832923FE6A");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0760BE5CCF");

            entity.HasIndex(e => e.AccountNumber, "users_accountnumber_unique").IsUnique();

            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.CityDistrict).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Patronymic).HasMaxLength(50);
            entity.Property(e => e.Street).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);

            entity.HasOne(d => d.Tariffs).WithMany(p => p.Users)
                .HasForeignKey(d => d.TariffsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_tariffsid_foreign");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
