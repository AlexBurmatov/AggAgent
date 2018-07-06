using System;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.device;
using com.tibbo.aggregate.common.server;

namespace AggregateExamples
{
    internal class ClientExample
    {
        public static void run()
        {
            var rls = new RemoteLinkServer(AggreGateNetworkDevice.DEFAULT_ADDRESS, RemoteLinkServer.DEFAULT_PORT,
                                           RemoteLinkServer.DEFAULT_USERNAME, RemoteLinkServer.DEFAULT_PASSWORD);

            var rlc = new RemoteLinkServerController(rls, false);
            rlc.connect();
            rlc.login();

            ContextManager cm = rlc.getContextManager();

            var serverVersion =
                cm.getRoot().getVariable(RootContextConstants.V_VERSION).rec().getString(
                    RootContextConstants.VF_VERSION_VERSION);

            Console.WriteLine("Server version: " + serverVersion);

            Console.ReadKey(false);

            rlc.disconnect();

            Console.ReadKey(false);
        }
    }
}