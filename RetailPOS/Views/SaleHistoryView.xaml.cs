using RetailPOS.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RetailPOS.Views;

public partial class SaleHistoryView : UserControl
{
    private readonly SaleHistoryViewModel _viewModel;

    public SaleHistoryView(SaleHistoryViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += SaleHistoryView_Loaded;
    }

    private async void SaleHistoryView_Loaded(
        object sender, RoutedEventArgs e)
    {
        Loaded -= SaleHistoryView_Loaded;
        await _viewModel.LoadAsync();
    }
}