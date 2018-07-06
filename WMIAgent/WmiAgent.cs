using System;
using System.ComponentModel;
using com.tibbo.aggregate.common.agent;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.device;

namespace WMIAgent
{
    public class WmiAgent : INotifyPropertyChanged
    {
        public const String V_SETTINGS = "settings";

        private const String VF_SETTING_STRING = "string";

        private static readonly TableFormat VFT_SETTINGS = new TableFormat(1, 100,
                                                                             "<" + VF_SETTING_STRING +
                                                                             "><S><D=String Field>");


        private static DataTable settings = new DataTable(VFT_SETTINGS, true);

        public String stringValue
        {
            get { return settings.rec().getString(VF_SETTING_STRING); }
            set { settings.rec().setValue(VF_SETTING_STRING, value); }
        }

        private AgentController agentController;

        public WmiAgent()
        {
            var rls = new RemoteLinkServer(AggreGateNetworkDevice.DEFAULT_ADDRESS, Agent.DEFAULT_PORT,
                                           RemoteLinkServer.DEFAULT_USERNAME, RemoteLinkServer.DEFAULT_PASSWORD);

            var agent = new Agent(rls, "java", false);
            agentController = new AgentController(agent);

            initializeAgentContext(agent.getContext());
        }

        private void initializeAgentContext(Context context)
        {
            addSettingsVariable(context);
            new ModificationTimeVariable().addTo(context);
        }

        private void addSettingsVariable(Context context)
        {
            var vd = new VariableDefinition(V_SETTINGS, VFT_SETTINGS, true, true, "Just Settings",
                                            ContextUtils.GROUP_REMOTE);
            vd.setGetter(new DelegatedVariableGetter((con, def, caller, request) => settings));
            vd.setSetter(new DelegatedVariableSetter((con, def, caller, request, value) =>
                {
                settings = value;
                notifyPropertyChanged("stringValue");
                }));
            context.addVariableDefinition(vd);
        }

        private void notifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void start()
        {
            agentController.start();
        }

        public void stop()
        {
            agentController.stop();
        }
    }
}