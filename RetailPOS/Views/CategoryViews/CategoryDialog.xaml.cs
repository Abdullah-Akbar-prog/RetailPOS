
using System.Windows;

namespace RetailPOS.Views;

public partial class CategoryDialog : Window
{
    public string EnteredCategoryName { get; private set; } = string.Empty;

    public CategoryDialog(
        string title,
        string? existingCategoryName = null)
    {
        InitializeComponent();

        DialogTitle.Text = title;
        CategoryNameTextBox.Text = existingCategoryName ?? string.Empty;

        Loaded += (_, _) =>
        {
            CategoryNameTextBox.Focus();
            CategoryNameTextBox.SelectAll();
        };
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        string name = CategoryNameTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            ValidationMessage.Text = "Category name is required.";
            CategoryNameTextBox.Focus();
            return;
        }

        EnteredCategoryName = name;
        DialogResult = true;
    }
}
