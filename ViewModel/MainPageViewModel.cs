using AlarmBle.Extensions;
using AlarmBle.Model;
using AlarmBle.Resources.Localization;
using AlarmBle.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System.Globalization;

namespace AlarmBle.ViewModel;

[QueryProperty("RemoteDevice", "RemoteDevice")]
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
	[NotifyPropertyChangedFor( nameof( BeepStateToggled ) )]
	ICharacteristic beepState;
	[ObservableProperty]
	bool beepStateToggled;

	[ObservableProperty]
	[NotifyPropertyChangedFor( nameof( BlinkStateToggled ) )]
	ICharacteristic blinkState;
	[ObservableProperty]
	bool blinkStateToggled;

	[ObservableProperty]
	static string bikeImage;

	[ObservableProperty]
	static bool isBikeAnimated;

	[ObservableProperty]
	string statusAlarm;


	public MainPageViewModel( IBluetoothLE bluetoothLE, IAdapter adapter, IFingerprint fingerprint ) : base( bluetoothLE, adapter )
	{
		this.fingerprint = fingerprint;
		Application.Current.RequestedThemeChanged += ( sender, args ) =>
		{
			currentTheme = args.RequestedTheme;
			BikeImage = currentTheme is AppTheme.Light ? "moto_light.png" : "moto_dark.png";
		};
		currentTheme = Application.Current.RequestedTheme;
		StatusAlarm = LocalizationResourceManager.Instance["LabelStatus_Inactive"].ToString();
		BikeImage = currentTheme is AppTheme.Light ? "moto_light.png" : "moto_dark.png";
		IsBikeAnimated = false;
		BeepStateToggled = false;
		BlinkStateToggled = false;

	}
	[RelayCommand]
	async Task Authenticate()
	{
		IsBiometrics = Preferences.Get( "biometrics", false );
		if ( !IsBiometrics ) return;

		var request = new AuthenticationRequestConfiguration(
			AppResources.BiometricAuth_Title,
			AppResources.BiometricAuth_Reason )
		{
			CancelTitle = AppResources.BiometricCancelOptionText
		};
		var result = await fingerprint.AuthenticateAsync( request );
		if ( result.Authenticated )
		{
			// do secret stuff :)
			await Shell.Current.DisplayAlert(
				AppResources.DialogAuth_AccessGranted_Title,
				AppResources.DialogAuth_AccessGranted_Message,
				"OK" );

			if ( IsNotConnected )
			{
				var alertResult = await Shell.Current.DisplayAlert(
					AppResources.DialogConnection_Title,
					AppResources.DialogConnection_Message,
					AppResources.DialogButtonText_Yes,
					AppResources.DialogButtonText_No );

				if ( !alertResult ) return;

				await Shell.Current.GoToAsync( nameof( ScannerPage ), true );

			}
		}
		else
		{
			// not allowed to do secret stuff :(
			await Shell.Current.DisplayAlert( AppResources.DialogAuth_AccessDenied_Title, AppResources.DialogAuth_AccessDenied_Message, "OK" );
			Application.Current.Quit();
		}
	}

	async Task<bool> SetupCharacteristic( ICharacteristic characteristic )
	{
		await characteristic.ReadAsync();
		characteristic.ValueUpdated += Characteristic_ValueUpdated;
		await characteristic.StartUpdatesAsync();
		return (bool) await GetRemoteCharacteriticValue( characteristic );
	}
	[RelayCommand]
	async Task GetCharacteristicsState()
	{
		try
		{
			IsBusy = true;
			if ( RemoteDevice?.Device.State is Plugin.BLE.Abstractions.DeviceState.Connected )
			{
				AlarmState = await GetRemoteCharacteristic(
					RemoteDevice,
					AlarmServiceUuids.AlarmService,
					AlarmServiceUuids.State );
				AlarmStateToggled = await SetupCharacteristic( AlarmState );

				BeepState = await GetRemoteCharacteristic(
					RemoteDevice,
					AlarmServiceUuids.AlarmService,
					AlarmServiceUuids.Siren );
				BeepStateToggled = await SetupCharacteristic( BeepState );

				BlinkState = await GetRemoteCharacteristic(
					RemoteDevice,
					AlarmServiceUuids.AlarmService,
					AlarmServiceUuids.Blinkers );
				BlinkStateToggled = await SetupCharacteristic( BlinkState );
			}
		}
		catch ( Exception )
		{

			throw;
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void Characteristic_ValueUpdated( object sender, CharacteristicUpdatedEventArgs e )
	{
		switch ( e.Characteristic.Uuid )
		{
			case AlarmServiceUuids.State:
			AlarmState = e.Characteristic;
			AlarmStateToggled = (bool) GetRemoteCharacteriticValue( AlarmState ).Result;
			break;
			case AlarmServiceUuids.Siren:
			BeepState = e.Characteristic;
			BeepStateToggled = (bool) GetRemoteCharacteriticValue( BeepState ).Result;
			break;
			case AlarmServiceUuids.Blinkers:
			BlinkState = e.Characteristic;
			BlinkStateToggled = (bool) GetRemoteCharacteriticValue( BlinkState ).Result;
			break;
			default:
			break;
		}

	}

	[RelayCommand]
	async Task ActivateAlarm()
	{
		if ( AlarmStateToggled )
		{
			await OnToastAsync( AppResources.ToastText_AlreadyActive );
			return;
		}
		AnimateConfigurations( currentTheme, BeepStateToggled, BlinkStateToggled );
		await WriteRemoteCharacteristicValue( AlarmState );
		StatusAlarm = LocalizationResourceManager.Instance["LabelStatus_Active"].ToString();
		//AnimateConfigurations( currentTheme, BeepStateToggled, BlinkStateToggled );
	}

	[RelayCommand]
	async Task DeactivateAlarm()
	{
		if ( !AlarmStateToggled )
		{
			await OnToastAsync( AppResources.ToastText_AlreadyInactive );
			return;
		}
		AnimateConfigurations( currentTheme, BeepStateToggled, BlinkStateToggled, 2 );
		await WriteRemoteCharacteristicValue( AlarmState );
		StatusAlarm = LocalizationResourceManager.Instance["LabelStatus_Inactive"].ToString();
		//AnimateConfigurations( currentTheme, BeepStateToggled, BlinkStateToggled, 2 );
	}

	void AnimateConfigurations( AppTheme theme, bool beep, bool blink, int reapeat = 1 )
	{
		if ( blink || beep )
		{
			if ( blink && beep )
			{
				BikeAnimation( theme, "moto_beep_blink", reapeat );
				return;
			}
			else if ( blink && !beep )
			{
				BikeAnimation( theme, "moto_blink", reapeat );
				return;
			}
			else if ( !blink && beep )
			{
				BikeAnimation( theme, "moto_beep", reapeat );
				return;
			}
		}
	}

	[RelayCommand]
	async Task ToggleBeep()
	{
		if ( !BeepStateToggled )
			BikeAnimation( currentTheme, "moto_beep", 1 );
		await WriteRemoteCharacteristicValue( BeepState );
		//if ( BeepStateToggled )
		//	BikeAnimation( currentTheme, "moto_beep", 1 );
	}

	[RelayCommand]
	async Task ToggleBlink()
	{
		if ( !BlinkStateToggled )
			BikeAnimation( currentTheme, "moto_blink", 1 );
		await WriteRemoteCharacteristicValue( BlinkState );
		//if ( BlinkStateToggled )
		//	BikeAnimation( currentTheme, "moto_blink", 1 );
	}

	void BikeAnimation( AppTheme theme, string animation, int reapeat = -1 )
	{
		BikeImage = theme is AppTheme.Light ? animation + "_light.gif" : animation + "_dark.gif";
		IsBikeAnimated = true;
		if ( reapeat == -1 )
			return;
		if ( reapeat > 0 )
			timer = new( ResetAnimation, theme, reapeat * 500, Timeout.Infinite );

	}

	void ResetAnimation( object state )
	{
		var appTheme = (AppTheme) state;
		IsBikeAnimated = false;
		BikeImage = appTheme is AppTheme.Light ? "moto_light.png" : "moto_dark.png";
		timer?.Dispose();
	}

}
