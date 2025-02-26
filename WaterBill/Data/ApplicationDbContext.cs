using System;
using Microsoft.EntityFrameworkCore;
using WaterBill.Models;

namespace WaterBill.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<WaterConsumption> WaterConsumptions { get; set; }
}
