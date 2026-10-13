namespace RetailPOS.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public decimal CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public int StockQuantity { get; set; }

    public int ReorderLevel { get; set; } = 5;

    public bool IsActive { get; set; } = true;
}