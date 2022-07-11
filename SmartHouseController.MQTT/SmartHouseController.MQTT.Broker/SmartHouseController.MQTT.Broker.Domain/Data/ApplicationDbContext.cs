using Microsoft.EntityFrameworkCore;
using SmartHouseController.MQTT.Broker.Domain.Entities;

namespace SmartHouseController.MQTT.Broker.Domain.Data;

public class ApplicationDbContext: DbContext
{
    public DbSet<Staistics> Statistics { get; set; }
    private string DbPath { get; }
    
    public ApplicationDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "application.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Staistics>().HasKey(s => new { s.date, s.topic });
    }
}