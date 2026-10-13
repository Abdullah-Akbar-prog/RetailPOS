using Microsoft.EntityFrameworkCore;
using RetailPOS.Data;
using RetailPOS.Models;

namespace RetailPOS.Services;

public class ProductService
{
    private readonly PosDbContext _context;

    public ProductService(PosDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
            return;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}