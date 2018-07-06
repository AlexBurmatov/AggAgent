using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using com.tibbo.aggregate.common.command;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.server;
using com.tibbo.aggregate.common.util;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.device
{
    public class RemoteLinkServerController : AggreGateDeviceController
    {
        private const Int32 COMMAND_TIMEOUT = 600000; // Ms

        private StreamWrapper streamWrapper;


        public RemoteLinkServerController(AggreGateDevice device, Boolean async)
            : base(device, COMMAND_TIMEOUT)
        {
            setContextManager(
                new RemoteDeviceContextManager<AggreGateDevice>(this, async));
        }

        protected override Boolean connectImpl()
        {
            try
            {
                Logger.getLogger(Log.PROTOCOL).debug("Connecting to remote " + Cres.get().getString("ls") + " (" +
                                                     getDevice() + ")");

                var client = new TcpClient(getDeviceAsRemoteLinkServer().getAddress(), getDeviceAsRemoteLinkServer().getPort());

               SslStream ssl = new SslStream(
                    client.GetStream(),
                    false,
                    (sender, certificate, chain, sslpolicyerrors) => true
                    //, (sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers) => { return null; }
                    );

                //ssl.CipherAlgorithm = ;

                ssl.AuthenticateAsClient("");

                streamWrapper = new StreamWrapper(ssl, 0);

                setCommandBuffer(new CommandBuffer(streamWrapper, Command.START_CHAR, Command.END_CHAR));

                Logger.getLogger(Log.PROTOCOL).debug("Connection with remote LinkServer established");
            }
            catch (Exception ex)
            {
                throw new RemoteDeviceErrorException(
                    String.Format(Cres.get().getString("devErrConnecting"), getDevice().getDescription() + " (" + getDevice().getInfo() + ")") + ex.Message, ex);
            }

            base.connectImpl();

            getContextManager().setRoot(new RemoteDeviceContextProxy<RemoteLinkServer>(ContextUtils.CTX_ROOT, this));
            getContextManager().restart();

            try
            {
                var version = this.getContextManager().getRoot().getVariable(RootContextConstants.V_VERSION).get().ToString();

            }
            catch (Exception ex)
            {
                Logger.getLogger(Log.PROTOCOL).error("Server version check failure", ex);
            }

            return true;
        }

        private RemoteLinkServer getDeviceAsRemoteLinkServer()
        {
            return (RemoteLinkServer)getDevice();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override bool loginImpl()
        {
            this.getContextManager().restart();

            var loginInput = new DataTable(RootContextConstants.FIFT_LOGIN, getDeviceAsRemoteLinkServer().getUsername(),
                                           getDeviceAsRemoteLinkServer().getPassword());

            getContextManager().getRoot().callFunction(RootContextConstants.F_LOGIN, null, loginInput);

            getContextManager().getRoot().reinitialize();
            // Resets local cache, because root context was already initialized, but its visible entities changed after login

            return true;
        }

        public override void start()
        {
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void disconnect()
        {
            if (streamWrapper != null)
            {
                streamWrapper.Close();
            }

            if (getContextManager() != null)
            {
                getContextManager().stop();
            }

            base.disconnect();
        }

        protected override void send(OutgoingAggreGateCommand cmd)
        {
            cmd.send(streamWrapper);
        }
    }
}