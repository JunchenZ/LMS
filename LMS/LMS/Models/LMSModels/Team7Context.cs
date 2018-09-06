using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team7Context : DbContext
    {
        public virtual DbSet<Administrator> Administrator { get; set; }
        public virtual DbSet<AsgnCategory> AsgnCategory { get; set; }
        public virtual DbSet<Assignment> Assignment { get; set; }
        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Enrolled> Enrolled { get; set; }
        public virtual DbSet<Professor> Professor { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Submission> Submission { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=u1186580;Password=u1186580;Database=Team7");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.UId);

                entity.HasIndex(e => e.UId)
                    .HasName("uID_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("datetime");

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("fName")
                    .HasMaxLength(100);

                entity.Property(e => e.LName)
                    .IsRequired()
                    .HasColumnName("lName")
                    .HasMaxLength(100);

                entity.Property(e => e.Pwd)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<AsgnCategory>(entity =>
            {
                entity.HasKey(e => e.CatId);

                entity.HasIndex(e => e.ClaId)
                    .HasName("claID_idx");

                entity.Property(e => e.CatId)
                    .HasColumnName("catID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ClaId)
                    .HasColumnName("claID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Weight).HasColumnType("int(11)");

                entity.HasOne(d => d.Cla)
                    .WithMany(p => p.AsgnCategory)
                    .HasForeignKey(d => d.ClaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("claID");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.AId);

                entity.HasIndex(e => e.CatId)
                    .HasName("catID_idx");

                entity.Property(e => e.AId)
                    .HasColumnName("aID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CatId)
                    .HasColumnName("catId")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Contents)
                    .IsRequired()
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.Due).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Points).HasColumnType("int(11)");

                entity.Property(e => e.SType)
                    .HasColumnName("sType")
                    .HasColumnType("tinyint(4)");

                entity.HasOne(d => d.Cat)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => d.CatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("catID");
            });

            modelBuilder.Entity<Class>(entity =>
            {

                entity.Property(e => e.Year).HasColumnType("year");

                entity.HasIndex(e => e.CourseId)
                    .HasName("class_courseID_foreignKey_idx");

                entity.HasIndex(e => e.UId)
                    .HasName("uID_idx");

                entity.Property(e => e.ClassId)
                    .HasColumnName("classID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CourseId)
                    .HasColumnName("courseID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.End).HasColumnType("time");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Season)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.Property(e => e.Start).HasColumnType("time");

                entity.Property(e => e.UId)
                    .IsRequired()
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("class_courseID_foreignKey");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("class_uID_foreignKey");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasIndex(e => e.Subject)
                    .HasName("course_subject_foreignKey_idx");

                entity.Property(e => e.CourseId)
                    .HasColumnName("courseID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Number).HasColumnType("int(4)");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("course_subject_foreignKey");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Subject);

                entity.Property(e => e.Subject).HasMaxLength(4);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.HasKey(e => new { e.UId, e.ClaId });

                entity.HasIndex(e => e.ClaId)
                    .HasName("claID_idx");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.ClaId)
                    .HasColumnName("claID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Grade).HasColumnType("int(11)");

                entity.HasOne(d => d.Cla)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.ClaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("classID");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("uID");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.UId);

                entity.HasIndex(e => e.Subject)
                    .HasName("professor_dID_foreignKey_idx");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("datetime");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Professor)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("professor_subject_foreignKey");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.UId);

                entity.HasIndex(e => e.Subject)
                    .HasName("Subject_idx");

                entity.HasIndex(e => e.UId)
                    .HasName("uID_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("datetime");

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("fName")
                    .HasMaxLength(100);

                entity.Property(e => e.LName)
                    .IsRequired()
                    .HasColumnName("lName")
                    .HasMaxLength(100);

                entity.Property(e => e.Pwd)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Subject");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => new { e.AId, e.UId });

                entity.HasIndex(e => e.UId)
                    .HasName("uID_idx_1");

                entity.Property(e => e.AId)
                    .HasColumnName("aID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Contents).HasColumnType("varchar(8192)");

                entity.Property(e => e.Contents2).HasColumnType("blob");

                entity.Property(e => e.Score).HasColumnType("int(11)");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.A)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.AId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("aID");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("uID_foreignKey");
            });
        }
    }
}
