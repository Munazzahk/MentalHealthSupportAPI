using Microsoft.EntityFrameworkCore;
using MentalHealthSupportAPI.Models;

namespace MentalHealthSupportAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Psychologist> Psychologists => Set<Psychologist>();
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<CaseNote> CaseNotes => Set<CaseNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Brugernavn unikt
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // Psykolog - User (en til en)
        modelBuilder.Entity<Psychologist>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sag - User (en bruger, en sag)
        modelBuilder.Entity<Case>()
            .HasOne(c => c.User)
            .WithOne(u => u.AssignedCase)
            .HasForeignKey<Case>(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sag - Psykolog
        modelBuilder.Entity<Case>()
            .HasOne(c => c.Psychologist)
            .WithMany(p => p.Cases)
            .HasForeignKey(c => c.PsychologistId)
            .OnDelete(DeleteBehavior.Restrict);

        // Note - Sag (mange noter pr. sag)
        modelBuilder.Entity<CaseNote>()
            .HasOne(n => n.Case)
            .WithMany(c => c.Notes)
            .HasForeignKey(n => n.CaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}