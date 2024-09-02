using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace web_api_hospital.Model;

public partial class TestTaskHospitalContext : DbContext
{
    public TestTaskHospitalContext()
    {
    }

    public TestTaskHospitalContext(DbContextOptions<TestTaskHospitalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Office> Offices { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Plot> Plots { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("YourDatabaseConnection"));
        }
//       optionsBuilder.UseSqlServer("Server=home-pc;Database=testTask_hospital;user id=root;password=1234; TrustServerCertificate=True; Trusted_Connection=true");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("doctors");

            entity.Property(e => e.Firstname)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Patronymic)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Office).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.OfficeId)
                .HasConstraintName("FK_doctors_offices");

            entity.HasOne(d => d.Plot).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.PlotId)
                .HasConstraintName("FK_doctors_plots");

            entity.HasOne(d => d.Specialization).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_doctors_specializations");
        });

        modelBuilder.Entity<Office>(entity =>
        {
            entity.ToTable("offices");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("patients");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.Firstname)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Patronymic)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Plot).WithMany(p => p.Patients)
                .HasForeignKey(d => d.PlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_patients_plots");
        });

        modelBuilder.Entity<Plot>(entity =>
        {
            entity.ToTable("plots");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.ToTable("specializations");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
