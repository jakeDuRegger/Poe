using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poe.Helpers.Dependency_Injection;

namespace Poe;

/// <summary>
/// Interaction logic for App.xaml. This class is the entry point for your WPF application.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Provides a central mechanism to access service objects.
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// This method is called when the application starts up.
    /// It configures services and registers them with the dependency injection container.
    /// </summary>
    /// <param name="e">Contains the arguments for the startup event.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        // Create a new service collection for registering services.
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        // Build the service provider from the service collection.
        ServiceProvider = serviceCollection.BuildServiceProvider();

        base.OnStartup(e);
    }

    /// <summary>
    /// Configures services to be used in the application by registering them with the DI container.
    /// </summary>
    /// <param name="services">The collection of services to configure.</param>
    private void ConfigureServices(IServiceCollection services)
    {
        // Configure application settings by specifying where the configuration files are found.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Sets the base path for the configuration files.
            .AddJsonFile("config.json", optional: true, reloadOnChange: true) // Adds the JSON configuration file.
            .Build();

        // Register the configuration instance as a singleton service.
        services.AddSingleton<IConfiguration>(configuration);
        // Register the configuration service as a singleton so it can be reused throughout the application.
        services.AddSingleton<ConfigurationService>();
    }
}
