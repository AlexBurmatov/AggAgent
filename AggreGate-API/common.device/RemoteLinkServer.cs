using System;

namespace com.tibbo.aggregate.common.device
{
    public class RemoteLinkServer : AggreGateNetworkDevice
    {
        public const int DEFAULT_PORT = 6460;
        public const string DEFAULT_USERNAME = "admin";
        public const string DEFAULT_PASSWORD = "admin";

        protected String username;
        protected String password;

        static RemoteLinkServer()
        {
            type = Cres.get().getString("ls");
            id = "linkserver";
        }

        public RemoteLinkServer()
        {
            setPort(DEFAULT_PORT);
        }

        public RemoteLinkServer(String address, int port, String username, String password) : base(address, port)
        {
            this.username = username;
            this.password = password;
        }

        public String getPassword()
        {
            return password;
        }

        public String getUsername()
        {
            return username;
        }

        public void setUsername(String usernameString)
        {
            username = usernameString;
        }

        public void setPassword(String passwordString)
        {
            password = passwordString;
        }

        public override String getInfo()
        {
            return base.getInfo() +
                   (username != null ? ", " + Cres.get().getString("devUserColon") + " " + username : "");
        }
    }
}