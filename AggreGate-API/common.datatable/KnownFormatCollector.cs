namespace com.tibbo.aggregate.common.datatable
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class KnownFormatCollector
    {
        public KnownFormatCollector()
        {
        }

        public bool isKnown(int? formatId)
        {
            if (formatId == null)
            {
                return false;
            }
            return this.formatIds.ContainsKey((int)formatId);
        }

        public bool isMarked(int? formatId)
        {
            if (formatId == null)
            {
                return false;
            }

            if (!this.formatIds.ContainsKey((int)formatId))
            {
                return false;
            }
            return this.formatIds[(int)formatId];
        }

        public void makeKnown(int formatId, bool mark)
        {
            this.formatIds[formatId] = mark;
        }

        public void markAll()
        {
            // Trick required to avoid inability to upgrade a read lock
            HashSet<int> idsToMark = new HashSet<int>();

            foreach (var entry in formatIds)
            {
                if (! entry.Value)
                {
                    idsToMark.Add(entry.Key);
                }
            }

            if (idsToMark.Count == 0)
            {
                return;
            }

            foreach (var id in idsToMark)
            {
                formatIds[id] = true;
            }
        }

        private readonly ConcurrentDictionary<int, bool> formatIds = new ConcurrentDictionary<int, bool>();
    }
}