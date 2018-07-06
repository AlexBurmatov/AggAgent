using System;
using System.Threading;
using com.tibbo.aggregate.common;
using com.tibbo.aggregate.common.agent;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.device;
using JavaCompatibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AggreGateTests
{
    [TestClass]
    public class AgentTests
    {
        private const String AGENT_CONTEXT_NAME = "cSharp";
        private const String V_TO_BE_READ = "readingFlag";

        private static readonly TableFormat VFT_TEST = new TableFormat(1, 1, "<" + V_TO_BE_READ + "><B><D=Reading Flag>");

        private Agent agent;
        private RemoteLinkServerController controller;
        private static bool wasRead;

        [TestInitialize]
        public void SetUp()
        {
            var rls = new RemoteLinkServer(AggreGateNetworkDevice.DEFAULT_ADDRESS, Agent.DEFAULT_PORT,
                                           RemoteLinkServer.DEFAULT_USERNAME, RemoteLinkServer.DEFAULT_PASSWORD);
            agent = new Agent(rls, AGENT_CONTEXT_NAME, false);
        }

        [TestCleanup]
        public void TearDown()
        {
            //var server = new RemoteLinkServer(AggreGateNetworkDevice.DEFAULT_ADDRESS, RemoteLinkServer.DEFAULT_PORT,
            //                                  RemoteLinkServer.DEFAULT_USERNAME, RemoteLinkServer.DEFAULT_PASSWORD);


            //controller = new RemoteLinkServerController(server, false);

            
            //controller.connect();
            //controller.login();

            //var contextManager = controller.getContextManager();
            //var dc = contextManager.get("users.admin.devices");
            //dc.callFunction("delete", AGENT_CONTEXT_NAME);
        }


        [TestMethod]
        public void TestServerReadsVariable()
        {
            initializeAgentContext(this.agent.getContext());

            this.agent.connect();

            var start = DateTime.Now;

            var agentThread = this.createAndRunAgentThread();

           

            while (!this.agent.getContext().isSynchronized())
            {
                if (!agentThread.IsAlive)
                {
                    agentThread = this.createAndRunAgentThread();
                }

                if ((DateTime.Now - start).Duration().TotalSeconds > 60)
                {
                    throw new ContextException("Unable to synchronize");
                }
                Thread.Sleep(500);
            }

            Assert.IsTrue(wasRead);
            this.agent.disconnect();
        }

        private Thread createAndRunAgentThread()
        {
            var agentThread = new Thread(this.runAgent)
                                  {
                                      Name = "Agent Runner Thread", IsBackground = true
                                  };
            agentThread.Start();
            return agentThread;
        }

        public void runAgent()
        {
            try
            {
                while (true)
                {
                    this.agent.run();
                }
            }
            catch (Exception e)
            {
                Logger.getLogger(Log.TEST).error("Exception running agent", e);
            }
        }

        private static void initializeAgentContext(Context context)
        {
            var vd = new VariableDefinition(V_TO_BE_READ, VFT_TEST, true, true, "Test", ContextUtils.GROUP_REMOTE);
            vd.setGetter(
                new DelegatedVariableGetter(
                    (con, def, caller, request) => new DataRecord(VFT_TEST).addBoolean(wasRead = true).wrap()));
            context.addVariableDefinition(vd);
        }
    }
}