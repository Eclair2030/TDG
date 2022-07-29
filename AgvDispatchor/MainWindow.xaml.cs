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
        Thread TH_LIFT;                                //升降机控制线程
        Thread TH_CARRIER;                       //搬送车控制线程
        Thread TH_ROBOT;                          //作业车控制线程
        Thread TH_QUEUE_SUPPLY;           //来料升降机队列控制线程
        Thread TH_QUEUE_RETRIVE;          //回收升降机队列控制线程
        Thread TH_QUEUE_BATTERY;        //充电队列控制线程
        Thread TH_REQUEST;                      //物料请求线程
        Thread TH_ROLLBACK;                   //料车回收线程
        
        MessageManager MM;                  //界面textblock文本显示类
        StoreIO IO;                                         //升降机与仓库供料系统通讯对象

        List<Lifter> LIFTERS;                       //升降机列表
        List<Carrier> CARRIES;                  //搬送车列表

        bool CARRIER_CIRCLE, ROBOT_CIRCLE, LIFTER_CIRCLE, REQUEST_CIRCLE, QUEUE_CIRCLE, BATTERY_CIRCLE;
        string SELECT_CARRIER_CODE, SELECT_ROBOT_CODE, SELECT_LIFTER_CODE;
        int SLEEP_TIME;
        int SHELF_POSITIONS;                    //一个料车上的料总数

        public MainWindow()
        {
            InitializeComponent();
            LIFTERS = new List<Lifter>();
            CARRIES = new List<Carrier>();
            CARRIER_CIRCLE = ROBOT_CIRCLE = LIFTER_CIRCLE = REQUEST_CIRCLE = QUEUE_CIRCLE = BATTERY_CIRCLE = true;
            SLEEP_TIME = 5000;
            SHELF_POSITIONS = 36;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TH_LIFT != null)
            {
                TH_LIFT.Abort();
            }
            CARRIER_CIRCLE = false;
            ROBOT_CIRCLE = false;
            LIFTER_CIRCLE = false;
            REQUEST_CIRCLE = false;
            QUEUE_CIRCLE = false;
            BATTERY_CIRCLE = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //edge.NavigationCompleted += Edge_NavigationCompleted;
            //edge.Source = new Uri("http://192.168.71.50");

            MM = new MessageManager(tbMessage, slView, 20, 99);
            IO = new StoreIO();

            TH_LIFT = new Thread(LifterAction);
            TH_LIFT.Start();
            TH_CARRIER = new Thread(AllCarrierAction);
            TH_CARRIER.Start();
            TH_ROBOT = new Thread(AllRobotAction);
            TH_ROBOT.Start();
            TH_QUEUE_BATTERY = new Thread(ChargeQueueControl);
            TH_QUEUE_BATTERY.Start();
            TH_REQUEST = new Thread(RequestJudge);
            TH_REQUEST.Start();
            TH_ROLLBACK = new Thread(ShelfRollback);
            TH_ROLLBACK.Start();
            TH_QUEUE_SUPPLY = new Thread(SupplyQueueControl);
            TH_QUEUE_SUPPLY.Start();
            TH_QUEUE_RETRIVE = new Thread(RetriveQueueControl);
            TH_QUEUE_RETRIVE.Start();
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
                        RefreshLifterQueues();
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
                    try
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
                    }
                    catch (Exception)
                    {
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

        private void AllRobotAction()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                while (ROBOT_CIRCLE)
                {
                    try
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            lvRobots.Items.Clear();
                        }));
                        List<Robot> robots = DB.GetAllRobots();
                        if (robots != null)
                        {
                            for (int i = 0; i < robots.Count; i++)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    ListViewItem item = new ListViewItem();
                                    item.Content = robots[i];
                                    lvRobots.Items.Add(item);
                                }));
                            }
                            RefreshRobotDetails();
                        }
                        else
                        {
                            ShowCallbackMessage("thers is no Robots exist", MessageType.Exception);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    Thread.Sleep(SLEEP_TIME);
                }
            }
            else
            {
                ShowCallbackMessage("Robots action data base open fail", MessageType.Error);
            }
            DB.Close();
        }

        private void ChargeQueueControl()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                while (BATTERY_CIRCLE)
                {
                    try
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                        }));
                        List<Carrier> carriers = DB.FindLowPowerCarriers();
                        if (carriers != null)
                        {
                            for(int i = 0;i < carriers.Count;i++)
                            {
                                //Carrier去充电
                                if (DB.AddAGVToChargeQueue(carriers[i].Code) && DB.SetCarrierStatus(carriers[i].Code, CarrierStatus.Charge))
                                {
                                    ShowCallbackMessage(carriers[i].Code + " add to charge queue", MessageType.Result);
                                }
                                else
                                {
                                    ShowCallbackMessage(carriers[i].Code + " add to charge queue fail", MessageType.Error);
                                }
                            }
                        }
                        if (DB.ChargeQueueAutoCheck())
                        {
                            
                        }
                        else
                        {
                            ShowCallbackMessage("charge queue auto forward fail", MessageType.Error);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    Thread.Sleep(SLEEP_TIME);
                }
            }
            else
            {
                ShowCallbackMessage("charge queue control data base open fail", MessageType.Error);
            }
            DB.Close();
        }

        private void RequestJudge()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                try
                {
                    while (REQUEST_CIRCLE)
                    {
                        if (DB.ExistMaterialRequests() > 0)
                        {
                            List<Lifter> list = DB.GetLiftersWithStatus(LifterType.Supply, (int)SupplyLifterStatus.Avoid);
                            if (list != null)
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (true)   //用Sensor判断升降机下是否有小车就位
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        List<CarrierInLifterQueue> queue = DB.GetLifterQueue(list[i].Code);
                                        if (queue != null)
                                        {
                                            string code = string.Empty;
                                            for (int j = 0; j < queue.Count; j++)
                                            {
                                                if (Convert.ToInt32(queue[j].Number) == 0)
                                                {
                                                    code = queue[j].Code;
                                                    break;
                                                }
                                            }
                                            if (code != string.Empty)
                                            {
                                                for (int k = 0; k < SHELF_POSITIONS; k++)
                                                {
                                                    if (DB.AssignMaterialsToLifter(list[i].Code, k+1))
                                                    {
                                                        if (DB.MakeResponse())
                                                        {
                                                        }
                                                        else
                                                        {
                                                            ShowCallbackMessage("Update request to make response fail", MessageType.Error);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ShowCallbackMessage("Assign Materials to " + list[i].Code + " fail", MessageType.Error);
                                                    }
                                                }
                                                
                                                //排0号搬送车去往升降机
                                                //到达升降机后 DB.SetCarrierStatus(code, CarrierStatus.Initing);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Thread.Sleep(SLEEP_TIME);
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                ShowCallbackMessage("Material request system data base open fail", MessageType.Error);
            }
            DB.Close();
        }

        private void ShelfRollback()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                try
                {
                    while (REQUEST_CIRCLE)
                    {
                        List<Lifter> lifters = DB.GetLiftersWithStatus(LifterType.Retrive, (int)SupplyLifterStatus.Load);
                        if (lifters != null)
                        {
                            for (int i = 0; i < lifters.Count; i++)
                            {
                                if (true)   //Sensor判断升降机是否有搬送车就位
                                {
                                    continue;
                                }
                                else
                                {
                                    List<CarrierInLifterQueue> queue = DB.GetLifterQueue(lifters[i].Code);
                                    if (queue != null)
                                    {
                                        string code = string.Empty;
                                        for (int j = 0; j < queue.Count; j++)
                                        {
                                            if (Convert.ToInt32(queue[j].Number) == 0)
                                            {
                                                code = queue[j].Code;
                                                break;
                                            }
                                        }
                                        if (code != string.Empty)
                                        {
                                            //排0号搬送车去往升降机
                                            //到达升降机后 DB.SetCarrierStatus(code, CarrierStatus.Retrieving);
                                        }
                                    }
                                }
                            }
                        }
                        Thread.Sleep(SLEEP_TIME);
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                ShowCallbackMessage("Shelf roll back system data base open fail", MessageType.Error);
            }
            DB.Close();
        }

        private void SupplyQueueControl()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                while (QUEUE_CIRCLE)
                {
                    if (LIFTERS != null)
                    {
                        for (int i = 0; i < LIFTERS.Count; i++)
                        {
                            if (DB.LifterQueueAutoCheck(LIFTERS[i].Code))
                            {
                                ShowCallbackMessage("queue for " + LIFTERS[i].Code + " 1 step forward", MessageType.Default);
                            }
                            List<Carrier> carriers = DB.GetCarriersByStatus(CarrierStatus.Ready);
                            if (carriers != null && carriers.Count > 0)
                            {
                                if (DB.AddCarrierToShortestQueue(LifterType.Supply, carriers[0].Code))
                                {
                                    //小车去队列排队
                                    DB.SetCarrierStatus(carriers[0].Code, CarrierStatus.Init);
                                }
                                else
                                {
                                    ShowCallbackMessage("Queue control add carrier to queue fail", MessageType.Error);
                                }
                            }
                        }
                    }
                    Thread.Sleep(SLEEP_TIME);
                }
            }
            else
            {
                ShowCallbackMessage("Queue control data base open fail", MessageType.Error);
            }
            DB.Close();
        }

        private void RetriveQueueControl()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                while (QUEUE_CIRCLE)
                {
                    if (LIFTERS != null)
                    {
                        for (int i = 0; i < LIFTERS.Count; i++)
                        {
                            if (DB.LifterQueueAutoCheck(LIFTERS[i].Code))
                            {
                                ShowCallbackMessage("queue for " + LIFTERS[i].Code + " 1 step forward", MessageType.Default);
                            }
                            List<Carrier> carriers = DB.GetCarriersByStatus(CarrierStatus.Complete);
                            if (carriers != null && carriers.Count > 0)
                            {
                                if (DB.AddCarrierToShortestQueue(LifterType.Retrive, carriers[0].Code))
                                {
                                    //小车去队列排队
                                    DB.SetCarrierStatus(carriers[0].Code, CarrierStatus.Retrieve);
                                }
                                else
                                {
                                    ShowCallbackMessage("Queue control add carrier to queue fail", MessageType.Error);
                                }
                            }
                        }
                    }
                    Thread.Sleep(SLEEP_TIME);
                }
            }
            else
            {
                ShowCallbackMessage("Queue control data base open fail", MessageType.Error);
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

        private void RefreshLifterQueues()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                try
                {
                    Dispatcher.Invoke(new Action(() => 
                    { 
                        labSelectLifterCode.Content = "Lifter Code:" + SELECT_LIFTER_CODE;
                        lvLifterQueue.Items.Clear();
                    }));
                    if (SELECT_LIFTER_CODE != null && SELECT_LIFTER_CODE != string.Empty)
                    {
                        List<CarrierInLifterQueue> queue = DB.GetLifterQueue(SELECT_LIFTER_CODE);
                        if (queue != null)
                        {
                            for (int i = 0; i < queue.Count; i++)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    ListViewItem item = new ListViewItem();
                                    item.Content = queue[i];
                                    lvLifterQueue.Items.Add(item);
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
                ShowCallbackMessage("Refresh lifter: " + SELECT_LIFTER_CODE + " queue data base open fail", MessageType.Error);
            }
            DB.Close();

        }

        private void RefreshRobotDetails()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        labSelectRobotCode.Content = "Robot Code:" + SELECT_ROBOT_CODE;
                    }));
                }
                catch (Exception)
                {
                }
            }
            else
            {
                ShowCallbackMessage("Refresh Robot: " + SELECT_ROBOT_CODE + " queue data base open fail", MessageType.Error);
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
            ListViewItem item = (ListViewItem)lvLifters.SelectedValue;
            if (item != null)
            {
                SELECT_LIFTER_CODE = ((Lifter)item.Content).Code;
            }
            RefreshLifterQueues();
        }

        private void lvRobots_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem item = (ListViewItem)lvRobots.SelectedValue;
            if (item != null)
            {
                SELECT_ROBOT_CODE = ((Robot)item.Content).Code;
            }
            RefreshRobotDetails();
        }

        private void btnTurn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbDevice.Text != null && cbDevice.Text != string.Empty)
                {
                    DbAccess DB = new DbAccess();
                    if (DB.Open() && DB.SetRequestOnByDeviceID(cbDevice.Text))
                    {
                        MM.AddText("Device: "+ cbDevice.Text + " shelf turn arround complete, refresh requests", MessageType.Result);
                    }
                    DB.Close();
                }
            }
            catch (Exception)
            {
                MM.AddText("Device: " + cbDevice.Text + " shelf turn arround unsuccessful", MessageType.Error);
            }
        }
    }
}
