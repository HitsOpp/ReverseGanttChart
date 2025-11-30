using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Models;
using ReverseGanttChart.Models.Team;
using ReverseGanttChart.Models.Project;

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
    
    // Project related entities
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<TaskStage> TaskStages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // UserSubject configuration
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

        // Subject configuration
        modelBuilder.Entity<Subject>()
            .HasOne(s => s.CreatedBy)
            .WithMany(u => u.CreatedSubjects)
            .HasForeignKey(s => s.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subject>()
            .Property(s => s.Color)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("blue");
        
        // Team configuration
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

        // TeamMember configuration
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

        // Project configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.HasOne(p => p.Subject)
                .WithMany()
                .HasForeignKey(p => p.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProjectTask configuration
        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(t => t.Id);
            
            entity.HasOne(t => t.Project)
                .WithMany()
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(t => t.ParentTask)
                .WithMany(t => t.Subtasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TaskStage configuration
        modelBuilder.Entity<TaskStage>(entity =>
        {
            entity.HasKey(ts => ts.Id);
            
            entity.HasOne(ts => ts.Task)
                .WithMany(t => t.Stages)
                .HasForeignKey(ts => ts.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(ts => ts.CompletedBy)
                .WithMany()
                .HasForeignKey(ts => ts.CompletedById)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}