
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Data;

namespace RetailPOS.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IDbContextFactory<PosDbContext> _factory;

    [ObservableProperty]
    private int totalProducts;

    [ObservableProperty]
    private int lowStockProducts;

    [ObservableProperty]
    private int totalSales;

    [ObservableProperty]
    private decimal todayRevenue;

    [ObservableProperty]
    private decimal totalRevenue;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public DashboardViewModel(
        IDbContextFactory<PosDbContext> factory)
    {
        _factory = factory;
    }

    public async Task LoadAsync()
    {
        try
        {
            await using var context =
                await _factory.CreateDbContextAsync();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            TotalProducts = await context.Products.CountAsync();

            LowStockProducts = await context.Products
                .CountAsync(p => p.IsActive &&
                    p.StockQuantity <= p.ReorderLevel);

            TotalSales = await context.Sales.CountAsync();

            TodayRevenue = await context.Sales
                .Where(s => s.SaleDate >= today &&
                            s.SaleDate < tomorrow)
                .SumAsync(s => (decimal?)s.GrandTotal) ?? 0m;

            TotalRevenue = await context.Sales
                .SumAsync(s => (decimal?)s.GrandTotal) ?? 0m;

            StatusMessage = string.Empty;
        }
        catch (Exception)
        {
            StatusMessage =
                "Unable to load dashboard data. Check your database connection.";
        }
    }
}
