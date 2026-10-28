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

                entity.Property(b => b.Name).IsRequired().HasMaxLength(100);
                entity.Property(b => b.AcademicYear).IsRequired();
                entity.Property(b => b.Description).HasMaxLength(500);

                // Relationship to Institute
                entity.HasOne(b => b.Institute)
                      .WithMany()
                      .HasForeignKey(b => b.InstituteId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relationship to User (CreatedBy)
                entity.HasOne(b => b.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(b => b.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Composite Index to prevent duplicate batch names in the same institute
                entity.HasIndex(b => new { b.InstituteId, b.Name }).IsUnique();
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
        }
    }
}
