namespace com.tibbo.aggregate.common.util
{
    using System;

    public class AgOperationsMeasurer
    {
        public AgOperationsMeasurer(String nameString, long outputLimitLong)
        {
            this.name = nameString;
            this.outputLimit = outputLimitLong;
            this.start = DateTime.Now;
        }

        public void Count(int dataPacketSize)
        {
            this.callCounter++;
            this.counter++;
            this.somethingCounter += dataPacketSize;
            if (this.counter > this.outputLimit)
            {
                this.counter = 0;
                var delta = DateTime.Now - start;

                Console.WriteLine("+++++++++++++++++++++++++");
                Console.WriteLine("+ " + name + " - " + "Calls: " + this.callCounter);
                Console.WriteLine("+ " + "  " + (int)(this.callCounter / delta.TotalMilliseconds * 1000) +
                                  " calls per second");
                Console.WriteLine("+ " + name + " - " + "Somethings: " + this.somethingCounter);
                Console.WriteLine("+ " + "  " + (int)(this.somethingCounter / delta.TotalMilliseconds * 1000) +
                                  " somethings per second");
                Console.WriteLine("+ GC TotalMemory: " + GC.GetTotalMemory(false)/(1024*1024) + " Mb / 1000 Mb"); // From OPCDA.GC_MEMORY_MAX_SIZE
                Console.WriteLine("+++++++++++++++++++++++++");

                start = DateTime.Now;
                this.callCounter = 0;
                this.somethingCounter = 0;
            }
        }

        private String name;
        private DateTime start = DateTime.MinValue;

        private volatile int callCounter;
        private volatile int somethingCounter;

        private volatile int counter;

        private long outputLimit;

    }
}
