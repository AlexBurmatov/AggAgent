using System;

namespace com.tibbo.aggregate.common.device
{
    public class AggreGateNetworkDevice : AggreGateDevice
    {
        public const String DEFAULT_ADDRESS = "127.0.0.1";

        protected String address;
        protected int port;

        static AggreGateNetworkDevice()
        {
            type = "Generic AggreGate Network Device";
            id = "network";
        }

        public AggreGateNetworkDevice()
        {
            address = DEFAULT_ADDRESS;
        }

        public AggreGateNetworkDevice(String address, int port)
        {
            this.address = address;
            this.port = port;
        }

        public String getAddress()
        {
            return address;
        }

        public int getPort()
        {
            return port;
        }

        public void setPort(int portString)
        {
            port = portString;
        }

        public void setAddress(String addressString)
        {
            address = addressString;
        }

        public override String getInfo()
        {
            return address + ":" + port;
        }
    }
}