
using RetailPOS.Models;
using RetailPOS.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RetailPOS.Views;

public partial class BrandView : UserControl
{
    private BrandViewModel? ViewModel =>
        DataContext as BrandViewModel;

    public BrandView(BrandViewModel brandViewModel)
    {
        InitializeComponent();
        DataContext = brandViewModel;
        Loaded += BrandView_Loaded;
    }

    private async void BrandView_Loaded(
        object sender, RoutedEventArgs e)
    {
        Loaded -= BrandView_Loaded;

        if (ViewModel is null)
        {
            MessageBox.Show(
                "BrandViewModel is not assigned to this view.",
                "RetailPOS",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            await ViewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to load brands.\n\n{ex.Message}",
                "Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void AddButton_Click(
        object sender, RoutedEventArgs e)
    {
        if (ViewModel is null)
            return;

        var dialog = new BrandDialog("Add Brand")
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            await ViewModel.AddBrandAsync(
                dialog.EnteredBrandName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to add brand.\n\n{ex.Message}",
                "Save Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void EditButton_Click(
        object sender, RoutedEventArgs e)
    {
        if (ViewModel is null ||
            (sender as FrameworkElement)?.Tag is not Brand brand)
            return;

        var dialog = new BrandDialog(
            "Edit Brand",
            brand.BrandName)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            await ViewModel.UpdateBrandAsync(
                brand,
                dialog.EnteredBrandName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to update brand.\n\n{ex.Message}",
                "Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void DeleteButton_Click(
        object sender, RoutedEventArgs e)
    {
        if (ViewModel is null ||
            (sender as FrameworkElement)?.Tag is not Brand brand)
            return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete '{brand.BrandName}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            await ViewModel.DeleteBrandAsync(brand);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to delete brand.\n\n{ex.Message}",
                "Delete Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
