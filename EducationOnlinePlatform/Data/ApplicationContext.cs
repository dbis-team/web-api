using System;
using System.IO;
using EducationOnlinePlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace EducationOnlinePlatform
{
    public class ApplicationContext: IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationContext(DbContextOptions options)
            : base(options) {
        }
        public DbSet<SubjectFile> Files { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<UserInEducationSet> UserInEducationSet { get; set; }

        public DbSet<EducationSet> EducationSets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<Role>();
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

            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<IdentityUserToken<Guid>>();
            modelBuilder.Ignore<IdentityUserLogin<Guid>>();
            modelBuilder.Ignore<IdentityUserClaim<Guid>>();
            modelBuilder.Ignore<IdentityRoleClaim<Guid>>();
            modelBuilder.Ignore<IdentityRole<Guid>>();
            modelBuilder.Ignore<IdentityUserRole<Guid>>();
            modelBuilder.Entity<User>()

                .Ignore(c => c.AccessFailedCount)
                .Ignore(c => c.LockoutEnabled)
                .Ignore(c => c.TwoFactorEnabled)
                .Ignore(c => c.ConcurrencyStamp)
                .Ignore(c => c.LockoutEnd)
                .Ignore(c => c.TwoFactorEnabled)
                .Ignore(c => c.LockoutEnd)
                .Ignore(c => c.AccessFailedCount)
                .Ignore(c => c.PhoneNumberConfirmed)
                .Ignore(c => c.PhoneNumber);
            //modelBuilder.ForNpgsqlHasEnum("Role", Enum.GetNames(typeof(Role)));
            //modelBuilder.Entity<IdentityUser>().ToTable("Users");//to change the name of table.

        }
    }
}