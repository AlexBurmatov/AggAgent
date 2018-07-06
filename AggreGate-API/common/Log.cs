using System;

namespace com.tibbo.aggregate.common
{
    using JavaCompatibility;

    public class Log
    {
        public const String ALERTS = "ag.alerts";

        public const String BINDINGS = "ag.bindings";

        public const String COMMANDS = "ag.commands";
        public const String COMMANDS_DS = "ag.commands.device_server";
        public const String COMMANDS_CLIENT = "ag.commands.client";
        public const String COMMANDS_AGENT = "ag.commands.agent";

        public const String CDATA = "ag.commondata";

        public const String CLIENTS = "ag.clients";

        public const String CONFIG = "ag.config";

        public static readonly Logger CONTEXT = Logger.getLogger("ag.context");
        public static readonly Logger CONTEXT_INFO = Logger.getLogger("ag.context.info");

        public static readonly Logger CONTEXT_CHILDREN = Logger.getLogger("ag.context.children");
        public static readonly Logger CONTEXT_VARIABLES = Logger.getLogger("ag.context.variables");
        public static readonly Logger CONTEXT_FUNCTIONS = Logger.getLogger("ag.context.functions");

        public static readonly Logger CONTEXT_EVENTS = Logger.getLogger("ag.context.events");

        public const String CONTEXT_ACTIONS = "ag.context.actions";

        public const String CORE = "ag.core";
        public const String CORE_THREAD = "ag.core.thread";

        public const String DATABASE = "ag.database";

        public const String DEVICE = "ag.device";
        public const String DEVICE_DISCOVERY = "ag.device.discovery";
        public const String DEVICE_SYNCHRONIZATION = "ag.device.sync";
        public const String DEVICE_AGENT = "ag.device.agent";

        public const String DEVICE_FILE = "ag.device.file";

        public const String DEVICEBROWSER = "ag.device_browser";

        public const String DEVICESERVER = "ag.device_server";

        public const String DEVICESERVER_INBANDS = "ag.device_server.inbands";

        public const String DATATABLE = "ag.data_table";
        public const String DATATABLE_FILTER = "ag.data_table.filter";

        public const String DATATABLEEDITOR = "ag.data_table_editor";

        public const String DNS = "ag.dns";

        public const String ENTITYSELECTOR = "ag.entity_selector";

        public const String EVENTFILTER = "ag.eventfilters";

        public const String EVENTLOG = "ag.event_log";

        public const String EXPRESSIONBUILDER = "ag.expression_builder";

        public const String EXPRESSIONS = "ag.expressions";

        public const String FAVOURITES = "ag.favourites";

        public const String GROUPS = "ag.groups";

        public const String GUI = "ag.gui";

        public const String GUIBUILDER = "ag.gui_builder";

        public const String GUIDE = "ag.guide";

        public const String MAIL = "ag.mail";

        public const String NETADMIN = "ag.net_admin";

        public static readonly Logger PERFORMANCE = Logger.getLogger("ag.performance");

        public const String PLUGINS = "ag.plugins";

        public const String PROPERTIESEDITOR = "ag.properties_editor";

        public const String PROTOCOL = "ag.protocol";

        public static readonly Logger PROTOCOL_CACHING = Logger.getLogger("ag.protocol.caching");

        public const String QUERIES = "ag.queries";

        public const String REPORTS = "ag.reports";

        public const String RESOURCE = "ag.resource";

        public const String SCHEDULER = "ag.scheduler";

        public const String SCRIPTS = "ag.scripts";

        public const String SECURITY = "ag.security";

        public const String STDOUT = "ag.stdout";

        public const String STDERR = "ag.stderr";

        public const String SYSTEMTREE = "ag.system_tree";

        public const String TEST = "ag.test";

        public const String TRACKERS = "ag.trackers";

        public const String USERS = "ag.users";

        public const String WEB = "ag.web";

        public const String WIDGETS = "ag.widgets";

        public const String LOGGING_CONFIG_FILENAME = "logging.xml";
        public const int LOGGING_CONFIG_CHECK_INTERVAL = 10000;

        public static void start()
        {
            start("");
        }

        public static void start(String homeDirectory)
        {
        }
    }
}