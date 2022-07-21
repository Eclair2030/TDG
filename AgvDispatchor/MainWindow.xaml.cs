using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AgvDispatchor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpWebRequest REQUEST;
        HttpWebResponse RESPONSE;
        Thread TH_LIFT;                     //升降机控制线程
        Thread TH_CARRIER;             //搬送车控制线程
        Thread TH_ROBOT;                //作业车控制线程
        Thread TH_QUEUE;                //队列控制线程
        
        MessageManager MM;

        List<Lifter> LIFTERS;               //升降机列表
        List<Carrier> CARRIES;          //搬送车列表

        bool CARRIER_CIRCLE, LIFTER_CIRCLE;
        string SELECT_CARRIER_CODE;
        int SLEEP_TIME;

        public MainWindow()
        {
            InitializeComponent();
            LIFTERS = new List<Lifter>();
            CARRIES = new List<Carrier>();
            CARRIER_CIRCLE = LIFTER_CIRCLE = true;
            SLEEP_TIME = 5000;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TH_LIFT != null)
            {
                TH_LIFT.Abort();
            }
            CARRIER_CIRCLE = false;
            LIFTER_CIRCLE = false;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //edge.NavigationCompleted += Edge_NavigationCompleted;
            //edge.Source = new Uri("http://192.168.71.50");

            MM = new MessageManager(tbMessage, slView, 20, 99);
            
            TH_LIFT = new Thread(LifterAction);
            TH_LIFT.Start();
            TH_CARRIER = new Thread(AllCarrierAction);
            TH_CARRIER.Start();
        }

        public void ShowCallbackMessage(string msg, MessageType mt)
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>{ MM.AddText(msg, mt); }));
            }
            catch (Exception)
            {
            }
        }

        private void LifterAction()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                while (LIFTER_CIRCLE)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        lvLifters.Items.Clear();
                    }));
                    LIFTERS = DB.GetAllLiftersWithType(LifterType.None);
                    if (LIFTERS != null && LIFTERS.Count > 0)
                    {
                        for (int i = 0; i < LIFTERS.Count; i++)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                ListViewItem item = new ListViewItem();
                                item.Content = LIFTERS[i];
                                lvLifters.Items.Add(item);
                            }));
                            LIFTERS[i].Message = ShowCallbackMessage;
                            Thread thOneLift;
                            if (LIFTERS[i].Type == ((int)LifterType.Retrive).ToString())
                            {
                                thOneLift = new Thread(LIFTERS[i].RetriveLifterAction);
                            }
                            else
                            {
                                thOneLift = new Thread(LIFTERS[i].SupplyLifterAction);
                            }
                            thOneLift.Start(LIFTERS[i]);
                        }
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            MM.AddText("there is no lifter exist", MessageType.Exception);
                        }));
                    }
                    Thread.Sleep(SLEEP_TIME);
                }
            }
            else
            {
                ShowCallbackMessage("Lifter action data base open fail", MessageType.Error);
            }
            DB.Close();
        }

        private void AllCarrierAction()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                while (CARRIER_CIRCLE)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        lvCarriers.Items.Clear();
                    }));
                    CARRIES = DB.GetAllCarriers();
                    if (CARRIES != null && CARRIES.Count > 0)
                    {
                        for (int i = 0; i < CARRIES.Count; i++)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                ListViewItem item = new ListViewItem();
                                item.Content = CARRIES[i];
                                lvCarriers.Items.Add(item);
                            }));
                            CARRIES[i].Message = ShowCallbackMessage;
                            Thread thOneCarrier = new Thread(CARRIES[i].CarrierAction);
                            thOneCarrier.Start(CARRIES[i]);
                        }
                        RefreshMaterialsOnCarrier();
                    }
                    else
                    {
                        ShowCallbackMessage("thers is no Carriers exist", MessageType.Exception);
                    }
                    Thread.Sleep(SLEEP_TIME);
                }
            }
            else
            {
                ShowCallbackMessage("Carrier action data base open fail", MessageType.Error);
            }
            DB.Close();
        }

        private void RefreshMaterialsOnCarrier()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                try
                {
                    Dispatcher.Invoke(new Action(() => { labSelectCarrierCode.Content = "Carrier Code: " + SELECT_CARRIER_CODE; }));
                    for (int i = 1; i < 37; i++)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            Label lab = FindName("labIndex" + i) as Label;
                            if (lab != null)
                            {
                                lab.Background = Brushes.White;
                                lab.Content = "Empty";
                            }
                        }));
                    }
                    if (SELECT_CARRIER_CODE != null && SELECT_CARRIER_CODE != string.Empty)
                    {
                        List<Material> materials = DB.GetMaterialsByCarrierCode(SELECT_CARRIER_CODE);
                        if (materials != null)
                        {
                            for (int i = 0; i < materials.Count; i++)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    string index = materials[i].CarrierIndex;
                                    Label lab = FindName("labIndex" + index) as Label;
                                    if (lab != null)
                                    {
                                        lab.Background = Brushes.Lime;
                                        lab.Content = "Device :" + materials[i].TargetDeviceCode + " , Index: " + materials[i].TargetDeviceIndex;
                                    }
                                }));
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                ShowCallbackMessage("Refresh material list data base open fail", MessageType.Error);
            }
            DB.Close();
            
        }

        private void lvCarriers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem item = (ListViewItem)lvCarriers.SelectedValue;
            if (item != null)
            {
                SELECT_CARRIER_CODE = ((Carrier)item.Content).Code;
            }
            RefreshMaterialsOnCarrier();
        }

        private void lvLifters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
