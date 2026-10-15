using Microsoft.EntityFrameworkCore;
using RetailPOS.Data;
using RetailPOS.Models;

namespace RetailPOS.Services;

public class ProductService
{
    private readonly IDbContextFactory<PosDbContext> _contextFactory;

    public ProductService(
        IDbContextFactory<PosDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // Get all products with their Brand and Category
    public async Task<List<Product>> GetAllAsync()
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        return await context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    // Add a new product
    public async Task AddAsync(Product product)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        context.Products.Add(product);

        await context.SaveChangesAsync();
    }

    // Update an existing product
    public async Task UpdateAsync(Product product)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        context.Products.Update(product);

        await context.SaveChangesAsync();
    }

    // Delete a product
    public async Task DeleteAsync(int id)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        var product = await context.Products.FindAsync(id);

        if (product is null)
            return;

        context.Products.Remove(product);

        await context.SaveChangesAsync();
    }
}