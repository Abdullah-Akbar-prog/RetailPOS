using Microsoft.EntityFrameworkCore;
using RetailPOS.Data;
using RetailPOS.Models;

namespace RetailPOS.Services;

public class CategoryService
{
    private readonly PosDbContext _context;

    public CategoryService(PosDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null)
            return;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}