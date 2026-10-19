
using CommunityToolkit.Mvvm.ComponentModel;
using RetailPOS.Models;
using RetailPOS.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace RetailPOS.ViewModels;

public partial class BrandViewModel : ObservableObject
{
    private readonly BrandService _service;
    private readonly ICollectionView _brandsView;

    public ObservableCollection<Brand> Brands { get; } = new();

    public ICollectionView BrandsView => _brandsView;

    [ObservableProperty]
    private Brand? selectedBrand;

    [ObservableProperty]
    private string searchText = string.Empty;

    public BrandViewModel(BrandService service)
    {
        _service = service;

        _brandsView = CollectionViewSource.GetDefaultView(Brands);
        _brandsView.Filter = FilterBrand;
    }

    private bool FilterBrand(object item)
    {
        if (item is not Brand brand)
            return false;

        return string.IsNullOrWhiteSpace(SearchText)
            || (brand.BrandName?.Contains(
                SearchText.Trim(),
                StringComparison.OrdinalIgnoreCase) ?? false);
    }

    partial void OnSearchTextChanged(string value)
    {
        _brandsView.Refresh();
    }

    public async Task LoadAsync()
    {
        var brands = await _service.GetAllAsync();

        Brands.Clear();

        foreach (var brand in brands)
            Brands.Add(brand);

        _brandsView.Refresh();
    }

    public async Task AddBrandAsync(string brandName)
    {
        brandName = brandName.Trim();

        if (brandName.Length == 0)
            return;

        await _service.AddAsync(new Brand
        {
            BrandName = brandName
        });

        await LoadAsync();
    }

    public async Task UpdateBrandAsync(
        Brand selectedBrand,
        string brandName)
    {
        brandName = brandName.Trim();

        if (brandName.Length == 0)
            return;

        // Load a fresh entity so EF Core updates the current database row.
        var updatedBrand = new Brand
        {
            BrandId = selectedBrand.BrandId,
            BrandName = brandName
        };

        await _service.UpdateAsync(updatedBrand);

        await LoadAsync();
    }

    public async Task DeleteBrandAsync(Brand selectedBrand)
    {
        await _service.DeleteAsync(selectedBrand.BrandId);

        SelectedBrand = null;

        await LoadAsync();
    }
}
