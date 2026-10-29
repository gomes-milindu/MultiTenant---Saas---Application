using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Academic;
using WebApplication1.Models.Auth;
using WebApplication1.Models.System;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<ForgotPassword> ForgotPassword { get; set; }

        public DbSet<Institute> Institutes { get; set; }

        public DbSet<Batch> Batches => Set<Batch>();

        public DbSet<Subject> Subjects { get; set; }

        public DbSet<Class> Classes  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<User>()
                .HasOne(u => u.Credential)
                .WithOne(c => c.User)
                .HasForeignKey<UserCredential>(c => c.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId);

            // Automatically convert all table names to lowercase (users, usercredentials, userprofiles, etc.)
            //foreach (var entity in modelBuilder.Model.GetEntityTypes())
            //{
            //    var tableName = entity.GetTableName();
            //    if (tableName != null)
            //    {
            //        entity.SetTableName(tableName.ToLowerInvariant());
            //    }
            //}

            modelBuilder.Entity<Institute>(entity =>
            {
                entity.ToTable("Institutes");
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Name).IsRequired().HasMaxLength(150);
                entity.Property(i => i.Description).HasMaxLength(500);

                // Relationship to Users (Owner)
                entity.HasOne(i => i.OwnerUser)
                      .WithMany()
                      .HasForeignKey(i => i.OwnerUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship to Users (Creator)
                entity.HasOne(i => i.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(i => i.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Batch>(entity =>
            {
                entity.ToTable("Batches");
                entity.HasKey(b => b.Id);

                // Foreign Key to Institute
                entity.HasOne(b => b.Institute)
                      .WithMany()
                      .HasForeignKey(b => b.InstituteId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Foreign Key to Teacher (Owner)
                entity.HasOne(b => b.OwnerTeacher)
                      .WithMany()
                      .HasForeignKey(b => b.OwnerTeacherId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign Key to Creator (Audit)
                entity.HasOne(b => b.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(b => b.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 🟢 1. Foreign Key එක වෙනුවෙන් InstituteId එකට වෙනම Index එකක් (MySQL Error එක නැතිවීමට මෙය අනිවාර්යයි)
                entity.HasIndex(b => b.InstituteId);

                // 🟢 2. අලුත් Composite Unique Index එක (Institute + Teacher + Name)
                entity.HasIndex(b => new { b.InstituteId, b.OwnerTeacherId, b.Name }).IsUnique();
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subjects");
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Description).HasMaxLength(300);

                // Owner (Teacher) Relationship
                entity.HasOne(s => s.OwnerUser)
                      .WithMany()
                      .HasForeignKey(s => s.OwnerUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Creator (Admin or Teacher) Relationship
                entity.HasOne(s => s.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(s => s.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // එකම Teacher ට එකම නමින් Subject දෙකක් සෑදීම වැළැක්වීමට
                entity.HasIndex(s => new { s.OwnerUserId, s.Name }).IsUnique();
            });


            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("Classes");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Title).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Schedule).HasMaxLength(150);

                // Relationship to Institute
                entity.HasOne(c => c.Institute)
                      .WithMany()
                      .HasForeignKey(c => c.InstituteId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relationship to Batch
                entity.HasOne(c => c.Batch)
                      .WithMany()
                      .HasForeignKey(c => c.BatchId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship to Subject
                entity.HasOne(c => c.Subject)
                      .WithMany()
                      .HasForeignKey(c => c.SubjectId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship to Teacher (User)
                entity.HasOne(c => c.Teacher)
                      .WithMany()
                      .HasForeignKey(c => c.TeacherUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship to Creator (User)
                entity.HasOne(c => c.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(c => c.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // එකම Institute එකක් තුළ එකම Batch එකට එකම විෂය සඳහා එකම මාතෘකාවෙන් Classes duplicate වීම වැළැක්වීමට
                entity.HasIndex(c => new { c.InstituteId, c.BatchId, c.SubjectId, c.Title }).IsUnique();
            });
        }
    }
}
