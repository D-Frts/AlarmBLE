using AlarmBle.Resources.Localization;
using AlarmBle.View;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE.Abstractions.Contracts;
using System.Globalization;


namespace AlarmBle.ViewModel;

public partial class AppSettingsViewModel : BaseViewModel
{
	public AppSettingsViewModel( IBluetoothLE bluetoothLE, IAdapter adapter ) : base( bluetoothLE, adapter )
	{
		IsBiometrics = Preferences.Get( "biometrics", false );
	}
	//[RelayCommand]
	//async Task SetAppTheme()
	//{
	//	string title = LocalizationResourceManager.Instance["DialogTheme_Title"].ToString();
	//	string cancelBtn = LocalizationResourceManager.Instance["CancelText"].ToString(); ;
	//	string lightTheme = LocalizationResourceManager.Instance["DialogTheme_OptionLight"].ToString();
	//	string darkTheme = LocalizationResourceManager.Instance["DialogTheme_OptionDark"].ToString();
	//	string systemTheme = LocalizationResourceManager.Instance["DialogTheme_OptionSystem"].ToString();

	//	var result = await Shell.Current.DisplayActionSheet( title, cancelBtn, null, systemTheme, lightTheme, darkTheme );

	//	AppTheme theme;
	//	switch ( result )
	//	{
	//		case "System (Default)":
	//		case "Sistema (Padrão)":
	//		theme = Application.Current.PlatformAppTheme;
	//		break;
	//		case "Light":
	//		case "Claro":
	//		theme = AppTheme.Light;
	//		break;
	//		case "Dark":
	//		case "Escuro":
	//		theme = AppTheme.Dark;
	//		break;
	//		default:
	//		return;
	//	}
	//	Application.Current.UserAppTheme = theme;
	//	Preferences.Set( "theme", Application.Current.UserAppTheme.ToString() );
	//}

	[RelayCommand]
	async Task SetAppBiometrics( ToggledEventArgs e )
	{
		Preferences.Set( "biometrics", e.Value );
		IsBiometrics = e.Value;
		await Task.CompletedTask;
	}

	[RelayCommand]
	async Task SetAppLanguage()
	{
		string title = LocalizationResourceManager.Instance["DialogLanguage_Title"].ToString();
		string cancelBtn = LocalizationResourceManager.Instance["CancelText"].ToString(); ;
		string ptBr = LocalizationResourceManager.Instance["DialogLanguage_OptionPtBR"].ToString();
		string enUS = LocalizationResourceManager.Instance["DialogLanguage_OptionEnUS"].ToString();


		var result = await Shell.Current.DisplayActionSheet( title, cancelBtn, null, enUS, ptBr );
		CultureInfo newCulture;
		var oldCulture = Preferences.Get( "language", "en-US" );

		switch ( result )
		{
			case "English USA":
			newCulture = new CultureInfo( "en-US" );
			break;
			case "Português Brasil":
			newCulture = new CultureInfo( "pt-BR" );
			break;
			default:
			return;
		}
		if ( newCulture.Name == oldCulture ) return;
		LocalizationResourceManager.Instance.SetCulture( newCulture );
		Preferences.Set( "language", newCulture.Name );
		await Shell.Current.GoToAsync( $"///{nameof( MainPage )}", true );


	}
}
