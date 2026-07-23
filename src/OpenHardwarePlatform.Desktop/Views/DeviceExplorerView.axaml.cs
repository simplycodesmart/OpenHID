using Avalonia.Controls;
using Avalonia.Input;
using OpenHardwarePlatform.Desktop.Models;
using OpenHardwarePlatform.Desktop.ViewModels;

namespace OpenHardwarePlatform.Desktop.Views;

public partial class DeviceExplorerView : UserControl
{
    public DeviceExplorerView()
    {
        InitializeComponent();
    }

    private void OnDeviceCardPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border border || border.Tag is not DeviceInfoModel device)
            return;

        if (App.Services.GetService(typeof(MainViewModel)) is MainViewModel main)
        {
            main.SelectedDevice = device;
            main.SelectedNavIndex = 1;
        }
    }
}
