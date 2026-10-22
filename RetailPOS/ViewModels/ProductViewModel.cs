
using CommunityToolkit.Mvvm.ComponentModel;
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

    public ProductViewModel(
        ProductService productService,
        BrandService brandService,
        CategoryService categoryService)
    {
        _productService = productService;
        _brandService = brandService;
        _categoryService = categoryService;
    }

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

    public async Task<bool> AddProductAsync(
        string name,
        string sku,
        decimal costPrice,
        decimal sellingPrice,
        int stockQuantity,
        int reorderLevel,
        bool isActive,
        Brand? brand,
        Category? category)
    {
        if (!IsValid(name, sku, costPrice, sellingPrice,
                stockQuantity, reorderLevel, brand, category))
            return false;

        var product = new Product
        {
            Name = name.Trim(),
            SKU = sku.Trim(),
            CostPrice = costPrice,
            SellingPrice = sellingPrice,
            StockQuantity = stockQuantity,
            ReorderLevel = reorderLevel,
            IsActive = isActive,
            BrandId = brand!.BrandId,
            CategoryId = category!.CategoryId
        };

        await _productService.AddAsync(product);
        await LoadAsync();

        return true;
    }

    public async Task<bool> UpdateProductAsync(
        Product product,
        string name,
        string sku,
        decimal costPrice,
        decimal sellingPrice,
        int stockQuantity,
        int reorderLevel,
        bool isActive,
        Brand? brand,
        Category? category)
    {
        if (!IsValid(name, sku, costPrice, sellingPrice,
                stockQuantity, reorderLevel, brand, category))
            return false;

        product.Name = name.Trim();
        product.SKU = sku.Trim();
        product.CostPrice = costPrice;
        product.SellingPrice = sellingPrice;
        product.StockQuantity = stockQuantity;
        product.ReorderLevel = reorderLevel;
        product.IsActive = isActive;
        product.BrandId = brand!.BrandId;
        product.CategoryId = category!.CategoryId;

        await _productService.UpdateAsync(product);
        await LoadAsync();

        return true;
    }

    public async Task DeleteProductAsync(Product product)
    {
        await _productService.DeleteAsync(product.Id);
        await LoadAsync();
    }

    private static bool IsValid(
        string name,
        string sku,
        decimal costPrice,
        decimal sellingPrice,
        int stockQuantity,
        int reorderLevel,
        Brand? brand,
        Category? category)
    {
        return !string.IsNullOrWhiteSpace(name)
            && !string.IsNullOrWhiteSpace(sku)
            && costPrice >= 0
            && sellingPrice >= 0
            && stockQuantity >= 0
            && reorderLevel >= 0
            && brand is not null
            && category is not null;
    }
}
