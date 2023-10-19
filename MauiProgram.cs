using AlarmBle.View;
using AlarmBle.ViewModel;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using SkiaSharp.Views.Maui.Controls.Hosting;

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
                .UseSkiaSharp()
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

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<ScannerPage>();
            builder.Services.AddTransient<DeviceSettingsPage>();

			builder.Services.AddSingleton<MainPageViewModel>();
			builder.Services.AddSingleton<ScannerViewModel>();
            builder.Services.AddTransient<DeviceSettingsViewModel>();

            return builder.Build();
        }
    }
}