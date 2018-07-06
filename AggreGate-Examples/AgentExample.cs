using System;
using System.Threading;
using com.tibbo.aggregate.common.agent;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.datatable.field;
using com.tibbo.aggregate.common.device;
using com.tibbo.aggregate.common.@event;

namespace AggregateExamples
{
    internal class AgentExample
    {
        private const String V_SETTING = "setting";
        private const String V_PERIOD = "period";
        private const String F_OPERATION = "operation";
        private const String E_EVENT = "event";

        private const String VF_SETTING_STRING = "string";
        private const String VF_SETTING_INTEGER = "integer";
        private const String VF_PERIOD_PERIOD = "period";
        private const String FIF_OPERATION_LIMIT = "limit";
        private const String FOF_OPERATION_RESULT = "result";
        private const String EF_EVENT_DATA = "data";

        private static readonly TableFormat VFT_SETTING = new TableFormat(1, 100);

        private static readonly TableFormat VFT_PERIOD = new TableFormat(1, 1,
                                                                           "<" + VF_PERIOD_PERIOD +
                                                                           "><L><A=5000><D=Event Generation Period><V=<L=100 100000000>><E=" +
                                                                           LongFieldFormat.EDITOR_PERIOD + ">");

        private static readonly TableFormat FIFT_OPERATION = new TableFormat(1, 1,
                                                                               "<" + FIF_OPERATION_LIMIT + "><I><A=100>");

        private static readonly TableFormat FOFT_OPERATION = new TableFormat(1, 1, "<" + FOF_OPERATION_RESULT + "><I>");

        private static readonly TableFormat EFT_EVENT = new TableFormat(1, 1, "<" + EF_EVENT_DATA + "><F>");

        static AgentExample()
        {
            //VFT_SETTING.addField("<" + VF_SETTING_STRING + "><S><D=String Field>");
            //VFT_SETTING.addField("<" + VF_SETTING_INTEGER + "><I><D=Integer Field>");
        }

        private static DataTable setting = new DataTable(VFT_SETTING, true);


        private static Int64 period = 3000;


        public static void run()
        {
            while (true)
            {
                Thread eventGenerator = null;
                try
                {
                    var rls = new RemoteLinkServer(AggreGateNetworkDevice.DEFAULT_ADDRESS, Agent.DEFAULT_PORT,
                                                   RemoteLinkServer.DEFAULT_USERNAME, RemoteLinkServer.DEFAULT_PASSWORD);

                    var agent = new Agent(rls, "LoraAgent", true);

                    initializeAgentContext(agent.getContext());

                    agent.connect();


                    eventGenerator = new Thread(() =>
                                       {
                                           var random = new Random();
                                           while (true)
                                           {
                                               Thread.Sleep((Int32)period);

                                               if (agent.getContext().isSynchronized())
                                               {
                                                   agent.getContext().fireEvent(E_EVENT, EventLevel.INFO, (float)(random.Next() * 1000000));
                                                   //context.fireEvent(AbstractContext.E_UPDATED, name, variableValue);
                                               }
                                           }
                                       })
                    { IsBackground = true };
                    eventGenerator.Start();


                    //while (!Console.KeyAvailable)
                    while (true)
                    {
                        //Console.Out.WriteLine("...");
                        agent.run();
                    }

                }
                catch (Exception ex)
                {

                    if (eventGenerator != null)
                        eventGenerator.Abort();

                    Console.Out.WriteLine(ex.ToString());
                }
            }
        }

