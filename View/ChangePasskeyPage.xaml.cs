using AlarmBle.ViewModel;

namespace AlarmBle.View;

public partial class ChangePasskeyPage : ContentPage
{
	public ChangePasskeyPage(ChangePasskeyViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

}