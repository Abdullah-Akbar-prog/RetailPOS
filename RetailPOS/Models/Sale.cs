namespace RetailPOS.Models;

public class Sale
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; } = DateTime.Now;

    public decimal Subtotal { get; set; }

    public decimal Discount { get; set; }

    public decimal GrandTotal { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal Change { get; set; }

    public ICollection<SaleItem> Items { get; set; }
        = new List<SaleItem>();
}