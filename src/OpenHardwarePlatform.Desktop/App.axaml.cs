using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OpenHardwarePlatform.Abstractions;
using OpenHardwarePlatform.Core;
using OpenHardwarePlatform.Desktop.ViewModels;
using OpenHardwarePlatform.Desktop.Views;
using OpenHardwarePlatform.Transports.Hid;

namespace OpenHardwarePlatform.Desktop;

public class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        services.AddSingleton<DeviceManager>();
        services.AddSingleton<IDeviceManager>(sp => sp.GetRequiredService<DeviceManager>());
        services.AddSingleton<IDeviceTransport, HidTransport>();
        services.AddSingleton<HidTransportService>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<DeviceExplorerViewModel>();
        services.AddSingleton<DeviceInspectorViewModel>();
        services.AddSingleton<ConnectionManagerViewModel>();
        services.AddSingleton<DescriptorExplorerViewModel>();
        services.AddSingleton<HidConsoleViewModel>();
        services.AddSingleton<LiveMonitorViewModel>();
        services.AddSingleton<PacketDecoderViewModel>();
        services.AddSingleton<PacketLoggerViewModel>();
        services.AddSingleton<AutomationViewModel>();
        services.AddSingleton<ReverseEngineeringViewModel>();
        services.AddSingleton<DocumentationGeneratorViewModel>();
        Services = services.BuildServiceProvider();

        var manager = Services.GetRequiredService<DeviceManager>();
        manager.RegisterTransport(Services.GetRequiredService<IDeviceTransport>());
        manager.StartHotPlugMonitoring();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
