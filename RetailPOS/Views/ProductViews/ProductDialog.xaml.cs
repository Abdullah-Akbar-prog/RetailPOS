
using RetailPOS.Models;
using RetailPOS.ViewModels;
using System.Globalization;
using System.Windows;

namespace RetailPOS.Views;

public partial class ProductDialog : Window
{
    private readonly ProductViewModel _viewModel;
    private readonly Product? _product;

    public ProductDialog(ProductViewModel viewModel, Product? product = null)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _product = product;

        BrandComboBox.ItemsSource = _viewModel.Brands;
        CategoryComboBox.ItemsSource = _viewModel.Categories;

        if (_product is null)
        {
            DialogHeading.Text = "Add New Product";
            ActiveCheckBox.IsChecked = true;
            ReorderLevelTextBox.Text = "5";
        }
        else
        {
            DialogHeading.Text = "Edit Product";

            NameTextBox.Text = _product.Name;
            SkuTextBox.Text = _product.SKU;
            CostPriceTextBox.Text = _product.CostPrice.ToString(CultureInfo.CurrentCulture);
            SellingPriceTextBox.Text = _product.SellingPrice.ToString(CultureInfo.CurrentCulture);
            StockQuantityTextBox.Text = _product.StockQuantity.ToString();
            ReorderLevelTextBox.Text = _product.ReorderLevel.ToString();
            ActiveCheckBox.IsChecked = _product.IsActive;

            BrandComboBox.SelectedItem = _viewModel.Brands
                .FirstOrDefault(b => b.BrandId == _product.BrandId);

            CategoryComboBox.SelectedItem = _viewModel.Categories
                .FirstOrDefault(c => c.CategoryId == _product.CategoryId);
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        ValidationMessage.Text = string.Empty;

        if (!decimal.TryParse(CostPriceTextBox.Text, NumberStyles.Number,
                CultureInfo.CurrentCulture, out var costPrice)
            || !decimal.TryParse(SellingPriceTextBox.Text, NumberStyles.Number,
                CultureInfo.CurrentCulture, out var sellingPrice)
            || !int.TryParse(StockQuantityTextBox.Text, out var stockQuantity)
            || !int.TryParse(ReorderLevelTextBox.Text, out var reorderLevel))
        {
            ValidationMessage.Text = "Enter valid numeric values for prices and quantities.";
            return;
        }

        bool saved;

        try
        {
            if (_product is null)
            {
                saved = await _viewModel.AddProductAsync(
                    NameTextBox.Text,
                    SkuTextBox.Text,
                    costPrice,
                    sellingPrice,
                    stockQuantity,
                    reorderLevel,
                    ActiveCheckBox.IsChecked == true,
                    BrandComboBox.SelectedItem as Brand,
                    CategoryComboBox.SelectedItem as Category);
            }
            else
            {
                saved = await _viewModel.UpdateProductAsync(
                    _product,
                    NameTextBox.Text,
                    SkuTextBox.Text,
                    costPrice,
                    sellingPrice,
                    stockQuantity,
                    reorderLevel,
                    ActiveCheckBox.IsChecked == true,
                    BrandComboBox.SelectedItem as Brand,
                    CategoryComboBox.SelectedItem as Category);
            }

            if (!saved)
            {
                ValidationMessage.Text =
                    "Complete all required fields and enter non-negative numbers.";
                return;
            }

            DialogResult = true;
        }
        catch (Exception ex)
        {
            ValidationMessage.Text = $"Could not save product: {ex.Message}";
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