        private static void initializeAgentContext(Context context)
        {
            var random = new Random();
            var vd = new VariableDefinition(V_SETTING, VFT_SETTING, true, true, "Tabular Setting",
                                            ContextUtils.GROUP_REMOTE);
            vd.setGetter(new DelegatedVariableGetter((con, def, caller, request) => setting));
            vd.setSetter(new DelegatedVariableSetter((con, def, caller, request, value) => { setting = value; }));
            context.addVariableDefinition(vd);

            vd = new VariableDefinition(V_PERIOD, VFT_PERIOD, true, true, "Event Generation Period",
                                        ContextUtils.GROUP_REMOTE);
            vd.setGetter(
                new DelegatedVariableGetter(
                    (con, def, caller, request) => new DataRecord(VFT_PERIOD).addLong(period).wrap()));
            vd.setSetter(
                new DelegatedVariableSetter(
                    (con, def, caller, request, value) => { period = value.rec().getLong(VF_PERIOD_PERIOD); }));
            context.addVariableDefinition(vd);
            var fd = new FunctionDefinition(F_OPERATION, FIFT_OPERATION, FOFT_OPERATION, "Agent Operation",
                                            ContextUtils.GROUP_REMOTE);
            fd.setImplementation(new DelegatedFunctionImplementation((con, def, caller, request, parameters) =>
                {
                    var limit = parameters.rec().getInt(FIF_OPERATION_LIMIT);
                    return new DataRecord(def.getOutputFormat()).addInt(random.Next() * limit).wrap();
                }));
            context.addFunctionDefinition(fd);

            var ed = new EventDefinition(E_EVENT, EFT_EVENT, "Agent Event", ContextUtils.GROUP_REMOTE);
            context.addEventDefinition(ed);

            context.addEventListener(AgentContext.E_EVENT_CONFIRMED, new DefaultContextEventListener<CallerController<CallerData>>(
                (anEvent) =>
                {
                    Console.Out.WriteLine(("Server has confirmed event with ID: " + anEvent.getData().rec().getLong(AgentContext.EF_EVENT_CONFIRMED_ID)));
                }));

            context.addEventListener(AgentContext.E_EVENT_ACKNOWLEDGED, new DefaultContextEventListener<CallerController<CallerData>>(
                (anEvent) =>
                {
                    Console.Out.WriteLine(("User has confirmed event with data: " + anEvent.ToString()));
                }));

            //// For assets adding you should create DataTable with AgentContext.FOFT_ASSET format, add addVariableDefinition with AgentContext.V_ASSETS name and getter for it with created DataTable.
            //// You can add assets by adding records to created DataTable for previous step. For example:

            //var assetDataTable = new DataTable(AgentContext.FOFT_ASSET, false);
            //var assetVariable = new VariableDefinition(AgentContext.V_ASSETS, AgentContext.FOFT_ASSET, true, false, null);
            //assetVariable.setGetter(new DelegatedVariableGetter((con, def, caller, request) => assetDataTable));
            //context.addVariableDefinition(assetVariable);

            //var assetSub = new DataRecord(AgentContext.FOFT_ASSET);
            //assetSub.setValue(0, "Test Subasset"); //id
            //assetSub.setValue(1, "Test Subasset"); //dexscription
            //assetSub.setValue(2, true); // enable
            //assetSub.setValue(3, null); //subset

            //var assetRoot = new DataRecord(AgentContext.FOFT_ASSET);
            //assetRoot.setValue(0, "Test Asset"); // id
            //assetRoot.setValue(1, "Test Asset"); // description
            //assetRoot.setValue(2, true); // enable
            //assetRoot.setValue(3, new DataTable(assetSub)); // subasset

            //assetDataTable.addRecord(assetRoot);

            //// If we add some assets here, we'll need to make sure group of all entities is remote|assetId|subassetID [|...].
            //// Entity group can then be constructed via ContextUtils.createGroup(ContextUtils.GROUP_REMOTE, "Test Asset") ContextUtils.createGroup(ContextUtils.GROUP_REMOTE, "Test Asset", "Test Subasset")
            //// For example:

            //var variableExample = new VariableDefinition(V_SETTING, VFT_SETTING, true, true, "Test Variable", ContextUtils.createGroup(ContextUtils.GROUP_REMOTE, "Test Asset"));
        }
    }
}