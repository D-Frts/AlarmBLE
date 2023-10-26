using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE.Abstractions.Contracts;

namespace AlarmBle.ViewModel;

public partial class ChangePasskeyViewModel : BaseViewModel
{
	[ObservableProperty]
	bool isPassoword;

	[ObservableProperty]
	[NotifyPropertyChangedFor( nameof( CurrentPasskey ) )]
	string currentPasskeyDisplay;
	public string CurrentPasskey { get; set; } = "123456";
	[ObservableProperty]
	string newPasskey;

	public ChangePasskeyViewModel( IBluetoothLE bluetoothLE, IAdapter adapter ) : base( bluetoothLE, adapter )
	{
		IsPassoword = true;
		CurrentPasskeyDisplay = IsPassoword ? "******" : CurrentPasskey;
		NewPasskey = "";
	}

	[RelayCommand]
	void TogglePasswordVisibility()
	{
		IsPassoword = !IsPassoword;
		CurrentPasskeyDisplay = IsPassoword ? "******" : CurrentPasskey;
		
	}
	[RelayCommand]
	async Task SaveNewPasskey()
	{
		if ( NewPasskey.Length < 6 ) return;
		var changeAnswer = await Shell.Current.DisplayAlert( "Change Passkey", "Are you sure you want to change the passkey?", "Yes", "No" );
		if ( !changeAnswer ) return;
		CurrentPasskey = NewPasskey;
		await OnToastAsync( "Passkey Saved" );
		NewPasskey = "";
		IsPassoword = false;
		TogglePasswordVisibility();
	}

}
