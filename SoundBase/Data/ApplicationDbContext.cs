using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoundBase.Models;

namespace SoundBase.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectInvite> ProjectInvite { get; set; }
        public DbSet<ProjectUser> ProjectUser { get; set; }
        public DbSet<MemberRole> MemberRole { get; set; }
        public DbSet<Track> Track { get; set; }
        public DbSet<TrackNote> TrackNote { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            modelBuilder.Entity<Project>()
                .Property(b => b.DateCreated)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Track>()
                .Property(b => b.DateUploaded)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<TrackNote>()
                .Property(b => b.DatePosted)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ProjectInvite>()
                .Property(b => b.DateInvited)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ProjectUser>()
                .Property(b => b.DateAdded)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ChatMessage>()
                .Property(b => b.DatePosted)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}