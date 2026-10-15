
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailPOS.Models;
using RetailPOS.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RetailPOS.ViewModels;

public partial class SaleViewModel : ObservableObject
{
    private readonly ProductService _productService;
    private readonly SaleService _saleService;

    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<SaleItem> CartItems { get; } = new();

    [ObservableProperty]
    private Product? selectedProduct;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GrandTotal))]
    [NotifyPropertyChangedFor(nameof(Change))]
    private decimal discount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Change))]
    private decimal amountPaid;

    [ObservableProperty]
    private int quantity = 1;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public decimal Subtotal =>
        CartItems.Sum(item => item.LineTotal);

    public decimal GrandTotal =>
        Math.Max(0, Subtotal - Discount);

    public decimal Change =>
        Math.Max(0, AmountPaid - GrandTotal);

    public SaleViewModel(
        ProductService productService,
        SaleService saleService)
    {
        _productService = productService;
        _saleService = saleService;

        CartItems.CollectionChanged += CartItems_CollectionChanged;
    }

    private void CartItems_CollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(Change));
    }

    public async Task LoadAsync()
    {
        var products = await _productService.GetAllAsync();

        Products.Clear();

        foreach (var product in products.Where(
                     p => p.IsActive && p.StockQuantity > 0))
        {
            Products.Add(product);
        }
    }

    [RelayCommand]
    private void AddToCart()
    {
        StatusMessage = string.Empty;

        if (SelectedProduct is null)
        {
            StatusMessage = "Please select a product.";
            return;
        }

        if (Quantity <= 0)
        {
            StatusMessage = "Quantity must be greater than zero.";
            return;
        }

        var existing = CartItems.FirstOrDefault(
            item => item.ProductId == SelectedProduct.Id);

        int alreadyAdded = existing?.Quantity ?? 0;

        if (Quantity + alreadyAdded > SelectedProduct.StockQuantity)
        {
            StatusMessage = "Not enough stock available.";
            return;
        }

        if (existing is not null)
        {
            existing.Quantity += Quantity;
            existing.LineTotal =
                existing.Quantity * existing.UnitPrice;

            // Refresh the grid after updating an item.
            var index = CartItems.IndexOf(existing);
            CartItems[index] = existing;
        }
        else
        {
            CartItems.Add(new SaleItem
            {
                ProductId = SelectedProduct.Id,
                ProductName = SelectedProduct.Name,
                Quantity = Quantity,
                UnitPrice = SelectedProduct.SellingPrice,
                LineTotal = Quantity * SelectedProduct.SellingPrice
            });
        }

        Quantity = 1;
        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(Change));
    }

    [RelayCommand]
    private void RemoveItem(SaleItem? item)
    {
        if (item is null)
            return;

        CartItems.Remove(item);

        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(Change));
    }

    [RelayCommand]
    private void ClearCart()
    {
        CartItems.Clear();
        Discount = 0;
        AmountPaid = 0;
        StatusMessage = string.Empty;
    }

    [RelayCommand]
    private async Task CheckoutAsync()
    {
        StatusMessage = string.Empty;

        if (CartItems.Count == 0)
        {
            StatusMessage = "The cart is empty.";
            return;
        }

        if (Discount < 0 || Discount > Subtotal)
        {
            StatusMessage = "Enter a valid discount amount.";
            return;
        }

        if (AmountPaid < GrandTotal)
        {
            StatusMessage = "Amount paid is less than the grand total.";
            return;
        }

        var sale = new Sale
        {
            InvoiceNumber = $"INV-{DateTime.Now:yyyyMMddHHmmssfff}",
            SaleDate = DateTime.Now,
            Subtotal = Subtotal,
            Discount = Discount,
            GrandTotal = GrandTotal,
            AmountPaid = AmountPaid,
            Change = Change,
            Items = CartItems.Select(item => new SaleItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.LineTotal
            }).ToList()
        };

        try
        {
            await _saleService.CreateSaleAsync(sale);

            StatusMessage = $"Sale completed. Invoice: {sale.InvoiceNumber}";

            ClearCart();
            await LoadAsync();
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }
        catch (Exception)
        {
            StatusMessage = "Could not complete the sale. Please check the database and try again.";
        }
    }
}
