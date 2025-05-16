using System;

namespace Truestory.Core.Entities;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public Dictionary<string, dynamic>? Data { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
