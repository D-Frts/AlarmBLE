using AlarmBle.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmBle.ViewModel;

public partial class AlarmSettingsViewModel : BaseViewModel
{
	[ObservableProperty]
	double sliderValue;

	public AlarmSettingsViewModel( IBluetoothLE bluetoothLE, IAdapter adapter ) : base( bluetoothLE, adapter )
	{
	}
	[RelayCommand]
	async Task ChangeSliderValue()
	{
		await Shell.Current.DisplayAlert( "Silder Value", $"{(int)SliderValue}", "OK" );
	}
	[RelayCommand]
	void SliderStepper(ValueChangedEventArgs e )
	{
		double stepValue = 10.0;
		var newStep = Math.Round( e.NewValue / stepValue );
		SliderValue = newStep * stepValue;

	}
	[RelayCommand]
	async Task GoToChangePasskeyPage()
	{
		await Shell.Current.GoToAsync( nameof(ChangePasskeyPage), true );
	}
}
