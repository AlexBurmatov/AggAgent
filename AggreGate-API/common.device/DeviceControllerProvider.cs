using System;

namespace com.tibbo.aggregate.common.device
{
    public interface DeviceControllerProvider
    {
        AggreGateDeviceController getDeviceController(String name);
    }
}