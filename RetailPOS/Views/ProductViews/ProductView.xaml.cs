
using RetailPOS.Models;
using RetailPOS.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RetailPOS.Views;

public partial class ProductView : UserControl
{
    private readonly ProductViewModel _viewModel;

    public ProductView(ProductViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += async (_, _) => await _viewModel.LoadAsync();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ProductDialog(_viewModel)
        {
            Owner = Window.GetWindow(this)
        };

        dialog.ShowDialog();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not Product product)
            return;

        var dialog = new ProductDialog(_viewModel, product)
        {
            Owner = Window.GetWindow(this)
        };

        dialog.ShowDialog();
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not Product product)
            return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete '{product.Name}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            await _viewModel.DeleteProductAsync(product);
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(
                $"Could not delete product: {ex.Message}",
                "Delete Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void ProductsDataGrid_LoadingRow(
        object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
    }
}
