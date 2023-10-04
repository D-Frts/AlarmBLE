using AlarmBle.View;

namespace AlarmBle;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute( nameof( MainPage ), typeof( MainPage ) );
        Routing.RegisterRoute( nameof( DeviceSettingsPage ), typeof( DeviceSettingsPage ) );
        Routing.RegisterRoute( nameof( ScannerPage ), typeof( ScannerPage ) );
    }
}