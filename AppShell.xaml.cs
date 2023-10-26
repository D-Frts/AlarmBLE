using AlarmBle.View;
using System.Globalization;

namespace AlarmBle;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute( nameof( MainPage ), typeof( MainPage ) );       
        Routing.RegisterRoute( nameof( ScannerPage ), typeof( ScannerPage ) );
		Routing.RegisterRoute( nameof( SettingsPage ), typeof( SettingsPage ) );
		Routing.RegisterRoute( nameof( AppSettingsPage ), typeof( AppSettingsPage ) );
		Routing.RegisterRoute( nameof(AlarmSettingsPage ), typeof( AlarmSettingsPage ) );
		Routing.RegisterRoute( nameof( ChangePasskeyPage ), typeof( ChangePasskeyPage ) );

		var language = Preferences.Get( "language", "en-US" );
		var culture = new CultureInfo( language );
		LocalizationResourceManager.Instance.SetCulture( culture );
		//var theme = Preferences.Get( "theme", AppTheme.Light.ToString() );
		//Application.Current.UserAppTheme = theme is "Light" ? AppTheme.Light : AppTheme.Dark;
	}
}