using Microsoft.EntityFrameworkCore;
using RaceDay.Models;
using System.Collections.Generic;

namespace RaceDay.Data;

public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique indexes
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Category>().HasIndex(c => new { c.EventId, c.Name }).IsUnique();
        modelBuilder.Entity<Enrolment>().HasIndex(e => new { e.ParticipantId, e.CategoryId }).IsUnique();

        // Event -> Categories (CASCADE)
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Event)
            .WithMany(e => e.Categories)
            .HasForeignKey(c => c.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Event -> WeatherForecasts (CASCADE)
        modelBuilder.Entity<WeatherForecast>()
        .HasOne(w => w.Event)
        .WithMany(e => e.WeatherForecasts)
        .HasForeignKey(w => w.EventId)
        .OnDelete(DeleteBehavior.Cascade);

        // Event -> Results (NO ACTION — prevents cascade cycle)
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Event)
            .WithMany(e => e.Results)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.NoAction);

        // User -> Events (NO ACTION — organiser cannot be cascade-deleted)
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Organiser)
            .WithMany(u => u.OrganisedEvents)
            .HasForeignKey(e => e.OrganiserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Category -> Results (NO ACTION — prevents another cascade path)
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Category)
            .WithMany(c => c.Results)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        // User -> Results (NO ACTION)
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Participant)
            .WithMany(u => u.Results)
            .HasForeignKey(r => r.ParticipantId)
            .OnDelete(DeleteBehavior.NoAction);

        // Category -> Enrolments (CASCADE)
        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Category)
            .WithMany(c => c.Enrolments)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Enrolments (NO ACTION)
        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Participant)
            .WithMany(u => u.Enrolments)
            .HasForeignKey(e => e.ParticipantId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}