using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using ToDoList.Models;
using ToDoList.ViewModels;
using ToDoList.Views;

namespace ToDoList.Configurations
{
    public static class DIConfiguration
    {
        public static IServiceProvider? ServiceProvider { get; set; }

        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
               .ConfigureServices((context, services) =>
               {
                   services.RegisterServices();
               });

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            //Rejestracja DbContext:
            services.AddDbContext<ToDoListDbContext>(
                options => options.UseSqlServer(ConfigurationManager.ConnectionStrings["toDoListDatabase"].ConnectionString));


            services.AddSingleton(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainMenuViewModel>()
            });

            services.AddTransient<MainMenuViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainAppViewModel>();

            return services;
        }
    }
}
