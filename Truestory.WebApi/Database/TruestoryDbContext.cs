using System;
using Microsoft.EntityFrameworkCore;
using Truestory.Core.Entities;

namespace Truestory.WebApi.Database;

public class TruestoryDbContext(DbContextOptions<TruestoryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}
