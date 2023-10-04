using AlarmBle.Model;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE.Abstractions.Contracts;

namespace AlarmBle.ViewModel;

public partial class ScannerViewModel : BaseViewModel
{
    public ScannerViewModel( IBluetoothLE bluetoothLE, IAdapter adapter ) : base( bluetoothLE, adapter )
    {
        IsBusy = false;
    }

    [RelayCommand]
    async Task SearchDevices()
    {
        var permission = await GetPermissions();
        if ( permission )
            await StartScan();
    }
    [RelayCommand]
    async Task StopSearch()
    {
        await StopScan();
    }
    [RelayCommand]
    async Task Connect( BleDevice device )
    {
        if ( IsScanning )
        {
            await StopSearch();
        }
        //await DisplayAlert( $"Make Connection to device: {device.Device.Name}" );
        await ConnectToDevice( device );
        if ( IsNotBonded )
            await BondToDevice( device );
    }
    static async Task DisplayAlert( string message )
    {
        await Shell.Current.DisplayAlert( string.Empty, message, "OK" );

    }
}
