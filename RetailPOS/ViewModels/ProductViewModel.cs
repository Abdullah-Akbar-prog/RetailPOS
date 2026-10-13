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
    private string productName = string.Empty;

    [ObservableProperty]
    private string sku = string.Empty;

    [ObservableProperty]
    private decimal purchasePrice;

    [ObservableProperty]
    private decimal salePrice;

    [ObservableProperty]
    private int stockQuantity;

    [ObservableProperty]
    private int minimumStockLevel = 5;

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
        if (string.IsNullOrWhiteSpace(ProductName) ||
            string.IsNullOrWhiteSpace(SKU) ||
            SelectedBrand is null ||
            SelectedCategory is null)
        {
            return;
        }

        if (PurchasePrice < 0 || SalePrice < 0 ||
            StockQuantity < 0 || MinimumStockLevel < 0)
        {
            return;
        }

        if (SelectedProduct is null)
        {
            var product = new Product
            {
                ProductName = ProductName.Trim(),
                SKU = SKU.Trim(),
                PurchasePrice = PurchasePrice,
                SalePrice = SalePrice,
                StockQuantity = StockQuantity,
                MinimumStockLevel = MinimumStockLevel,
                BrandId = SelectedBrand.BrandId,
                CategoryId = SelectedCategory.CategoryId
            };

            await _productService.AddAsync(product);
        }
        else
        {
            SelectedProduct.ProductName = ProductName.Trim();
            SelectedProduct.SKU = SKU.Trim();
            SelectedProduct.PurchasePrice = PurchasePrice;
            SelectedProduct.SalePrice = SalePrice;
            SelectedProduct.StockQuantity = StockQuantity;
            SelectedProduct.MinimumStockLevel = MinimumStockLevel;
            SelectedProduct.BrandId = SelectedBrand.BrandId;
            SelectedProduct.CategoryId = SelectedCategory.CategoryId;

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

        ProductName = SelectedProduct.ProductName;
        SKU = SelectedProduct.SKU;
        PurchasePrice = SelectedProduct.PurchasePrice;
        SalePrice = SelectedProduct.SalePrice;
        StockQuantity = SelectedProduct.StockQuantity;
        MinimumStockLevel = SelectedProduct.MinimumStockLevel;

        SelectedBrand = Brands.FirstOrDefault(
            b => b.BrandId == SelectedProduct.BrandId);

        SelectedCategory = Categories.FirstOrDefault(
            c => c.CategoryId == SelectedProduct.CategoryId);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedProduct is null)
            return;

        await _productService.DeleteAsync(SelectedProduct.ProductId);
        Clear();
        await LoadAsync();
    }

    [RelayCommand]
    private void Clear()
    {
        SelectedProduct = null;
        ProductName = string.Empty;
        SKU = string.Empty;
        PurchasePrice = 0;
        SalePrice = 0;
        StockQuantity = 0;
        MinimumStockLevel = 5;
        SelectedBrand = null;
        SelectedCategory = null;
    }
}