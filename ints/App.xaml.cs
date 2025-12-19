using ints.Data;
using ints.Services;
using ints.ViewModels;
using ints.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ints
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            var sc = new ServiceCollection();
            ConfigureServices(sc);
            Services = sc.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
        
            services.AddDbContextFactory<AppDbContext>(opt =>
                opt.UseSqlServer("Data Source=DIROSE;Initial Catalog=ints;Persist Security Info=True;User ID=dirosethd;Password=1612;Encrypt=False"));

            // Сервисы
            services.AddSingleton<ApiClient>();
            services.AddSingleton<AuthApiService>();
            services.AddSingleton<ShipmentsApiService>();
            services.AddSingleton<IDbService, DbService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<CarsViewModel>();
            services.AddTransient<DriversViewModel>();
            services.AddTransient<FuelTypesViewModel>();
            services.AddTransient<ShipmentsViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<RegisterWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var login = Services.GetRequiredService<LoginWindow>();
            bool? ok = login.ShowDialog();

            if (ok == true)
            {
                var main = Services.GetRequiredService<MainWindow>();
                MainWindow = main;
                main.Show();

                ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            else
            {
                Shutdown();
            }
        }
    }
}
