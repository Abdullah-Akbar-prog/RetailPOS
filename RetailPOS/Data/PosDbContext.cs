using Microsoft.EntityFrameworkCore;
using RetailPOS.Models;

namespace RetailPOS.Data;

public class PosDbContext : DbContext
{
    public PosDbContext(DbContextOptions<PosDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Sale> Sales { get; set; }

    public DbSet<SaleItem> SaleDetails { get; set; }

    public DbSet<Brand> Brands { get; set; }

    public DbSet<Category> Categories { get; set; }
}