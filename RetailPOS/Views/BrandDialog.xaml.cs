
using System.Windows;

namespace RetailPOS.Views;

public partial class BrandDialog : Window
{
    public string EnteredBrandName { get; private set; } = string.Empty;

    public BrandDialog(string title, string? existingBrandName = null)
    {
        InitializeComponent();

        DialogTitle.Text = title;
        BrandNameTextBox.Text = existingBrandName ?? string.Empty;

        Loaded += (_, _) =>
        {
            BrandNameTextBox.Focus();
            BrandNameTextBox.SelectAll();
        };
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        string name = BrandNameTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            ValidationMessage.Text = "Brand name is required.";
            BrandNameTextBox.Focus();
            return;
        }

        EnteredBrandName = name;
        DialogResult = true;
    }
}
