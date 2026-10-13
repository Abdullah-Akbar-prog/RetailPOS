using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RetailPOS.Data;
using System.Windows;

namespace RetailPOS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; } = null!;

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<PosDbContext>(options =>
                        options.UseSqlServer(
                            context.Configuration.GetConnectionString("DefaultConnection")));
                })
                .Build();
        }
    }

}
