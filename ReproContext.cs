using System;
using Microsoft.EntityFrameworkCore;

public class ReproEntity
{
    public Guid Id { get; set; }
    public DateTime MyTime { get; set; }
}

public class WorkingEntity
{
    public Guid Id { get; set; }
    public DateTime MyTime { get;set; }
}

public class ReproContext : DbContext
{
    private static readonly string _reproConnectionString = 
        "Data Source=.\\SQLEXPRESS;Initial Catalog=ReproDb;Integrated Security=True;";
    public virtual DbSet<ReproEntity> ReproEntity { get; set; }
    public virtual DbSet<WorkingEntity> WorkingEntity { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_reproConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReproEntity>(e =>
            e.Property("MyTime").HasColumnType("smalldatetime"));
    }
}