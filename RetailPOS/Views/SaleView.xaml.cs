using RetailPOS.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RetailPOS.Views;

public partial class SaleView : UserControl
{
    private readonly SaleViewModel _viewModel;

    public SaleView(SaleViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += SaleView_Loaded;
    }

    private async void SaleView_Loaded(
        object sender, RoutedEventArgs e)
    {
        Loaded -= SaleView_Loaded;
        await _viewModel.LoadAsync();
    }
}