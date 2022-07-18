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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TH_LIFT != null)
            {
                TH_LIFT.Abort();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //edge.NavigationCompleted += Edge_NavigationCompleted;
            //edge.Source = new Uri("http://192.168.71.50");

            MM = new MessageManager(tbMessage, slView, 20, 99);
            DB = new DbAccess();
            TH_LIFT = new Thread(LifterAction);
            TH_LIFT.Start();
        }

        private void LifterAction()
        {
            if (DB.Open())
            {
                Dispatcher.Invoke(new Action(() => 
                {
                    MM.AddText("Data base open success", MessageType.Result);
                }));
                string[] codes = DB.GetAllRetriveLiftersWithType(LifterType.Retrive);
                if (codes != null)
                {
                    for (int i = 0; i < codes.Length; i++)
                    {
                        string carrierCode = DB.QueryCarriersAtLifter(CarrierStatus.Retrieving, codes[i]);
                        if (carrierCode != string.Empty)
                        {
                            Thread th1 = new Thread(OneLift);
                            th1.Start();
                        }
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
            else
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MM.AddText("Data base open fail", MessageType.Error);
                }));
            }
        }

        private void OneLift()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Dispatcher.Invoke(new Action(() =>
                {
                    MM.AddText("here is lifter 1", MessageType.Default);
                }));
            }
            
        }
        
    }
}
