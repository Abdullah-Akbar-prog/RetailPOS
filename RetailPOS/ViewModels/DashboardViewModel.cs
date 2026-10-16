
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Data;
using SkiaSharp;

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

    // Chart properties
    public ISeries[] RevenueSeries { get; private set; } =
        Array.Empty<ISeries>();

    public Axis[] RevenueXAxes { get; private set; } =
        Array.Empty<Axis>();

    public Axis[] RevenueYAxes { get; private set; } =
        Array.Empty<Axis>();

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

            // Total products
            TotalProducts = await context.Products.CountAsync();

            // Active products at or below their reorder level
            LowStockProducts = await context.Products
                .CountAsync(p => p.IsActive &&
                                 p.StockQuantity <= p.ReorderLevel);

            // Total recorded sales
            TotalSales = await context.Sales.CountAsync();

            // Today's revenue
            TodayRevenue = await context.Sales
                .Where(s => s.SaleDate >= today &&
                            s.SaleDate < tomorrow)
                .SumAsync(s => (decimal?)s.GrandTotal) ?? 0m;

            // Revenue across all recorded sales
            TotalRevenue = await context.Sales
                .SumAsync(s => (decimal?)s.GrandTotal) ?? 0m;

            // Load revenue for the last seven calendar days
            await LoadRevenueChartAsync(context, today);

            StatusMessage = string.Empty;
        }
        catch (Exception)
        {
            StatusMessage =
                "Unable to load dashboard data. Check your database connection.";
        }
    }

    private async Task LoadRevenueChartAsync(
        PosDbContext context,
        DateTime today)
    {
        var startDate = today.AddDays(-6);
        var endDate = today.AddDays(1);

        // Fetch daily revenue from SQL Server
        var dailySales = await context.Sales
            .Where(s => s.SaleDate >= startDate &&
                        s.SaleDate < endDate)
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                Revenue = g.Sum(s => s.GrandTotal)
            })
            .ToListAsync();

        // Convert results into a date-to-revenue lookup
        var revenueByDate = dailySales.ToDictionary(
            x => x.Date.Date,
            x => (double)x.Revenue);

        // Include all seven dates, even when no sales occurred
        var dates = Enumerable.Range(0, 7)
            .Select(i => startDate.AddDays(i))
            .ToArray();

        var labels = dates
            .Select(d => d.ToString("dd MMM"))
            .ToArray();

        var revenues = dates
            .Select(d => revenueByDate.TryGetValue(
                d.Date, out var amount) ? amount : 0d)
            .ToArray();

        SetupRevenueChart(labels, revenues);
    }

    private void SetupRevenueChart(
        string[] dates,
        double[] revenues)
    {
        RevenueSeries =
        [
            new LineSeries<double>
            {
                Name = "Revenue",
                Values = revenues,
                Fill = null,
                GeometrySize = 8,
                Stroke = new SolidColorPaint(
                    new SKColor(13, 148, 136))
                {
                    StrokeThickness = 3
                }
            }
        ];

        RevenueXAxes =
        [
            new Axis
            {
                Labels = dates,
                LabelsRotation = 0
            }
        ];

        RevenueYAxes =
        [
            new Axis
            {
                Name = "Revenue",
                MinLimit = 0
            }
        ];

        OnPropertyChanged(nameof(RevenueSeries));
        OnPropertyChanged(nameof(RevenueXAxes));
        OnPropertyChanged(nameof(RevenueYAxes));
    }
}
