using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailPOS.Models;
using RetailPOS.Services;
using System.Collections.ObjectModel;

namespace RetailPOS.ViewModels;

public partial class BrandViewModel : ObservableObject
{
    private readonly BrandService _service;

    public ObservableCollection<Brand> Brands { get; } = new();

    [ObservableProperty]
    private Brand? selectedBrand;

    [ObservableProperty]
    private string brandName = string.Empty;

    public BrandViewModel(BrandService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        var brands = await _service.GetAllAsync();

        Brands.Clear();
        foreach (var brand in brands)
            Brands.Add(brand);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(BrandName))
            return;

        if (SelectedBrand is null)
        {
            await _service.AddAsync(new Brand
            {
                BrandName = BrandName.Trim()
            });
        }
        else
        {
            SelectedBrand.BrandName = BrandName.Trim();
            await _service.UpdateAsync(SelectedBrand);
        }

        Clear();
        await LoadAsync();
    }

    [RelayCommand]
    private void Edit()
    {
        if (SelectedBrand is not null)
            BrandName = SelectedBrand.BrandName;
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedBrand is null)
            return;

        await _service.DeleteAsync(SelectedBrand.BrandId);

        Clear();
        await LoadAsync();
    }

    [RelayCommand]
    private void Clear()
    {
        SelectedBrand = null;
        BrandName = string.Empty;
    }
}