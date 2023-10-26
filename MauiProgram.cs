using AlarmBle.View;
using AlarmBle.ViewModel;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace AlarmBle
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts( fonts =>
                {
                    fonts.AddFont( "OpenSans-Regular.ttf", "OpenSansRegular" );
                    fonts.AddFont( "OpenSans-Semibold.ttf", "OpenSansSemibold" );
                } );

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IBluetoothLE>( CrossBluetoothLE.Current );
            builder.Services.AddSingleton<IAdapter>( CrossBluetoothLE.Current.Adapter );
            builder.Services.AddSingleton<IFingerprint>( CrossFingerprint.Current );

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<ScannerPage>();
			builder.Services.AddSingleton<SettingsPage>();
			builder.Services.AddSingleton<AppSettingsPage>();
			builder.Services.AddSingleton<AlarmSettingsPage>();
			builder.Services.AddTransient<ChangePasskeyPage>();

			builder.Services.AddSingleton<MainPageViewModel>();
			builder.Services.AddSingleton<ScannerViewModel>();
			builder.Services.AddSingleton<AppSettingsViewModel>();
			builder.Services.AddSingleton<AlarmSettingsViewModel>();
			builder.Services.AddTransient<ChangePasskeyViewModel>();


			return builder.Build();
        }
    }
}