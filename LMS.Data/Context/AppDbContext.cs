using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    using LMS.Models.Entities;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    namespace LMS.Data.Context;

    public class AppDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Lead> Leads => Set<Lead>();
        public DbSet<LeadRemark> LeadRemarks => Set<LeadRemark>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 🔹 Soft Delete Filters
            builder.Entity<Lead>()
                .HasQueryFilter(l => !l.IsDeleted);

            builder.Entity<ApplicationUser>()
                .HasQueryFilter(u => !u.IsDeleted);

            // 🔹 User self reference (Manager → Agents)
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Manager)
                .WithMany(m => m.Agents)
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Lead → Assigned Agent
            builder.Entity<Lead>()
                .HasOne(l => l.AssignedAgent)
                .WithMany(u => u.AssignedLeads)
                .HasForeignKey(l => l.AssignedAgentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 LeadRemark → Lead
            builder.Entity<LeadRemark>()
                .HasOne(r => r.Lead)
                .WithMany(l => l.Remarks)
                .HasForeignKey(r => r.LeadId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 LeadRemark → ChangedBy
            builder.Entity<LeadRemark>()
                .HasOne(r => r.ChangedBy)
                .WithMany()
                .HasForeignKey(r => r.ChangedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
