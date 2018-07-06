using System;
using com.tibbo.aggregate.common.agent;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;

namespace AggreGatePerformanceTest
{
    internal class UpdateManager
    {
        public UpdateManager(long period, int updatersCount, int variablesPerUpdater, Agent agent)
        {
            Period = period;
            Agent = agent;

            _updaters = new VariableUpdater[updatersCount];

            var eventTableFormat = agent.getContext().getEventDefinition(AbstractContext.E_UPDATED).getFormat();

            for (int i = 0; i < updatersCount; i++)
            {
                _updaters[i] = new VariableUpdater(Agent.getContext(), variablesPerUpdater, i * variablesPerUpdater, VFT_TEST_VARIABLE, eventTableFormat, period);

            }
        }

        public void Start()
        {
            Console.Out.WriteLine("[" + DateTime.Now + "] Starting " + this._updaters.Length + " updaters");

            for (int i = 0; i < this._updaters.Length; i++)
            {
                _updaters[i].Start();
            }
        }

        public void Stop()
        {
            for (int i = 0; i < this._updaters.Length; i++)
            {
                if (_updaters[i] != null)
                {
                    _updaters[i].Stop();
                }
            }
        }

        public void SetupVariables()
        {
            Console.Out.WriteLine("--- Adding variable definitions");

            for (int i = 0; i < this._updaters.Length; i++)
            {
                this._updaters[i].AddVariableDefinitions();
            }

            Console.Out.WriteLine("  ... " + " variable definitions added");
        }


        private readonly VariableUpdater[] _updaters;

        public long Period { get; set; }

        public Agent Agent { get; set; }


        private static readonly TableFormat VFT_TEST_VARIABLE;

        public const String VF_TEST_VARIABLE_VALUE = "value";
        public const String VF_TEST_VARIABLE_TIMESTAMP = "timestamp";
        public const String VF_TEST_VARIABLE_QUALITY = "quality";


        static UpdateManager()
        {
            VFT_TEST_VARIABLE = new TableFormat(1, 1);

            VFT_TEST_VARIABLE.addField("<" + VF_TEST_VARIABLE_VALUE + ">" + "<E>" + "<D=Value>" + "<A=1>");
            VFT_TEST_VARIABLE.addField("<" + VF_TEST_VARIABLE_TIMESTAMP + ">" + "<D>" + "<D=Timestamp>" +
                                       "<A=2000-02-01 12:00:00.000>");
            VFT_TEST_VARIABLE.addField("<" + VF_TEST_VARIABLE_QUALITY + ">" + "<I>" + "<D=Quality>" + "<A=0>");
        }

    }
}