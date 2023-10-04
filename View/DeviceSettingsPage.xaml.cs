using AlarmBle.ViewModel;

namespace AlarmBle.View;

public partial class DeviceSettingsPage : ContentPage
{
	public DeviceSettingsPage( DeviceSettingsViewModel deviceSettingsViewModel )
	{
		InitializeComponent();
		BindingContext = deviceSettingsViewModel;
	}
}