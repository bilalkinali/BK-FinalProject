using ContentModerationService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ContentModerationService.Infrastructure;

public class ContentModerationContext : DbContext
{
    public ContentModerationContext(DbContextOptions<ContentModerationContext> options) : base(options)
    {
    }

    public DbSet<ModerationResult> ModerationResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ModerationResult>(entity =>
        {
            entity.HasIndex(e => e.CorrelationId).IsUnique();
        });
    }
}