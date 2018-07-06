using System;
using System.Collections.Generic;
using System.Drawing;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.util;

namespace com.tibbo.aggregate.common.@event
{
    using System.Runtime.CompilerServices;

    using com.tibbo.aggregate.common.datatable.field;

    public class EventUtils
    {
        private static readonly Color COLOR_FATAL = Color.FromArgb(240, 120, 120);
        private static readonly Color COLOR_ERROR = Color.FromArgb(240, 190, 120);
        private static readonly Color COLOR_WARNING = Color.FromArgb(240, 240, 120);
        private static readonly Color COLOR_INFO = Color.FromArgb(160, 240, 120);
        private static readonly Color COLOR_NOTICE = Color.FromArgb(120, 150, 240);
        private static readonly Color COLOR_NONE = Color.FromArgb(230, 230, 230);

        public const String FIELD_SEVERITY_STATS_COLOR = "color";
        public const String FIELD_SEVERITY_STATS_NUMBER = "number";
        public const String FIELD_SEVERITY_STATS_LEVEL = "level";

        private static readonly TableFormat SEVERITY_STATS_FORMAT = new TableFormat();

        static EventUtils()
        {
            IntFieldFormat ff = (IntFieldFormat) FieldFormat.create("<" + FIELD_SEVERITY_STATS_LEVEL + "><I><D=" +
                                                Cres.get().getString(FIELD_SEVERITY_STATS_LEVEL) + ">");
            ff.setSelectionValues(EventLevel.getSelectionValues());
            SEVERITY_STATS_FORMAT.addField(ff);

            SEVERITY_STATS_FORMAT.addField(
                FieldFormat.create("<" + FIELD_SEVERITY_STATS_NUMBER + "><I><D=" + Cres.get().getString("efEventCount") + ">"));
            SEVERITY_STATS_FORMAT.addField(FieldFormat.create("<" + FIELD_SEVERITY_STATS_COLOR + "><C><F=H>"));
        }

        private static readonly Random ID_GENERATOR = new Random();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static  long generateEventId()
        {
            return (long)Math.Abs(ID_GENERATOR.NextDouble() * long.MaxValue);
        }




        public static List<EventDefinition> getEventDefinitions(ContextManager cm, String contextMask, String eventsMask, CallerController<CallerData> caller)
        {
            var events = new List<EventDefinition>();
            var contexts = ContextUtils.expandContextMaskToContexts(contextMask, cm, caller);

            foreach (var context in contexts)
            {
                events.AddRange(getEvents(context, eventsMask, caller));
            }

            return events;
        }

        public static List<EventDefinition> getEvents(Context context, String eventsMask,
                                                      CallerController<CallerData> caller)
        {
            var events = new List<EventDefinition>();

            if (eventsMask.Equals(ContextUtils.ENTITY_GROUP_MASK))
            {
                foreach (var ed in context.getEventDefinitions(caller))
                {
                    if (ed.getGroup() != null && !ContextUtils.GROUP_SYSTEM.Equals(ed.getGroup()))
                    {
                        events.Add(ed);
                    }
                }
            }
            else
            {
                var ed = context.getEventDefinition(eventsMask);
                if (ed != null)
                {
                    events.Add(ed);
                }
            }

            return events;
        }

        public static Boolean matchesToMask(String eventMask, EventDefinition ed)
        {
            if (ContextUtils.ENTITY_GROUP_MASK.Equals(eventMask))
            {
                return ed.getGroup() != null && !ContextUtils.GROUP_SYSTEM.Equals(ed.getGroup());
            }

            return ed.getName().Equals(eventMask);
        }

        // Result of this function may differ from the result of matchesToMask(String, EventDefinition)
        // because if doesn't check for the event's group name
        public static Boolean matchesToMask(String eventMask, String eventString)
        {
            return ContextUtils.ENTITY_GROUP_MASK.Equals(eventMask) || Util.equals(eventString, eventMask);
        }

        public static DataTable createSeverityStatisticsTable(int none, int notice, int info, int warning, int error,
                                                              int fatal)
        {
            var stats = new DataTable(SEVERITY_STATS_FORMAT);
            stats.addRecord().addInt(EventLevel.NONE).addInt(none).addColor(COLOR_NONE);
            stats.addRecord().addInt(EventLevel.NOTICE).addInt(notice).addColor(COLOR_NOTICE);
            stats.addRecord().addInt(EventLevel.INFO).addInt(info).addColor(COLOR_INFO);
            stats.addRecord().addInt(EventLevel.WARNING).addInt(warning).addColor(COLOR_WARNING);
            stats.addRecord().addInt(EventLevel.ERROR).addInt(error).addColor(COLOR_ERROR);
            stats.addRecord().addInt(EventLevel.FATAL).addInt(fatal).addColor(COLOR_FATAL);
            return stats;
        }
    }
}