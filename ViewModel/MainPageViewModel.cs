using AlarmBle.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace AlarmBle.ViewModel;


public partial class MainPageViewModel : BaseViewModel
{
	[ObservableProperty]
	string switchImage;
	AppTheme currentTheme;

	[ObservableProperty]
	[NotifyPropertyChangedFor( nameof( AlarmStateToggled ) )]
	ICharacteristic alarmState;
	[ObservableProperty]
	bool alarmStateToggled;
	public MainPageViewModel( IBluetoothLE bluetoothLE, IAdapter adapter ) : base( bluetoothLE, adapter )
	{
		Application.Current.RequestedThemeChanged += ( sender, args ) =>
		{
			currentTheme = args.RequestedTheme;
		};
		currentTheme = Application.Current.RequestedTheme;
		
	}

	async Task GetAlarmState()
	{
		AlarmState = await GetRemoteCharacteristic( RemoteDevice, AlarmServiceUuids.AlarmService, AlarmServiceUuids.State );
		AlarmState.ValueUpdated += AlarmState_ValueUpdated;
		AlarmStateToggled = (bool) await GetRemoteCharacteriticValue( AlarmState );		
		if ( AlarmStateToggled )
		{
			SwitchImage = currentTheme is AppTheme.Light ? "on_switch_light.png" : "on_switch_dark.png";
		}
		else
		{
			SwitchImage = currentTheme is AppTheme.Light ? "off_switch_light.png" : "off_switch_dark.png";
		}
	}
	private async void AlarmState_ValueUpdated( object sender, CharacteristicUpdatedEventArgs e )
	{
		AlarmState = e.Characteristic;
		AlarmStateToggled = (bool) await GetRemoteCharacteriticValue( AlarmState );
		if ( AlarmStateToggled )
		{
			SwitchImage = currentTheme is AppTheme.Light ? "on_switch_light.png" : "on_switch_dark.png";
		}
		else
		{
			SwitchImage = currentTheme is AppTheme.Light ? "off_switch_light.png" : "off_switch_dark.png";
		}
	}
	async Task ToggleAlarmState()
	{

	}

	//[RelayCommand]
	//void ToggleSwitchImage()
	//{
	//	switchToggled = !switchToggled;
	//	if ( currentTheme is AppTheme.Light )
	//	{
	//		if(switchToggled)
	//			SwitchImage = "on_switch_light.png";
	//		else
	//			SwitchImage = "off_switch_light.png";
	//	}
	//	if ( currentTheme is AppTheme.Dark )
	//	{
	//		if ( switchToggled )
	//			SwitchImage = "on_switch_dark.png";
	//		else
	//			SwitchImage = "off_switch_dark.png";
	//	}


	//}



}
