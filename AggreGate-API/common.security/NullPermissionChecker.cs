using System;
using System.Collections.Generic;
using com.tibbo.aggregate.common.context;

namespace com.tibbo.aggregate.common.security
{
    using com.tibbo.aggregate.common.util;

    public class NullPermissionChecker : PermissionChecker
    {
        public AgDictionary<object, string> getPermissionLevels()
        {
            return null;
        }

        public Boolean has(CallerController<CallerData> caller, Permissions requiredPermissions, Context accessedContext)
        {
            return true;
        }

        public Boolean canSee(Permissions perms, String context, ContextManager cm)
        {
            return true;
        }

        public String getLevel(Permissions perms, string context, ContextManager cm)
        {
            return DefaultPermissionChecker.NULL_PERMISSIONS;
        }

        public Boolean isValid(String permType)
        {
            return true;
        }

        public String canActivate(Permissions has, Permissions need, ContextManager cm)
        {
            return null;
        }
    }
}