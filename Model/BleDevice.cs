using Plugin.BLE.Abstractions.Contracts;

namespace AlarmBle.Model;

public class BleDevice
{
    public IDevice Device { get; }

    public BleDevice( IDevice nativeDevice )
    {
        Device = nativeDevice;
    }
}
