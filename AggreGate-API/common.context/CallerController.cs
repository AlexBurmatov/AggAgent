using System;
using System.Collections.Generic;
using com.tibbo.aggregate.common.security;

namespace com.tibbo.aggregate.common.context
{
    using com.tibbo.aggregate.common.util;

    public interface CallerController<T> where T : CallerData
    {
        string getUsername();

        string getInheritedUsername();

        string getEffectiveUsername();

        bool isPermissionCheckingEnabled();

        Permissions getPermissions();

        PermissionCache getPermissionCache();

        bool isLoggedIn();

        void login(string username, string inheritedUsername, Permissions permissons);

        void logout();

        bool isHeadless();

        string getType();

        string getAddress();

        DateTime getCreationTime();

        T getCallerData();

        AgDictionary<string, string> getProperties();

        void sendFeedback(int level, string message);
    }
}