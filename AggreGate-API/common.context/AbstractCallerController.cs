using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using com.tibbo.aggregate.common.security;

namespace com.tibbo.aggregate.common.context
{
    using com.tibbo.aggregate.common.util;

    public abstract class AbstractCallerController<T> : CallerController<T> where T : CallerData
    {
        //private static final Set<CallerController> CONTROLLERS = Collections.newSetFromMap(new WeakHashMap());

        private string username;

        private readonly T callerData;

        private bool loggedIn;

        private string type;

        private string address;

        private readonly DateTime creationTime = DateTime.Now;

        private readonly AgDictionary<string, string> properties = new AgDictionary<string, string>();

        protected AbstractCallerController(T callerData)
        {
            this.callerData = callerData;
            //synchronized(CONTROLLERS)
            //{
            //    CONTROLLERS.add(this);
            //}
        }

        public virtual bool isLoggedIn()
        {
            return this.loggedIn;
        }

        public virtual bool isPermissionCheckingEnabled()
        {
            return true;
        }

        public PermissionCache getPermissionCache()
        {
            return null;
        }

        public T getCallerData()
        {
            return this.callerData;
        }

        public AgDictionary<string, string> getProperties()
        {
            return properties;
        }

        protected void setLoggedIn(bool loggedInBoolean)
        {
            loggedIn = loggedInBoolean;
        }

        public void sendFeedback(int level, string message)
        {
            // Do nothing
        }

        public static IList<CallerController<CallerData>> getControllers()
        {
            //List<CallerController> list = new LinkedList();
            //
            //synchronized(CONTROLLERS)
            //{
            //    for (CallerController cc : CONTROLLERS)
            //    {
            //        list.add(cc);
            //    }
            //}
            //
            //return Collections.unmodifiableList(list);

            var list = new List<CallerController<CallerData>>();
            return new ReadOnlyCollection<CallerController<CallerData>>(list);
        }

        public override string ToString()
        {
            return this.GetType().Name + "(" + (this.loggedIn ? "logged in" : "not logged in") + ")";
        }

        public virtual Permissions getPerms()
        {
            return null;
        }

        public abstract bool isHeadless();

        public Permissions getPermissions()
        {
            return null;
        }

        public string getUsername()
        {
            return this.username;
        }

        protected void setUsername(string usernameString)
        {
            this.username = usernameString;
        }

        public string getInheritedUsername()
        {
            return null;
        }

        public string getEffectiveUsername()
        {
            string inheritedUsername = this.getInheritedUsername();
            return inheritedUsername != null ? inheritedUsername : this.getUsername();
        }

        public string getType()
        {
            return this.type;
        }

        public void setType(string typeString)
        {
            type = typeString;
        }

        public string getAddress()
        {
            return address;
        }

        public void setAddress(string addressString)
        {
            address = addressString;
        }

        public void login(string username, string inheritedUsername, Permissions permissons)
        {
            this.setUsername(username);
        }

        public DateTime getCreationTime()
        {
            return this.creationTime;
        }

        public virtual void logout()
        {
            if (this.callerData != null)
            {
                this.callerData.cleanup();
            }
        }
    }
}