using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EducationOnlinePlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace EducationOnlinePlatform
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<UserInEducationSet> UserInEducationSet { get; set; }

        public DbSet<EducationSet> EducationSets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Uid=postgres; Pwd=0000; Host=34.89.220.166; Database=EducationOnlinePlatform");
        }
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
            //Role Fluent Api
            modelBuilder.Entity<Role>()
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
            base.OnModelCreating(modelBuilder);
        }
    }
}
