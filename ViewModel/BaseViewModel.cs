using AlarmBle.Model;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.ObjectModel;
using Debug = System.Diagnostics.Debug;

namespace AlarmBle.ViewModel;

public partial class BaseViewModel : ObservableObject
{
    public IBluetoothLE BluetoothLE { get; }
    public IAdapter Adapter { get; }

    public BaseViewModel( IBluetoothLE bluetoothLE, IAdapter adapter )
    {
        BluetoothLE = bluetoothLE;
        Adapter = adapter;
        GetBondedDevices( AlarmServiceUuids.AlarmService );
    }

    void GetBondedDevices( string service = null )
    {
        var bondedDevices = Adapter.GetSystemConnectedOrPairedDevices( new[ ] { new Guid( AlarmServiceUuids.AlarmService ) } );
        foreach ( var device in bondedDevices )
        {
            BondedDevices?.Add( new BleDevice( device ) );
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor( nameof( IsNotBusy ) )]
    bool isBusy;
    [ObservableProperty]
    [NotifyPropertyChangedFor( nameof( IsNotRefreshing ) )]
    bool isRefreshing;
    [ObservableProperty]
    [NotifyPropertyChangedFor( nameof( IsNotScanning ) )]
    bool isScanning;
    [ObservableProperty]
    [NotifyPropertyChangedFor( nameof( IsNotConnected ) )]
    bool isConnected;
    [ObservableProperty]
    [NotifyPropertyChangedFor( nameof( IsNotBonded ) )]
    bool isBonded;
    [ObservableProperty]
    int scanTimeout = 5000;
    public ObservableCollection<BleDevice> DiscoveredDevices { get; set; } = new();
    public ObservableCollection<BleDevice> BondedDevices { get; set; } = new();

    [ObservableProperty]
    BleDevice bleDevice;

    public bool IsNotBusy => !IsBusy;
    public bool IsNotRefreshing => !IsRefreshing;
    public bool IsNotScanning => !IsScanning;
    public bool IsNotConnected => !IsConnected;
    public bool IsNotBonded => !IsBonded;

    public virtual async Task<bool> GetPermissions()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if ( status != PermissionStatus.Granted )
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        return status is PermissionStatus.Granted;
    }
    public virtual async Task StartScan()
    {
        try
        {
            if ( Adapter.IsScanning ) return;
            IsScanning = true;
            //Configura alguns parâmetros de escaneamento
            Adapter.ScanTimeout = ScanTimeout;
            Adapter.ScanMode = ScanMode.Balanced;
            //Comportamento na ocorrência dos eventos
            Adapter.ScanTimeoutElapsed += ( sender, args ) => IsScanning = Adapter.IsScanning;
            Adapter.DeviceDiscovered += ( sender, args ) =>
            {
                //verifica se a coleção ja contém o dispositivo encontrado
                if ( !DiscoveredDevices.Where( device => device.Device.Id == args.Device.Id ).Any() )
                {
                    DiscoveredDevices?.Add( new BleDevice( args.Device ) );
                }
            };
            //Configura um filtro para encontrar somente dispositivos com o devido Serviço
            ScanFilterOptions scanFilter = new()
            {
                ServiceUuids = new[ ] { new Guid( AlarmServiceUuids.AlarmService ) }
            };
            DiscoveredDevices?.Clear();
            await Adapter.StartScanningForDevicesAsync( scanFilter );
            //await Adapter.StartScanningForDevicesAsync();


        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"{ex.Message}" );
            await Shell.Current.DisplayAlert( "Error", $"{ex.Message}", "OK" );
            //TODO: Retorna para outra página após exibir mensagem da falha
            return;
        }
        finally
        {
            //IsBusy = false;
        }
    }
    public virtual async Task StopScan()
    {
        try
        {
            if ( IsNotScanning ) return;
            await Adapter.StopScanningForDevicesAsync();
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"{ex.Message}" );
            await OnToastAsync( $"{ex.Message}", ToastDuration.Long, 18 );
            //TODO:
            return;
        }
        finally
        {
            IsScanning = Adapter.IsScanning;
        }
    }
    public virtual async Task ConnectToDevice( BleDevice device )
    {
        try
        {
            IsBusy = true;
            if ( IsConnected )
            {
                IsBusy = false;
                return;
            }
            Adapter.DeviceConnected += ( sender, args ) =>
            {
                IsConnected = true;
            };
            ConnectParameters param = new( autoConnect: true );
            await Adapter.ConnectToDeviceAsync( device.Device, param );
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"{ex.Message}" );
            await OnToastAsync( $"{ex.Message}", ToastDuration.Long, 18 );
            //TODO:
            return;
        }
        finally
        {
            IsBusy = false;
        }
    }
    public virtual async Task DisconnectFromDevice( BleDevice device )
    {
        try
        {
            IsBusy = true;
            if ( IsNotConnected )
            {
                IsBusy = false;
                return;
            }
            Adapter.DeviceDisconnected += ( sender, args ) =>
            {
                IsConnected = false;
            };
            await Adapter.DisconnectDeviceAsync( device.Device );
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"{ex.Message}" );
            await OnToastAsync( $"{ex.Message}", ToastDuration.Long, 18 );
            //TODO:
            return;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public virtual async Task BondToDevice( BleDevice device )
    {
        try
        {
            IsBusy = true;
            IsBonded = device.Device.BondState is DeviceBondState.Bonded;
            if ( IsBonded )
            {
                IsBusy = false;
                return;
            }
            Adapter.DeviceBondStateChanged += ( sender, args ) =>
            {
                IsBonded = args.Device.BondState is DeviceBondState.Bonded;
                if ( IsBonded && !BondedDevices.Where( device => device.Device.Id == args.Device.Id ).Any() )
                {
                    BondedDevices.Add( new BleDevice( args.Device ) );
                }
            };
            await Adapter.BondAsync( device.Device );
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( $"{ex.Message}" );
            await OnToastAsync( $"{ex.Message}", ToastDuration.Long, 18 );
            //TODO:
            return;
        }
        finally
        {
            IsBusy = false;
        }

    }

    public virtual async Task OnToastAsync( string message, ToastDuration duration = ToastDuration.Short, int fontSize = 14 )
    {
        CancellationTokenSource cancellationTokenSource = new();
        var toast = Toast.Make( message, duration, fontSize );
        await toast.Show( cancellationTokenSource.Token );

    }

}
