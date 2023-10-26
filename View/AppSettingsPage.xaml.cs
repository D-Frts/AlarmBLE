using AlarmBle.Resources.Localization;
using AlarmBle.ViewModel;
using System.Globalization;

namespace AlarmBle.View;

public partial class AppSettingsPage : ContentPage
{
	public AppSettingsPage( AppSettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	private void Switch_Toggled( object sender, ToggledEventArgs e )
	{

    }
}