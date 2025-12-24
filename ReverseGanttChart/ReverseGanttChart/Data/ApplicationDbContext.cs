using Microsoft.EntityFrameworkCore;
using ReverseGanttChart.Models;
using ReverseGanttChart.Models.Team;
using ReverseGanttChart.Models.Project;
using ReverseGanttChart.Models.Project.ReverseGanttChart.Models.Project;

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
    
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<TaskStage> TaskStages { get; set; }
    
    public DbSet<TeamTaskProgress> TeamTaskProgress { get; set; }
    public DbSet<TeamStageProgress> TeamStageProgress { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.IsTeacher)
                .IsRequired()
                .HasDefaultValue(false);
        });
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

        modelBuilder.Entity<Subject>()
            .Property(s => s.Color)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("blue");
        
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
        
        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(t => t.Id);
            
            entity.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaskStage>(entity =>
        {
            entity.HasKey(ts => ts.Id);
            
            entity.HasOne(ts => ts.Task)
                .WithMany(t => t.Stages)
                .HasForeignKey(ts => ts.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TeamTaskProgress>(entity =>
        {
            entity.HasKey(ttp => ttp.Id);
            
            entity.HasOne(ttp => ttp.Task)
                .WithMany()
                .HasForeignKey(ttp => ttp.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(ttp => ttp.Team)
                .WithMany()
                .HasForeignKey(ttp => ttp.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(ttp => ttp.CompletedBy)
                .WithMany()
                .HasForeignKey(ttp => ttp.CompletedById)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasIndex(ttp => new { ttp.TaskId, ttp.TeamId })
                .IsUnique();
        });

        modelBuilder.Entity<TeamStageProgress>(entity =>
        {
            entity.HasKey(tsp => tsp.Id);
            
            entity.HasOne(tsp => tsp.Stage)
                .WithMany()
                .HasForeignKey(tsp => tsp.StageId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(tsp => tsp.Team)
                .WithMany()
                .HasForeignKey(tsp => tsp.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(tsp => tsp.CompletedBy)
                .WithMany()
                .HasForeignKey(tsp => tsp.CompletedById)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasIndex(tsp => new { tsp.StageId, tsp.TeamId })
                .IsUnique();
        });
    }
}