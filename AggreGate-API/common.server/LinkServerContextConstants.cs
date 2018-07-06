using System;

namespace com.tibbo.aggregate.common.server
{
    public class LinkServerContextConstants
    {
        public const String V_VDATA = "vdata";
        public const String V_VISIBLE_CHILDREN = "visibleChildren";
        public const String V_CONTEXT_STATUS = "contextStatus";
        public const String V_ACTIONS = "actions";
        public const String V_VARIABLE_STATUSES = "variableStatuses";

        public const String F_REORDER = "reorder";
        public const String F_INIT_ACTION = "initAction";
        public const String F_STEP_ACTION = "stepAction";

        public const String E_ACTION_ADDED = "actionAdded";
        public const String E_ACTION_REMOVED = "actionRemoved";
        public const String E_ACTION_STATE_CHANGED = "actionStateChanged";
        public const String E_VISIBLE_INFO_CHANGED = "visibleInfoChanged";
        public const String E_VISIBLE_CHILD_REMOVED = "visibleChildRemoved";
        public const String E_VISIBLE_CHILD_ADDED = "visibleChildAdded";
        public const String E_CONTEXT_STATUS_CHANGED = "contextStatusChanged";
        public const String E_VARIABLE_STATUS_CHANGED = "variableStatusChanged";

        public const String VF_VDATA_ACTIONS = "actions";
        public const String VF_VDATA_VISIBLE_INFO = "visibleInfo";
        public const String VF_VDATA_VISIBLE_CHILDREN = "visibleChildren";
        public const String VF_VISIBLE_INFO_DYNAMIC = "dynamic";
        public const String VF_VISIBLE_INFO_CHILDREN_REORDERABLE = "childrenReorderable";
        public const String VF_VISIBLE_INFO_EXPANDED = "expanded";
        public const String VF_VISIBLE_CHILDREN_PATH = "path";
        public const String VF_CONTEXT_STATUS_STATUS = "status";
        public const String VF_CONTEXT_STATUS_COMMENT = "comment";

        public const String FIELD_VARIABLE_STATUSES_NAME = "name";
        public const String FIELD_VARIABLE_STATUSES_STATUS = "status";
        public const String FIELD_VARIABLE_STATUSES_COMMENT = "comment";

        public const String FIF_INIT_ACTION_INPUT_DATA = "inputData";
        public const String FIF_INIT_ACTION_INITIAL_PARAMETERS = "initialParameters";
        public const String FIF_INIT_ACTION_ACTION_NAME = "actionName";

        public const String FOF_INIT_ACTION_ACTION_ID = "actionId";
        public const String FOF_STEP_ACTION_ACTION_COMMAND = "actionCommand";

        public const String EF_CONTEXT_STATUS_CHANGED_STATUS = "status";
        public const String EF_CONTEXT_STATUS_CHANGED_COMMENT = "comment";
        public const String EF_CONTEXT_STATUS_CHANGED_OLD_STATUS = "oldStatus";
        public const String EF_VISIBLE_CHILD_REMOVED_PATH = "path";
        public const String EF_VISIBLE_CHILD_ADDED_PATH = "path";
        public const String EF_ACTION_REMOVED_ACTION = "action";
        public const String EF_ACTION_ADDED_ACTION = "action";
    }
}