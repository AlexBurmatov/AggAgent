using System;

namespace com.tibbo.aggregate.common.server
{
    public class UtilsContextConstants
    {
        public const String F_DEBUG = "debug";
        public const String F_VARIABLE_ACTIONS = "variableActions";
        public const String F_EVENT_ACTIONS = "eventActions";
        public const String F_INIT_ACTIONS = "initActions";
        public const String F_GET_DATA = "getData";
        public const String F_GET_FORMAT = "getFormat";
        public const String F_CHART_DATA = "chartData";
        public const String F_ACTIONS_BY_MASK = "actionsByMask";
        public const String F_EVENTS_BY_MASK = "eventsByMask";
        public const String F_VARIABLES_BY_MASK = "variablesByMask";
        public const String F_EVENT_FIELDS = "eventFields";
        public const String F_VARIABLE_FIELDS = "variableFields";

        public const String VF_VARIABLE_ACTIONS_ICON = "icon";
        public const String VF_VARIABLE_ACTIONS_DESCRIPTION = "description";
        public const String VF_VARIABLE_ACTIONS_NAME = "name";
        public const String VF_VARIABLE_ACTIONS_CONTEXT = "context";
        public const String VF_EVENT_ACTIONS_ICON = "icon";
        public const String VF_EVENT_ACTIONS_DESCRIPTION = "description";
        public const String VF_EVENT_ACTIONS_NAME = "name";
        public const String VF_EVENT_ACTIONS_CONTEXT = "context";

        public const String FIF_INIT_ACTIONS_CONTEXT = "context";
        public const String FIF_INIT_ACTIONS_ACTION_NAME = LinkServerContextConstants.FIF_INIT_ACTION_ACTION_NAME;

        public const String FIF_INIT_ACTIONS_INITIAL_PARAMETERS =
            LinkServerContextConstants.FIF_INIT_ACTION_INITIAL_PARAMETERS;

        public const String FIF_INIT_ACTIONS_INPUT_DATA = LinkServerContextConstants.FIF_INIT_ACTION_INPUT_DATA;
        public const String FIF_GET_DATA_ID = "id";

        public const String FOF_INIT_ACTIONS_ACTION_ID = "actionId";
        public const String FOF_GET_DATA_DATA = "data";
        public const String FOF_GET_FORMAT_DATA = "data";
    }
}