using System;

using com.tibbo.aggregate.common.agent;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.datatable.field;
using com.tibbo.aggregate.common.device;

namespace AggreGatePerformanceTest
{
    class PerformanceTestingAgent
    {
        private const int UPDATERS_COUNT = 1;
        private const int DEFAULT_VARIABLE_COUNT = 40000;
        private const int DEFAULT_PERIOD = 500;


        public void Run()
        {

            Console.WriteLine("Starting PerformanceTestingAgent");
            //            while (true)
            //            {
            try
            {
                this._agent = this.CreateAgent();
                this.InitializeAgentContext();
                this._agent.connect();

                this.updateManager = new UpdateManager(this.Period, UPDATERS_COUNT, this.VariableCount, this._agent);
                this.updateManager.SetupVariables();

                this.updateManager.Start();

                //using (
                //    var timer = new Timer(
                //        x => { updateManager.Start();}, 
                //        null, DELAY, Timeout.Infinite))
                //{
                    while (true)
                    {
                        Console.Out.WriteLine("[" + DateTime.Now + "] agent.run");
                        _agent.run();
                    }
                //}
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
            }
            //            }
            //            // ReSharper disable once FunctionNeverReturns
            Console.ReadKey(true);
        }

        private Agent CreateAgent()
        {
            var rls = new RemoteLinkServer(AggreGateNetworkDevice.DEFAULT_ADDRESS, Agent.DEFAULT_PORT, RemoteLinkServer.DEFAULT_USERNAME, RemoteLinkServer.DEFAULT_PASSWORD);
            return new Agent(rls, "performanceTestingAgent", true);
        }

        private void InitializeAgentContext()
        {
            var context = _agent.getContext();

            Console.Out.WriteLine("=== Reinitializing Agent Context");
            
            var performanceSettingsVariableDefinition = new VariableDefinition(
                V_PERFORMANCE_TESTING,
                VFT_PERFORMANCE_TESTING, true, true, "Performance Testing",
                ContextUtils.GROUP_REMOTE);
            performanceSettingsVariableDefinition.setGetter(new DelegatedVariableGetter((con, def, caller, request) => this.PerformanceSettings));
            performanceSettingsVariableDefinition.setSetter(new DelegatedVariableSetter((con, def, caller, request, value) => { this.PerformanceSettings = value; }));
            context.addVariableDefinition(performanceSettingsVariableDefinition);
        }


        private long Period
        {
            get { return _performanceSettings.rec().getLong(VF_PERFORMANCE_TESTING_PERIOD); }
        }

        private int VariableCount
        {
            get { return _performanceSettings.rec().getInt(VF_PERFORMANCE_TESTING_VARIABLE_COUNT); }
        }


        UpdateManager updateManager;
        Agent _agent;


        private DataTable PerformanceSettings
        {
            get { return _performanceSettings; }
            set
            {
                Console.Out.WriteLine(("New performance settings: " + value));
                _performanceSettings = value;
            }
        }

        private DataTable _performanceSettings = new DataTable(VFT_PERFORMANCE_TESTING, true);


        private const String V_PERFORMANCE_TESTING = "performanceTesting";

        private static readonly TableFormat VFT_PERFORMANCE_TESTING;
        private const String VF_PERFORMANCE_TESTING_VARIABLE_COUNT = "variableCount";
        private const String VF_PERFORMANCE_TESTING_PERIOD = "period";

        static PerformanceTestingAgent()
        {
            VFT_PERFORMANCE_TESTING = new TableFormat(1, 1);
            VFT_PERFORMANCE_TESTING.addField(
                "<" + VF_PERFORMANCE_TESTING_VARIABLE_COUNT + ">" +
                "<I>" +
                "<D=" + "Variable Count>" +
                "<A=" + DEFAULT_VARIABLE_COUNT + ">");
            VFT_PERFORMANCE_TESTING.addField(
                "<" + VF_PERFORMANCE_TESTING_PERIOD + ">" +
                "<L>" +
                "<D=" + "Update Period" + ">" +
                "<E=" + LongFieldFormat.EDITOR_PERIOD + ">" +
                "<O=" + LongFieldFormat.encodePeriodEditorOptions(0 /*TimeHelper.MILLISECOND*/, 2 /*TimeHelper.MINUTE*/) +
                ">" +
                "<A=" + DEFAULT_PERIOD + ">");
        }
    }
}