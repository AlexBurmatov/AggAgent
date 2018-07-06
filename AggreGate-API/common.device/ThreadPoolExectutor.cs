using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace com.tibbo.aggregate.common.device
{
    public class ThreadPoolExectutor
    {
        private List<AbstractClientController.ProcessCommandTask> tasks = new List<AbstractClientController.ProcessCommandTask>();

        public Boolean awaitTermination(Int32 timeoutSeconds)
        {
            var startTime = DateTime.Now;
            foreach (var each in tasks)
            {
                each.interrupt();
            }

            while ((DateTime.Now - startTime).Duration().Seconds < timeoutSeconds)
            {
                if (tasks.Find(each => !each.isFinished) == null)
                    return true;
                Thread.Sleep(300);
            }
            return false;
        }

        public void submit(AbstractClientController.ProcessCommandTask aTask)
        {
            tasks.Add(aTask);
            ThreadPool.QueueUserWorkItem(aTask.call);

            foreach (var each in tasks.ToList().Where(each => each.isFinished))
            {
                tasks.Remove(each);
            }

            //var curr = new Thread(aTask.call);
            //curr.Start();
        }
    }
}