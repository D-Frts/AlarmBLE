using AlarmBle.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace AlarmBle.ViewModel;


public partial class MainPageViewModel : BaseViewModel
{
	static AppTheme currentTheme;
	Timer timer;

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
	public MainPageViewModel( IBluetoothLE bluetoothLE, IAdapter adapter ) : base( bluetoothLE, adapter )
	{
		Application.Current.RequestedThemeChanged += ( sender, args ) =>
		{
			currentTheme = args.RequestedTheme;
			MotoImage = currentTheme is AppTheme.Light ? "moto_light.png" : "moto_dark.png";
		};
		currentTheme = Application.Current.RequestedTheme;
		StatusAlarm = "Inactive";
		MotoImage = currentTheme is AppTheme.Light ? "moto_light.png" : "moto_dark.png";
		IsMotoAnimated = false;
		BeepStateToggled = false;
		BlinkStateToggled = false;
	}

	async Task GetAlarmState()
	{
		AlarmState = await GetRemoteCharacteristic( RemoteDevice, AlarmServiceUuids.AlarmService, AlarmServiceUuids.State );
		AlarmStateToggled = (bool) await GetRemoteCharacteriticValue( AlarmState );
	}

	[RelayCommand]
	void ActivateAlarm()
	{
		AlarmStateToggled = true;
		StatusAlarm = "Active";
		if ( BlinkStateToggled || BeepStateToggled )
		{
			if ( BlinkStateToggled && BeepStateToggled )
			{
				MotoAnimation( "moto_beep_blink" );
			}
			else if ( BlinkStateToggled && !BeepStateToggled )
			{
				MotoAnimation( "moto_blink" );
			}
			else if ( !BlinkStateToggled && BeepStateToggled )
			{
				MotoAnimation( "moto_beep" );
			}
		}
	}

	[RelayCommand]
	void DeactivateAlarm()
	{
		AlarmStateToggled = false;
		StatusAlarm = "Inactive";
		if ( BlinkStateToggled || BeepStateToggled )
		{
			if ( BlinkStateToggled && BeepStateToggled )
			{
				MotoAnimation( "moto_beep_blink", 2 );
			}
			else if ( BlinkStateToggled && !BeepStateToggled )
			{
				MotoAnimation( "moto_blink", 2 );
			}
			else if ( !BlinkStateToggled && BeepStateToggled )
			{
				MotoAnimation( "moto_beep", 2 );
			}
		}
	}

	[RelayCommand]
	void ToggleBeep()
	{
		BeepStateToggled = !BeepStateToggled;
		if ( BeepStateToggled )
			MotoAnimation( "moto_beep" );
	}

	[RelayCommand]
	void ToggleBlink()
	{
		BlinkStateToggled = !BlinkStateToggled;
		if ( BlinkStateToggled )
			MotoAnimation("moto_blink");


	}
	void MotoAnimation(string animation, int iteration = 1)
	{
		MotoImage = currentTheme is AppTheme.Light ? animation + "_light.gif" : animation + "_dark.gif";
		IsMotoAnimated = true;
		if (iteration != 0)
			timer = new( ResetAnimation, null, iteration*500, Timeout.Infinite );
	}

	void ResetAnimation( object state )
	{
		IsMotoAnimated = false;
		MotoImage = currentTheme is AppTheme.Light ? "moto_light.png" : "moto_dark.png";
		timer?.Dispose();
	}

}
