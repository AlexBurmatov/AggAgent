namespace com.tibbo.aggregate.common.context
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class PermissionCache
    {
        private readonly ConcurrentDictionary<string, string> effectiveLevels = new ConcurrentDictionary<string, string>();

        public string getLevel(string context)
        {
            string value;
            this.effectiveLevels.TryGetValue(context, out value);
            return value;
        }

        public void cacheLevel(string context, string level)
        {
            this.effectiveLevels[context] = level;
        }
    }
}