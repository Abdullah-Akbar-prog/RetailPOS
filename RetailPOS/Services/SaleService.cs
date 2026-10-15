
using Microsoft.EntityFrameworkCore;
using RetailPOS.Data;
using RetailPOS.Models;

namespace RetailPOS.Services;

public class SaleService
{
    private readonly IDbContextFactory<PosDbContext> _contextFactory;

    public SaleService(
        IDbContextFactory<PosDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task CreateSaleAsync(Sale sale)
    {
        await using var context =
            await _contextFactory.CreateDbContextAsync();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        foreach (var item in sale.Items)
        {
            var product = await context.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (product is null || !product.IsActive)
            {
                throw new InvalidOperationException(
                    $"Product '{item.ProductName}' is unavailable.");
            }

            if (item.Quantity <= 0 ||
                product.StockQuantity < item.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for {product.Name}. Available: {product.StockQuantity}.");
            }

            // Use the current database price.
            item.ProductName = product.Name;
            item.UnitPrice = product.SellingPrice;
            item.LineTotal = item.Quantity * item.UnitPrice;

            product.StockQuantity -= item.Quantity;
        }

        sale.Subtotal = sale.Items.Sum(item => item.LineTotal);
        sale.GrandTotal = sale.Subtotal - sale.Discount;

        if (sale.Discount < 0 || sale.Discount > sale.Subtotal)
        {
            throw new InvalidOperationException(
                "The discount amount is invalid.");
        }

        if (sale.AmountPaid < sale.GrandTotal)
        {
            throw new InvalidOperationException(
                "Payment is less than the grand total.");
        }

        sale.Change = sale.AmountPaid - sale.GrandTotal;

        context.Sales.Add(sale);

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
