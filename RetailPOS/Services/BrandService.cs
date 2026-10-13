using Microsoft.EntityFrameworkCore;
using RetailPOS.Data;
using RetailPOS.Models;

namespace RetailPOS.Services;

public class BrandService
{
    private readonly PosDbContext _context;

    public BrandService(PosDbContext context)
    {
        _context = context;
    }

    public async Task<List<Brand>> GetAllAsync()
    {
        return await _context.Brands
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Brand brand)
    {
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var brand = await _context.Brands.FindAsync(id);

        if (brand is null)
            return;

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
    }
}