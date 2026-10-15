using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailPOS.Models;
using RetailPOS.Services;
using System.Collections.ObjectModel;

namespace RetailPOS.ViewModels;

public partial class ProductViewModel : ObservableObject
{
    private readonly ProductService _productService;
    private readonly BrandService _brandService;
    private readonly CategoryService _categoryService;

    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<Brand> Brands { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();

    [ObservableProperty]
    private Product? selectedProduct;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string sku = string.Empty;

    [ObservableProperty]
    private decimal costPrice;

    [ObservableProperty]
    private decimal sellingPrice;

    [ObservableProperty]
    private int stockQuantity;

    [ObservableProperty]
    private int reorderLevel = 5;

    [ObservableProperty]
    private bool isActive = true;

    [ObservableProperty]
    private Brand? selectedBrand;

    [ObservableProperty]
    private Category? selectedCategory;

    public ProductViewModel(
        ProductService productService,
        BrandService brandService,
        CategoryService categoryService)
    {
        _productService = productService;
        _brandService = brandService;
        _categoryService = categoryService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        var products = await _productService.GetAllAsync();
        var brands = await _brandService.GetAllAsync();
        var categories = await _categoryService.GetAllAsync();

        Products.Clear();
        foreach (var product in products)
            Products.Add(product);

        Brands.Clear();
        foreach (var brand in brands)
            Brands.Add(brand);

        Categories.Clear();
        foreach (var category in categories)
            Categories.Add(category);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) ||
            string.IsNullOrWhiteSpace(Sku) ||
            SelectedBrand is null ||
            SelectedCategory is null)
        {
            return;
        }

        if (CostPrice < 0 ||
            SellingPrice < 0 ||
            StockQuantity < 0 ||
            ReorderLevel < 0)
        {
            return;
        }

        if (SelectedProduct is null)
        {
            var product = new Product
            {
                Name = Name.Trim(),
                SKU = Sku.Trim(),
                CostPrice = CostPrice,
                SellingPrice = SellingPrice,
                StockQuantity = StockQuantity,
                ReorderLevel = ReorderLevel,
                IsActive = IsActive,
                Brand = SelectedBrand,
                Category = SelectedCategory
            };

            await _productService.AddAsync(product);
        }
        else
        {
            SelectedProduct.Name = Name.Trim();
            SelectedProduct.SKU = Sku.Trim();
            SelectedProduct.CostPrice = CostPrice;
            SelectedProduct.SellingPrice = SellingPrice;
            SelectedProduct.StockQuantity = StockQuantity;
            SelectedProduct.ReorderLevel = ReorderLevel;
            SelectedProduct.IsActive = IsActive;
            SelectedProduct.Brand = SelectedBrand;
            SelectedProduct.Category = SelectedCategory;

            await _productService.UpdateAsync(SelectedProduct);
        }

        Clear();
        await LoadAsync();
    }

    [RelayCommand]
    private void Edit()
    {
        if (SelectedProduct is null)
            return;

        Name = SelectedProduct.Name;
        Sku = SelectedProduct.SKU;
        CostPrice = SelectedProduct.CostPrice;
        SellingPrice = SelectedProduct.SellingPrice;
        StockQuantity = SelectedProduct.StockQuantity;
        ReorderLevel = SelectedProduct.ReorderLevel;
        IsActive = SelectedProduct.IsActive;

        SelectedBrand = Brands.FirstOrDefault(
            b => b.BrandId == SelectedProduct.Brand.BrandId);

        SelectedCategory = Categories.FirstOrDefault(
            c => c.CategoryId == SelectedProduct.Category.CategoryId);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedProduct is null)
            return;

        await _productService.DeleteAsync(SelectedProduct.Id);

        Clear();
        await LoadAsync();
    }

    [RelayCommand]
    private void Clear()
    {
        SelectedProduct = null;
        Name = string.Empty;
        Sku = string.Empty;
        CostPrice = 0;
        SellingPrice = 0;
        StockQuantity = 0;
        ReorderLevel = 5;
        IsActive = true;
        SelectedBrand = null;
        SelectedCategory = null;
    }
}