using RetailPOS.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RetailPOS.Views;

public partial class DashboardView : UserControl
{
    private readonly DashboardViewModel _viewModel;

    public DashboardView(DashboardViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += DashboardView_Loaded;
    }

    private async void DashboardView_Loaded(
        object sender, RoutedEventArgs e)
    {
        Loaded -= DashboardView_Loaded;
        await _viewModel.LoadAsync();
    }
}