using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using com.tibbo.aggregate.common.data;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.@event;
using com.tibbo.aggregate.common.security;
using com.tibbo.aggregate.common.util;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.context
{
    using System.Globalization;
    using System.Windows.Documents;

    using com.tibbo.aggregate.common.expression;

    public abstract class AbstractContext : Context
    {
        #region constants

        private const string IMPLEMENTATION_METHOD_PREFIX = "callF";
        private const string SETTER_METHOD_PREFIX = "setV";
        private const string GETTER_METHOD_PREFIX = "getV";

        public const string V_INFO = "info";
        public const string V_CHILDREN = "children";
        public const string V_VARIABLES = "variables";
        public const string V_FUNCTIONS = "functions";
        public const string V_EVENTS = "events";

        public const string F_GET_COPY_DATA = "getCopyData";
        public const string F_COPY = "copy";
        public const string F_COPY_TO_CHILDREN = "copyToChildren";

        public const string E_INFO = "info";
        public const string E_UPDATED = "updated";
        public const string E_CHANGE = "change";
        public const string E_DESTROYED = "destroyed";
        public const string E_INFO_CHANGED = "infoChanged";
        public const string E_EVENT_REMOVED = "eventRemoved";
        public const string E_EVENT_ADDED = "eventAdded";
        public const string E_FUNCTION_REMOVED = "functionRemoved";
        public const string E_FUNCTION_ADDED = "functionAdded";
        public const string E_VARIABLE_REMOVED = "variableRemoved";
        public const string E_VARIABLE_ADDED = "variableAdded";
        public const string E_CHILD_REMOVED = "childRemoved";
        public const string E_CHILD_ADDED = "childAdded";

        public const string VF_INFO_DESCRIPTION = "description";
        public const string VF_INFO_TYPE = "type";
        public const string VF_INFO_GROUP = "group";
        public const string VF_INFO_ICON = "icon";

        public const string VF_CHILDREN_NAME = "name";

        public const string FIF_COPY_DATA_RECIPIENTS = "recipients";
        public const string FIF_COPY_DATA_GROUP = "group";

        public const string FOF_COPY_DATA_NAME = "name";
        public const string FOF_COPY_DATA_DESCRIPTION = "description";
        public const string FOF_COPY_DATA_REPLICATE = "replicate";
        public const string FOF_COPY_DATA_FIELDS = "fields";
        public const string FOF_COPY_DATA_VALUE = "value";

        public const string FIF_REPLICATE_FIELDS_NAME = "name";
        public const string FIF_REPLICATE_FIELDS_DESCRIPTION = "description";
        public const string FIF_REPLICATE_FIELDS_REPLICATE = "replicate";

        public const string FIF_COPY_DATA_RECIPIENTS_RECIPIENT = "recipient";

        public const string EF_INFO_INFO = "info";

        public const string EF_EVENT_REMOVED_NAME = "name";
        public const string EF_FUNCTION_REMOVED_NAME = "name";
        public const string EF_VARIABLE_REMOVED_NAME = "name";

        public const string EF_CHILD_REMOVED_CHILD = "child";
        public const string EF_CHILD_ADDED_CHILD = "child";

        private const string FIELD_REPLICATE_CONTEXT = "context";
        public const string FIELD_REPLICATE_VARIABLE = "variable";
        public const string FIELD_REPLICATE_SUCCESSFUL = "successful";
        public const string FIELD_REPLICATE_ERRORS = "errors";

        public const string EF_UPDATED_VARIABLE = "variable";
        public const string EF_UPDATED_VALUE = "value";
        public const string EF_UPDATED_USER = "user";

        public const string EF_CHANGE_VARIABLE = "variable";
        public const string EF_CHANGE_VALUE = "value";
        public const string EF_CHANGE_DATA = "data";

        public const string FIELD_VD_HELP_ID = "helpId";
        public const string FIELD_VD_ICON_ID = "iconId";
        public const string FIELD_VD_GROUP = "group";
        public const string FIELD_VD_HELP = "help";
        public const string FIELD_VD_WRITABLE = "writable";
        public const string FIELD_VD_READABLE = "readable";
        public const string FIELD_VD_DESCRIPTION = "description";
        public const string FIELD_VD_FORMAT = "format";
        public const string FIELD_VD_NAME = "name";

        public const string FIELD_FD_NAME = "name";
        public const string FIELD_FD_INPUTFORMAT = "inputformat";
        public const string FIELD_FD_OUTPUTFORMAT = "outputformat";
        public const string FIELD_FD_DESCRIPTION = "description";
        public const string FIELD_FD_HELP = "help";
        public const string FIELD_FD_GROUP = "group";
        public const string FIELD_FD_ICON_ID = "iconId";
        public const string FIELD_FD_CONCURRENT = "concurrent";
        public const string FIELD_FD_PERMISSIONS = "permissions";

        public const string FIELD_ED_ICON_ID = "iconId";
        public const string FIELD_ED_GROUP = "group";
        public const string FIELD_ED_LEVEL = "level";
        public const string FIELD_ED_HELP = "help";
        public const string FIELD_ED_DESCRIPTION = "description";
        public const string FIELD_ED_FORMAT = "format";
        public const string FIELD_ED_NAME = "name";

        protected static readonly TableFormat VARIABLE_DEFINITION_FORMAT = new TableFormat();

        private static void initializeVariableDefinitionFormat()
        {
            VARIABLE_DEFINITION_FORMAT.addField("<" + FIELD_VD_NAME + "><S>");
            VARIABLE_DEFINITION_FORMAT.addField("<" + FIELD_VD_FORMAT + "><S><F=N>");
            VARIABLE_DEFINITION_FORMAT.addField("<" + FIELD_VD_DESCRIPTION + "><S><F=N>");
            VARIABLE_DEFINITION_FORMAT.addField("<" + FIELD_VD_READABLE + "><B>");
            VARIABLE_DEFINITION_FORMAT.addField("<" + FIELD_VD_WRITABLE + "><B>");
            VARIABLE_DEFINITION_FORMAT.addField("<" + FIELD_VD_HELP + "><S><F=N>");
            VARIABLE_DEFINITION_FORMAT.addField("<" + FIELD_VD_GROUP + "><S><F=N>");
            VARIABLE_DEFINITION_FORMAT.addField("<" + FIELD_VD_ICON_ID + "><S><F=N>");
            VARIABLE_DEFINITION_FORMAT.addField("<" + FIELD_VD_HELP_ID + "><S><F=N>");
        }

        private static readonly TableFormat EF_VARIABLE_ADDED = (TableFormat)VARIABLE_DEFINITION_FORMAT.Clone();

        private static void initializeEfVariableAdded()
        {
            EF_VARIABLE_ADDED.setMinRecords(1);
            EF_VARIABLE_ADDED.setMaxRecords(1);
        }

        protected static readonly TableFormat FUNCTION_DEFINITION_FORMAT = new TableFormat();

        private static void initializeFunictionDefinitionFormat()
        {
            FUNCTION_DEFINITION_FORMAT.addField("<" + FIELD_FD_NAME + "><S>");
            FUNCTION_DEFINITION_FORMAT.addField("<" + FIELD_FD_INPUTFORMAT + "><S><F=N>");
            FUNCTION_DEFINITION_FORMAT.addField("<" + FIELD_FD_OUTPUTFORMAT + "><S><F=N>");
            FUNCTION_DEFINITION_FORMAT.addField("<" + FIELD_FD_DESCRIPTION + "><S><F=N>");
            FUNCTION_DEFINITION_FORMAT.addField("<" + FIELD_FD_HELP + "><S><F=N>");
            FUNCTION_DEFINITION_FORMAT.addField("<" + FIELD_FD_GROUP + "><S><F=N>");
            FUNCTION_DEFINITION_FORMAT.addField("<" + FIELD_FD_ICON_ID + "><S><F=N>");
        }

        public static readonly TableFormat EF_FUNCTION_ADDED = (TableFormat)FUNCTION_DEFINITION_FORMAT.Clone();

        private static void initializeEfFunctionAdded()
        {
            EF_FUNCTION_ADDED.setMinRecords(1);
            EF_FUNCTION_ADDED.setMaxRecords(1);
        }

        protected static readonly TableFormat EVENT_DEFINITION_FORMAT = new TableFormat();

        private static void initializeEventDefinitionFormat()
        {
            EVENT_DEFINITION_FORMAT.addField("<" + FIELD_ED_NAME + "><S>");
            EVENT_DEFINITION_FORMAT.addField("<" + FIELD_ED_FORMAT + "><S><F=N>");
            EVENT_DEFINITION_FORMAT.addField("<" + FIELD_ED_DESCRIPTION + "><S><F=N>");
            EVENT_DEFINITION_FORMAT.addField("<" + FIELD_ED_HELP + "><S><F=N>");
            EVENT_DEFINITION_FORMAT.addField("<" + FIELD_ED_LEVEL + "><I>");
            EVENT_DEFINITION_FORMAT.addField("<" + FIELD_ED_GROUP + "><S><F=N>");
            EVENT_DEFINITION_FORMAT.addField("<" + FIELD_ED_ICON_ID + "><S><F=N>");
        }

        private static readonly TableFormat EF_EVENT_ADDED = (TableFormat)EVENT_DEFINITION_FORMAT.Clone();

        private static void initializeEfEventAdded()
        {
            EF_EVENT_ADDED.setMinRecords(1);
            EF_EVENT_ADDED.setMaxRecords(1);
        }

        protected static readonly TableFormat VFT_CHILDREN =
            FieldFormat.create("<" + VF_CHILDREN_NAME + "><S>").wrap();

        protected static readonly TableFormat INFO_DEFINITION_FORMAT = new TableFormat(1, 1);

        private static void initializeInfoDefinitionFormat()
        {
            INFO_DEFINITION_FORMAT.addField("<" + VF_INFO_DESCRIPTION + "><S><F=N><D=" +
                                            Cres.get().getString(VF_INFO_DESCRIPTION) + ">");
            INFO_DEFINITION_FORMAT.addField("<" + VF_INFO_TYPE + "><S><D=" + Cres.get().getString(VF_INFO_TYPE) + ">");
            INFO_DEFINITION_FORMAT.addField("<" + VF_INFO_GROUP + "><S><F=N><D=" + Cres.get().getString(VF_INFO_GROUP) +
                                            ">");
            INFO_DEFINITION_FORMAT.addField("<" + VF_INFO_ICON + "><S><F=N><D=" + Cres.get().getString("conIconId") +
                                            ">");
        }

        private static readonly TableFormat FIFT_GET_COPY_DATA = new TableFormat(1, 1);

        private static void initializeFiftGetCopyData()
        {
            FIFT_GET_COPY_DATA.addField("<" + FIF_COPY_DATA_GROUP + "><S><F=N>");
            FIFT_GET_COPY_DATA.addField("<" + FIF_COPY_DATA_RECIPIENTS + "><T><F=N>");
        }

        public static readonly TableFormat FIFT_GET_COPY_DATA_RECIPIENTS =
            FieldFormat.create("<" + FIF_COPY_DATA_RECIPIENTS_RECIPIENT + "><S>").wrap();

        public static readonly TableFormat REPLICATE_INPUT_FORMAT = new TableFormat();

        private static void initializeReplicateInputFormat()
        {
            REPLICATE_INPUT_FORMAT.addField("<" + FOF_COPY_DATA_NAME + "><S><F=RHK>");
            REPLICATE_INPUT_FORMAT.addField("<" + FOF_COPY_DATA_DESCRIPTION + "><S><F=R><D=" +
                                            Cres.get().getString("variable") + ">");
            REPLICATE_INPUT_FORMAT.addField("<" + FOF_COPY_DATA_REPLICATE + "><B><A=0><D=" +
                                            Cres.get().getString("replicate") + ">");
            REPLICATE_INPUT_FORMAT.addField("<" + FOF_COPY_DATA_FIELDS + "><T><D=" + Cres.get().getString("fields") +
                                            ">");
            REPLICATE_INPUT_FORMAT.addField("<" + FOF_COPY_DATA_VALUE + "><T><D=" + Cres.get().getString("value") + ">");
        }

        public static readonly TableFormat FIFT_REPLICATE_FIELDS = new TableFormat();

        private static void initializeFiftReplicateFields()
        {
            FIFT_REPLICATE_FIELDS.addField("<" + FIF_REPLICATE_FIELDS_NAME + "><S><F=RHK>");
            FIFT_REPLICATE_FIELDS.addField("<" + FIF_REPLICATE_FIELDS_DESCRIPTION + "><S><F=R><D=" +
                                           Cres.get().getString("field") + ">");
            FIFT_REPLICATE_FIELDS.addField("<" + FIF_REPLICATE_FIELDS_REPLICATE + "><B><A=1><D=" +
                                           Cres.get().getString("replicate") + ">");
            FIFT_REPLICATE_FIELDS.setNamingExpression("print({}, '{" + FIF_REPLICATE_FIELDS_REPLICATE + "} ? {" +
                                                      FIF_REPLICATE_FIELDS_DESCRIPTION + "} : null', ', ')");
        }

        public static readonly TableFormat REPLICATE_OUTPUT_FORMAT = new TableFormat();

        private static void initializeReplicateOutpuotFormat()
        {
            REPLICATE_OUTPUT_FORMAT.addField("<" + FIELD_REPLICATE_VARIABLE + "><S><D=" +
                                             Cres.get().getString("variable") + ">");
            REPLICATE_OUTPUT_FORMAT.addField("<" + FIELD_REPLICATE_SUCCESSFUL + "><B><D=" +
                                             Cres.get().getString("successful") + ">");
            REPLICATE_OUTPUT_FORMAT.addField("<" + FIELD_REPLICATE_ERRORS + "><S><D=" + Cres.get().getString("errors") +
                                             ">");
        }

        protected static readonly TableFormat REPLICATE_TO_CHILDREN_OUTPUT_FORMAT = new TableFormat();

        private static void initializeReplicateToChildrenOutputFormat()
        {
            REPLICATE_TO_CHILDREN_OUTPUT_FORMAT.addField("<" + FIELD_REPLICATE_CONTEXT + "><S><D=" +
                                                         Cres.get().getString("context") + ">");
            REPLICATE_TO_CHILDREN_OUTPUT_FORMAT.addField("<" + FIELD_REPLICATE_VARIABLE + "><S><D=" +
                                                         Cres.get().getString("variable") + ">");
            REPLICATE_TO_CHILDREN_OUTPUT_FORMAT.addField("<" + FIELD_REPLICATE_SUCCESSFUL + "><B><D=" +
                                                         Cres.get().getString("successful") + ">");
            REPLICATE_TO_CHILDREN_OUTPUT_FORMAT.addField("<" + FIELD_REPLICATE_ERRORS + "><S><D=" +
                                                         Cres.get().getString("errors") + ">");
        }

        public static readonly TableFormat EF_UPDATED = new TableFormat(1, 1);

        private static void initializeEfUpdated()
        {
            EF_UPDATED.addField("<" + EF_UPDATED_VARIABLE + "><S>");
            EF_UPDATED.addField("<" + EF_UPDATED_VALUE + "><T>");
            EF_UPDATED.addField("<" + EF_UPDATED_USER + "><S><F=N>");
        }


        private static readonly TableFormat EF_CHANGE = new TableFormat(1, 1);

        private static void initializeEfChange()
        {
            EF_CHANGE.addField("<" + EF_CHANGE_VARIABLE + "><S>");
            EF_CHANGE.addField("<" + EF_CHANGE_VALUE + "><T><F=N>");
            EF_CHANGE.addField("<" + EF_CHANGE_DATA + "><S><F=N>");
        }

        private static readonly TableFormat EF_INFO = new TableFormat(1, 1,
                                                                      "<" + EF_INFO_INFO + "><S><D=" +
                                                                      Cres.get().getString("info") + ">");

        private static readonly TableFormat EF_VARIABLE_REMOVED = new TableFormat(1, 1,
                                                                                  "<" + EF_VARIABLE_REMOVED_NAME +
                                                                                  "><S>");

        private static readonly TableFormat EF_EVENT_REMOVED = new TableFormat(1, 1,
                                                                               "<" + EF_EVENT_REMOVED_NAME + "><S>");

        private static readonly TableFormat EF_FUNCTION_REMOVED = new TableFormat(1, 1,
                                                                                  "<" + EF_FUNCTION_REMOVED_NAME +
                                                                                  "><S>");

        private static readonly TableFormat EF_CHILD_REMOVED = new TableFormat(1, 1,
                                                                               "<" + EF_CHILD_REMOVED_CHILD + "><S>");

        private static readonly TableFormat EF_CHILD_ADDED = new TableFormat(1, 1, "<" + EF_CHILD_ADDED_CHILD + "><S>");


        private static readonly int DEFAULT_EVENT_LEVEL = -1;

        private static readonly Permissions DEFAULT_PERMISSIONS = DefaultPermissionChecker.getNullPermissions();

        public const string CALLER_CONTROLLER_PROPERTY_DEBUG = "debug";
        public const string CALLER_CONTROLLER_PROPERTY_NO_UPDATED_EVENTS = "no_update_events";
        public const string CALLER_CONTROLLER_PROPERTY_NO_CHANGE_EVENTS = "no_change_events";
        public const string CALLER_CONTROLLER_PROPERTY_NO_STATISTICS = "no_statistics";
        public const string CALLER_CONTROLLER_PROPERTY_NO_VALIDATION = "no_validation";



        public const int INDEX_HIGHEST = 400;
        public const int INDEX_VERY_HIGH = 300;
        public const int INDEX_HIGH = 200;
        public const int INDEX_HIGHER = 100;
        public const int INDEX_NORMAL = 0;
        public const int INDEX_LOWER = -100;
        public const int INDEX_LOW = -200;
        public const int INDEX_VERY_LOW = -300;
        public const int INDEX_LOWEST = -400;



        private const int VERY_LOW_PERFORMANCE_THRESHOLD = 30000;
        private const int LOW_PERFORMANCE_THRESHOLD = 5000;

        protected const int SORT_THRESHOLD = 10000;

        static AbstractContext()
        {
            initializeVariableDefinitionFormat();
            initializeEfVariableAdded();
            initializeFunictionDefinitionFormat();
            initializeEfFunctionAdded();
            initializeEventDefinitionFormat();
            initializeEfEventAdded();
            initializeInfoDefinitionFormat();
            initializeFiftGetCopyData();
            initializeReplicateInputFormat();
            initializeFiftReplicateFields();
            initializeReplicateOutpuotFormat();
            initializeReplicateToChildrenOutputFormat();
            initializeEfUpdated();
            initializeEfChange();
        }

        #endregion

        #region instance variables

        private ContextManager contextManager;

        // private readonly List<VariableDefinition> variableDefinitions = new List<VariableDefinition>();
        private ConcurrentDictionary<string, VariableData> variableData = new ConcurrentDictionary<string, VariableData>();
        private ReaderWriterLockSlim variableDataLock = new ReaderWriterLockSlim();


        // private readonly List<FunctionDefinition> functionDefinitions = new List<FunctionDefinition>();
        private ConcurrentDictionary<string, FunctionData> functionData = new ConcurrentDictionary<string, FunctionData>();
        private ReaderWriterLockSlim functionDataLock = new ReaderWriterLockSlim();

        // private readonly List<EventData> eventData = new List<EventData>();
        private ConcurrentDictionary<string, EventData> eventData = new ConcurrentDictionary<string, EventData>();
        private ReaderWriterLockSlim eventDataLock = new ReaderWriterLockSlim();

        private string name;
        private string description;
        private string type;
        private string group;
        private string iconId;

        private Context parent;

        private bool setupComplete;
        private bool started;
        private bool stopped;

        private int? index;

        private bool permissionCheckingEnabled = true;
        private Permissions permissions;
        private Permissions childrenViewPermissions;
        private PermissionChecker permissionChecker = new NullPermissionChecker();

        private readonly AgList<Context> children = new AgList<Context>();
        private readonly AgDictionary<string, Context> childrenMap = new AgDictionary<string, Context>();
        private readonly ReaderWriterLockSlim childrenLock = new ReaderWriterLockSlim();

        private bool valueCheckingEnabled = true;

        private bool childrenSortingEnabled = true;
        private bool entitySortingEnabled;

        private bool fireUpdateEvents = true;

        private string fullName;

        private string path; // Cached, for internal use

        #endregion instance variables

        #region instance creation

        protected AbstractContext()
        {
        }

        protected AbstractContext(string name)
        {
            this.setName(name);
        }

        #endregion instance creation

        #region initialize

        public void setup(ContextManager aContextManager)
        {
            this.setContextManager(aContextManager);
            this.setup();
        }

        protected void setup()
        {
            try
            {
                if (this.setupComplete)
                {
                    return;
                }

                this.setupPermissions();

                this.setupMyself();

                this.setupComplete = true;

                this.setupChildren();
            }
            catch (Exception ex)
            {
                throw new ContextRuntimeException("Error setting up context '" + this.ToString() + "': " + ex.Message, ex);
            }
        }

        public void setupPermissions()
        {
        }

        public virtual void setupMyself()
        {
            var vd = new VariableDefinition(V_INFO, INFO_DEFINITION_FORMAT, true, false, Cres.get().getString("conContextProps"), ContextUtils.GROUP_SYSTEM);
            vd.setHidden(true);
            vd.setReadPermissions(DefaultPermissionChecker.getNullPermissions());
            this.addVariableDefinition(vd);
            vd = new VariableDefinition(V_VARIABLES, VARIABLE_DEFINITION_FORMAT, true, false, Cres.get().getString("conVarList"));
            vd.setHidden(true);
            vd.setReadPermissions(DefaultPermissionChecker.getNullPermissions());
            this.addVariableDefinition(vd);
            vd = new VariableDefinition(V_FUNCTIONS, FUNCTION_DEFINITION_FORMAT, true, false, Cres.get().getString("conFuncList"));
            vd.setHidden(true);
            vd.setReadPermissions(DefaultPermissionChecker.getNullPermissions());
            this.addVariableDefinition(vd);
            vd = new VariableDefinition(V_EVENTS, EVENT_DEFINITION_FORMAT, true, false,
                                        Cres.get().getString("conEvtList"));
            vd.setHidden(true);
            vd.setReadPermissions(DefaultPermissionChecker.getNullPermissions());
            this.addVariableDefinition(vd);
            vd = new VariableDefinition(V_CHILDREN, VFT_CHILDREN, true, false, Cres.get().getString("conChildList"));
            vd.setHidden(true);
            vd.setReadPermissions(DefaultPermissionChecker.getNullPermissions());
            this.addVariableDefinition(vd);

            var fd = new FunctionDefinition(F_GET_COPY_DATA, FIFT_GET_COPY_DATA, REPLICATE_INPUT_FORMAT,
                                            "Get Replication Source Data");
            fd.setHidden(true);
            this.addFunctionDefinition(fd);
            fd = new FunctionDefinition(F_COPY, REPLICATE_INPUT_FORMAT, REPLICATE_OUTPUT_FORMAT,
                                        Cres.get().getString("conCopyProperties"));
            fd.setHidden(true);
            this.addFunctionDefinition(fd);
            fd = new FunctionDefinition(F_COPY_TO_CHILDREN, REPLICATE_INPUT_FORMAT, REPLICATE_TO_CHILDREN_OUTPUT_FORMAT,
                                        Cres.get().getString("conCopyToChildren"));
            fd.setHidden(true);
            this.addFunctionDefinition(fd);
            var ed = new EventDefinition(E_INFO, EF_INFO, Cres.get().getString("conInfoEvtDesc"),
                                         ContextUtils.GROUP_DEFAULT);
            ed.setExpirationPeriod((Int64)1000 * 60 * 60 * 24 * 365); // 1 year
            ed.setLevel(EventLevel.INFO);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_CHILD_ADDED, EF_CHILD_ADDED, Cres.get().getString("conChildAdded"),
                                     ContextUtils.GROUP_SYSTEM);
            ed.setSynchronous(true);
            ed.setHidden(true);
            ed.setPermissions(DefaultPermissionChecker.getNullPermissions());
            ed.setExpirationPeriod(0);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_CHILD_REMOVED, EF_CHILD_REMOVED, Cres.get().getString("conChildRemoved"),
                                     ContextUtils.GROUP_SYSTEM);
            ed.setSynchronous(true);
            ed.setHidden(true);
            ed.setPermissions(DefaultPermissionChecker.getNullPermissions());
            ed.setExpirationPeriod(0);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_VARIABLE_ADDED, EF_VARIABLE_ADDED, Cres.get().getString("conVarAdded"),
                                     ContextUtils.GROUP_SYSTEM);
            ed.setHidden(true);
            ed.setPermissions(DefaultPermissionChecker.getNullPermissions());
            ed.setExpirationPeriod(0);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_VARIABLE_REMOVED, EF_VARIABLE_REMOVED, Cres.get().getString("conVarRemoved"),
                                     ContextUtils.GROUP_SYSTEM);
            ed.setHidden(true);
            ed.setPermissions(DefaultPermissionChecker.getNullPermissions());
            ed.setExpirationPeriod(0);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_FUNCTION_ADDED, EF_FUNCTION_ADDED, Cres.get().getString("conFuncAdded"),
                                     ContextUtils.GROUP_SYSTEM);
            ed.setHidden(true);
            ed.setPermissions(DefaultPermissionChecker.getNullPermissions());
            ed.setExpirationPeriod(0);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_FUNCTION_REMOVED, EF_FUNCTION_REMOVED, Cres.get().getString("conFuncRemoved"),
                                     ContextUtils.GROUP_SYSTEM);
            ed.setHidden(true);
            ed.setPermissions(DefaultPermissionChecker.getNullPermissions());
            ed.setExpirationPeriod(0);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_EVENT_ADDED, EF_EVENT_ADDED, Cres.get().getString("conEvtAdded"),
                                     ContextUtils.GROUP_SYSTEM);
            ed.setHidden(true);
            ed.setPermissions(DefaultPermissionChecker.getNullPermissions());
            ed.setExpirationPeriod(0);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_EVENT_REMOVED, EF_EVENT_REMOVED, Cres.get().getString("conEvtRemoved"),
                                     ContextUtils.GROUP_SYSTEM);
            ed.setHidden(true);
            ed.setPermissions(DefaultPermissionChecker.getNullPermissions());
            ed.setExpirationPeriod(0);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_INFO_CHANGED, INFO_DEFINITION_FORMAT, Cres.get().getString("conInfoChanged"),
                                     ContextUtils.GROUP_SYSTEM);
            ed.setHidden(true);
            ed.setPermissions(DefaultPermissionChecker.getNullPermissions());
            ed.setExpirationPeriod(0);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_UPDATED, EF_UPDATED, Cres.get().getString("conUpdated"),
                                     ContextUtils.GROUP_SYSTEM);
            ed.setHidden(true);
            ed.setExpirationPeriod(0);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_CHANGE, EF_CHANGE, "Change", ContextUtils.GROUP_SYSTEM);
            ed.setHidden(true);
            ed.setExpirationPeriod(0);
            //ed.setDedicatedTablePreferred(true);
            this.addEventDefinition(ed);
            ed = new EventDefinition(E_DESTROYED, TableFormat.EMPTY_FORMAT, Cres.get().getString("conDestroyedPermanently"),

ContextUtils.GROUP_SYSTEM);
            ed.setSynchronous(true);
            ed.setHidden(true);
            ed.setPermissions(DefaultPermissionChecker.getNullPermissions());
            ed.setExpirationPeriod(0);
            this.addEventDefinition(ed);
        }

        public void setupChildren()
        {
        }

        #endregion initialize

        public void teardown()
        {

        }



        public void start()
        {
            foreach (var child in new List<Context>(this.children))
            {
                child.start();
            }

            this.started = true;
        }

        public void stop()
        {
            this.stopped = true;

            foreach (var child in new List<Context>(this.children))
            {
                child.stop();
            }

            this.started = false;
        }

        public int CompareTo(object o)
        {
            if (o is AbstractContext)
            {
                var c = (AbstractContext)o;
                if (this.getIndex() != null || c.getIndex() != null)
                {
                    return (c.getIndex() ?? 0).CompareTo((int?)(this.getIndex() ?? 0));
                }

                return this.getName().CompareTo(c.getName());
            }

            return 0;
        }

        public virtual List<Context> getChildren(CallerController<CallerData> caller)
        {
            if (!this.checkPermissions(this.getChildrenViewPermissions(), caller, this))
            {
                if (Log.CONTEXT_CHILDREN.isDebugEnabled())
                {
                    Log.CONTEXT_CHILDREN.debug("Access to child '" + this.name + "' denied in context '" + this.getPath() + "'");
                }

                return new List<Context>();
            }

            List<Context> childList = new List<Context>(this.children);

            childList.RemoveAll((each) => !this.shouldSeeChild(caller, each));

            return childList;
        }

        private bool shouldSeeChild(CallerController<CallerData> caller, Context cur)
        {
            return this.checkPermissions(cur.getPermissions(), caller, cur) || this.canSee(caller, cur);
        }

        private bool canSee(CallerController<CallerData> caller, Context con)
        {
            if (!this.permissionCheckingEnabled)
            {
                return true;
            }

            return this.getPermissionChecker().canSee(caller != null ? caller.getPermissions() : null, con.getPath(),

this.getContextManager());
        }

        public List<Context> getChildren()
        {
            return this.getChildren(null);
        }

        public List<Context> getVisibleChildren(CallerController<CallerData> caller)
        {
            return this.getChildren(caller);
        }

        public List<Context> getVisibleChildren()
        {
            return this.getVisibleChildren(null);
        }

        public bool isMapped()
        {
            return false;
        }

        public List<Context> getMappedChildren(CallerController<CallerData> caller)
        {
            return this.isMapped() ? this.getVisibleChildren(caller) : this.getChildren(caller);
        }

        public List<Context> getMappedChildren()
        {
            return this.getMappedChildren(null);
        }

        public string getName()
        {
            return this.name;
        }

        public virtual string getDescription()
        {
            return this.description;
        }

        public void setDescription(string descriptionString)
        {
            var old = this.description;
            this.description = descriptionString;

            if (old == null || !old.Equals(description))
            {
                this.contextInfoChanded();
            }
        }

        public Context getParent()
        {
            return this.parent;
        }

        public bool hasParent(Context parentContext)
        {
            Context root = this;

            do
            {
                root = root.getParent();
                if (root == parentContext)
                {
                    return true;
                }
            }
            while (root.getParent() != null);

            return false;
        }

        public Context getRoot()
        {
            Context root = this;

            while (root.getParent() != null)
            {
                root = root.getParent();
            }

            return root;
        }

        public Context get(string contextName, CallerController<CallerData> caller)
        {
            if (contextName == null)
            {
                return null;
            }

            var relative = ContextUtils.isRelative(contextName);

            if (relative)
            {
                contextName = contextName.Substring(1);
            }

            var cur = relative ? this : this.getRoot();

            if (contextName.Length == 0)
            {
                return cur;
            }

            var lastName = this.getRoot().getName();
            var names = contextName.Split(ContextUtils.CONTEXT_NAME_SEPARATOR[0]);
            foreach (var child in names)
            {
                if (child.Length == 0)
                {
                    return null;
                }

                if (cur == null)
                {
                    break;
                }

                lastName = cur.getName();
                cur = cur.getChild(child, caller);
            }

            if (cur == null)
            {
                Log.CONTEXT_CHILDREN.debug("Context '" + contextName + "' not found in '" + this.getPath() + "', last found: '" +

lastName + "'");
            }

            return cur;
        }

        public Context get(string contextName)
        {
            return this.get(contextName, null);
        }

        public Permissions getPermissions()
        {
            if (!this.permissionCheckingEnabled)
            {
                return DefaultPermissionChecker.getNullPermissions();
            }

            return this.permissions ??
                   (this.getParent() != null ? this.getParent().getPermissions() : DefaultPermissionChecker.getNullPermissions());
        }

        protected void setName(string nameString)
        {
            if (!ContextUtils.isValidContextName(nameString))
            {
                throw new ArgumentException("Invalid context nameString: " + nameString);
            }

            this.name = nameString;
        }

        public void setParent(Context parentContext)
        {
            this.parent = parentContext;
        }

        protected void setPermissions(Permissions aPermissions)
        {
            this.permissions = aPermissions;
        }

        protected void setPermissionChecker(PermissionChecker aPermissionChecker)
        {
            this.permissionChecker = aPermissionChecker;
        }

        protected void setFireUpdateEvents(bool aBoolean)
        {
            this.fireUpdateEvents = aBoolean;
        }

        protected bool isFireUpdateEvents()
        {
            return this.fireUpdateEvents;
        }

        protected virtual void clear()
        {
            this.removeAllChildren();

            this.variableData.Clear();

            this.functionData.Clear();

            this.eventData.Clear();
        }

        private void setContextManager(ContextManager aContextManager)
        {
            if (this.contextManager != null && this.contextManager != aContextManager)
            {
                throw new InvalidOperationException("Context manager already set");
            }

            this.contextManager = aContextManager;
        }

        protected void setChildrenViewPermissions(Permissions aPermissions)
        {
            this.childrenViewPermissions = aPermissions;
        }

        protected void setChildrenSortingEnabled(bool aBoolean)
        {
            this.childrenSortingEnabled = aBoolean;
        }

        public bool isChildrenSortingEnabled()
        {
            return this.childrenSortingEnabled;
        }

        protected void setEntitySortingEnabled(bool aBoolean)
        {
            this.entitySortingEnabled = aBoolean;
        }

        protected void setValueCheckingEnabled(bool aBoolean)
        {
            this.valueCheckingEnabled = aBoolean;
        }

        protected Permissions updatePermissions(Permissions aPermissions, string path, string pathToChange)
        {
            if (aPermissions != null)
            {
                foreach (var perm in aPermissions)
                {
                    if (Util.equals(perm.getEntity(), pathToChange))
                    {
                        perm.setEntity(path);
                    }
                }
            }

            return aPermissions;
        }

        protected void checkPermissions(Permissions needPermissions, CallerController<CallerData> caller)
        {
            if (!this.checkPermissions(needPermissions, caller, this))
            {
                throw new ContextSecurityException(string.Format(Cres.get().getString("conAccessDenied"), this.getPath(), caller

!= null ? caller.getPermissions().ToString() : "", needPermissions));
            }
        }

        public bool checkPermissions(Permissions needPermissions, CallerController<CallerData> caller, Context accessedContext)
        {
            if (!this.permissionCheckingEnabled)
            {
                return true;
            }

            return this.getPermissionChecker().has(caller, needPermissions, accessedContext);
        }

        
        public void addChild(Context child)
        {
            this.addChild(child, null);
        }

        public void addChild(Context child, int? indexInteger)
        {
            this.getChildrenLock().EnterWriteLock();
            try
            {

                Context existing = this.getChildWithoutCheckingPerms(child.getName());
                if (existing != null)
                {
                    throw new ArgumentException(string.Format(Cres.get().getString("conChildExists"), child.getName(), this.getPath()));
                }

                if (indexInteger != null)
                {
                    if (this.childrenSortingEnabled)
                    {
                        throw new InvalidOperationException("Cannot add child with pre-defined index as children sorting is enabled");
                    }
                    this.children.Insert((int)indexInteger, child);
                }
                else
                {
                    this.children.Add(child);
                }
                this.childrenMap[child.getName().ToLower(CultureInfo.GetCultureInfo("en"))] = child;

                //// Disabling sorting for large child sets to avoid performance degradation. Children management should be anyway performed via groups in this case.
                //if (this.childrenSortingEnabled && this.children.Count < SORT_THRESHOLD)
                //{
                //    Collections.sort(children);
                //}
            }
            finally
            {
                this.getChildrenLock().ExitWriteLock();
            }

            try
            {
                child.setParent(this);

                child.setup(this.getContextManager());

                if (this.setupComplete && this.fireUpdateEvents)
                {
                    this.fireEvent(E_CHILD_ADDED, child.getName());
                }

                if (this.getContextManager() != null)
                {
                    // If a child added already has own children, contextAdded() won't be called for them
                    this.getContextManager().contextAdded(child);
                }
            }
            catch (Exception ex)
            {
                this.getChildrenLock().EnterWriteLock();
                try
                {
                    this.childrenMap.Remove(child.getName());
                    this.children.Remove(child);
                    throw new ContextRuntimeException("Error adding child '" + child.ToString() + "' to context '" + this.ToString() + "': " + ex.Message, ex);
                }
                finally
                {
                    this.getChildrenLock().ExitWriteLock();
                }
            }

            Log.CONTEXT_CHILDREN.debug("Added child '" + child.getName() + "' to '" + this.getPath()); // + "' in " + (System.currentTimeMillis() - startTime) + " ms");
        }

        public void removeFromParent()
        {
            if (this.getParent() != null)
            {
                this.getParent().removeChild(this);
                this.setParent(null);
            }
            else
            {
                Log.CONTEXT_CHILDREN.debug("Can't remove context '" + this.getPath() + "' from its parent: no parent context was set");
            }
        }

        public void destroy(bool moving)
        {
            if (!moving)
            {
                this.stop();
                this.destroyChildren(false);
            }

            if (this.fireUpdateEvents)
            {
                var ed = this.getEventDefinition(E_DESTROYED);
                if (ed != null)
                {
                    this.fireEvent(ed.getName());
                }
            }

            this.eventDataLock.EnterReadLock();
            try
            {
                foreach (var ed in this.eventData.Values)
                {
                    var logger = Log.CONTEXT_EVENTS;
                    if (logger.isDebugEnabled())
                    {
                        logger.debug("Removing all listeners of event '" + ed.getDefinition().getName() + "'");
                    }

                    ed.clearListeners();
                }
            }
            finally
            {
                this.eventDataLock.ExitReadLock();
            }

            this.removeFromParent();
        }

        protected void destroyChildren(bool moving)
        {
            foreach (var child in new List<Context>(this.children))
            {
                child.destroy(moving);
            }
        }

        public void removeChild(Context child)
        {
            this.getChildrenLock().EnterWriteLock();
            try
            {
                if (this.children.Contains(child))
                {
                    if (this.getContextManager() != null)
                    {
                        this.getContextManager().contextRemoved(child);
                    }

                    this.children.Remove(child);

                    if (this.setupComplete && this.fireUpdateEvents)
                    {
                        this.fireEvent(E_CHILD_REMOVED, child.getName());
                    }
                }
            }
            finally
            {
                this.getChildrenLock().ExitWriteLock();
            }
        }

        public void removeChild(string nameString)
        {
            var con = this.getChildWithoutCheckingPerms(nameString);

            if (con != null)
            {
                this.removeChild(con);
                return;
            }

            Log.CONTEXT_CHILDREN.debug("Remove error: child '" + nameString + "' not found in context " + this.getPath());
        }

        protected void reorderChild(Context child, int indexInteger)
        {
            if (this.childrenSortingEnabled)
            {
                throw new InvalidOperationException("Cannot reorder children when children sorting is enabled");
            }

            var oi = this.children.IndexOf(child);

            if (!this.children.Contains(child)) return;

            this.children.Remove(child);
            this.children.Insert(indexInteger - (oi < indexInteger ? 1 : 0), child);
        }

        public void destroyChild(Context child, bool moving)
        {
            child.destroy(moving);
        }

        public void destroyChild(string nameString, bool moving)
        {
            var con = this.getChildWithoutCheckingPerms(nameString);

            if (con != null)
            {
                this.destroyChild(con, moving);
                return;
            }

            Log.CONTEXT_CHILDREN.warn("Destroy error: child '" + nameString + "' not found in context " + this.getPath());
        }

        public void removeAllChildren()
        {
            var temp = new List<Context>(this.children);

            foreach (var child in temp)
            {
                this.removeChild(child);
            }
        }

        public void updatePrepare()
        {

        }

        protected void movePrepare(string oldPath, string oldName, string newPath, string newName)
        {

        }


        protected void moveInternal(string oldPath, string oldName, string newPath, string newName)
        {
            this.setName(newName);
            this.updatePermissions(this.permissions, newPath, oldPath);
            this.updatePermissions(this.childrenViewPermissions, newPath, oldPath);

            this.childrenLock.EnterReadLock();
            try
            {
                foreach (Context child in this.children)
                {
                    ((AbstractContext)child).moveInternal(ContextUtils.createName(oldPath, child.getName()), child.getName(),

ContextUtils.createName(newPath, child.getName()), child.getName());
                }
            }
            finally
            {
                this.childrenLock.ExitReadLock();
            }
        }

        protected void moveFinalize(string oldPath, string oldName, string newPath, string newName)
        {

        }

        public void move(Context newParent, string newName)
        {
            this.move(this.getPath(), newParent, newName);
        }

        private void move(string oldPath, Context newParent, string newName)
        {
            Log.CONTEXT.debug("Moving context " + this.getPath() + " to " + newParent.getPath() + " and/or renaming to " +

newName);

            var oldName = this.getName();

            string newPath = ContextUtils.createName(newParent.getPath(), newName);

            this.movePrepare(oldPath, oldName, newPath, newName);

            this.getParent().destroyChild(this, true);

            this.moveInternal(oldPath, oldName, newPath, newName);

            newParent.addChild(this);

            this.moveFinalize(oldPath, oldName, newPath, newName);
        }

        public virtual Context getChild(string nameString, CallerController<CallerData> caller)
        {
            if (!this.checkPermissions(this.getChildrenViewPermissions(), caller, this))
            {
                return null;
            }

            var child = this.getChildWithoutCheckingPerms(nameString);

            if (child != null && this.shouldSeeChild(caller, child))
            {
                return child;
            }

            return null;
        }

        public Context getChild(string nameString)
        {
            return this.getChild(nameString, null);
        }

        private Context getChildWithoutCheckingPerms(string nameString)
        {
            lock (this.children)
            {
                foreach (Context cur in this.children)
                {
                    if (cur.getName().Equals(nameString))
                    {
                        return cur;
                    }
                }
            }

            return null;
        }


        public string getPath()
        {
            if (this.getParent() == null)
            {
                return this.createPath();
            }

            if (this.path == null)
            {
                this.path = this.createPath();
            }

            return this.path;
        }

        private string createPath()
        {
            Context con = this;
            string nm = this.getName();

            do
            {
                con = con.getParent();
                if (con != null)
                {
                    if (con.getParent() != null)
                    {
                        nm = con.getName() + ContextUtils.CONTEXT_NAME_SEPARATOR + nm;
                    }
                }
            }
            while (con != null);

            return nm;
        }

        public virtual bool addEventListener(string name, ContextEventListener listener)
        {
            return addEventListener(name, listener, false);
        }

        public virtual bool addEventListener(string name, ContextEventListener listener, bool weak)
        {
            var ed = this.getEventData(name);

            if (ed == null)
            {
                throw new ArgumentException(Cres.get().getString("conEvtNotAvail") + name);
            }

            lock (ed)
            {
                try
                {
                    checkPermissions(ed.getDefinition().getPermissions() != null ? ed.getDefinition().getPermissions() :

getPermissions(), listener.getCallerController());
                }
                catch (ContextSecurityException ex)
                {
                    Log.CONTEXT_EVENTS.warn("Error adding listener '" + listener + "' of event '" + ed.getDefinition().getName() +

"' in context '" + this.getPath() + "': " + ex.Message, new Exception());
                    return false;
                }

                var logger = Log.CONTEXT_EVENTS;
                if (logger.isDebugEnabled())
                {
                    logger.debug("Adding '" + listener + "' as listener of event '" + ed.getDefinition().getName() + "' in '" +

this.getPath() + "'");
                }

                return ed.addListener(listener);
            }
        }

        public virtual bool removeEventListener(string nameString, ContextEventListener listener)
        {
            var ed = this.getEventData(nameString);

            if (ed == null)
            {
                Log.CONTEXT_EVENTS.warn("Error removing listener of event '" + nameString + "' in context '" + this.getPath() +

"': event definition not found");
                return false;
            }

            var logger = Log.CONTEXT_EVENTS;
            if (logger.isDebugEnabled())
            {
                logger.debug("Removing '" + listener + "' listener of event '" + ed.getDefinition().getName() + "' in '" +

this.getPath() + "'");
            }

            lock (ed)
            {
                return ed.removeListener(listener);
            }
        }

        public virtual List<VariableDefinition> getVariableDefinitions(CallerController<CallerData> caller)
        {
            return this.getVariableDefinitions(caller, false);
        }

        public List<VariableDefinition> getVariableDefinitions(CallerController<CallerData> caller, bool includeHidden)
        {
            var list = new List<VariableDefinition>();
            var debug = caller != null && caller.getProperties().ContainsKey(CALLER_CONTROLLER_PROPERTY_DEBUG);

            this.variableDataLock.EnterReadLock();
            try
            {
                foreach (var d in this.variableData.Values)
                {
                    VariableDefinition def = d.getDefinition();
                    if ((caller == null || caller.isPermissionCheckingEnabled()) && !includeHidden && def.isHidden() && !debug)
                    {
                        continue;
                    }

                    //var rmsg = this.checkPermissionsMessage(def.getReadPermissions() ?? this.getPermissions(), caller);
                    //var wmsg = this.checkPermissionsMessage(def.getWritePermissions() ?? this.getPermissions(), caller);
                    var readAccessGranted = this.checkPermissions(def.getReadPermissions() != null ? def.getReadPermissions() :

this.getPermissions(), caller, this);
                    var writeAccessGranted = this.checkPermissions(def.getWritePermissions() != null ? def.getWritePermissions() :

this.getPermissions(), caller, this);

                    if (!readAccessGranted && !writeAccessGranted)
                    {
                        continue;
                    }

                    if ((def.isReadable() == readAccessGranted) && (def.isWritable() == writeAccessGranted))
                    {
                        list.Add(def);
                    }
                    else
                    {
                        var clone = def.Clone() as VariableDefinition;

                        clone.setReadable(def.isReadable() ? readAccessGranted : false);
                        clone.setWritable(def.isWritable() ? writeAccessGranted : false);

                        list.Add(clone);
                    }
                }
            }
            finally
            {
                this.variableDataLock.ExitReadLock();
            }

            return list;
        }

        public List<VariableDefinition> getVariableDefinitions()
        {
            return this.getVariableDefinitions((CallerController<CallerData>)null);
        }

        public List<VariableDefinition> getVariableDefinitions(CallerController<CallerData> caller, string groupString)
        {
            var defs = new List<VariableDefinition>();

            foreach (var vd in this.getVariableDefinitions(caller))
            {
                if (vd.getGroup() != null && ContextUtils.getBaseGroup(vd.getGroup()).Equals(groupString))
                {
                    defs.Add(vd);
                }
            }

            return defs;
        }

        public List<VariableDefinition> getVariableDefinitions(string groupString)
        {
            return this.getVariableDefinitions(null, groupString);
        }

        public PermissionChecker getPermissionChecker()
        {
            return this.permissionChecker;
        }

        public Permissions getChildrenViewPermissions()
        {
            return this.childrenViewPermissions ?? this.getPermissions();
        }

        public ContextManager getContextManager()
        {
            return this.contextManager;
        }

        public bool isSetupComplete()
        {
            return this.setupComplete;
        }

        public bool isStarted()
        {
            return this.started;
        }

        public bool isStopped()
        {
            return this.stopped;
        }

        public bool isInitializedInfo()
        {
            return this.setupComplete;
        }

        public bool isInitializedChildren()
        {
            return this.setupComplete;
        }

        public bool isInitializedVariables()
        {
            return this.setupComplete;
        }

        public bool isInitializedFunctions()
        {
            return this.setupComplete;
        }

        public bool isInitializedEvents()
        {
            return this.setupComplete;
        }



        public virtual List<FunctionDefinition> getFunctionDefinitions(CallerController<CallerData> caller)
        {
            return this.getFunctionDefinitions(caller, false);
        }

        public List<FunctionDefinition> getFunctionDefinitions(CallerController<CallerData> caller, bool includeHidden)
        {
            var list = new List<FunctionDefinition>();
            var debug = caller != null ? caller.getProperties().ContainsKey(CALLER_CONTROLLER_PROPERTY_DEBUG) : false;

            this.functionDataLock.EnterReadLock();
            try
            {
                foreach (var d in this.functionData.Values)
                {
                    var def = d.getDefinition();

                    if (!this.checkPermissions(def.getPermissions() != null ? def.getPermissions() : this.getPermissions(),

caller, this))
                    {
                        continue;
                    }

                    if ((caller == null || caller.isPermissionCheckingEnabled()) && !includeHidden && def.isHidden() && !debug)
                    {
                        continue;
                    }

                    list.Add(def);
                }
            }
            finally
            {
                this.functionDataLock.ExitReadLock();
            }

            return list;
        }

        public List<FunctionDefinition> getFunctionDefinitions()
        {
            return this.getFunctionDefinitions((CallerController<CallerData>)null);
        }

        public List<FunctionDefinition> getFunctionDefinitions(CallerController<CallerData> caller, string groupString)
        {
            var defs = new List<FunctionDefinition>();

            foreach (var fd in this.getFunctionDefinitions(caller))
            {
                if (fd.getGroup() != null && ContextUtils.getBaseGroup(fd.getGroup()).Equals(groupString))
                {
                    defs.Add(fd);
                }
            }

            return defs;
        }

        public List<FunctionDefinition> getFunctionDefinitions(string groupString)
        {
            return this.getFunctionDefinitions(null, groupString);
        }

        protected ReaderWriterLockSlim getChildrenLock()
        {
            return this.childrenLock;
        }

        public virtual string getType()
        {
            return this.type ?? ContextUtils.getTypeForClass(this.GetType());
        }

        public bool isPermissionCheckingEnabled()
        {
            return this.permissionCheckingEnabled;
        }

        public virtual string getIconId()
        {
            return this.iconId;
        }

        public int? getIndex()
        {
            return this.index;
        }

        public string getGroup()
        {
            return this.group;
        }

        public String getLocalRoot()
        {
            return ContextUtils.CTX_ROOT;
        }

        public bool isProxy()
        {
            return false;
        }

        public bool isDistributed()
        {
            return false;
        }

        public string getRemoteRoot()
        {
            return null;
        }

        public string getRemotePath()
        {
            return this.getPath();
        }

        public string getRemotePrimaryRoot()
        {
            return null;
        }



        public void setType(string typeString)
        {
            if (!ContextUtils.isValidContextType(typeString))
            {
                throw new ArgumentException("Illegal context typeString: " + typeString);
            }

            var old = this.type;
            this.type = typeString;

            if (old != null && old.Equals(typeString)) return;
            if (!this.setupComplete || !this.fireUpdateEvents) return;
            var ed = this.getEventDefinition(E_INFO_CHANGED);
            if (ed != null)
            {
                this.fireEvent(E_INFO_CHANGED, this.getDescription(), this.getType(), this.getGroup(), this.getIconId());
            }
        }

        protected void setPermissionCheckingEnabled(bool aBoolean)
        {
            this.permissionCheckingEnabled = aBoolean;
        }

        protected void setIconId(string aString)
        {
            this.iconId = aString;
        }

        private void contextInfoChanded()
        {
            if (this.setupComplete)
            {
                ContextManager cm = this.getContextManager();
                if (cm != null)
                {
                    cm.contextInfoChanged(this);
                }

                if (this.fireUpdateEvents)
                {
                    EventDefinition ed = this.getEventDefinition(E_INFO_CHANGED);
                    if (ed != null)
                    {
                        fireEvent(E_INFO_CHANGED, createContextInfoTable());
                    }
                }
            }
        }
        
        protected void setIndex(int indexInteger)
        {
            this.index = indexInteger;
        }

        public void setGroup(string groupString)
        {
            this.group = groupString;
        }

        public virtual List<EventDefinition> getEventDefinitions(CallerController<CallerData> caller)
        {
            return getEventDefinitions(caller, false);
        }

        public List<EventDefinition> getEventDefinitions(CallerController<CallerData> caller, bool includeHidden)
        {
            List<EventDefinition> list = new List<EventDefinition>();
            bool debug = caller != null ? caller.getProperties().ContainsKey(CALLER_CONTROLLER_PROPERTY_DEBUG) : false;

            this.eventDataLock.EnterReadLock();
            try
            {
                foreach (EventData d in this.eventData.Values)
                {
                    if (!this.checkPermissions(d.getDefinition().getPermissions() != null ? d.getDefinition().getPermissions() : this.getPermissions(), caller, this))
                    {
                        continue;
                    }

                    if ((caller == null || caller.isPermissionCheckingEnabled()) && !includeHidden && d.getDefinition().isHidden() && !debug)
                    {
                        continue;
                    }

                    list.Add(d.getDefinition());
                }
            }
            finally
            {
                eventDataLock.ExitReadLock();
            }

            return list;
        }

        public List<EventDefinition> getEventDefinitions()
        {
            return this.getEventDefinitions((CallerController<CallerData>)null);
        }

        public List<EventDefinition> getEventDefinitions(CallerController<CallerData> caller, String group)
        {
            List<EventDefinition> res = new List<EventDefinition>();

            foreach (EventDefinition ed in this.getEventDefinitions(caller))
            {
                if (ed.getGroup() != null && (Util.equals(group, ed.getGroup()) || ed.getGroup().StartsWith(group + ContextUtils.ENTITY_GROUP_SEPARATOR)))
                {
                    res.Add(ed);
                }
            }

            return res;
        }

        public List<EventDefinition> getEventDefinitions(string group)
        {
            return this.getEventDefinitions(null, group);
        }



        private DataTable getVariable(VariableDefinition def, CallerController<CallerData> caller, RequestController<RequestData> request)
        {
            //var startTime = DateTime.Now;

            this.setupVariables();

            VariableData data = this.getVariableData(def.getName());

            def.getReadWriteLock().EnterReadLock();

            try
            {
                this.checkPermissions(def.getReadPermissions() ?? this.getPermissions(), caller);

                if (Log.CONTEXT_VARIABLES.isDebugEnabled())
                {
                    Log.CONTEXT_VARIABLES.debug(
                        "Trying to get variable '" + def.getName() + "' from context '" + this.getPath() + "'");
                }

                DataTable result1 = this.executeGetter(data, caller, request);

                if (result1.isInvalid())
                {
                    throw new ContextException(result1.getInvalidationMessage());
                }

                result1 = this.checkVariableValue(def, result1, caller);

                return result1;

            }
            catch (Exception ex)
            {
                throw new ContextException(string.Format(Cres.get().getString("conErrGettingVar"), def.ToString(), this.ToString()) + ex.Message, ex);
            }
            finally
            {
                def.getReadWriteLock().ExitReadLock();
                data.registerGetOperation();
            }
        }

        private DataTable checkVariableValue(VariableDefinition def, DataTable val, CallerController<CallerData> caller)
        {
            if (!this.valueCheckingEnabled)
            {
                return val;
            }

            var value = val;

            if (caller == null || !caller.getProperties().ContainsKey(CALLER_CONTROLLER_PROPERTY_NO_VALIDATION))
            {
                string msg = this.checkVariableValueFormat(def, value);

                if (msg != null)
                {
                    Log.CONTEXT_VARIABLES.debug("Invalid value of variable '" + def.getName() + "': " + msg);
                    DataTable newValue = this.getDefaultValue(def);

                    DataTableReplication.copy(value, newValue, true, true, true);

                    value = newValue;
                    this.checkVarValue(def, value);

                }
            }

            return value;
        }

        private DataTable executeGetter(VariableData data, CallerController<CallerData> caller, RequestController<RequestData> request)
        {
            DataTable result = this.executeGetterMethod(data, caller, request);
            if (result != null)
            {
                return result;
            }

            VariableDefinition def = data.getDefinition();

            if (def.getGetter() != null)
            {
                result = def.getGetter().get(this, def, caller, request);
            }
            if (result != null)
            {
                return result;
            }

            result = this.getVariableImpl(def, caller, request);
            if (result != null)
            {
                return result;
            }

            return this.executeDefaultGetter(def, caller, false, true); // Setting check to false as we'll check value later
        }

        private string checkVariableValueFormat(VariableDefinition def, DataTable table)
        {
            if (!this.valueCheckingEnabled)
            {
                return null;
            }

            TableFormat requiredFormat = def.getFormat();

            if (requiredFormat != null)
            {
                string msg = table.conformMessage(requiredFormat);
                if (msg != null)
                {
                    return "Invalid format: " + msg;
                }
            }

            return null;
        }


        private DataTable executeGetterMethod(VariableData data, CallerController<CallerData> caller,

RequestController<RequestData> request)
        {
            if (!data.isGetterCached())
            {
                Type[] parameters = { typeof(VariableDefinition), typeof(CallerController<CallerData>), typeof

(RequestController<RequestData>) };

                try
                {
                    MethodInfo getter1 = this.GetType().GetMethod(GETTER_METHOD_PREFIX + data.getDefinition().getName(),

parameters);
                    if (getter1 == null)
                    {
                        return null;
                    }
                    data.setGetterMethod(getter1);
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    data.setGetterCached(true);
                }
            }

            MethodInfo getter = data.getGetterMethod();

            if (getter != null)
            {
                try
                {
                    return (DataTable)getter.Invoke(this, new object[] { data.getDefinition(), caller, request });
                }
                catch (TargetInvocationException ex)
                {
                    throw new ContextException(ex.InnerException.Message, ex.InnerException);
                }
            }

            return null;
        }



        private DataTable executeGetter(VariableDefinition def, CallerController<CallerData> caller,
                                        RequestController<RequestData> request, MethodInfo getter)
        {
            Log.CONTEXT_VARIABLES.debug("Found variable getter method, invoking");

            DataTable result;
            try
            {
                result = (DataTable)getter.Invoke(this, new object[] { def, caller, request });
            }
            catch (TargetInvocationException ex1)
            {
                throw new ContextException(ex1.InnerException.Message, ex1);
            }

            if (result == null)
            {
                throw new ContextException("Variable getter returned null");
            }

            return result;
        }

        private void checkVarValue(VariableDefinition def, DataTable table)
        {
            if (!this.valueCheckingEnabled)
            {
                return;
            }

            var requiredFormat = def.getFormat();

            if (requiredFormat != null)
            {
                var msg = table.conformMessage(requiredFormat);
                if (msg != null)
                {
                    throw new ContextException("Invalid format: " + msg);
                }
            }
        }

        public DataTable executeDefaultGetter(string nameString, CallerController<CallerData> caller)
        {
            return this.executeDefaultGetter(nameString, caller, true);
        }

        public DataTable executeDefaultGetter(string name, CallerController<CallerData> caller, bool check)
        {
            return this.executeDefaultGetter(name, caller, check, true);
        }


        public DataTable executeDefaultGetter(string nameString, CallerController<CallerData> caller, bool check, bool

createDefault)
        {
            var def = this.getVariableDefinition(nameString);

            if (def == null)
            {
                throw new ContextException(string.Format(Cres.get().getString("conVarNotFound"), nameString, this.getPath()));
            }

            return this.executeDefaultGetter(def, caller, check, createDefault);
        }

        private DataTable executeDefaultGetter(VariableDefinition def, CallerController<CallerData> caller, bool check, bool

createDefault)
        {
            var value = this.executeDefaultGetterImpl(def, caller);

            if (value == null)
            {
                return createDefault ? this.getDefaultValue(def) : null;
            }

            return check ? this.checkVariableValue(def, value, caller) : value;
        }

        protected DataTable executeDefaultGetterImpl(VariableDefinition vd, CallerController<CallerData> caller)
        {
            return this.getDefaultValue(vd);
        }

        public override int GetHashCode()
        {
            if (this.getParent() == null)
            {
                return base.GetHashCode();
            }

            const int prime = 31;
            var result = 1;
            var root = this.getRoot();
            var path = this.getPath();
            result = prime * result + ((root == null) ? 0 : root.GetHashCode());
            result = prime * result + ((path == null) ? 0 : path.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj == null)
            {
                return false;
            }

            var other = (AbstractContext)obj;
            if (this.getRoot() != other.getRoot())
            {
                return false;
            }
            if (!Util.equals(this.getPath(), other.getPath()))
            {
                return false;
            }
            return true;
        }

        public DataTable getVariable(string nameString, CallerController<CallerData> caller,
                                     RequestController<RequestData> request)
        {
            return this.getVariable(this.getAndCheckVariableDefinition(nameString), caller, request);
        }

        public DataTable getVariable(string nameString, CallerController<CallerData> caller)
        {
            return this.getVariable(this.getAndCheckVariableDefinition(nameString), caller, null);
        }

        public DataTable getVariable(string nameString)
        {
            return this.getVariable(this.getAndCheckVariableDefinition(nameString), null, null);
        }

        protected virtual DataTable getVariableImpl(VariableDefinition def, CallerController<CallerData> caller,
                                                    RequestController<RequestData> request)
        {
            return null;
        }

        private void setVariable(VariableDefinition def, CallerController<CallerData> caller, RequestController<RequestData> request, DataTable value)
        {
            var startTime = DateTime.Now;

            this.setupVariables();

            VariableData data = this.getVariableData(def.getName());

            bool readLockedBySameThread = data.getReadWriteLock().IsReadLockHeld;

            if (!readLockedBySameThread)
            {
                def.getReadWriteLock().EnterWriteLock();
            }

            try
            {
                if (value == null)
                {
                    throw new ContextException("Value cannot be NULL");
                }

                var resultingValue = value;


                try
                {
                    this.checkPermissions(
                        def.getWritePermissions() != null ? def.getWritePermissions() : this.getPermissions(),
                        caller);

                    if (!def.isWritable() && caller != null && caller.isPermissionCheckingEnabled())
                    {
                        throw new ContextException(Cres.get().getString("conVarReadOnly"));
                    }

                    if (Log.CONTEXT_VARIABLES.isDebugEnabled())
                    {
                        Log.CONTEXT_VARIABLES.debug(
                            "Trying to set variable '" + def.getName() + "' in context '" + this.getPath() + "'");
                    }



                    if (value.isInvalid())
                    {
                        throw new ContextException(value.getInvalidationMessage());
                    }

                    if (value.getTimestamp() == null)
                    {
                        value.setTimestamp(new DateTime());
                    }

                    // Preventing changes to read-only fields to be made by "client" callers (i.e. ones with permission checking enabled)
                    if (def.getFormat() != null && def.getFormat().hasReadOnlyFields() && caller != null
                        && caller.isPermissionCheckingEnabled())
                    {
                        resultingValue = this.getVariable(def, caller, request);
                        DataTableReplication.copy(value, resultingValue, false, true, true);
                        this.checkVarValue(def, resultingValue);
                    }

                    if (caller == null || !caller.getProperties().ContainsKey(CALLER_CONTROLLER_PROPERTY_NO_VALIDATION))
                    {
                        string msg = this.checkVariableValueFormat(def, resultingValue);

                        if (msg != null)
                        {
                            Log.CONTEXT_VARIABLES.debug(
                                "Invalid value of variable '" + def.getName() + "': " + msg + " (value: "
                                + resultingValue + ")");
                            value = resultingValue;
                            resultingValue = this.getVariable(def, caller, request);

                            DataTableReplication.copy(value, resultingValue, true, true, true);

                            this.checkVariableValueFormat(def, resultingValue);
                        }
                    }

                    if (def.isLocalCachingEnabled())
                    {
                        data.setValue(null); // Resetting cache
                    }

                    this.executeSetter(data, caller, request, resultingValue);

                    this.variableUpdated(def, caller, resultingValue);

                    var endTime = DateTime.Now;
                    var milliseconds = (endTime - startTime).Milliseconds;
                    if (milliseconds > LOW_PERFORMANCE_THRESHOLD)
                    {
                        var msg = "Setting value of variable '" + def + "' in context '" + this.getPath() + "' took "
                                  + milliseconds + " milliseconds";
                        if (milliseconds > VERY_LOW_PERFORMANCE_THRESHOLD)
                        {
                            Log.PERFORMANCE.info(msg);
                        }
                        else
                        {
                            Log.PERFORMANCE.debug(msg);
                        }
                    }
                }
                catch (ValidationException ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ContextException(string.Format(Cres.get().getString("conErrSettingVar"), def.ToString(), ToString())

+ ex.Message, ex);
                }
            }
            finally
            {
                if (!readLockedBySameThread)
                {
                    def.getReadWriteLock().ExitWriteLock();
                }
                data.registerSetOperation();
            }
        }

        protected void variableUpdated(VariableDefinition def, CallerController<CallerData> caller, DataTable value)
        {
            this.fireUpdatedEvent(def, caller, value);

            this.fireChangeEvent(def, caller, new DateTime(), value);
        }

        protected void fireUpdatedEvent(VariableDefinition def, CallerController<CallerData> caller, DataTable value)
        {
            bool callerAllowsUpdatedEvents = caller == null || !caller.getProperties().ContainsKey

(CALLER_CONTROLLER_PROPERTY_NO_UPDATED_EVENTS);

            if (this.isAllowUpdatedEvents(def) && callerAllowsUpdatedEvents)
            {
                EventDefinition ed = this.getEventDefinition(E_UPDATED);
                if (ed != null)
                {
                    this.fireEvent(E_UPDATED, def.getName(), value, caller != null ? caller.getUsername() : null);
                }
            }
        }

        public void fireChangeEvent(VariableDefinition def, CallerController<CallerData> caller, DateTime timestamp, DataTable

value)
        {
            bool callerAllowsChangeEvents = caller == null || !caller.getProperties().ContainsKey

(CALLER_CONTROLLER_PROPERTY_NO_CHANGE_EVENTS);

            if (this.isAllowUpdatedEvents(def) && callerAllowsChangeEvents)
            {
                EventDefinition ed = this.getEventDefinition(E_CHANGE);
                if (ed != null)
                {
                    this.fireEvent(E_UPDATED, def.getName(), value, caller != null ? caller.getUsername() : null);
                }
            }
        }

        protected bool isAllowUpdatedEvents(VariableDefinition def)
        {
            return this.setupComplete && this.fireUpdateEvents && def != null && def.isAllowUpdateEvents();
        }

        protected virtual void setupVariables()
        {
        }

        private void executeSetter(
            VariableData data,
            CallerController<CallerData> caller,
            RequestController<RequestData> request,
            DataTable value)
        {
            VariableDefinition def = data.getDefinition();

            if (this.executeSetterMethod(data, caller, request, value))
            {
                return;
            }

            if (def.getSetter() != null)
            {
                if (def.getSetter().set(this, def, caller, request, value))
                {
                    return;
                }
            }

            if (this.setVariableImpl(def, caller, request, value))
            {
                return;
            }

            this.executeDefaultSetter(def, caller, value);
        }





        private bool executeSetterMethod(VariableData data, CallerController<CallerData> caller, RequestController<RequestData>

request, DataTable value)
        {
            if (!data.isSetterCached())
            {
                Type[] parameters = { typeof(VariableDefinition), typeof(CallerController<CallerData>), typeof

(RequestController<RequestData>), typeof(DataTable) };
                try
                {
                    MethodInfo setter1 = this.GetType().GetMethod(SETTER_METHOD_PREFIX + data.getDefinition().getName(),

parameters);
                    if (setter1 == null)
                    {
                        return false;
                    }
                    data.setSetterMethod(setter1);
                }
                finally
                {
                    data.setSetterCached(true);
                }
            }

            MethodInfo setter = data.getSetterMethod();

            if (setter != null)
            {
                try
                {
                    setter.Invoke(this, new object[] { data.getDefinition(), caller, request, value });

                    return true;
                }
                catch (TargetInvocationException ex)
                {
                    throw new ContextException(ex.InnerException.Message, ex.InnerException);
                }
            }

            return false;
        }

        protected DataTable getDefaultValue(VariableDefinition def)
        {
            return def.getDefaultValue() ?? new DataTable(def.getFormat(), true);
        }

        public void executeDefaultSetter(string nameString, CallerController<CallerData> caller, DataTable value)
        {
            var def = this.getVariableDefinition(nameString);

            if (def == null)
            {
                throw new ContextException(string.Format(Cres.get().getString("conVarNotFound"), nameString, this.getPath()));
            }

            this.executeDefaultSetter(def, caller, value);
        }

        public void executeDefaultSetter(VariableDefinition def, CallerController<CallerData> caller, DataTable value)
        {
            this.executeDefaultSetterImpl(def, caller, value);
        }

        protected void executeDefaultSetterImpl(VariableDefinition vd, CallerController<CallerData> caller,
                                                DataTable value)
        {
            throw new NotSupportedException();
        }

        public void setVariable(string nameString, CallerController<CallerData> caller,
                                RequestController<RequestData> request, DataTable value)
        {
            var def = this.getAndCheckVariableDefinition(nameString);
            this.setVariable(def, caller, request, value);
        }

        public void setVariable(string nameString, CallerController<CallerData> caller, DataTable value)
        {
            this.setVariable(nameString, caller, null, value);
        }

        public void setVariable(string nameString, DataTable value)
        {
            this.setVariable(nameString, null, null, value);
        }

        public void setVariable(string nameString, CallerController<CallerData> caller, params object[] value)
        {
            var def = this.getAndCheckVariableDefinition(nameString);
            this.setVariable(nameString, caller, null, new DataTable(def.getFormat(), value));
        }

        public void setVariable(string nameString, params object[] value)
        {
            this.setVariable(nameString, null, value);
        }

        protected virtual bool setVariableImpl(VariableDefinition def, CallerController<CallerData> caller,
                                                  RequestController<RequestData> request,
                                                  DataTable value)
        {
            return false;
        }

        private VariableDefinition getAndCheckVariableDefinition(string nameString)
        {
            this.setupVariables();

            var def = this.getVariableDefinition(nameString);
            if (def == null)
            {
                throw new ContextException(string.Format(Cres.get().getString("conVarNotFound"), nameString, this.getPath()));
            }

            return def;
        }

        public bool setVariableField(string variable, string field, object value, CallerController<CallerData> cc)
        {
            return this.setVariableField(variable, field, 0, value, cc);
        }

        public bool setVariableField(string variable, string field, int record, object value,
                                        CallerController<CallerData> cc)
        {
            var tab = this.getVariable(variable, cc);
            var old = tab.getRecord(record).getValue(field);
            tab.getRecord(record).setValue(field, value);
            this.setVariable(variable, cc, tab);
            return old == null ? value != null : !old.Equals(value);
        }

        public void setVariableField(string variable, string field, object value, string compareField,
                                     object compareValue, CallerController<CallerData> cc)
        {
            var tab = this.getVariable(variable, cc);
            var rec = tab.select(compareField, compareValue);
            if (rec != null)
            {
                rec.setValue(field, value);
            }
            else
            {
                throw new ContextException("Record with " + compareField + "=" + compareValue + " not found");
            }

            this.setVariable(variable, cc, tab);
        }

        public void addVariableRecord(string variable, CallerController<CallerData> cc, DataRecord record)
        {
            var tab = this.getVariable(variable, cc);
            tab.addRecord(record);
            this.setVariable(variable, cc, tab);
        }

        public void addVariableRecord(string variable, CallerController<CallerData> cc, params object[] recordData)
        {
            var tab = this.getVariable(variable, cc);
            var rec = tab.addRecord();
            for (var i = 0; i < recordData.Length; i++)
            {
                rec.addValue(recordData[i]);
            }

            this.setVariable(variable, cc, tab);
        }

        public void removeVariableRecords(string variable, CallerController<CallerData> cc, string field, object value)
        {
            var tab = this.getVariable(variable, cc);
            tab.removeRecords(rec => Util.equals(rec.getValue(field), value));

            this.setVariable(variable, cc, tab);
        }

        private DataTable callFunction(FunctionDefinition def, CallerController<CallerData> caller, RequestController<RequestData>

request, DataTable parameters)
        {
            var startTime = DateTime.Now;

            this.setupFunctions();

            FunctionData data = this.getFunctionData(def.getName());

            if (!def.isConcurrent())
            {
                this.Lock(request, data.getExecutionLock());
            }

            try
            {
                this.checkPermissions(def.getPermissions() ?? this.getPermissions(), caller);

                Type[] callerParams = { typeof(FunctionDefinition), typeof(CallerController<CallerData>), typeof

(RequestController<RequestData>), typeof(DataTable) };
                try
                {
                    var contextClass = this.GetType();
                    Log.CONTEXT_FUNCTIONS.debug("Trying to call function '" + def.getName() + "' of context '" + this.getPath() +

"'");

                    if (def.getPermissions() != null)
                    {
                        this.checkPermissions(def.getPermissions(), caller);
                    }

                    var requiredInputFormat = def.getInputFormat();
                    var requiredOutputFormat = def.getOutputFormat();

                    parameters.validate(this, this.getContextManager(), caller);

                    if (this.valueCheckingEnabled && requiredInputFormat != null && (caller == null || !caller.getProperties

().ContainsKey(CALLER_CONTROLLER_PROPERTY_NO_VALIDATION)))
                    {
                        var msg = parameters.conformMessage(requiredInputFormat);
                        if (msg != null)
                        {
                            Log.CONTEXT_FUNCTIONS.debug("Invalid input format of function '" + def.getName() + "': " + msg);

                            DataTable newParameters = new DataTable(def.getInputFormat(), true);

                            DataTableReplication.copy(parameters, newParameters, true, true, true);
                            parameters = newParameters;

                            msg = parameters.conformMessage(requiredInputFormat);
                            if (msg != null)
                            {
                                throw new ContextException("Invalid format: " + msg);
                            }
                        }
                    }

                    DataTable result = this.executeImplementation(data, caller, request, parameters);

                    if (result.isInvalid())
                    {
                        throw new ContextException(result.getInvalidationMessage());
                    }

                    if (result.getRecordCount() == 0 && result.getFormat().getFieldCount() == 0)
                    {
                        result.setFormat(def.getOutputFormat());
                    }

                    if (this.valueCheckingEnabled && requiredOutputFormat != null && (caller == null || !caller.getProperties().ContainsKey(CALLER_CONTROLLER_PROPERTY_NO_VALIDATION)))
                    {
                        var msg = result.conformMessage(requiredOutputFormat);
                        if (msg != null)
                        {
                            throw new ContextException("Function '" + def.getName() + "' of context '" + this.getPath() + "' returned value of invalid format: " + msg);
                        }
                    }

                    var endTime = DateTime.Now;
                    var milliseconds = (endTime - startTime).Milliseconds;
                    if (milliseconds > LOW_PERFORMANCE_THRESHOLD)
                    {
                        if (milliseconds > VERY_LOW_PERFORMANCE_THRESHOLD)
                        {
                            Log.PERFORMANCE.info("Function '" + def + "' in context '" + this.getPath() + "' was executing for " + (endTime - startTime) + " milliseconds");
                        }
                        else
                        {
                            Log.PERFORMANCE.debug("Function '" + def + "' in context '" + this.getPath() + "' was executing for " + (endTime - startTime) + " milliseconds");
                        }
                    }

                    return result;
                }
                catch (ValidationException ex)
                {
                    throw ex;
                }
                catch (ContextException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ContextException(string.Format(Cres.get().getString("conErrCallingFunc"), def.ToString(),

this.ToString()) + ex.Message, ex);
                }
            }
            finally
            {
                if (!def.isConcurrent())
                {
                    data.getExecutionLock().Unlock();
                }
                data.registerExecution();
            }
        }


        private DataTable executeImplementation(FunctionData data, CallerController<CallerData> caller,

RequestController<RequestData> request, DataTable parameters)
        {
            DataTable result = executeImplementationMethod(data, caller, request, parameters);

            if (result != null)
            {
                return result;
            }

            FunctionDefinition def = data.getDefinition();

            if (def.getImplementation() != null)
            {
                result = def.getImplementation().execute(this, def, caller, request, parameters);

                if (result != null)
                {
                    return result;
                }

                return this.getDefaultFunctionOutput(def);
            }

            result = this.callFunctionImpl(def, caller, request, parameters);

            if (result != null)
            {
                return result;
            }

            throw new ContextException(string.Format(Cres.get().getString("conFuncNotImpl"), def.getName(), this.getPath()));
        }

        private DataTable executeImplementationMethod(FunctionData data, CallerController<CallerData> caller,

RequestController<RequestData> request, DataTable parameters)
        {
            FunctionDefinition def = data.getDefinition();

            if (!data.isImplementationCached())
            {
                Type[] callerParams = { typeof(FunctionDefinition), typeof(CallerController<CallerData>), typeof

(RequestController<RequestData>), typeof(DataTable) };
                try
                {
                    MethodInfo implementation1 = this.GetType().GetMethod(IMPLEMENTATION_METHOD_PREFIX + def.getName(),

callerParams);
                    if (implementation1 == null)
                    {
                        return null;
                    }
                    data.setImplementationMethod(implementation1);
                }
                finally
                {
                    data.setImplementationCached(true);
                }
            }

            var implementation = data.getImplementationMethod();

            if (implementation != null)
            {
                try
                {
                    DataTable result = (DataTable)implementation.Invoke(this, new object[] { def, caller, request, parameters });

                    if (result != null)
                    {
                        return result;
                    }

                    return getDefaultFunctionOutput(def);
                }
                catch (TargetInvocationException ex)
                {
                    var cause = ex.InnerException;
                    throw new ContextException(cause.Message, cause);
                }
            }

            return null;
        }

        private DataTable getDefaultFunctionOutput(FunctionDefinition def)
        {
            TableFormat format = def.getOutputFormat();
            return format != null ? new DataTable(format, true) : new DataTable();
        }


        protected virtual void setupFunctions()
        {
        }

        public DataTable callFunction(string nameString, CallerController<CallerData> caller,
                                      RequestController<RequestData> request,
                                      DataTable parameters)
        {
            var def = this.getAndCheckFunctionDefinition(nameString);
            return this.callFunction(def, caller, request, parameters);
        }

        public DataTable callFunction(string nameString, CallerController<CallerData> caller, DataTable parameters)
        {
            return this.callFunction(nameString, caller, null, parameters);
        }

        public DataTable callFunction(string nameString, DataTable parameters)
        {
            return this.callFunction(this.getAndCheckFunctionDefinition(nameString), null, null, parameters);
        }

        public DataTable callFunction(string nameString)
        {
            var def = this.getAndCheckFunctionDefinition(nameString);
            return this.callFunction(def, null, null, new DataTable(def.getInputFormat(), true));
        }

        public DataTable callFunction(string nameString, CallerController<CallerData> caller)
        {
            var def = this.getAndCheckFunctionDefinition(nameString);
            return this.callFunction(def, caller, null, new DataTable(def.getInputFormat(), true));
        }

        public DataTable callFunction(string nameString, CallerController<CallerData> caller, params object[] parameters)
        {
            var def = this.getAndCheckFunctionDefinition(nameString);
            return this.callFunction(nameString, caller, new DataTable(def.getInputFormat(), parameters));
        }

        public DataTable callFunction(string nameString, params object[] parameters)
        {
            return this.callFunction(nameString, null, parameters);
        }

        protected virtual DataTable callFunctionImpl(FunctionDefinition def, CallerController<CallerData> caller,
                                                     RequestController<RequestData> request, DataTable parameters)
        {
            return null;
        }

        private FunctionDefinition getAndCheckFunctionDefinition(string nameString)
        {
            this.setupFunctions();

            var def = this.getFunctionDefinition(nameString);

            if (def == null)
            {
                throw new ContextException(string.Format(Cres.get().getString("conFuncNotFound"), nameString, this.getPath()));
            }

            return def;
        }

        public void addVariableDefinition(VariableDefinition def)
        {
            if (this.getVariableDefinition(def.getName()) != null)
            {
                throw new ArgumentException("Variable '" + def.getName() + "' already defined in context '" + this.getPath() +

"'");
            }

            this.variableDataLock.EnterReadLock();
            try
            {
                this.variableData[def.getName().ToLower()] = new VariableData(def);

                if (this.setupComplete && this.fireUpdateEvents && !def.isHidden())
                {
                    this.fireVariableAdded(def);
                }

                if (this.getContextManager() != null)
                {
                    this.getContextManager().variableAdded(this, def);
                }
            }
            finally
            {
                this.variableDataLock.ExitReadLock();
            }
        }

        private void fireVariableAdded(VariableDefinition def)
        {
            EventDefinition ed = this.getEventDefinition(E_VARIABLE_ADDED);
            if (ed != null)
            {
                this.fireEvent(ed.getName(), new DataTable(this.varDefToDataRecord(def, null)));
            }
        }

        public void removeVariableDefinition(string nameString)
        {
            this.removeVariableDefinition(this.getVariableDefinition(nameString));
        }

        private void removeVariableDefinition(VariableDefinition def)
        {
            if (def == null)
            {
                return;
            }

            VariableData data = null;

            this.variableDataLock.EnterWriteLock();
            try
            {
                this.variableData.TryRemove(def.getName().ToLower(), out data);
            }
            finally
            {
                this.variableDataLock.ExitWriteLock();
            }


            if (this.setupComplete && this.fireUpdateEvents && !def.isHidden())
            {
                var ed = this.getEventDefinition(E_VARIABLE_REMOVED);
                if (ed != null)
                {
                    this.fireEvent(ed.getName(), def.getName());
                }
            }

            if (this.getContextManager() != null)
            {
                this.getContextManager().variableRemoved(this, def);
            }
        }


        public void addFunctionDefinition(FunctionDefinition def)
        {
            if (this.getFunctionDefinition(def.getName()) != null)
            {
                throw new ArgumentException("Function '" + def.getName() + "' already defined in context '" + this.getPath() + "'");
            }

            this.functionDataLock.EnterWriteLock();
            try
            {
                this.functionData[def.getName().ToLower()] = new FunctionData(def);

                if (this.setupComplete && this.fireUpdateEvents && !def.isHidden())
                {
                    this.fireFunctionAdded(def);
                }

                if (this.getContextManager() != null)
                {
                    this.getContextManager().functionAdded(this, def);
                }
            }
            finally
            {
                this.functionDataLock.ExitWriteLock();
            }
        }

        protected void fireFunctionAdded(FunctionDefinition def)
        {
            EventDefinition ed = this.getEventDefinition(E_FUNCTION_ADDED);
            if (ed != null)
            {
                this.fireEvent(ed.getName(), new DataTable(this.funcDefToDataRecord(def, null)));
            }
        }


        public void removeFunctionDefinition(string nameString)
        {
            this.removeFunctionDefinition(this.getFunctionDefinition(nameString));
        }

        private void removeFunctionDefinition(FunctionDefinition def)
        {
            if (def == null)
            {
                return;
            }

            FunctionData data;

            this.functionDataLock.EnterWriteLock();
            try
            {
                this.functionData.TryRemove(def.getName().ToLower(), out data);
            }
            finally
            {
                this.functionDataLock.ExitWriteLock();
            }

            Monitor.Enter(data);
            try
            {
                if (this.setupComplete && this.fireUpdateEvents && !def.isHidden())
                {
                    var ed = this.getEventDefinition(E_FUNCTION_REMOVED);
                    if (ed != null)
                    {
                        this.fireEvent(ed.getName(), def.getName());
                    }
                }

                if (this.getContextManager() != null)
                {
                    this.getContextManager().functionRemoved(this, def);
                }
            }
            finally
            {
                Monitor.Exit(data);
            }
        }

        public void addEventDefinition(EventDefinition def)
        {
            if (this.getEventDefinition(def.getName()) != null)
            {
                throw new ArgumentException("Event '" + def.getName() + "' already defined in context '" + this.getPath() + "'");
            }

            this.eventDataLock.EnterWriteLock();
            try
            {
                this.eventData[def.getName().ToLower()] = new EventData(def);



                if (this.setupComplete && this.fireUpdateEvents && !def.isHidden())
                {
                    this.fireEventAdded(def);
                }

                if (this.getContextManager() != null)
                {
                    this.getContextManager().eventAdded(this, def);
                }
            }
            finally
            {
                this.eventDataLock.ExitWriteLock();
            }
        }


        protected void fireEventAdded(EventDefinition def)
        {
            EventDefinition ed = this.getEventDefinition(E_EVENT_ADDED);
            if (ed != null)
            {
                this.fireEvent(ed.getName(), new DataTable(this.evtDefToDataRecord(def, null)));
            }
        }


        public void removeEventDefinition(string nameString)
        {
            this.removeEventDefinition(this.getEventDefinition(nameString));
        }

        private void removeEventDefinition(EventDefinition def)
        {
            if (def == null)
            {
                return;
            }

            this.eventDataLock.EnterWriteLock();
            try
            {
                EventData data;
                if (!this.eventData.TryRemove(def.getName().ToLower(), out data)) return;

                if (this.setupComplete && this.fireUpdateEvents && !def.isHidden())
                {
                    var ed = this.getEventDefinition(E_EVENT_REMOVED);
                    if (ed != null)
                    {
                        this.fireEvent(ed.getName(), def.getName());
                    }
                }

                if (this.getContextManager() != null)
                {
                    this.getContextManager().eventRemoved(this, def);
                }

            }
            finally
            {
                this.eventDataLock.ExitWriteLock();
            }
        }

        public VariableData getVariableData(string nameString)
        {
            VariableData value;
            this.variableData.TryGetValue(nameString.ToLower(), out value); // .toLowerCase(Locale.ENGLISH)];
            return value;
        }

        public virtual VariableDefinition getVariableDefinition(string nameString)
        {
            VariableData data = this.getVariableData(nameString);
            return data != null ? data.getDefinition() : null;
        }

        public VariableDefinition getVariableDefinition(string nameString, CallerController<CallerData> caller)
        {
            foreach (var vd in this.getVariableDefinitions(caller, true))
            {
                if (vd.getName().Equals(nameString))
                {
                    return vd;
                }
            }

            return null;
        }

        public FunctionData getFunctionData(string name)
        {
            FunctionData value;
            this.functionData.TryGetValue(name.ToLower(), out value);
            return value;
        }

        public virtual FunctionDefinition getFunctionDefinition(string nameString)
        {
            FunctionData data = this.getFunctionData(nameString);
            return data != null ? data.getDefinition() : null;
        }

        public FunctionDefinition getFunctionDefinition(string nameString, CallerController<CallerData> caller)
        {
            foreach (var fd in this.getFunctionDefinitions(caller, true))
            {
                if (fd.getName().Equals(nameString))
                {
                    return fd;
                }
            }

            return null;
        }

        public virtual EventData getEventData(string nameString)
        {
            EventData value;
            this.eventData.TryGetValue(nameString.ToLower(), out value);
            return value;
        }

        public EventDefinition getEventDefinition(string nameString)
        {
            var ed = this.getEventData(nameString);
            return ed != null ? ed.getDefinition() : null;
        }

        public EventDefinition getEventDefinition(string nameString, CallerController<CallerData> caller)
        {
            EventDefinition def = this.getEventDefinition(nameString);

            if (def == null)
            {
                return null;

            }

            bool accessGranted = this.checkPermissions(def.getPermissions() != null ? def.getPermissions() : this.getPermissions

(), caller, this);

            return accessGranted ? def : null;
        }

        private EventDefinition getAndCheckEventDefinition(string name)
        {
            this.setupEvents();

            EventDefinition def = this.getEventDefinition(name);

            if (def == null)
            {
                throw new ContextRuntimeException(string.Format(Cres.get().getString("conEvtNotAvailExt"), name, this.getPath()));
            }

            return def;
        }

        protected void setupEvents()
        {
        }

        protected void postEvent(Event ev, EventDefinition ed, CallerController<CallerData> caller, FireEventRequestController

request)
        {
        }

        protected void updateEvent(Event ev, EventDefinition ed, CallerController<CallerData> caller, FireEventRequestController

request)
        {
        }

        protected void postEvent(Event ev, EventDefinition ed, long? customExpirationPeriod)
        {
        }



        protected Event fireEvent(EventDefinition ed, DataTable data, int? level, long? id, DateTime? creationtime, int? listener, CallerController<CallerData> caller, FireEventRequestController request, Permissions aPermissions)
        {
            if (id == null)
            {
                id = EventUtils.generateEventId();
            }

            var eventToFire = new Event(this.getPath(), ed, level == DEFAULT_EVENT_LEVEL ? ed.getLevel() : level, data, id, creationtime, this.permissions);

            return this.fireEvent(ed, eventToFire, listener, caller, request);
        }

        protected Event fireEvent(Event anEvent)
        {
            return this.fireEvent(this.getAndCheckEventDefinition(anEvent.getName()), anEvent, null, null, null);
        }

        protected Event fireEvent(EventDefinition ed, Event anEvent, int? listener, CallerController<CallerData> caller, FireEventRequestController request)
        {
            Logger logger = Log.CONTEXT_EVENTS;

            if (caller != null)
            {
                try
                {
                    this.checkPermissions(ed.getFirePermissions() != null ? ed.getFirePermissions() : this.getPermissions(), caller);
                }
                catch (ContextSecurityException ex)
                {
                    throw new ContextRuntimeException(ex);
                }
            }

  


            if (logger.isDebugEnabled())
            {
                logger.debug("Event fired: " + anEvent);
            }

            anEvent.setListener(listener);

            if (request != null)
            {
                anEvent.setOriginator(request.getOriginator());
            }

            EventData edata = this.getEventData(ed.getName());




            if (anEvent.getData().isInvalid())
            {
                throw new ContextRuntimeException(anEvent.getData().getInvalidationMessage());
            }

            if (ed.getFormat() != null)
            {
                String msg = anEvent.getData().conformMessage(ed.getFormat());
                if (msg != null)
                {
                    logger.debug("Wrong format data for event '" + ed + "' in context '" + this.ToString() + "': " + msg);
                    DataTable newData = new DataTable(ed.getFormat(), true);
                    DataTableReplication.copy(anEvent.getData(), newData);
                    anEvent.setData(newData);
                }
            }



            Event processed = request != null ? request.process(anEvent) : anEvent;

            if (processed == null)
            {
                return null;
            }

            Event duplicate = null;


            if (this.contextManager != null) // && (duplicate == null))
            {
                this.contextManager.queue(edata, anEvent);
            }

            return anEvent;
        }

        public Event fireEvent(String name, int level, CallerController<CallerData> caller, FireEventRequestController request, Permissions permissions, DataTable data)
        {
            EventDefinition ed = this.getAndCheckEventDefinition(name);
            return fireEvent(ed, data, level, null, null, null, caller, request, permissions);
        }

        public Event fireEvent(String name, DataTable data, int? level, long? id, DateTime? creationtime, int? listener, CallerController<CallerData> caller, FireEventRequestController request)
        {
            return this.fireEvent(this.getAndCheckEventDefinition(name), data, level, id, creationtime, listener, caller, request, null);
        }

        public Event fireEvent(String name, DataTable data)
        {
            return this.fireEvent(getAndCheckEventDefinition(name), data, DEFAULT_EVENT_LEVEL, null, null, null, null, null, null);
        }

        public Event fireEvent(String name, CallerController<CallerData> caller, DataTable data)
        {
            return this.fireEvent(this.getAndCheckEventDefinition(name), data, DEFAULT_EVENT_LEVEL, null, null, null, caller, null, null);
        }

        public Event fireEvent(String name, int level, DataTable data)
        {
            return this.fireEvent(this.getAndCheckEventDefinition(name), data, level, null, null, null, null, null, null);
        }

        public Event fireEvent(String name, int level, CallerController<CallerData> caller, DataTable data)
        {
            return this.fireEvent(this.getAndCheckEventDefinition(name), data, level, null, null, null, caller, null, null);
        }

        public Event fireEvent(String nameString)
        {
            EventDefinition ed = this.getAndCheckEventDefinition(nameString);
            return this.fireEvent(ed, new DataTable(ed.getFormat(), true), DEFAULT_EVENT_LEVEL, null, null, null, null, null, null);
        }

        public Event fireEvent(String nameString, CallerController<CallerData> caller)
        {
            EventDefinition ed = this.getAndCheckEventDefinition(nameString);
            return this.fireEvent(ed, new DataTable(ed.getFormat(), true), DEFAULT_EVENT_LEVEL, null, null, null, caller, null, null);
        }

        public Event fireEvent(string name, params object[] data)
        {
            EventDefinition ed = this.getAndCheckEventDefinition(name);
            return this.fireEvent(ed, new DataTable(ed.getFormat(), data), DEFAULT_EVENT_LEVEL, null, null, null, null, null, null);
        }


        protected CallerController<CallerData> getEventBindingsCallerController()
        {
            return null;
        }


        public void removeVariableValue(VariableDefinition def)
        {
            throw new NotSupportedException();
        }

        private void Lock(RequestController<RequestData> request, ReentrantLock aLock)
        {
            long? lockTimeout = (request != null && request.getLockTimeout() != null) ? request.getLockTimeout() : null;

            if (lockTimeout != null)
            {
                try
                {
                    if (!aLock.tryLock((long)lockTimeout))
                    {
                        throw new ContextException(Cres.get().getString("conLockFailed"));
                    }
                }
                catch (Exception ex)
                {
                    throw new ContextException(Cres.get().getString("interrupted"));
                }
            }
            else
            {
                aLock.Lock();
            }
        }


        public override string ToString()
        {
            var desc = this.getDescription();
            return desc ?? this.getPath();
        }

        public string toDetailedString()
        {
            return this.description != null ? this.description + " (" + this.getPath() + ")" : this.getPath();
        }

        public void accept(ContextVisitor<Context> visitor)
        {
            visitor.visit(this);

            var temp = new List<Context>(this.children);

            foreach (var child in temp)
            {
                child.accept(visitor);
            }
        }

        public abstract DataTable decodeRemoteDataTable(TableFormat format, string s);
        public abstract void reinitialize();
        public abstract DataTable callRemoteFunction(string name, TableFormat outputFormat, DataTable parameters);
        public abstract DataTable getRemoteVariable(TableFormat format, string nameString);

        public DataTable getVvariables(VariableDefinition def, CallerController<CallerData> caller,
                                       RequestController<RequestData> request)
        {
            var ans = new DataTable(def.getFormat());
            foreach (var vardef in this.getVariableDefinitions(caller))
            {
                ans.addRecord(this.varDefToDataRecord(vardef, caller));
            }

            return ans;
        }

        protected string encodeFormat(TableFormat format, CallerController<CallerData> caller)
        {
            return format != null ? format.encode(false) : null;
        }

        protected virtual TableFormat decodeFormat(string source, CallerController<CallerData> caller)
        {
            return source != null ? new TableFormat(source, new ClassicEncodingSettings(false)) : null;
        }

        protected DataRecord varDefToDataRecord(VariableDefinition vardef)
        {
            return this.varDefToDataRecord(vardef, null);
        }

        private DataRecord varDefToDataRecord(VariableDefinition vardef, CallerController<CallerData> caller)
        {
            return
                new DataRecord(VARIABLE_DEFINITION_FORMAT).addString(vardef.getName()).addString(this.encodeFormat

(vardef.getFormat(), caller)).addString(vardef.getDescription()).addBoolean(
                    vardef.isReadable()).addBoolean(vardef.isWritable()).addString(vardef.getHelp()).addString(
                    vardef.getGroup()).addString(vardef.getIconId()).addString(vardef.getHelpId());
        }

        protected VariableDefinition varDefFromDataRecord(DataRecord rec)
        {
            return this.varDefFromDataRecord(rec, null);
        }

        private VariableDefinition varDefFromDataRecord(DataRecord rec, CallerController<CallerData> caller)
        {
            var format = rec.getString(FIELD_VD_FORMAT);

            var readable = (bool)rec.getBoolean(FIELD_VD_READABLE);

            var writable = (bool)rec.getBoolean(FIELD_VD_WRITABLE);

            var def = new VariableDefinition(rec.getString(FIELD_VD_NAME), this.decodeFormat(format, caller), readable, writable, rec.getString(FIELD_VD_DESCRIPTION), rec.getString(FIELD_VD_GROUP));

            def.setHelp(rec.getString(FIELD_VD_HELP));
            def.setIconId(rec.getString(FIELD_VD_ICON_ID));

            if (rec.hasField(FIELD_VD_HELP_ID))
            {
                def.setHelpId(rec.getString(FIELD_VD_HELP_ID));
            }

            return def;
        }

        public DataTable getVfunctions(VariableDefinition def, CallerController<CallerData> caller,
                                       RequestController<RequestData> request)
        {
            var ans = new DataTable(def.getFormat());
            foreach (var funcdef in this.getFunctionDefinitions(caller))
            {
                ans.addRecord(this.funcDefToDataRecord(funcdef, caller));
            }

            return ans;
        }

        protected DataRecord funcDefToDataRecord(FunctionDefinition funcdef)
        {
            return this.funcDefToDataRecord(funcdef, null);
        }

        private DataRecord funcDefToDataRecord(FunctionDefinition funcdef, CallerController<CallerData> caller)
        {
            return
                new DataRecord(FUNCTION_DEFINITION_FORMAT).addString(funcdef.getName()).addString(this.encodeFormat

(funcdef.getInputFormat(), caller)).addString(this.encodeFormat(funcdef.getOutputFormat(),
                                                                                           caller)).addString(
                    funcdef.getDescription()).addString(funcdef.getHelp()).addString(funcdef.getGroup());
        }

        protected FunctionDefinition funcDefFromDataRecord(DataRecord rec)
        {
            return this.funcDefFromDataRecord(rec, null);
        }

        private FunctionDefinition funcDefFromDataRecord(DataRecord rec, CallerController<CallerData> caller)
        {
            string function = rec.getString(FIELD_FD_NAME);

            TableFormat inputFormat;
            try
            {
                inputFormat = this.decodeFormat(rec.getString(FIELD_FD_INPUTFORMAT), caller);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error decoding input format of function '" + function + "': " + ex.Message, ex);
            }

            TableFormat outputFormat;
            try
            {
                outputFormat = decodeFormat(rec.getString(FIELD_FD_OUTPUTFORMAT), caller);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error decoding output format of function '" + function + "': " + ex.Message, ex);
            }

            FunctionDefinition def = new FunctionDefinition(function, inputFormat, outputFormat, rec.getString(FIELD_FD_DESCRIPTION), rec.getString(FIELD_FD_GROUP));

            def.setHelp(rec.getString(FIELD_FD_HELP));
            def.setIconId(rec.getString(FIELD_FD_ICON_ID));

            if (rec.hasField(FIELD_FD_CONCURRENT) && rec.getBoolean(FIELD_FD_CONCURRENT) != null)
            {
                def.setConcurrent((bool)rec.getBoolean(FIELD_FD_CONCURRENT));
            }

            if (rec.hasField(FIELD_FD_PERMISSIONS) && rec.getString(FIELD_FD_PERMISSIONS) != null)
            { def.setPermissions(new Permissions(rec.getString(FIELD_FD_PERMISSIONS))); }

            return def;
        }

        public DataTable getVevents(VariableDefinition def, CallerController<CallerData> caller, RequestController<RequestData> request)
        {
            var ans = new DataTable(def.getFormat());
            foreach (var ed in this.getEventDefinitions(caller))
            {
                ans.addRecord(this.evtDefToDataRecord(ed, caller));
            }

            return ans;
        }

        protected DataRecord evtDefToDataRecord(EventDefinition evdef)
        {
            return this.evtDefToDataRecord(evdef, null);
        }

        private DataRecord evtDefToDataRecord(EventDefinition evdef, CallerController<CallerData> caller)
        {
            return
                new DataRecord(EVENT_DEFINITION_FORMAT).addString(evdef.getName()).addString(this.encodeFormat(evdef.getFormat(),

caller)).addString(evdef.getDescription()).addString(evdef.getHelp())
                    .addInt(evdef.getLevel()).addString(evdef.getGroup());
        }

        protected EventDefinition evtDefFromDataRecord(DataRecord rec)
        {
            return this.evtDefFromDataRecord(rec, null);
        }

        private EventDefinition evtDefFromDataRecord(DataRecord rec, CallerController<CallerData> caller)
        {
            var format = rec.getString(FIELD_ED_FORMAT);

            var def = new EventDefinition(rec.getString(FIELD_ED_NAME), this.decodeFormat(format, caller),
                                          rec.getString(
                                              FIELD_ED_DESCRIPTION), rec.getString(FIELD_ED_GROUP));

            def.setHelp(rec.getString(FIELD_ED_HELP));
            def.setLevel(rec.getInt(FIELD_ED_LEVEL));
            def.setIconId(rec.getString(FIELD_ED_ICON_ID));
            return def;
        }

        public DataTable getVchildren(VariableDefinition def, CallerController<CallerData> caller,
                                      RequestController<RequestData> request)
        {
            var ans = new DataTable(def.getFormat());
            foreach (var con in this.getChildren(caller))
            {
                ans.addRecord().addString(con.getName());
            }

            return ans;
        }

        public DataTable getVinfo(VariableDefinition def, CallerController<CallerData> caller,
                                  RequestController<RequestData> request)
        {
            return new DataTable(def.getFormat(), this.getDescription(), this.getType(), this.getGroup(), this.getIconId());
        }

        protected DataTable createContextInfoTable()
        {
            return new DataTable(INFO_DEFINITION_FORMAT, this.getDescription(), this.getType(), this.getGroup(), this.getIconId(), this.getLocalRoot(), this.getRemoteRoot(), this.getRemotePath(), this.getRemotePrimaryRoot(), isMapped());
        }

        public DataTable callFgetCopyData(FunctionDefinition def, CallerController<CallerData> caller, RequestController<RequestData> request, DataTable parameters)
        {
            var result = new DataTable(def.getOutputFormat().Clone() as TableFormat);

            var groupName = parameters.rec().getString(VF_INFO_GROUP);

            List<Context> recipients = null;

            var recipientsTable = parameters.rec().getDataTable(FIF_COPY_DATA_RECIPIENTS);

            if (recipientsTable != null)
            {
                recipients = new List<Context>();

                foreach (var rec in recipientsTable)
                {
                    var recipient = this.getContextManager().get(rec.getString(FIF_COPY_DATA_RECIPIENTS_RECIPIENT), caller);

                    if (recipient != null)
                    {
                        recipients.Add(recipient);
                    }
                }
            }

            foreach (var vd in this.getVariableDefinitions(caller))
            {
                if (groupName != null && !groupName.Equals(vd.getGroup()))
                {
                    continue;
                }

                if (groupName == null && vd.getGroup() == null)
                {
                    continue;
                }

                if (!vd.isReadable())
                {
                    continue;
                }

                if (vd.getFormat() == null || !vd.getFormat().isReplicated())
                {
                    continue;
                }

                if (recipients != null)
                {
                    var skip = true;

                    foreach (var recipient in recipients)
                    {
                        var rvd = recipient.getVariableDefinition(vd.getName());

                        if (rvd != null && rvd.isWritable() &&
                            (rvd.getFormat() == null || rvd.getFormat().isReplicated()))
                        {
                            skip = false;
                        }
                    }

                    if (skip)
                    {
                        continue;
                    }
                }

                var value = this.getVariable(vd.getName(), caller);

                var format = (TableFormat)value.getFormat().Clone();

                var fields = new DataTable(FIFT_REPLICATE_FIELDS);

                foreach (FieldFormat ff in format)
                {
                    if (ff.isNotReplicated())
                    {
                        ff.setReadonly(true);
                    }

                    if (!ff.isHidden() && !ff.isReadonly() && !ff.isNotReplicated())
                    {
                        fields.addRecord().addString(ff.getName()).addString(ff.ToString()).addBoolean(true);
                    }
                }

                result.addRecord().addString(vd.getName()).addString(vd.getDescription()).addBoolean(false).addDataTable(fields).addDataTable(value);
            }

            result.fixRecords();

            return result;
        }

        public DataTable callFcopy(FunctionDefinition def, CallerController<CallerData> caller, RequestController<RequestData> request, DataTable parameters)
        {
            var result = new DataTable(def.getOutputFormat());

            foreach (var rec in parameters)
            {
                if ((bool)!rec.getBoolean(FOF_COPY_DATA_REPLICATE))
                {
                    continue;
                }

                var varName = rec.getString(FOF_COPY_DATA_NAME);
                var providedDesc = rec.getString(FOF_COPY_DATA_DESCRIPTION);
                var varValue = rec.getDataTable(FOF_COPY_DATA_VALUE);

                var targetVd = this.getVariableDefinition(varName);

                if (targetVd == null)
                {
                    result.addRecord().addString(providedDesc).addBoolean(false).addString(
                        Cres.get().getString("conVarNotFoundInTgt"));
                    continue;
                }

                var varDesc = targetVd.getDescription();

                if (!targetVd.isWritable())
                {
                    result.addRecord().addString(varDesc).addBoolean(false).addString(
                        Cres.get().getString("conVarNotWritableInTgt"));
                    continue;
                }

                DataTable tgtVal;

                try
                {
                    tgtVal = this.getVariable(varName, caller);
                }
                catch (ContextException ex)
                {
                    result.addRecord().addString(varDesc).addBoolean(false).addString(
                        Cres.get().getString("conErrGettingTgtVar") + ex.Message);
                    continue;
                }

                var fields = new List<string>();
                foreach (var fieldRec in rec.getDataTable(FOF_COPY_DATA_FIELDS))
                {
                    if ((bool)fieldRec.getBoolean(FIF_REPLICATE_FIELDS_REPLICATE))
                    {
                        fields.Add(fieldRec.getString(FIF_REPLICATE_FIELDS_NAME));
                    }
                }

                var tableCopyErrors = DataTableReplication.copy(varValue, tgtVal, false, false, true, fields);

                DataTableUtils.inlineData(tgtVal, this.getContextManager(), caller);

                try
                {
                    this.setVariable(targetVd, caller, request, tgtVal);
                }
                catch (ContextException ex)
                {
                    Log.CONTEXT_FUNCTIONS.warn("Error setting variable during context copy", ex);
                    result.addRecord().addString(varDesc).addBoolean(false).addString(
                        Cres.get().getString("conErrSettingTgtVar") + ex.Message);
                    continue;
                }

                if (tableCopyErrors.Count > 0)
                {
                    result.addRecord().addString(varDesc).addBoolean(false).addString(StringUtils.print(
                                                                                          tableCopyErrors, "; "));
                }
                else
                {
                    result.addRecord().addString(varDesc).addBoolean(true);
                }
            }

            return result;
        }

        public DataTable callFcopyToChildren(FunctionDefinition def, CallerController<CallerData> caller,
                                             RequestController<RequestData> request, DataTable parameters)
        {
            return this.copyTo(def, caller, request, parameters, this.getChildren(caller));
        }

        protected DataTable copyTo(FunctionDefinition def, CallerController<CallerData> caller,
                                   RequestController<RequestData> request, DataTable parameters,
                                   List<Context> childrenList)
        {
            var result = new DataTable(def.getOutputFormat());

            foreach (var child in childrenList)
            {
                var conDesc = child.getDescription() ?? child.getPath();
                DataTable conRes;

                try
                {
                    conRes = child.callFunction(F_COPY, caller, request, parameters);
                }
                catch (ContextException ex)
                {
                    result.addRecord().addString(conDesc).addString(null).addBoolean(false).addString(ex.Message);
                    continue;
                }

                foreach (var rec in conRes)
                {
                    result.addRecord().addString(conDesc).addString(rec.getString(FIELD_REPLICATE_VARIABLE)).addBoolean(
                        (bool)rec.getBoolean(FIELD_REPLICATE_SUCCESSFUL)).addString(rec.getString(FIELD_REPLICATE_ERRORS));
                }
            }

            return result;
        }

    }
}