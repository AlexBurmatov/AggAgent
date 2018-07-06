namespace com.tibbo.aggregate.common.context
{
    using System.Threading;

    public class ReentrantLock
    {
        public ReentrantLock()
        {
        }

        public bool tryLock(long lockTimeoutMs)
        {
            return Monitor.TryEnter(this, (int)lockTimeoutMs);
        }

        public void Lock()
        {
            Monitor.Enter(this);
        }

        public void Unlock()
        {
            Monitor.Exit(this);
        }
    }
}