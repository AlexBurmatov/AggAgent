using System;
using System.Threading;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.device;
using com.tibbo.aggregate.common.server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AggreGateTests
{
    [TestClass]
    public class ClientTests
    {
        private RemoteLinkServer server;
        private RemoteLinkServerController controller;
        private RemoteDeviceContextManager<AggreGateDevice> contextManager;

        private const String DEVICE_NAME = "dotNet";
        private const String ALERT_NAME = "dotNet";
        private const String DESCRIPTION = "Created from .NET";

        [TestInitialize]
        public void SetUp()
        {
            this.server = new RemoteLinkServer(
                AggreGateNetworkDevice.DEFAULT_ADDRESS,
                RemoteLinkServer.DEFAULT_PORT,
                RemoteLinkServer.DEFAULT_USERNAME,
                RemoteLinkServer.DEFAULT_PASSWORD);

            this.controller = new RemoteLinkServerController(this.server, false);
            this.controller.connect();
            this.controller.login();

            Thread.Sleep(500);

            this.contextManager = this.controller.getContextManager();
        }

        [TestCleanup]
        public void TearDown()
        {
            var dc = this.contextManager.get("users.admin.devices");
            dc.callFunction("delete", DEVICE_NAME);

            dc = this.contextManager.get("users.admin.alerts");
            dc.callFunction("delete", ALERT_NAME);

            this.controller.disconnect();
        }

        [TestMethod]
        public void TestGettingVersion()
        {
            var serverVersion = this.contextManager.getRoot().getVariable(RootContextConstants.V_VERSION).rec().getString(RootContextConstants.VF_VERSION_VERSION);
            Assert.IsTrue(serverVersion.StartsWith("5"));
        }

        [TestMethod]
        public void TestCreatingDevice()
        {
            var dc = this.createDevice();
            var device = dc.getChild(DEVICE_NAME);
            var actualResult = device.getName();
            Assert.IsTrue(actualResult.Equals(DEVICE_NAME));
        }

        [TestMethod]
        public void TestCreatingAlert()
        {
            var dc = this.createAlert();
            var result = dc.getChild(ALERT_NAME);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMain()
        {
            var triggered = false;
            var devices = this.contextManager.get("users.admin.devices");
            devices.callFunction("add", "com.tibbo.linkserver.plugin.device.virtual", DEVICE_NAME, DESCRIPTION);

            var alerts = contextManager.get("users.admin.alerts");
            alerts.callFunction("create", ALERT_NAME, DESCRIPTION);

            var device = devices.getChild(DEVICE_NAME);

            var alert = alerts.getChild(ALERT_NAME);
            var triggers = alert.getVariable("variableTriggers");
            var record = triggers.addRecord();
            record.setValue("mask", "users.admin.devices" + "." + DEVICE_NAME);
            record.setValue("variable", "boolean");
            record.setValue("expression", "{boolean} == true");
            record.setValue("period", 1L);
            record.setValue("delay", 1L);
            alert.setVariable("variableTriggers", triggers);

            alert.addEventListener("alert", new DefaultContextEventListener<CallerController<CallerData>>(ev => triggered = true));

            var s = new Semaphore(0, 1);

            device.addEventListener("contextStatusChanged", new DefaultContextEventListener<CallerController<CallerData>>(
                    ev =>
                        {
                            if (device.getVariableDefinition("boolean") != null)
                            {
                                s.Release();
                            }
                        }));

            s.WaitOne(2000);

            var variable = device.getVariable("boolean");
            variable.rec().setValue("boolean", true);
            device.setVariable("boolean", variable);

            for (var i = 0; i < 50; i++)
            {
                Thread.Sleep(100);
                if (triggered)
                {
                    break;
                }
            }

            Assert.IsTrue(triggered);
        }

        private Context createDevice()
        {
            var dc = this.contextManager.get("users.admin.devices");
            dc.callFunction("add", "com.tibbo.linkserver.plugin.device.virtual", DEVICE_NAME, DESCRIPTION);
            return dc;
        }

        private Context createAlert()
        {
            var dc = this.contextManager.get("users.admin.alerts");
            dc.callFunction("create", ALERT_NAME, DESCRIPTION);
            return dc;
        }
    }
}