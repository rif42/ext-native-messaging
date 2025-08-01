using System.Configuration;
using System.Data;
using System.Windows;

namespace desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        // Set shutdown mode to close when main window closes
        ShutdownMode = ShutdownMode.OnMainWindowClose;
    }
}

