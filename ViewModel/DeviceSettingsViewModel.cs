using Plugin.BLE.Abstractions.Contracts;

namespace AlarmBle.ViewModel
{
    public class DeviceSettingsViewModel : BaseViewModel
    {
        public DeviceSettingsViewModel( IBluetoothLE bluetoothLE, IAdapter adapter ) : base( bluetoothLE, adapter )
        {
        }
    }
}
