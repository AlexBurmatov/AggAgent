using com.tibbo.aggregate.common.datatable;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using com.tibbo.aggregate.common.agent;
using com.tibbo.aggregate.common.command;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.device;
using com.tibbo.aggregate.common.util;

namespace AggreGate_TestPerfAgent
{
    class TestPerfAgent
    {
        private const string V_SETTING = "setting";
        private const string V_STATS = "stats";

        private const string VF_SETTING_FLOAT = "float";

        private const string VF_STATS_COUNT = "count";
        private const string VF_STATS_RATE = "rate";

        private static TableFormat VFT_SETTING = new TableFormat(1, 1);

        private static TableFormat VFT_STATISTICS = new TableFormat(1, 1);

        private static int SettingCount = 40000;
        private static int Period = 500;

        private static long START_TIME = DateTime.Now.Ticks;

        private static long COUNT = 0;

        private static long index = 0;

        private static Random random = new Random();
        private static Agent agent;


        static TestPerfAgent()
        {
            VFT_SETTING.addField("<" + VF_SETTING_FLOAT + "><F><D=Float Field>");

            VFT_STATISTICS.addField("<" + VF_STATS_COUNT + "><L><D=Update Count>");
            VFT_STATISTICS.addField("<" + VF_STATS_RATE + "><L><D=Updates per Second>");
        }

        private static volatile int THREADS = 0;
        private Timer _timer;

        static void Main(string[] args)
        {
            if (args.Length > 2)
            {
                SettingCount = Convert.ToInt32(args[0]);
                Period = Convert.ToInt32(args[1]);
            }

            new TestPerfAgent().Run();
        }

        class MyThread
        {
            public MyThread(Thread th)
            {
                th.IsBackground = false;
                th.Start();
            }
        }

        private void Run()
        {
            Console.WriteLine("Starting TestPerfAgent");

            RemoteLinkServer rls = new RemoteLinkServer(AggreGateNetworkDevice.DEFAULT_ADDRESS, Agent.DEFAULT_PORT, RemoteLinkServer.DEFAULT_USERNAME, RemoteLinkServer.DEFAULT_PASSWORD);
            agent = new Agent(rls, "performanceTestingAgent", true);

            initializeAgentContext(agent.getContext());

            agent.connect();
            agent.run();


            new MyThread(new Thread(AgentProcess));

            _timer = new Timer(
                x =>
                {

                    if (agent.getContext().isSynchronized())
                    {
                        new MyThread(new Thread(Process));
                        // Console.WriteLine("Create task, size: " + THREADS);
                    }
                }, null, 0, Period);

            while (Thread.CurrentThread.IsAlive)
            {
                Thread.Sleep(50);
            }

            // Disconnecting from the server
            agent.disconnect();
        }

        private void AgentProcess(object o)
        {
            while (Thread.CurrentThread.IsAlive)
            {
                agent.run();
            }
        }

        private void Process(object stateInfo)
        {
            try
            {
                THREADS++;

                //                Console.WriteLine("-->");
                long start = DateTime.Now.Ticks;
                Process2();
                //Console.WriteLine("<--,  time: " + ((DateTime.Now.Ticks - start) / 10000));
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                THREADS--;
            }
        }

        private static void Process2()
        {
            if (!agent.getContext().isSynchronized()) return;


            for (int j = 0; j < SettingCount; j++)
            {
                DataTable setting = new DataTable(VFT_SETTING, true);

                setting.rec().setValue(VF_SETTING_FLOAT, (float)(random.NextDouble() * 1000000));
                setting.setQuality(192);
                setting.setTimestamp(DateTime.Now);

                string variable = V_SETTING + j;
                agent.getContext().fireEvent(AbstractContext.E_UPDATED, variable, setting);

                COUNT++;

                if (COUNT % 200000 == 0)
                {
                    Console.WriteLine("Sent " + COUNT + ", rate " + COUNT * 1000 / ((DateTime.Now.Ticks - START_TIME)/10000));

                    START_TIME = DateTime.Now.Millisecond;
                    COUNT = 0;
                }
            }
        }

        private void initializeAgentContext(AgentContext context)
        {
            for (int i = 0; i < SettingCount; i++)
            {
                VariableDefinition vd = new VariableDefinition(V_SETTING + i.ToString(), VFT_SETTING, true, false, "Setting " + i.ToString(), ContextUtils.GROUP_REMOTE);
                vd.setGetter(new DelegatedVariableGetter((con, def, caller, request) => PerformanceSettings));
                context.addVariableDefinition(vd);
            }

            VariableDefinition svd = new VariableDefinition(V_STATS, VFT_STATISTICS, true, false, "Statistics", ContextUtils.GROUP_REMOTE);

            svd.setGetter(new DelegatedVariableGetter((con, def, caller, request) => StatisticsSettings));
            context.addVariableDefinition(svd);
        }

        private DataTable PerformanceSettings
        {
            get
            {
                DataTable setting = new DataTable(VFT_SETTING, true);
                setting.rec().setValue(VF_SETTING_FLOAT, (float)(random.NextDouble() * 1000000));
                return setting;
            }
        }

        private DataTable StatisticsSettings
        {
            get
            {
                return new DataTable(VFT_STATISTICS, COUNT, COUNT * 1000 / (DateTime.Now.Millisecond - START_TIME));
            }
        }
    }
}
