using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineJobPortal.Models;

namespace OnlineJobPortal.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<JobCategory> JobCategories => Set<JobCategory>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Job>()
            .HasOne(j => j.Category)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Job>()
            .HasOne(j => j.Employer)
            .WithMany(u => u.PostedJobs)
            .HasForeignKey(j => j.EmployerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<JobApplication>()
            .HasOne(a => a.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<JobApplication>()
            .HasOne(a => a.User)
            .WithMany(u => u.Applications)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<JobApplication>()
            .HasIndex(a => new { a.JobId, a.UserId })
            .IsUnique();

        builder.Entity<JobCategory>().HasData(
            new JobCategory { Id = 1, Name = "Software Development" },
            new JobCategory { Id = 2, Name = "DevOps & Cloud" },
            new JobCategory { Id = 3, Name = "UI/UX Design" },
            new JobCategory { Id = 4, Name = "Data Science" });
    }
}
