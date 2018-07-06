namespace com.tibbo.aggregate.common.util
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class AgTimeMeasurer
    {
        public AgTimeMeasurer(String nameString)
        {
            name = nameString;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void StartOperation()
        {
            this.current++;
            this.total++;

            this.stopwatch.Start();
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public void FinishOperation()
        {
            this.stopwatch.Stop();

            if (this.current > 100000)
            {
                Console.WriteLine("=========================");
                Console.WriteLine("=== " + name + " - " + "Elapsed Time: " + this.stopwatch.Elapsed);
                Console.WriteLine("=== " + name + " - " + "Operations: " + this.total);
                Console.WriteLine("=== " + name + " - " + "Operations per second: " + (this.total / this.stopwatch.ElapsedMilliseconds * 1000));
                Console.WriteLine("=========================");

                this.current = 0;
            }
        }

        private string name;
        private int current = 0;
        private int total = 0;
        private readonly Stopwatch stopwatch = new Stopwatch();
    }
}