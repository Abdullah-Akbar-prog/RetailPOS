using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailPOS.Models;
using RetailPOS.Services;
using System.Collections.ObjectModel;

namespace RetailPOS.ViewModels;

public partial class CategoryViewModel : ObservableObject
{
    private readonly CategoryService _service;

    public ObservableCollection<Category> Categories { get; } = new();

    [ObservableProperty]
    private Category? selectedCategory;

    [ObservableProperty]
    private string categoryName = string.Empty;

    public CategoryViewModel(CategoryService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        var categories = await _service.GetAllAsync();

        Categories.Clear();
        foreach (var category in categories)
            Categories.Add(category);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(CategoryName))
            return;

        if (SelectedCategory is null)
        {
            await _service.AddAsync(new Category
            {
                CategoryName = CategoryName.Trim()
            });
        }
        else
        {
            SelectedCategory.CategoryName = CategoryName.Trim();
            await _service.UpdateAsync(SelectedCategory);
        }

        Clear();
        await LoadAsync();
    }

    [RelayCommand]
    private void Edit()
    {
        if (SelectedCategory is not null)
            CategoryName = SelectedCategory.CategoryName;
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedCategory is null)
            return;

        await _service.DeleteAsync(SelectedCategory.CategoryId);

        Clear();
        await LoadAsync();
    }

    [RelayCommand]
    private void Clear()
    {
        SelectedCategory = null;
        CategoryName = string.Empty;
    }
}