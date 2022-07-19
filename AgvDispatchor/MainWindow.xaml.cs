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
        DbAccess DB;
        MessageManager MM;

        List<Lifter> LIFTERS;               //升降机列表
        List<Carrier> CARRIES;          //搬送车列表

        bool CARRIER_CIRCLE;
        string SELECT_CARRIER_CODE;
        int SLEEP_TIME;

        public MainWindow()
        {
            InitializeComponent();
            LIFTERS = new List<Lifter>();
            CARRIES = new List<Carrier>();
            CARRIER_CIRCLE = true;
            SLEEP_TIME = 5000;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TH_LIFT != null)
            {
                TH_LIFT.Abort();
            }
            CARRIER_CIRCLE = false;
            DB.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //edge.NavigationCompleted += Edge_NavigationCompleted;
            //edge.Source = new Uri("http://192.168.71.50");

            MM = new MessageManager(tbMessage, slView, 20, 99);
            DB = new DbAccess();
            if (DB.Open())
            {
                ShowCallbackMessage("Data base open success", MessageType.Result);
            }
            else
            {
                ShowCallbackMessage("Data base open fail", MessageType.Error);
            }
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
            LIFTERS = DB.GetAllRetriveLiftersWithType(LifterType.None);
            if (LIFTERS.Count > 0)
            {
                for (int i = 0; i < LIFTERS.Count; i++)
                {
                    Thread thOneLift;
                    if (LIFTERS[i].Type == ((int)LifterType.Retrive).ToString())
                    {
                        thOneLift = new Thread(OneRetriveLift);
                    }
                    else
                    {
                        thOneLift = new Thread(OneRetriveLift);
                    }
                    thOneLift.Start(LIFTERS[i]);
                }
            }
            else
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MM.AddText("thers is no retrive lifter exist", MessageType.Exception);
                }));
            }
        }

        private void OneRetriveLift(object code)
        {
            Lifter lifter = code as Lifter;
            while (true)
            {
                string strResult = DB.ExistCarriersAtLifter(lifter.Code);
                if (strResult != null && strResult != string.Empty)
                {
                    if (lifter.Status == ((int)LifterStatus.Idle).ToString())
                    {
                        if (DB.SetLifterStatus(LifterStatus.Fall, lifter.Code))             //还要加上执行升降机下降操作的代码
                        {

                        }
                        else
                        {
                            Dispatcher.Invoke(new Action(() =>{ MM.AddText(lifter.Code + " action fail at" + LifterStatus.Fall, MessageType.Error); }));
                            Thread.Sleep(SLEEP_TIME);
                        }
                    }
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        MM.AddText("there is no carrier at lifter right now", MessageType.Default);
                    }));
                    Thread.Sleep(SLEEP_TIME);
                }
            }
            
            
        }

        private void AllCarrierAction()
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

        private void RefreshMaterialsOnCarrier()
        {
            Dispatcher.Invoke(new Action(() =>{ labSelectCarrierCode.Content = "Carrier Code: " + SELECT_CARRIER_CODE; }));
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
                for (int i = 0; i < materials.Count; i++)
                {
                    Dispatcher.Invoke(new Action(() => 
                    {
                        string index = materials[i].CarrierIndex;
                        Label lab = FindName("labIndex" + index) as Label;
                        if (lab != null)
                        {
                            lab.Background = Brushes.Lime;
                            lab.Content = "Device :" + materials[i].TargetDeviceCode + " , Device Index: " + materials[i].TargetDeviceIndex;
                        }
                    }));
                }
            }
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
    }
}
