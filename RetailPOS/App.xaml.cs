using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RetailPOS.Data;
using RetailPOS.Services;
using RetailPOS.ViewModels;
using RetailPOS.Views;
using System.Windows;

namespace RetailPOS
{
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; } = null!;

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Read the connection string
                    var connectionString =
                        context.Configuration.GetConnectionString(
                            "DefaultConnection");

                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        throw new InvalidOperationException(
                            "Connection string 'DefaultConnection' was not found. " +
                            "Check appsettings.json.");
                    }

                    // Database: create a separate DbContext when needed
                    services.AddDbContextFactory<PosDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    // Services
                    services.AddTransient<ProductService>();
                    services.AddTransient<BrandService>();
                    services.AddTransient<CategoryService>();
                    services.AddTransient<SaleService>();

                    // ViewModels
                    services.AddTransient<ProductViewModel>();
                    services.AddTransient<CategoryViewModel>();
                    services.AddTransient<BrandViewModel>();
                    services.AddTransient<SaleViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<SaleHistoryViewModel>();

                    // Views
                    services.AddTransient<ProductView>();
                    services.AddTransient<BrandView>();
                    services.AddTransient<CategoryView>();
                    services.AddTransient<SaleView>();
                    services.AddTransient<DashboardView>();
                    services.AddTransient<SaleHistoryView>();

                    // Main Window
                    services.AddTransient<MainWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await AppHost.StartAsync();

            var mainWindow =
                AppHost.Services.GetRequiredService<MainWindow>();

            MainWindow = mainWindow;
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            AppHost.Dispose();

            base.OnExit(e);
        }
    }
}