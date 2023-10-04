using AlarmBle.ViewModel;

namespace AlarmBle.View;

public partial class ScannerPage : ContentPage
{
	public ScannerPage(ScannerViewModel scannerViewModel)
	{
		InitializeComponent();
		BindingContext = scannerViewModel;
	}
}