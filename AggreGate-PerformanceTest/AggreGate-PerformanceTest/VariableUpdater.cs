namespace AggreGatePerformanceTest
{
    using System;
    using System.Threading;

    using com.tibbo.aggregate.common.agent;
    using com.tibbo.aggregate.common.context;
    using com.tibbo.aggregate.common.datatable;

    internal class VariableUpdater
    {
        public VariableUpdater(AgentContext agentContext, int variableCount, int startVariableNumber, TableFormat variableFormat, TableFormat eventTableFormat, long period)
        {
            this._context = agentContext;
            this._variables = new DataTable[variableCount];

            this._startVariableNumber = startVariableNumber;

            for (int i = 0; i < variableCount; i++)
            {
                var dataTable = new DataRecord(variableFormat).wrap();
                this._variables[i] = new DataTable(eventTableFormat, "variable" + (this._startVariableNumber + i), dataTable);
            }
            this._period = period;
        }

        public void Start()
        {
            this._timer = new Timer(
                x =>
                {
                    UpdateVariables();
                }, null, 0, _period);
        }

        public void Stop()
        {
            this._timer.Dispose();
            this._timer = null;
        }

        private void UpdateVariables()
        {
            //Console.Out.WriteLine("[" + DateTime.Now + "] Going to update " + this._variables.Length + " variables");

            if (!_context.isSynchronized())
            {
                //Console.Out.WriteLine("[" + DateTime.Now + "] " + "Context is NOT synchronized, skipping update");
                return;
            }

            //Console.Out.WriteLine("[" + DateTime.Now + "] " + "Context is synchronized, updating");
            for (int i = 0; i < this._variables.Length; i++)
            {
                ////Console.Out.WriteLine("... updating " + "variable" + i);

                //var valueRecord = new DataRecord(VFT_TEST_VARIABLE);
                var dataTable = this._variables[i];
                var valueRecord = dataTable.rec().getDataTable("value").rec();
                valueRecord.setValue(0, _random.NextDouble() * 1000000);
                valueRecord.setValue(1, DateTime.Now);
                valueRecord.setValue(2, _random.Next(0, 100));

                dataTable.setTimestamp(DateTime.Now);
                dataTable.setQuality(123);

                //context.fireEvent(AbstractContext.E_UPDATED, "variable" + i, dataTable);
                //_context.fireEvent(
                //    AbstractContext.E_UPDATED, //String name,
                //    dataTable, // DataTable data,
                //    -1, // int? level,
                //    null, //long? id,
                //    null, //DateTime? creationtime, 
                //    null, //int? listener,
                //    null, // CallerController< CallerData > caller,
                //    null  // FireEventRequestController request
                //    );
                this._context.fireEvent(AbstractContext.E_UPDATED, "variable" + (this._startVariableNumber + i), dataTable, null);


                //context.setVariable("variable" + i, valueRecord.wrap());
            }
            //Console.Out.WriteLine("[" + DateTime.Now + "] !!! Updated !!!");
        }


        public void AddVariableDefinitions()
        {
            for (int i = 0; i < this._variables.Length; i++)
            {
                var variableDefinition = new VariableDefinition(
                    "variable" + (_startVariableNumber + i),
                    this._variables[i].getFormat(), 
                    true, 
                    true, 
                    "Variable" + (_startVariableNumber + i),
                    ContextUtils.GROUP_REMOTE);
                _context.addVariableDefinition(variableDefinition);
            }
        }


        private readonly AgentContext _context;

        private readonly int _startVariableNumber;
        private readonly DataTable[] _variables;

        private readonly Random _random = new Random();


        private readonly long _period;
        private Timer _timer;

    }
}