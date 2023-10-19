using AlarmBle.Model;
using AlarmBle.Resources.Localization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace AlarmBle.ViewModel;


public partial class MainPageViewModel : BaseViewModel
{
	AppTheme currentTheme;
	Timer timer;
	IFingerprint fingerprint;

	[ObservableProperty]
	[NotifyPropertyChangedFor( nameof( AlarmStateToggled ) )]
	ICharacteristic alarmState;
	[ObservableProperty]
	bool alarmStateToggled;
	[ObservableProperty]
	bool beepStateToggled;
	[ObservableProperty]
	bool blinkStateToggled;
	[ObservableProperty]
	static string motoImage;
	[ObservableProperty]
	static bool isMotoAnimated;
	[ObservableProperty]
	string statusAlarm;


	public MainPageViewModel( IBluetoothLE bluetoothLE, IAdapter adapter, IFingerprint fingerprint ) : base( bluetoothLE, adapter )
	{
		Application.Current.RequestedThemeChanged += ( sender, args ) =>
		{
			currentTheme = args.RequestedTheme;
			MotoImage = currentTheme is AppTheme.Light ? "moto_light.png" : "moto_dark.png";
		};
		currentTheme = Application.Current.RequestedTheme;
		StatusAlarm = AppResources.LabelStatus_Inactive;
		MotoImage = currentTheme is AppTheme.Light ? "moto_light.png" : "moto_dark.png";
		IsMotoAnimated = false;
		BeepStateToggled = false;
		BlinkStateToggled = false;
		this.fingerprint = fingerprint;
	}
	[RelayCommand]
	async Task Authenticate()
	{
		var request = new AuthenticationRequestConfiguration( AppResources.BiometricAuth_Title, AppResources.BiometricAuth_Reason )
		{
			CancelTitle = AppResources.BiometricCancelOptionText
		};
		var result = await fingerprint.AuthenticateAsync( request );
		if ( result.Authenticated )
		{
			// do secret stuff :)
			await Shell.Current.DisplayAlert( AppResources.DialogAuth_AccessGranted_Title, AppResources.DialogAuth_AccessGranted_Message, "OK" );
		}
		else
		{
			// not allowed to do secret stuff :(
			await Shell.Current.DisplayAlert( AppResources.DialogAuth_AccessDenied_Title, AppResources.DialogAuth_AccessDenied_Message, "OK" );
			Application.Current.Quit();
		}
	}

	async Task GetAlarmState()
	{
		AlarmState = await GetRemoteCharacteristic( RemoteDevice, AlarmServiceUuids.AlarmService, AlarmServiceUuids.State );
		AlarmStateToggled = (bool) await GetRemoteCharacteriticValue( AlarmState );
	}

	[RelayCommand]
	async Task ActivateAlarm()
	{
		if ( AlarmStateToggled )
		{
			await OnToastAsync( AppResources.ToastText_AlreadyActive );
			return;
		}
		AlarmStateToggled = true;
		StatusAlarm = AppResources.LabelStatus_Active;
		AnimateConfigurations( currentTheme, BeepStateToggled, BlinkStateToggled );
	}

	[RelayCommand]
	async Task DeactivateAlarm()
	{
		if ( !AlarmStateToggled )
		{
			await OnToastAsync( AppResources.ToastText_AlreadyInactive );
			return;
		}
		AlarmStateToggled = false;
		StatusAlarm = AppResources.LabelStatus_Inactive;
		AnimateConfigurations( currentTheme, BeepStateToggled, BlinkStateToggled, 2 );
	}

	void AnimateConfigurations(AppTheme theme, bool beep, bool blink, int reapeat = 1 )
	{
		if ( blink || beep )
		{
			if ( blink && beep )
			{
				MotoAnimation( theme, "moto_beep_blink", reapeat );
				return;
			}
			else if ( blink && !beep )
			{
				MotoAnimation( theme, "moto_blink", reapeat );
				return;
			}
			else if ( !blink && beep )
			{
				MotoAnimation( theme, "moto_beep", reapeat );
				return;
			}
		}
	}

	[RelayCommand]
	void ToggleBeep()
	{
		BeepStateToggled = !BeepStateToggled;
		if ( BeepStateToggled )
			MotoAnimation( currentTheme, "moto_beep", 1 );
	}

	[RelayCommand]
	void ToggleBlink()
	{
		BlinkStateToggled = !BlinkStateToggled;
		if ( BlinkStateToggled )
			MotoAnimation( currentTheme, "moto_blink", 1 );


	}
	void MotoAnimation( AppTheme theme, string animation, int reapeat = -1 )
	{
		MotoImage = theme is AppTheme.Light ? animation + "_light.gif" : animation + "_dark.gif";
		IsMotoAnimated = true;
		if ( reapeat == -1 )
			return;
		if ( reapeat > 0 )
			timer = new( ResetAnimation, theme, reapeat * 500, Timeout.Infinite );

	}

	void ResetAnimation( object state )
	{
		var appTheme = (AppTheme) state;
		IsMotoAnimated = false;
		MotoImage = appTheme is AppTheme.Light ? "moto_light.png" : "moto_dark.png";
		timer?.Dispose();
	}

}
