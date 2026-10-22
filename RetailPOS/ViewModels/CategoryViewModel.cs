
using CommunityToolkit.Mvvm.ComponentModel;
using RetailPOS.Models;
using RetailPOS.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace RetailPOS.ViewModels;

public partial class CategoryViewModel : ObservableObject
{
    private readonly CategoryService _service;
    private readonly ICollectionView _categoriesView;

    public ObservableCollection<Category> Categories { get; } = new();

    public ICollectionView CategoriesView => _categoriesView;

    [ObservableProperty]
    private Category? selectedCategory;

    [ObservableProperty]
    private string searchText = string.Empty;

    public CategoryViewModel(CategoryService service)
    {
        _service = service;

        _categoriesView = CollectionViewSource.GetDefaultView(Categories);
        _categoriesView.Filter = FilterCategory;
    }

    private bool FilterCategory(object item)
    {
        if (item is not Category category)
            return false;

        return string.IsNullOrWhiteSpace(SearchText)
            || (category.CategoryName?.Contains(
                SearchText.Trim(),
                StringComparison.OrdinalIgnoreCase) ?? false);
    }

    partial void OnSearchTextChanged(string value)
    {
        _categoriesView.Refresh();
    }

    public async Task LoadAsync()
    {
        var categories = await _service.GetAllAsync();

        Categories.Clear();

        foreach (var category in categories)
            Categories.Add(category);

        _categoriesView.Refresh();
    }

    public async Task AddCategoryAsync(string categoryName)
    {
        categoryName = categoryName.Trim();

        if (categoryName.Length == 0)
            return;

        await _service.AddAsync(new Category
        {
            CategoryName = categoryName
        });

        await LoadAsync();
    }

    public async Task UpdateCategoryAsync(
        Category selectedCategory, string categoryName)
    {
        categoryName = categoryName.Trim();

        if (categoryName.Length == 0)
            return;

        var updatedCategory = new Category
        {
            CategoryId = selectedCategory.CategoryId,
            CategoryName = categoryName
        };

        await _service.UpdateAsync(updatedCategory);
        await LoadAsync();
    }

    public async Task DeleteCategoryAsync(Category category)
    {
        await _service.DeleteAsync(category.CategoryId);

        SelectedCategory = null;
        await LoadAsync();
    }
}
