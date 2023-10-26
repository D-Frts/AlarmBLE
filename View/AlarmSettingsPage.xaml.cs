using AlarmBle.ViewModel;

namespace AlarmBle.View;

public partial class AlarmSettingsPage : ContentPage
{
	public AlarmSettingsPage( AlarmSettingsViewModel viewModel )
	{
		InitializeComponent();
		BindingContext = viewModel;
	}


}