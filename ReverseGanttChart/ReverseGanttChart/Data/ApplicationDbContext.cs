using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Models;

namespace ReverseGanttChart.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<UserSubject> UserSubjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserSubject>()
            .HasKey(us => us.Id);

        modelBuilder.Entity<UserSubject>()
            .HasOne(us => us.User)
            .WithMany(u => u.UserSubjects)
            .HasForeignKey(us => us.UserId);

        modelBuilder.Entity<UserSubject>()
            .HasOne(us => us.Subject)
            .WithMany(s => s.UserSubjects)
            .HasForeignKey(us => us.SubjectId);

        modelBuilder.Entity<UserSubject>()
            .HasIndex(us => new { us.UserId, us.SubjectId })
            .IsUnique();

        modelBuilder.Entity<Subject>()
            .HasOne(s => s.CreatedBy)
            .WithMany(u => u.CreatedSubjects)
            .HasForeignKey(s => s.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}