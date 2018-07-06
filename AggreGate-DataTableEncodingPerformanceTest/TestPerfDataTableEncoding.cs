using System;
using System.Collections.Generic;
using com.tibbo.aggregate.common.util;

namespace AggreGate_DataTableEncodingPerformanceTest
{
    using System.Threading;
    using System.Threading.Tasks;

    using com.tibbo.aggregate.common;
    using com.tibbo.aggregate.common.context;
    using com.tibbo.aggregate.common.datatable;

    public class TestPerfDataTableEncoding
    {
        static void Main()
        {
            try
            {
                Log.start();

                TableFormat tf = new TableFormat(1, 1);
                tf.addField(FieldFormat.DATE_FIELD, "date");
                tf.addField(FieldFormat.FLOAT_FIELD, "value");
                tf.addField(FieldFormat.INTEGER_FIELD, "quality");

                DataTable nested = new DataTable(tf, DateTime.Now, 12345.67890f, 123);

                ClassicEncodingSettings ces = new ClassicEncodingSettings(false);
                ces.setFormatCache(new FormatCache());

                //ces.setKnownFormatCollector(new KnownFormatCollector());
                ces.setKnownFormatCollector(new KnownFormatCollector());

                DataTable table = new DataTable(AbstractContext.EF_UPDATED, "test", nested, null);

                int tablesPerTask = 10000;

                // Encoding

                //System.Threading.WaitCallback etask = new System.Threading.WaitCallback()
                Action etask = () =>
                    {
                        for (int i = 0; i < tablesPerTask; i++)
                        {
                            table.encode(ces);
                        }
                    };

                execute(tablesPerTask, etask);

                // Decoding

                // table.encode(ces); // First time format will be encoded into table, and we'd like to test decoding tables without format
                //
                // final String encoded = table.encode(ces);
                //
                // Callable dtask = new Callable()
                // {
                // @Override
                // public Object call() throws Exception
                // {
                // try
                // {
                // for (int i = 0; i < tablesPerTask; i++)
                // {
                // new DataTable(encoded, ces, true);
                // }
                // }
                // catch (Exception ex)
                // {
                // ex.printStackTrace();
                // }
                // return null;
                // }
                // };
                //
                // execute(tablesPerTask, dtask);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static void execute(int tablesPerTask, Action action) //throws InterruptedException
        {
            int availableProcessors = Environment.ProcessorCount;

            Console.WriteLine("CPUs: " + availableProcessors);


            //ExecutorService es = Executors.newFixedThreadPool(availableProcessors);
            var tasks = new List<Task>();

            int taskNum = 1000;

            var st = DateTime.Now;

            try
            {
                for (int i = 0; i < taskNum; i++)
                {
                    tasks.Add(Task.Factory.StartNew(action));
                }
                //es.invokeAll(tasks);    
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while adding tasks: " + ex.Message);
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while waiting for task completion: " + ex.Message);
            }

            var time = (DateTime.Now - st).TotalMilliseconds;
            time = time / 1000;

            float taskCount = taskNum * tablesPerTask;

            Console.WriteLine("Time: " + time);

            Console.WriteLine("Tables per second: " + (long)(taskCount / time));
        }
    }
}
