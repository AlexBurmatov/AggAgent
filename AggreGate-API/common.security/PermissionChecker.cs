using System;
using System.Collections.Generic;
using com.tibbo.aggregate.common.context;

namespace com.tibbo.aggregate.common.security
{
    using com.tibbo.aggregate.common.util;

    public interface PermissionChecker
    {

        /**
         * Returns a map of all permission levels supported by the checker. Each level is defined by a name string and description string.
         */
        AgDictionary<object, string> getPermissionLevels();


        /**
         * Returns true if permission level {@code permissionLevel} is supported by the checker.
         */
        bool isValid(String permissionLevel);

        /**
         * Returns true if {@code caller} is allowed to access an object those permissions are {@code requiredPermissions}.
         */
        bool has(CallerController<CallerData> caller, Permissions requiredPermissions, Context accessedContext);

        /**
         * Returns the effective permission level of the calling party (those permissions are identified by {@code perms}) in the {@code context}.
         */
        string getLevel(Permissions perms, string context, ContextManager cm);

        /**
         * Returns true if the calling party (those permissions are identified by {@code perms}) can see {@code context} among children of its parent context because it has non-null permissions for one or
         * more direct/nested children of {@code context}.
         */
        bool canSee(Permissions perms, string context, ContextManager cm);

        /**
         * Checks whether the calling party (those permissions are identified by {@code has}) can set permissions of some other party to {@code need}.
         */
        string canActivate(Permissions has, Permissions need, ContextManager cm);
    }
}