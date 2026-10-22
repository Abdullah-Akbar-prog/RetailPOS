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

        Loaded += ProductView_Loaded;
    }

    private async void ProductView_Loaded(
        object sender, RoutedEventArgs e)
    {
        Loaded -= ProductView_Loaded;
        await _viewModel.LoadAsync();
    }
}