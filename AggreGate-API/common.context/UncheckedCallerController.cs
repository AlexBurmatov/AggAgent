using System;
using com.tibbo.aggregate.common.data;
using com.tibbo.aggregate.common.security;

namespace com.tibbo.aggregate.common.context
{
    public class UncheckedCallerController : AbstractCallerController<CallerData>
    {
        private readonly Permissions permissions = DefaultPermissionChecker.getNullPermissions();
  
        public UncheckedCallerController() : base(null)
        {
        }

        public UncheckedCallerController(String username) : base(null)
        {
            this.setUsername(username);
        }

        public UncheckedCallerController(CallerData callerData) : base(callerData)
        {
        }

        public Permissions getPermissions()
        {
            return DefaultPermissionChecker.getNullPermissions();
        }

        public override bool isPermissionCheckingEnabled()
        {
            return false;
        }

        public String getUsername()
        {
            return null;
        }

        public override Boolean isLoggedIn()
        {
            return true;
        }

        public void login(String username, Permissions permissions)
        {
        }

        public override void logout()
        {
        }

        public override Boolean isHeadless()
        {
            return true;
        }

        public void handleContextEvent(Event anEvent)
        {
            throw new NotSupportedException();
        }
    }
}