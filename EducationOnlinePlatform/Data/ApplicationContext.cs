using System;
using System.IO;
using EducationOnlinePlatform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EducationOnlinePlatform
{
    public class ApplicationContext: IdentityDbContext<User, ApplicationRole, Guid>
    {
        public ApplicationContext(DbContextOptions options)
            : base(options) {}
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<UserInEducationSet> UserInEducationSet { get; set; }

        public DbSet<EducationSet> EducationSets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Addd the Postgres Extension for UUID generation
            modelBuilder.HasPostgresExtension("uuid-ossp");
            //User Fluent Api
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            //Education Set Fluent Api
            modelBuilder.Entity<EducationSet>()
                .Property(u => u.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
            modelBuilder.Entity<EducationSet>()
                .HasIndex(u => u.Name)
                .IsUnique();
            //Subject Fluent Api
            modelBuilder.Entity<Subject>()
                .Property(u => u.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
            //UserInEducationSet Fluent Api
            modelBuilder.Entity<UserInEducationSet>()
                .Property(u => u.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
            modelBuilder.Entity<UserInEducationSet>()
                .HasOne(uinc => uinc.EducationSet)
                .WithMany(es => es.UserInEducationSet)
                .HasForeignKey(uinc => uinc.EducationSetId);
            modelBuilder.Entity<UserInEducationSet>()
                .HasOne(uinc => uinc.User)
                .WithMany(u => u.UserInEducationSet)
                .HasForeignKey(uinc => uinc.UserId);
            modelBuilder.Entity<UserInEducationSet>()
                .Property(uinc => uinc.UserRole)
                .HasConversion<string>();

            //modelBuilder.Ignore<IdentityUserLogin>();
            //base.OnModelCreating(modelBuilder);
        }
    }
}
