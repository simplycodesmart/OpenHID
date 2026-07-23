using Avalonia.Controls;
using Avalonia.Interactivity;
using OpenHardwarePlatform.Desktop.ViewModels;

namespace OpenHardwarePlatform.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnNavClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not int index)
            return;

        if (DataContext is MainViewModel vm)
        {
            vm.SelectedNavIndex = index;
        }
    }
}
