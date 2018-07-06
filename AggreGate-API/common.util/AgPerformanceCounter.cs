using System;

namespace com.tibbo.aggregate.common.util
{
    public class AgPerformanceCounter
    {
        private static bool _isActive = true;

        private int _updatesCount;
        private DateTime _start = DateTime.MinValue;
        private String _message;



        public AgPerformanceCounter(String aString)
        {
            Initialize();

            _message = aString;
        }

        private void Initialize()
        {
            _updatesCount = 0;
            _start = DateTime.MinValue;
        }

        public void Count()
        {
            if (!_isActive)
            {
                Initialize();
                return;
            }

            lock (_message)
            {
                if (_start == DateTime.MinValue)
                {
                    _start = DateTime.Now;
                }

                _updatesCount++;

                if (_updatesCount < 100000)
                {
                    return;
                }


                var time = DateTime.Now - _start;

                Console.WriteLine("=========================");
                Console.WriteLine("=== Time to handle " + _updatesCount + " " + _message + ": " + time);
                Console.WriteLine("=== " + _message + " per second: " + (_updatesCount / time.TotalSeconds));
                Console.WriteLine("=========================");

                Initialize();
            }
        }
    }
}