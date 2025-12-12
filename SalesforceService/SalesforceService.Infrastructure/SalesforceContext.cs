using Microsoft.EntityFrameworkCore;
using SalesforceService.Domain.Entities;

namespace SalesforceService.Infrastructure;

public class SalesforceContext : DbContext
{
    public SalesforceContext(DbContextOptions<SalesforceContext> options) : base(options)
    {
    }

    public DbSet<InboundEvent> InboundEvents { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);
    //    modelBuilder.Entity<InboundEvent>(entity =>
    //    {
    //        entity.HasKey(e => e.EventId);
    //        entity.Property(e => e.TopicName).IsRequired();
    //        entity.Property(e => e.ReplayId).IsRequired();
    //        entity.Property(e => e.RecordId).IsRequired();
    //        entity.Property(e => e.ObjectType).IsRequired();
    //        entity.Property(e => e.CreatedAt).IsRequired();
    //    });
    //}
}