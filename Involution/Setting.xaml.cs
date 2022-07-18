using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Involution
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Window
    {
        private List<AxisPage> AXES;

        public Setting()
        {
            InitializeComponent();

            AXES = new List<AxisPage>();
        }

        private void SetPanel_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void SetPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(LoadAxisData);
            thread.Start();
        }

        private void lvAxisPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxisPage ap = lvAxisPage.SelectedItem as AxisPage;
            if (ap != null)
                LoadNodeData();
        }

        private void LoadAxisData()
        {
            AxisCommon ac;
            AXES = XmlOperator.LoadPositionData(out ac);
            Dispatcher.Invoke(() => 
            {
                tbxBasicU.Text = ac.BasicUnit;
                tbxModelU.Text = ac.ModelUnit;
                tbxUserU.Text = ac.UserUnit;
                tbxSpeedU.Text = ac.SpeedUnit;
                cbBasicMul.SelectedIndex = AxisCommon.GetIndexByMultipleValue(ac.BasicMultiple);
                cbModelMul.SelectedIndex = AxisCommon.GetIndexByMultipleValue(ac.ModelMultiple);
                cbUserMul.SelectedIndex = AxisCommon.GetIndexByMultipleValue(ac.UserMultiple);
                cbSpeedMul.SelectedIndex = AxisCommon.GetIndexByMultipleValue(ac.SpeedMultiple);
            });
            RefreshAxisesList();
        }

        private void LoadNodeData()
        {
            Dispatcher.Invoke(() =>
            {
                lvNode.Items.Clear();
            });
            List<AxisNode> nodes = AXES[lvAxisPage.SelectedIndex].NODES;
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    Dispatcher.Invoke(() => { lvNode.Items.Add(nodes[i]); });
                }
            }
        }

        private void RefreshAxisesList()
        {
            Dispatcher.Invoke(() => 
            {
                lvAxisPage.Items.Clear();
                for (int i = 0; i < AXES.Count; i++)
                {
                    lvAxisPage.Items.Add(AXES[i]);
                }
            });
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            AxisPage ap = new AxisPage(0);
            AXES.Add(ap);
            Dispatcher.Invoke(() =>
            {
                lvAxisPage.Items.Add(ap);
            });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < lvAxisPage.Items.Count; i++)
            {
                AxisPage ap = lvAxisPage.Items[i] as AxisPage;
                AXES[i] = ap;
            }
            int result = XmlOperator.SavePositionData(AXES);
            if (result == 0)
            {
                Dispatcher.Invoke(() =>
                {
                    labMessage.Content = "Save success.";
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    labMessage.Content = "Save failed.";
                });
            }
            
        }

        private void btnBasic_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbAllBasicType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbAllModelType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbAllUserType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbAllSpeedType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
