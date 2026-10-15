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
                    // Database
                    services.AddDbContext<PosDbContext>(options =>
                        options.UseSqlServer(
                            context.Configuration
                                .GetConnectionString("DefaultConnection")));

                    // Services
                    services.AddTransient<ProductService>();
                    services.AddTransient<BrandService>();
                    services.AddTransient<CategoryService>();

                    // ViewModels
                    services.AddTransient<ProductViewModel>();

                    // Views
                    services.AddTransient<ProductView>();

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