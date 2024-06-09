using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using ToDoList.Configurations;

namespace ToDoList
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = DIConfiguration.CreateHostBuilder().Build();
            DIConfiguration.ServiceProvider = builder.Services;

           //var mainWindow = DIConfiguration.ServiceProvider.GetRequiredService<MainWindow>();
           //mainWindow.Show();
        }
    }

}
