using System;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.tibbo.aggregate.webservice;

namespace AggreGateTests
{
    [TestClass]
    public class WebServiceTests 
    {
        [TestMethod]
        public void TestWebService()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            const string linkServerHost = "localhost";
            const string userName = "admin";
            const string userPassword = "admin";

            Console.Out.WriteLine("Exploring Device Servers that are using Link Service for user: " + userName);

            const string webService = "https://" + linkServerHost + ":8443/ws/services/ServerWebService";

            var lsc = new LinkServerClient(userName, userPassword, webService);

            // Constructing path of Device Servers context
            const string deviceServersContext = "users." + userName + ".deviceservers";

            // Getting "status" variable that contains status of all Device Servers
            var res = lsc.get(deviceServersContext, "status");

            // Iterating over all records
            for (var i = 0; i < res.getRecordCount(); i++)
            {
                // Getting Device Server name
                var dsName = res.getRecord(i).getString("name");

                // Getting Device Server online status
                bool online = (bool)res.getRecord(i).getBoolean("online");

                Console.Out.WriteLine("Device Server #" + i + ": '" + userName + "." + dsName + "', " + (online ? "online" : "offline"));
            }

            lsc.callByStringArray("users.admin.queries", "delete", new[] { "dotnet"});

            lsc.callByStringArray("users.admin.queries", "create", new[] {"dotnet", "Created from .NET" , "0", "select * from users:list()"});
        }

        public class TrustAllCertificatePolicy : ICertificatePolicy
        {
            public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest req, int problem)
            {
                return true;
            }
        }
    }
}