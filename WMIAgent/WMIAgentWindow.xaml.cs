using System;
using System.ComponentModel;
using System.Windows;
using com.tibbo.aggregate.common.agent;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.device;

namespace WMIAgent
{
    /// <summary>
    /// Interaction logic for WmiAgentWindow.xaml
    /// </summary>
    public partial class WmiAgentWindow : Window
    {
        private WmiAgent agent = new WmiAgent();

        public WmiAgentWindow()
        {
            InitializeComponent();
            DataContext = agent;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            agent.start();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            agent.stop();
        }
    }
}