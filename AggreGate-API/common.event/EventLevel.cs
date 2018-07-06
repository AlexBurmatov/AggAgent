using System;
using System.Collections.Generic;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.@event
{
    using com.tibbo.aggregate.common.datatable;

    public class EventLevel
    {
        private static readonly AgDictionary<int, string> LEVELS = new AgDictionary<int, string>();
        private static readonly AgDictionary<NullableObject, string> SELECTION_VALUES = new AgDictionary<NullableObject, string>();

        public const int NUM_LEVELS = 5;

        public const int NONE = 0;
        public const int NOTICE = 1;
        public const int INFO = 2;
        public const int WARNING = 3;
        public const int ERROR = 4;
        public const int FATAL = 5;

        static EventLevel()
        {
            LEVELS.Add(NOTICE, Cres.get().getString("conElNotice"));
            LEVELS.Add(INFO, Cres.get().getString("conElInfo"));
            LEVELS.Add(WARNING, Cres.get().getString("conElWarning"));
            LEVELS.Add(ERROR, Cres.get().getString("conElError"));
            LEVELS.Add(FATAL, Cres.get().getString("conElFatal"));
            LEVELS.Add(NONE, Cres.get().getString("conElNotDefined"));

            SELECTION_VALUES.Add(new NullableObject(0), Cres.get().getString("none"));

            for (int i = 1; i <= NUM_LEVELS; i++)
            {
                SELECTION_VALUES.Add(new NullableObject(i), getName(i));
            }
        }

        public static string getName(int level)
        {
            return LEVELS[level];
        }

        public static AgDictionary<NullableObject, string> getSelectionValues()
        {
            return new AgDictionary<NullableObject, string>(SELECTION_VALUES);
        }
    }
}