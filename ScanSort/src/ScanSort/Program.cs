using Microsoft.Extensions.DependencyInjection;
using ScanSort.Commands;
using ScanSort.Infrastructure;
using ScanSort.Presenters;
using ScanSort.Services;
using ScanSort.Views;

namespace ScanSort;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        ConfigureServices(services);

        using var provider = services.BuildServiceProvider();
        var form = provider.GetRequiredService<PageSorterForm>();
        Application.Run(form);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Infrastructure
        services.AddSingleton<IConfigService, ConfigService>();
        services.AddSingleton<CommandHistory>();

        // Services
        services.AddSingleton<IImageService, ImageService>();
        services.AddTransient<IPdfService, PdfService>();
        services.AddSingleton<IDeskewService, DeskewService>();
        services.AddTransient<IExportService, ExportService>();

        // Presenter
        services.AddTransient<PageSorterPresenter>();

        // View
        services.AddTransient<PageSorterForm>();
    }
}
