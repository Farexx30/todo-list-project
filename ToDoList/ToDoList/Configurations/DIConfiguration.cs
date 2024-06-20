using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Reflection;
using ToDoList.Models;
using ToDoList.Models.Entities;
using ToDoList.Services;
using ToDoList.Services.Repositories;
using ToDoList.ViewModels;

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
            //Rejestracja DbContextu:
            services.AddDbContext<ToDoListDbContext>(
                options => options.UseSqlServer(ConfigurationManager.ConnectionStrings["toDoListDatabase"].ConnectionString));

            //Rejestracja AutoMappera:
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            //Rejestracja serwisów/repozytoriów:           
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IUserContextService, UserContextService>();
            services.AddScoped<ILoginUserRepositoryService, UserRepositoryService>();
            services.AddScoped<IRegisterUserRepositoryService, UserRepositoryService>();
            services.AddScoped<ICategoryRepositoryService, CategoryRepositoryService>();
            services.AddScoped<IAssignmentRepositoryService, AssignmentRepositoryService>();
            services.AddScoped<IAssignmentStepRepositoryService, AssignmentStepRepositoryService>();


            services.AddSingleton<Func<Type, BaseViewModel>>(provider =>
                viewModelType => (BaseViewModel)provider.GetRequiredService(viewModelType));

            //Rejestracja zależności:
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            //Rejestracja MainWindow:
            services.AddSingleton(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            //Rejestracja ViewModelów:
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainMenuViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainAppViewModel>();

            return services;
        }
    }
}
