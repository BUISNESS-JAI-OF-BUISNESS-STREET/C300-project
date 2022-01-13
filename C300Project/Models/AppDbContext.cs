using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace fyp.Models
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Announcement> Announcement { get; set; }
        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<Quiz> Quiz { get; set; }
        public virtual DbSet<QuizQuestionBindDb> QuizQuestionBindDb { get; set; }
        public virtual DbSet<Result> Result { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<StudentClassBindDb> StudentClassBindDb { get; set; }
        public virtual DbSet<Teacher> Teacher { get; set; }
        public virtual DbSet<TeacherClassBindDb> TeacherClassBindDb { get; set; }
        public virtual DbSet<TeacherStudentBindDb> TeacherStudentBindDb { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.AccountId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.Property(e => e.Remarks)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Announcement)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Announcem__Class__153B1FDF");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.CorrectAns)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstOption)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FourthOption)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Questions)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.SecondOption)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Segment)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ThirdOption)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Topic)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.UserCodeNavigation)
                    .WithMany(p => p.Question)
                    .HasForeignKey(d => d.UserCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Question__UserCo__76B698BF");
            });

            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.Property(e => e.EndDt).HasColumnType("datetime");

                entity.Property(e => e.StartDt).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Topic)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.UserCodeNavigation)
                    .WithMany(p => p.Quiz)
                    .HasForeignKey(d => d.UserCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quiz__UserCode__7993056A");
            });

            modelBuilder.Entity<QuizQuestionBindDb>(entity =>
            {
                entity.ToTable("QuizQuestionBindDB");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuizQuestionBindDb)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuizQuest__Quest__7E57BA87");

                entity.HasOne(d => d.Quiz)
                    .WithMany(p => p.QuizQuestionBindDb)
                    .HasForeignKey(d => d.QuizId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuizQuest__QuizI__7F4BDEC0");
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.Property(e => e.AccountId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Topic)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MobileNo)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SchLvl)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Student__AddedBy__0504B816");
            });

            modelBuilder.Entity<StudentClassBindDb>(entity =>
            {
                entity.ToTable("StudentClassBindDB");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.StudentClassBindDb)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StudentCl__Class__0E8E2250");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.StudentClassBindDb)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StudentCl__Stude__0D99FE17");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MobileNo)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.Teacher)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Teacher__AddedBy__02284B6B");
            });

            modelBuilder.Entity<TeacherClassBindDb>(entity =>
            {
                entity.ToTable("TeacherClassBindDB");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.TeacherClassBindDb)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TeacherCl__Class__0ABD916C");

                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.TeacherClassBindDb)
                    .HasForeignKey(d => d.TeacherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TeacherCl__Teach__09C96D33");
            });

            modelBuilder.Entity<TeacherStudentBindDb>(entity =>
            {
                entity.ToTable("TeacherStudentBindDB");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.TeacherStudentBindDb)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TeacherSt__Stude__116A8EFB");

                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.TeacherStudentBindDb)
                    .HasForeignKey(d => d.TeacherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TeacherSt__Teach__125EB334");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
