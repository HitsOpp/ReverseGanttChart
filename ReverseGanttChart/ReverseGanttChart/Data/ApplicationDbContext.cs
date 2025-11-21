using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Models;
using ReverseGanttChart.Models.Team;

namespace ReverseGanttChart.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<UserSubject> UserSubjects { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }

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
        
        modelBuilder.Entity<Team>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.Subject)
            .WithMany()
            .HasForeignKey(t => t.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TeamMember>()
            .HasKey(tm => tm.Id);

        modelBuilder.Entity<TeamMember>()
            .HasOne(tm => tm.Team)
            .WithMany(t => t.TeamMembers)
            .HasForeignKey(tm => tm.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeamMember>()
            .HasOne(tm => tm.User)
            .WithMany()
            .HasForeignKey(tm => tm.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<TeamMember>()
            .HasIndex(tm => new { tm.UserId, tm.TeamId })
            .IsUnique();
    }
}