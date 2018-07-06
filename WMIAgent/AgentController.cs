using System.Threading;
using com.tibbo.aggregate.common.agent;

namespace WMIAgent
{
    internal class AgentController
    {
        private Thread thread;

        private ManualResetEvent waitHandler = new ManualResetEvent(false);
        private Agent agent;

        public AgentController(Agent anAgent)
        {
            agent = anAgent;

            thread = new Thread(() =>
                {
                while (true)
                {
                    waitHandler.WaitOne();
                    agent.run();
                }
                }) {IsBackground = true};
        }

        public void resume()
        {
            waitHandler.Set();
        }

        public void pause()
        {
            waitHandler.Reset();
        }

        public void start()
        {
            agent.connect();
            thread.Start();
            resume();
        }

        public void stop()
        {
            pause();
            thread.Interrupt();
            thread.Abort();
            Thread.Sleep(1000);
            agent.disconnect();
        }
    }
}