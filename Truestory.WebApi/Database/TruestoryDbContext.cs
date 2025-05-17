using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Truestory.WebApi.Entities;

namespace Truestory.WebApi.Database;

public class TruestoryDbContext(DbContextOptions<TruestoryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Data)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, dynamic>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, dynamic>()
            )
            .Metadata
            .SetValueComparer(new ValueComparer<Dictionary<string, dynamic>>(
                (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null),
                c => c == null ? 0 : JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),
                c => JsonSerializer.Deserialize<Dictionary<string, dynamic>>(JsonSerializer.Serialize(c, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!
            ));
    }
}
