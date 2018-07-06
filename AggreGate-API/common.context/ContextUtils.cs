using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using com.tibbo.aggregate.common.util;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.context
{
    public class ContextUtils
    {
        public const String CONTEXT_NAME_PATTERN = "\\w*";
        public const String CONTEXT_PATH_PATTERN = "[\\w|\\.]+";
        public const String CONTEXT_MASK_PATTERN = "[\\w|\\.|\\*]+";
        public const String CONTEXT_TYPE_PATTERN = "[\\w|\\.]+";
        public const String ENTITY_NAME_PATTERN = "\\w+";
        public const String IDENTIFIER_PATTERN = "\\w*";

        private const String CONTEXT_CLASS_SUFFIX = "Context";

        public const String CONTEXT_NAME_SEPARATOR = ".";
        public const String CONTEXT_TYPE_SEPARATOR = ".";
        public const String CONTEXT_GROUP_MASK = "*";
        public const String ENTITY_GROUP_MASK = "*";
        public const String CONTEXT_TYPE_ANY = "*";
        public const String ENTITY_GROUP_SEPARATOR = "|";

        public const String CTX_ROOT = "";
        public const String CTX_USERS = "users";
        public const String CTX_EVENTS = "events";
        public const String CTX_EVENT_FILTERS = "filters";
        public const String CTX_DEVICESERVERS = "deviceservers";
        public const String CTX_ADMINISTRATION = "administration";
        public const String CTX_DEVICES = "devices";
        public const String CTX_PLUGINS_GLOBAL_CONFIG = "plugins_config";
        public const String CTX_PLUGINS_USER_CONFIG = "plugins_config";
        public const String CTX_PLUGIN_CONFIG = "plugin_config";
        public const String CTX_CONFIG = "config";
        public const String CTX_EXTERNAL_DEVICE_SERVERS = "external_device_servers";
        public const String CTX_ALERTS = "alerts";
        public const String CTX_GROUPS = "groups";
        public const String CTX_DSGROUPS = "dsgroups";
        public const String CTX_DEVGROUPS = "devgroups";
        public const String CTX_USERGROUPS = "usergroups";
        public const String CTX_REPORTS = "reports";
        public const String CTX_COMMON_DATA = "common";
        public const String CTX_QUERIES = "queries";
        public const String CTX_JOBS = "jobs";
        public const String CTX_SCHEDULER = "scheduler";
        public const String CTX_WIDGETS = "widgets";
        public const String CTX_AUTORUN = "autorun";
        public const String CTX_FAVOURITES = "favourites";
        public const String CTX_SCRIPTS = "scripts";
        public const String CTX_TRACKERS = "trackers";
        public const String CTX_UTILS = "utilities";
        public const String CTX_DEBUG = "debug";

        public const String GROUP_DEFAULT = "default";
        public const String GROUP_SYSTEM = "system";
        public const String GROUP_REMOTE = "remote";
        public const String GROUP_CUSTOM = "custom";
        public const String GROUP_ACCESS = "access";


        public const int ENTITY_VARIABLE = 1;
        public const int ENTITY_FUNCTION = 2;
        public const int ENTITY_EVENT = 4;
        public const int ENTITY_ACTION = 8;

        public const String USERNAME_PATTERN = "%";

        public const String VARIABLES_GROUP_DS_SETTINGS = "ds_settings";


        public static Boolean matchesToMask(String mask, String name)
        {
            return matchesToMask(mask, name, false, false);
        }


        public static Boolean matchesToMask(String mask, String context, Boolean contextMayExtendMask,
                                            Boolean maskMayExtendContext)
        {
            if (mask == null || context == null)
            {
                return true;
            }

            if (!isMask(mask))
            {
                if (contextMayExtendMask && maskMayExtendContext)
                {
                    int length = Math.Max(mask.Length, context.Length);
                    return mask.Substring(0, length).Equals(context.Substring(0, length));
                }
                var equals = mask.Equals(context);

                if (maskMayExtendContext)
                {
                    return equals || mask.StartsWith(context + CONTEXT_NAME_SEPARATOR);
                }
                if (contextMayExtendMask)
                {
                    return equals || context.StartsWith(mask + CONTEXT_NAME_SEPARATOR);
                }
                return equals;
            }
            var maskParts = StringUtils.split(mask, CONTEXT_NAME_SEPARATOR[0]);
            var nameParts = StringUtils.split(context, CONTEXT_NAME_SEPARATOR[0]);

            if (maskParts.Count > nameParts.Count && !maskMayExtendContext)
            {
                return false;
            }

            if (maskParts.Count < nameParts.Count && !contextMayExtendMask)
            {
                return false;
            }

            for (var i = 0; i < Math.Min(maskParts.Count, nameParts.Count); i++)
            {
                if (maskParts[i].Equals(CONTEXT_GROUP_MASK) &&
                    !nameParts[i].Equals(CONTEXT_GROUP_MASK))
                {
                    continue;
                }
                if (!maskParts[i].Equals(nameParts[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static Boolean isMask(String name)
        {
            return name.Contains(CONTEXT_GROUP_MASK);
        }

        public static List<Context> expandContextMaskToContexts(String mask, ContextManager contextManager,
                                                                CallerController<CallerData> caller)
        {
            var res = new List<Context>();

            var paths = expandContextMaskToPaths(mask, contextManager, caller);

            foreach (var path in paths)
            {
                var con = contextManager.get(path, caller);
                if (con != null)
                {
                    res.Add(con);
                }
            }

            return res;
        }

        public static List<String> expandContextMaskToPaths(String mask,
                                                            ContextManager contextManager,
                                                            CallerController<CallerData> caller)
        {
            var result = new List<String>();

            var parts = StringUtils.split(mask, CONTEXT_NAME_SEPARATOR[0]);

            for (var i = 0; i < parts.Count; i++)
            {
                if (!parts[i].Equals(CONTEXT_GROUP_MASK)) continue;
                var head = new StringBuilder();

                for (var j = 0; j < i; j++)
                {
                    if (j > 0)
                    {
                        head.Append(CONTEXT_NAME_SEPARATOR);
                    }
                    head.Append(parts[j]);
                }

                var tail = new StringBuilder();

                for (var j = i + 1; j < parts.Count; j++)
                {
                    tail.Append(CONTEXT_NAME_SEPARATOR);
                    tail.Append(parts[j]);
                }

                result.AddRange(expandContextMaskPart(head.ToString(), tail.ToString(), contextManager, caller));
                return result;
            }

            if (contextManager.get(mask, caller) != null)
            {
                result.Add(mask);
            }

            return result;
        }


        private static List<String> expandContextMaskPart(String head, String tail,
                                                          ContextManager contextManager,
                                                          CallerController<CallerData> caller)
        {
            var result = new List<String>();

            var con = contextManager.get(head, caller);

            if (con == null)
            {
                return result;
            }

            foreach (var child in con.getChildren(caller))
            {
                result.AddRange(
                    expandContextMaskToPaths(head + CONTEXT_NAME_SEPARATOR + child.getName() + tail,
                                             contextManager, caller));
            }

            return result;
        }

        public static Boolean isRelative(String name)
        {
            return name.StartsWith(CONTEXT_NAME_SEPARATOR);
        }

        public static String createName(params String[] parts)
        {
            var res = new StringBuilder();

            for (var i = 0; i < parts.Length; i++)
            {
                if (i > 0)
                {
                    res.Append(CONTEXT_NAME_SEPARATOR);
                }

                res.Append(parts[i]);
            }

            return res.ToString();
        }

        public static String createGroup(params String[] parts)
        {
            var res = new StringBuilder();

            for (int i = 0; i < parts.Length; i++)
            {
                if (i == parts.Length - 1 && parts[i] == null)
                {
                    break;
                }

                if (i > 0)
                {
                    res.Append(ENTITY_GROUP_SEPARATOR);
                }

                res.Append(parts[i]);
            }

            return res.ToString();
        }

        public static String getBaseGroup(String group)
        {
            if (group == null)
            {
                return null;
            }

            var index = group.IndexOf(ENTITY_GROUP_SEPARATOR[0]);
            return index == -1 ? group : group.Substring(0, index);
        }

        public static String getTypeForClass(Type clazz)
        {
            var name = clazz.Name;
            name = name.Substring(0, 1).ToLower(CultureInfo.GetCultureInfo("en")) + name.Substring(1, name.Length - 1);
            if (name.EndsWith(CONTEXT_CLASS_SUFFIX))
            {
                name = name.Substring(0, name.Length - CONTEXT_CLASS_SUFFIX.Length);
            }
            return name;
        }

        public static Boolean isValidContextType(String s)
        {
            return CONTEXT_TYPE_ANY.Equals(s) || Pattern.matches(CONTEXT_TYPE_PATTERN, s);
        }

        //TODO: Implement pattern matching correctly! See also tests in ..\AggreGateAgent\AggreGateAgent Tests\AbstractContextTests.cs
        public static Boolean isValidContextName(String s)
        {
            return s != null && Pattern.matches(CONTEXT_NAME_PATTERN, s);
        }

        public static bool isValidContextNameChar(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_';
        }
    }
}
