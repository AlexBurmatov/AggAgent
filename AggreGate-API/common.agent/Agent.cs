using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.device;
using com.tibbo.aggregate.common.util;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.agent
{
    public class Agent
    {
        public const int DEFAULT_PORT = 6480;

        private const int SOCKET_TIMEOUT = 10000; // Ms

        private readonly RemoteLinkServer server;

        private readonly AgentContext context;

        private ContextManager contextManager;

        private AgentImplementationController controller;

        private FormatCache formatCache = new FormatCache();

        public Agent(RemoteLinkServer server, String name, bool eventConfirmation)
        {
            this.server = server;
            context = new AgentContext(server, name, eventConfirmation);
            contextManager = new AgentContextManager(this, context, false);
        }

        public void connect()
        {
            try
            {
                context.setSynchronized(false);

                Logger.getLogger(Log.PROTOCOL).debug("Connecting to remote " + Cres.get().getString("ls") + " (" + server + ")");

                Logger.getLogger(Log.PROTOCOL).debug("Connection with remote LinkServer established");

                contextManager.start();

                //Note: Code smell --- controller and format cache should not be an ivar
                controller = new AgentImplementationController(ConstructStreamWrapper(), contextManager, formatCache);

                contextManager.getRoot().accept(new DelegatedContextVisitor<Context>(aContext =>
                    {
                        foreach (var ed in aContext.getEventDefinitions())
                        {
                            if (ed.getGroup() != null)
                            {
                                aContext.addEventListener(ed.getName(), controller.getDefaultEventListener());
                            }
                        }
                    }));
            }
            catch (Exception ex)
            {
                throw new RemoteDeviceErrorException(
                    String.Format(Cres.get().getString("devErrConnecting"),
                                  server.getDescription() + " (" + server.getInfo() + ")") + ex.Message, ex);
            }
        }

        private StreamWrapper ConstructStreamWrapper()
        {
            var client = new TcpClient(server.getAddress(), server.getPort())
            {
                SendTimeout = SOCKET_TIMEOUT,
                ReceiveTimeout = SOCKET_TIMEOUT,
                NoDelay = false,
                SendBufferSize = 32 * 1024,
                ReceiveBufferSize = 16 * 1024,
            };

            return new StreamWrapper(client.GetStream(), 32 * 1024);
        }

        public void disconnect()
        {
            if (controller != null)
            {
                controller.shutdown();
            }
            if (contextManager != null)
            {
                contextManager.stop();
            }
            context.setSynchronized(false);
        }

        public void run()
        {
            controller.runImpl();
        }

        public AgentContext getContext()
        {
            return context;
        }

        public AgentImplementationController getController()
        {
            return controller;
        }

        private class AgentContextManager : DefaultContextManager
        {
            private readonly Agent owner;

            public AgentContextManager(Agent ownerAgent, Context rootContext, Boolean async)
                : base(async)
            {
                owner = ownerAgent;
                setRootAndStart(rootContext);
            }

            public override void eventAdded(Context con, EventDefinition ed)
            {
                base.eventAdded(con, ed);

                if (ed.getGroup() != null && owner.controller != null)
                {
                    con.addEventListener(ed.getName(), owner.controller.getDefaultEventListener());
                }
            }
        }
    }
}