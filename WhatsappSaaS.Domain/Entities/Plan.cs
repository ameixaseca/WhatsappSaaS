using System;
using System.Collections.Generic;

namespace WhatsappSaaS.Domain.Entities;

public class Plan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int MaxUsers { get; set; }
    public int MaxMessages { get; set; }
    public decimal Price { get; set; }
    public ICollection<Company> Companies { get; set; } = new List<Company>();
}
