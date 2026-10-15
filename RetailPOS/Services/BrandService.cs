using Microsoft.EntityFrameworkCore;
using RetailPOS.Data;
using RetailPOS.Models;

namespace RetailPOS.Services;

public class BrandService
{
    private readonly IDbContextFactory<PosDbContext> _contextFactory;

    public BrandService(IDbContextFactory<PosDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Brand>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Brands
            .AsNoTracking()
            .OrderBy(b => b.BrandName)
            .ToListAsync();
    }

    public async Task AddAsync(Brand brand)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.Brands.Add(brand);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Brand brand)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.Brands.Update(brand);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var brand = await context.Brands.FindAsync(id);

        if (brand is null)
            return;

        context.Brands.Remove(brand);
        await context.SaveChangesAsync();
    }
}