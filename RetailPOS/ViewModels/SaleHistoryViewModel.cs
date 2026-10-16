
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using RetailPOS.Data;
using RetailPOS.Models;
using System.Collections.ObjectModel;

namespace RetailPOS.ViewModels;

public partial class SaleHistoryViewModel : ObservableObject
{
    private readonly IDbContextFactory<PosDbContext> _factory;

    public ObservableCollection<Sale> Sales { get; } = new();

    public ObservableCollection<SaleItem> SelectedItems { get; } = new();

    [ObservableProperty]
    private Sale? selectedSale;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public SaleHistoryViewModel(
        IDbContextFactory<PosDbContext> factory)
    {
        _factory = factory;
    }

    partial void OnSelectedSaleChanged(Sale? value)
    {
        SelectedItems.Clear();

        if (value is null)
            return;

        foreach (var item in value.Items)
            SelectedItems.Add(item);
    }

    public async Task LoadAsync()
    {
        try
        {
            await using var context =
                await _factory.CreateDbContextAsync();

            var sales = await context.Sales
                .AsNoTracking()
                .Include(s => s.Items)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            Sales.Clear();

            foreach (var sale in sales)
                Sales.Add(sale);

            StatusMessage = $"{Sales.Count} sale(s) found.";
        }
        catch (Exception)
        {
            StatusMessage =
                "Unable to load sale history. Check your database connection.";
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        try
        {
            await using var context =
                await _factory.CreateDbContextAsync();

            var query = context.Sales
                .AsNoTracking()
                .Include(s => s.Items)
                .AsQueryable();

            var search = SearchText.Trim();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s =>
                    s.InvoiceNumber.Contains(search));
            }

            var results = await query
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            Sales.Clear();

            foreach (var sale in results)
                Sales.Add(sale);

            SelectedSale = null;
            StatusMessage = $"{Sales.Count} sale(s) found.";
        }
        catch (Exception)
        {
            StatusMessage = "Search failed. Please try again.";
        }
    }

    [RelayCommand]
    private async Task ResetSearchAsync()
    {
        SearchText = string.Empty;
        await LoadAsync();
    }
}
