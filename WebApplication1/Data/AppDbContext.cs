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
        }
    }
}
