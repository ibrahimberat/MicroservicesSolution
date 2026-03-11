using Microsoft.EntityFrameworkCore;
using LogService.Domain.Entities;

namespace LogService.Infrastructure.Data
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
        }

        public DbSet<LogEntry> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ServiceName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Level).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(4000);
                entity.Property(e => e.Exception).HasMaxLength(4000);
                entity.Property(e => e.StackTrace).HasMaxLength(8000);
                entity.Property(e => e.UserId).HasMaxLength(100);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.Path).HasMaxLength(500);
                entity.Property(e => e.Method).HasMaxLength(10);
                entity.Property(e => e.Properties).HasColumnType("nvarchar(max)");

                entity.HasIndex(e => e.ServiceName);
                entity.HasIndex(e => e.Level);
                entity.HasIndex(e => e.Timestamp);
            });
        }
    }
}