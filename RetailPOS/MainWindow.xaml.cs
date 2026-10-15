using Microsoft.Extensions.DependencyInjection;
using RetailPOS.Views;
using System.Windows;

namespace RetailPOS;

public partial class MainWindow : Window
{
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        NavigateTo("Dashboard");
    }

    private void Navigation_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not System.Windows.Controls.Button button)
            return;

        NavigateTo(button.Tag?.ToString() ?? "Dashboard");
    }

    private void NavigateTo(string page)
    {
        switch (page)
        {
            case "Products":
                PageTitle.Text = "Product Management";
                MainContent.Content =
                    _serviceProvider.GetRequiredService<ProductView>();
                break;

            case "Brands":
                PageTitle.Text = "Brand Management";
                // Register BrandView and load it here.
                MainContent.Content = _serviceProvider.GetRequiredService<BrandView>();
                break;

            case "Categories":
                PageTitle.Text = "Category Management";
                // Register CategoryView and load it here.
                MainContent.Content = _serviceProvider.GetRequiredService<CategoryView>();
                break;

            case "Sales":
                PageTitle.Text = "Point of Sale";
                MainContent.Content =
                     _serviceProvider.GetRequiredService<SaleView>();
                break;

            case "SaleHistory":
                PageTitle.Text = "Sale History";
                MainContent.Content = CreatePlaceholder("Sale History");
                break;

            default:
                PageTitle.Text = "Dashboard";
                MainContent.Content = CreatePlaceholder(
                    "Welcome to RetailPOS");
                break;
        }
    }

    private static UIElement CreatePlaceholder(string message)
    {
        return new System.Windows.Controls.Border
        {
            Margin = new Thickness(25),
            Background = System.Windows.Media.Brushes.White,
            CornerRadius = new CornerRadius(12),
            Child = new System.Windows.Controls.TextBlock
            {
                Text = message,
                FontSize = 24,
                FontWeight = FontWeights.SemiBold,
                Foreground =
                    new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(32, 41, 56)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
    }
}