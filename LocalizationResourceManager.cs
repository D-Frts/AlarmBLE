using AlarmBle.Resources.Localization;
using System.ComponentModel;
using System.Globalization;

namespace AlarmBle;

public class LocalizationResourceManager : INotifyPropertyChanged
{
	private LocalizationResourceManager()
	{
		AppResources.Culture = CultureInfo.CurrentCulture;
	}

	public static LocalizationResourceManager Instance { get; } = new();

	public object this[string resourceKey] 
		=> AppResources.ResourceManager.GetObject(resourceKey, AppResources.Culture) ?? Array.Empty<byte>();

	public event PropertyChangedEventHandler PropertyChanged;

	public void SetCulture(CultureInfo cultureInfo )
	{
		AppResources.Culture = cultureInfo;
		PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( null ) );
	}
}
