using Microsoft.EntityFrameworkCore;
using RetailPOS.Data;
using RetailPOS.Models;

namespace RetailPOS.Services;

public class CategoryService
{
    private readonly IDbContextFactory<PosDbContext> _contextFactory;

    public CategoryService(
        IDbContextFactory<PosDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // Get all categories
    public async Task<List<Category>> GetAllAsync()
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        return await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
    }

    // Add a new category
    public async Task AddAsync(Category category)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        context.Categories.Add(category);

        await context.SaveChangesAsync();
    }

    // Update an existing category
    public async Task UpdateAsync(Category category)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        context.Categories.Update(category);

        await context.SaveChangesAsync();
    }

    // Delete a category
    public async Task DeleteAsync(int id)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        var category = await context.Categories.FindAsync(id);

        if (category is null)
            return;

        context.Categories.Remove(category);

        await context.SaveChangesAsync();
    }
}