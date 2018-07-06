using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace TestPerfStringBuilder
{
    class TestPerfStringBuilder
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting TestPerfStringBuilder. IsServerGC: " + GCSettings.IsServerGC);

            long start = DateTime.Now.Ticks;

            StringBuilder result = new TestPerfStringBuilder().process(2048, 2048);

            Console.WriteLine("Time: " + ((DateTime.Now.Ticks - start) / 10000) + ", size: " + result.Length);
        }

        private StringBuilder process(int index, int size)
        {
            StringBuilder sb = new StringBuilder();
            if (index == 0)
                return sb;

            sb.Append("index: " + index + "(");
            for (int i = 0; i < size; i++)
            {
                sb.Append(i);
            }

            sb.Append(")");

            sb.Append(process(index - 1, size));

            return sb;
        }
    }
}
