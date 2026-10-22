
using RetailPOS.Models;
using RetailPOS.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RetailPOS.Views;

public partial class CategoryView : UserControl
{
    private CategoryViewModel _viewModels;

    public CategoryView(CategoryViewModel _viewModel)
    {
        InitializeComponent();

        _viewModels = _viewModel;
        DataContext = _viewModel;

        Loaded += async (_, _) => await _viewModel.LoadAsync();
    }

    private async void CategoryView_Loaded(
        object sender, RoutedEventArgs e)
    {
        Loaded -= CategoryView_Loaded;

        if (_viewModels is null)
        {
            MessageBox.Show(
                "CategoryViewModel is not assigned to this view.",
                "RetailPOS",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        try
        {
            await _viewModels.LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to load categories.\n\n{ex.Message}",
                "Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void CategoriesDataGrid_LoadingRow(
        object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = e.Row.GetIndex() + 1;
    }

    private async void AddButton_Click(
        object sender, RoutedEventArgs e)
    {
        if (_viewModels is null)
            return;

        var dialog = new CategoryDialog("Add Category")
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            await _viewModels.AddCategoryAsync(
                dialog.EnteredCategoryName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to add category.\n\n{ex.Message}",
                "Save Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void EditButton_Click(
        object sender, RoutedEventArgs e)
    {
        if (_viewModels is null ||
            (sender as FrameworkElement)?.Tag is not Category category)
            return;

        var dialog = new CategoryDialog(
            "Edit Category",
            category.CategoryName)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            await _viewModels.UpdateCategoryAsync(
                category, dialog.EnteredCategoryName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to update category.\n\n{ex.Message}",
                "Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void DeleteButton_Click(
        object sender, RoutedEventArgs e)
    {
        if (_viewModels is null ||
            (sender as FrameworkElement)?.Tag is not Category category)
            return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete '{category.CategoryName}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            await _viewModels.DeleteCategoryAsync(category);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to delete category.\n\n{ex.Message}",
                "Delete Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
