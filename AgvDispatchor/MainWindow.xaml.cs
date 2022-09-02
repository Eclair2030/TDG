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
    public delegate void SendMessage(string msg, MessageType mt);
    public delegate void RenderImage(int width, int height, int dpiX, int dpiY, PixelFormat pf, IntPtr data, int size, int stride);
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        FmsAction FMS;
        Thread TH_FIXED;                            //固定部件控制线程
        Thread TH_CARRIER;                       //搬送车控制线程
        Thread TH_ROBOT;                          //作业车控制线程

        MessageManager MM;                  //界面textblock文本显示类
        StoreIO IO;                                         //升降机与仓库供料系统通讯对象

        public static List<Lifter> LIFTERS;                       //升降机列表
        List<Carrier> CARRIES;                                      //搬送车列表
        List<Robot> ROBOTS;                                         //作业车列表
        public static List<Battery> BATTERYS;           //充电桩列表

        bool CARRIER_CIRCLE, ROBOT_CIRCLE, FIXED_CIRCLE, JOG_CIRCLE;
        string SELECT_CARRIER_CODE, SELECT_ROBOT_CODE, SELECT_LIFTER_CODE;
        int SLEEP_TIME;

        public MainWindow()
        {
            InitializeComponent();
            LIFTERS = new List<Lifter>();
            CARRIES = new List<Carrier>();
            CARRIER_CIRCLE = ROBOT_CIRCLE = FIXED_CIRCLE = true;
            JOG_CIRCLE = false;
            SLEEP_TIME = 5000;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CARRIER_CIRCLE = false;
            ROBOT_CIRCLE = false;
            FIXED_CIRCLE = false;
            JOG_CIRCLE = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //edge.NavigationCompleted += Edge_NavigationCompleted;
            //edge.Source = new Uri("http://192.168.71.50");

            MM = new MessageManager(tbMessage, slView, 20, 99);
            IO = new StoreIO();
            FMS = new FmsAction(ShowCallbackMessage);

            TH_FIXED = new Thread(FixedAction);
            TH_FIXED.Start();
            TH_CARRIER = new Thread(AllCarrierAction);
            TH_CARRIER.Start();
            //TH_ROBOT = new Thread(AllRobotAction);
            //TH_ROBOT.Start();
        }

        public void ShowCallbackMessage(string msg, MessageType mt)
        {
            try
            {
                Dispatcher.Invoke(new Action(() => { MM.AddText(msg, mt); }));
            }
            catch (Exception)
            {
            }
        }

        private void FixedAction()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                while (FIXED_CIRCLE)
                {
                    try
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
                                LIFTERS[i].Fms = FMS;
                                Thread thOneLift;
                                if (LIFTERS[i].Type == (int)LifterType.Retrive)
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
                            AllBatteryAction(DB);
                            LifterQueueControl(DB);
                            ChargeQueueControl(DB);
                        }
                        else
                        {
                            ShowCallbackMessage("there is no lifter exist", MessageType.Exception);
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
                ROBOTS = DB.GetAllRobots();
                for (int i = 0; i < ROBOTS.Count; i++)
                {
                    ROBOTS[i].Message = ShowCallbackMessage;
                    if (!ROBOTS[i].ArmInit())
                    {
                    }
                    if (ROBOTS[i].CameraInit(ShowCameraImage))
                    {
                    }
                }
                while (ROBOT_CIRCLE)
                {
                    try
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            lvRobots.Items.Clear();
                        }));
                        if (DB.RefreshAllRobots(ref ROBOTS) && ROBOTS != null)
                        {
                            for (int i = 0; i < ROBOTS.Count; i++)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    ListViewItem item = new ListViewItem();
                                    item.Content = ROBOTS[i];
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
                if (ROBOTS != null)
                {
                    for (int i = 0; i < ROBOTS.Count; i++)
                    {
                        ROBOTS[i].CameraClose();
                    }
                }
            }
            else
            {
                ShowCallbackMessage("Robots action data base open fail", MessageType.Error);
            }
            DB.Close();
        }

        private void AllBatteryAction(DbAccess DB)
        {
            BATTERYS = DB.GetAllBatterys();
            if (BATTERYS != null && BATTERYS.Count > 0)
            {
                for (int i = 0; i < BATTERYS.Count; i++)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        Rectangle rect = (Rectangle)FindName("rtBattery" + (i + 1));
                        if (rect != null)
                        {
                            if (BATTERYS[i].Status == (int)BatteryStatus.Idle)
                            {
                                rect.Fill = Brushes.Lime;
                            }
                            else
                            {
                                rect.Fill = Brushes.Red;
                            }
                        }
                    }));
                }
            }
            else
            {
                ShowCallbackMessage("thers is no Battery exist", MessageType.Exception);
            }
        }

        private void ChargeQueueControl(DbAccess DB)
        {
            string firstAgv = DB.ChargeQueueAutoCheck();
            if (firstAgv == null)
            {
                ShowCallbackMessage("charge queue auto forward fail", MessageType.Error);
            }
            else if (firstAgv == string.Empty)
            {
                //ShowCallbackMessage("charge queue auto 1 step forward", MessageType.Default);
            }
            else
            {
                for (int i = 0; i < BATTERYS.Count; i++)
                {
                    if (BATTERYS[i].Status == (int)BatteryStatus.Idle)
                    {
                        if (FMS.GetAgvInfo(firstAgv) == AgvState.IDLE.ToString())
                        {
                            if (FMS.AgvMove(firstAgv, BATTERYS[i].Position) == FmsActionResult.Success)
                            {
                                if (DB.SetCarrierStatus(firstAgv, CarrierStatus.Charging))
                                {
                                    if (DB.ChargeQueueDeleteZero())
                                    {
                                    }
                                    else
                                    {
                                        ShowCallbackMessage("Remove first agv from charge queue fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                    ShowCallbackMessage("Set agv: " + firstAgv + " status to charging fail", MessageType.Error);
                                }
                            }
                            else
                            {
                                ShowCallbackMessage("Agv: " + firstAgv + " move to battery: " + BATTERYS[i].Code + " fail", MessageType.Error);
                            }
                        }
                    }
                }
            }
        }

        private void LifterQueueControl(DbAccess DB)
        {
            if (LIFTERS != null)
            {
                for (int i = 0; i < LIFTERS.Count; i++)
                {
                    string firstCarr = DB.LifterQueueAutoCheck(LIFTERS[i].Code);
                    if (firstCarr == null)
                    {
                        //ShowCallbackMessage("queue for " + LIFTERS[i].Code + " auto forward fail or queue is empty", MessageType.Default);
                    }
                    else if (firstCarr == string.Empty)
                    {
                        ShowCallbackMessage("queue for " + LIFTERS[i].Code + " 1 step forward", MessageType.Default);
                    }
                    else
                    {
                        if ( (LIFTERS[i].Type == (int)LifterType.Supply && LIFTERS[i].Status == (int)SupplyLifterStatus.Avoid) ||
                            (LIFTERS[i].Type == (int)LifterType.Retrive && LIFTERS[i].Status == (int)RetriveLifterStatus.Load) )
                        {
                            if (FMS.AgvMove(firstCarr, LIFTERS[i].Position) == FmsActionResult.Success)
                            {
                                if (LIFTERS[i].Type == (int)LifterType.Supply)
                                {
                                    if (DB.SetCarrierStatus(firstCarr, CarrierStatus.Initing))
                                    { }
                                    else
                                    {
                                        ShowCallbackMessage("Carrier: " + firstCarr + " status set to initing fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                    if (DB.SetCarrierStatus(firstCarr, CarrierStatus.Retrieving))
                                    { }
                                    else
                                    {
                                        ShowCallbackMessage("Carrier: " + firstCarr + " status set to retrieving fail", MessageType.Error);
                                    }
                                }
                            }
                        }
                        else
                        {
                        }
                    }
                }
            }
        }

        private void RefreshMaterialsOnCarrier()
        {
            DbAccess DB = new DbAccess();
            if (DB.Open())
            {
                try
                {
                    Dispatcher.Invoke(new Action(() => { labSelectCarrierCode.Content = "Carrier Code: " + SELECT_CARRIER_CODE; }));
                    for (int i = 0; i < 36; i++)
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
                                        lab.Content = "Dev: " + materials[i].TargetDeviceCode + ", Area: " + materials[i].TargetDeviceArea + ", Index: " + materials[i].TargetDeviceIndex;
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
                    for (int i = 0; i < ROBOTS.Count; i++)
                    {
                        if (ROBOTS[i].Code == SELECT_ROBOT_CODE)
                        {
                            RobotPoint pos = ROBOTS[i].ReadActPos();
                            if (pos != null)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    labXpos.Content = pos.X.ToString();
                                    labYpos.Content = pos.Y.ToString();
                                    labZpos.Content = pos.Z.ToString();
                                    labRXpos.Content = pos.RX.ToString();
                                    labRYpos.Content = pos.RY.ToString();
                                    labRZpos.Content = pos.RZ.ToString();
                                    labJ1.Content = pos.J1.ToString();
                                    labJ2.Content = pos.J2.ToString();
                                    labJ3.Content = pos.J3.ToString();
                                    labJ4.Content = pos.J4.ToString();
                                    labJ5.Content = pos.J5.ToString();
                                    labJ6.Content = pos.J6.ToString();
                                }));
                            }
                            slSpeed.Value = Convert.ToDouble(ROBOTS[i].ReadOverride()) * 100;
                            break;
                        }
                    }
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

        private void ShowCameraImage(int width, int height, int dpiX, int dpiY, PixelFormat pf, IntPtr data, int size, int stride)
        {
            Dispatcher.Invoke(() =>
            {
                imgCamera.Source = BitmapSource.Create(width, height, dpiX, dpiY, pf, null, data, size, stride);
            });
        }

        private void KeepMove()
        {
            while (JOG_CIRCLE)
            {
                for (int i = 0; i < ROBOTS.Count; i++)
                {
                    if (ROBOTS[i].Code == SELECT_ROBOT_CODE)
                    {
                        if (ROBOTS[i].LongMoveEvent())
                        {
                        }
                        break;
                    }
                }
                Thread.Sleep(400);
            }
            ShowCallbackMessage("stop pressed", MessageType.Default);
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

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ROBOTS != null && ROBOTS.Count > 0)
                {
                    ROBOTS[0].Reset();
                }
            }
            catch (Exception ex)
            {
                ShowCallbackMessage(ex.Message, MessageType.Exception);
            }
        }

        private void btnPCS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ROBOTS != null && ROBOTS.Count > 0)
                {
                    double x = Convert.ToDouble(tbxXpos.Text.Trim());
                    double y = Convert.ToDouble(tbxYpos.Text.Trim());
                    double z = Convert.ToDouble(tbxZpos.Text.Trim());
                    double rx = Convert.ToDouble(tbxRXpos.Text.Trim());
                    double ry = Convert.ToDouble(tbxRYpos.Text.Trim());
                    double rz = Convert.ToDouble(tbxRZpos.Text.Trim());
                    ROBOTS[0].PCS2ACS(x, y, z, rx, ry, rz);
                }
            }
            catch (Exception ex)
            {
                ShowCallbackMessage(ex.Message, MessageType.Exception);
            }
        }

        private void btnGoL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ROBOTS != null && ROBOTS.Count > 0)
                {
                    RobotPoint rp = new RobotPoint();
                    rp.X = Convert.ToDouble(tbxXpos.Text.Trim());
                    rp.Y = Convert.ToDouble(tbxYpos.Text.Trim());
                    rp.Z = Convert.ToDouble(tbxZpos.Text.Trim());
                    rp.RX = Convert.ToDouble(tbxRXpos.Text.Trim());
                    rp.RY = Convert.ToDouble(tbxRYpos.Text.Trim());
                    rp.RZ = Convert.ToDouble(tbxRZpos.Text.Trim());
                    if (ROBOTS[0].MoveL(rp))
                    {
                        ShowCallbackMessage("MoveL has reached the target position", MessageType.Result);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowCallbackMessage(ex.Message, MessageType.Exception);
            }
        }

        private void btnGoJ_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ROBOTS != null && ROBOTS.Count > 0)
                {
                    double j1 = Convert.ToDouble(tbxJ1.Text.Trim());
                    double j2 = Convert.ToDouble(tbxJ2.Text.Trim());
                    double j3 = Convert.ToDouble(tbxJ3.Text.Trim());
                    double j4 = Convert.ToDouble(tbxJ4.Text.Trim());
                    double j5 = Convert.ToDouble(tbxJ5.Text.Trim());
                    double j6 = Convert.ToDouble(tbxJ6.Text.Trim());
                    ROBOTS[0].MoveJ(j1, j2, j3, j4, j5, j6);
                }
            }
            catch (Exception ex)
            {
                ShowCallbackMessage(ex.Message, MessageType.Exception);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Line l = new Line();
            //l.StrokeThickness = 1.5;
            //l.Stroke = new SolidColorBrush(Colors.Red);
            //l.X1 = 10;
            //l.X2 = 10;
            //l.Y1 = 10;
            //l.Y2 = 100;
            //cavMark.Children.Add(l);
        }

        private void btnStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ROBOTS != null && ROBOTS.Count > 0)
                {
                    for (int i = 0; i < ROBOTS.Count; i++)
                    {
                        if (ROBOTS[i].Code == SELECT_ROBOT_CODE)
                        {
                            ShowCallbackMessage("Robot: " + ROBOTS[i].Code + " status: " + ROBOTS[i].ReadCurFSM(), MessageType.Default);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowCallbackMessage(ex.Message, MessageType.Exception);
            }
        }

        private void slSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (ROBOTS != null && ROBOTS.Count > 0)
                {
                    for (int i = 0; i < ROBOTS.Count; i++)
                    {
                        if (ROBOTS[i].Code == SELECT_ROBOT_CODE)
                        {
                            if (ROBOTS[i].SetOverride(slSpeed.Value / 100))
                            {
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowCallbackMessage(ex.Message, MessageType.Exception);
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            tbxJ1.Text = labJ1.Content.ToString();
            tbxJ2.Text = labJ2.Content.ToString();
            tbxJ3.Text = labJ3.Content.ToString();
            tbxJ4.Text = labJ4.Content.ToString();
            tbxJ5.Text = labJ5.Content.ToString();
            tbxJ6.Text = labJ6.Content.ToString();
            tbxXpos.Text = labXpos.Content.ToString();
            tbxYpos.Text = labYpos.Content.ToString();
            tbxZpos.Text = labZpos.Content.ToString();
            tbxRXpos.Text = labRXpos.Content.ToString();
            tbxRYpos.Text = labRYpos.Content.ToString();
            tbxRZpos.Text = labRZpos.Content.ToString();
        }

        private void btnOrig_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SpaceJog_Down(object sender, MouseButtonEventArgs e)
        {
            Label lab = sender as Label;
            if (lab != null)
            {
                lab.Background = new SolidColorBrush(Color.FromArgb(255, 160, 230, 150));
                int axis = -1, direct = -1, state = 1;
                switch (lab.Name)
                {
                    case "labXplus":
                        axis = 0;
                        direct = 1;
                        break;
                    case "labXsub":
                        axis = 0;
                        direct = 0;
                        break;
                    case "labYplus":
                        axis = 1;
                        direct = 1;
                        break;
                    case "labYsub":
                        axis = 1;
                        direct = 0;
                        break;
                    case "labZplus":
                        axis = 2;
                        direct = 1;
                        break;
                    case "labZsub":
                        axis = 2;
                        direct = 0;
                        break;
                    case "labRXplus":
                        axis = 3;
                        direct = 1;
                        break;
                    case "labRXsub":
                        axis = 3;
                        direct = 0;
                        break;
                    case "labRYplus":
                        axis = 4;
                        direct = 1;
                        break;
                    case "labRYsub":
                        axis = 4;
                        direct = 0;
                        break;
                    case "labRZplus":
                        axis = 5;
                        direct = 1;
                        break;
                    case "labRZsub":
                        axis = 5;
                        direct = 0;
                        break;
                    default:
                        break;
                }
                for (int i = 0; i < ROBOTS.Count; i++)
                {
                    if (ROBOTS[i].Code == SELECT_ROBOT_CODE && axis != -1)
                    {
                        if (ROBOTS[i].LongJogL(axis, direct, state))
                        {
                            ShowCallbackMessage(lab.Name + " start to move space", MessageType.Default);
                            JOG_CIRCLE = true;
                            Thread thJog = new Thread(KeepMove);
                            thJog.Start();
                        }
                        break;
                    }
                }
            }
        }

        private void SpaceJog_Leave(object sender, MouseEventArgs e)
        {
            Label lab = sender as Label;
            if (lab != null)
            {
                lab.Background = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
                JOG_CIRCLE = false;
            }
        }

        private void SpaceJog_Up(object sender, MouseButtonEventArgs e)
        {
            Label lab = sender as Label;
            if (lab != null)
            {
                lab.Background = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
                JOG_CIRCLE = false;
            }
        }

        private void DegreeJog_Down(object sender, MouseButtonEventArgs e)
        {
            Label lab = sender as Label;
            if (lab != null)
            {
                lab.Background = new SolidColorBrush(Color.FromArgb(255, 160, 230, 150));
                int axis = -1, direct = -1, state = 1;
                switch (lab.Name)
                {
                    case "labJ1plus":
                        axis = 0;
                        direct = 1;
                        break;
                    case "labJ1sub":
                        axis = 0;
                        direct = 0;
                        break;
                    case "labJ2plus":
                        axis = 1;
                        direct = 1;
                        break;
                    case "labJ2sub":
                        axis = 1;
                        direct = 0;
                        break;
                    case "labJ3plus":
                        axis = 2;
                        direct = 1;
                        break;
                    case "labJ3sub":
                        axis = 2;
                        direct = 0;
                        break;
                    case "labJ4plus":
                        axis = 3;
                        direct = 1;
                        break;
                    case "labJ4sub":
                        axis = 3;
                        direct = 0;
                        break;
                    case "labJ5plus":
                        axis = 4;
                        direct = 1;
                        break;
                    case "labJ5sub":
                        axis = 4;
                        direct = 0;
                        break;
                    case "labJ6plus":
                        axis = 5;
                        direct = 1;
                        break;
                    case "labJ6sub":
                        axis = 5;
                        direct = 0;
                        break;
                    default:
                        break;
                }
                for (int i = 0; i < ROBOTS.Count; i++)
                {
                    if (ROBOTS[i].Code == SELECT_ROBOT_CODE && axis != -1)
                    {
                        if (ROBOTS[i].LongJogJ(axis, direct, state))
                        {
                            ShowCallbackMessage(lab.Name + " start to move degree", MessageType.Default);
                            JOG_CIRCLE = true;
                            Thread thJog = new Thread(KeepMove);
                            thJog.Start();
                        }
                        break;
                    }
                }
            }
        }

        private void DegreeJog_Leave(object sender, MouseEventArgs e)
        {
            Label lab = sender as Label;
            if (lab != null)
            {
                lab.Background = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
                JOG_CIRCLE = false;
            }
        }

        private void DegreeJog_Up(object sender, MouseButtonEventArgs e)
        {
            Label lab = sender as Label;
            if (lab != null)
            {
                lab.Background = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
                JOG_CIRCLE = false;
            }
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
                        ShowCallbackMessage("Device: "+ cbDevice.Text + " shelf turn arround complete, refresh requests", MessageType.Result);
                    }
                    DB.Close();
                }
            }
            catch (Exception)
            {
                ShowCallbackMessage("Device: " + cbDevice.Text + " shelf turn arround unsuccessful", MessageType.Error);
            }
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //string requestString = "{\"appoint_vehicle_id\" : 8," +
            //    "\"mission\" : [ {" +
            //    "\"type\" : \"move\",\"destination\" : 1,\"map_id\" : 14," +
            //    "\"action_name\" : \"\",\"action_id\" : 0,\"action_param1\" : 0,\"action_param2\" : 0 } ]," +
            //    "\"priority\" : 0,\"user_id\" : 1}";
            //Stream resStream = null;
            //byte[] body = Encoding.UTF8.GetBytes(requestString);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://192.168.30.101:8088/api/v2/orders");
            //request.Method = "POST";
            //request.KeepAlive = true;
            //request.Host = "192.168.30.101:8088";
            //request.ContentType = "application/json;charset=UTF-8";
            //request.ContentLength = body.Length;
            //request.Headers.Add("token", "YWRtaW4sMTk3NDg2OTYzMDc2OSw3ODBjOGI5Mzk2YzgxMWVjMDVmODQ0YmQ0YjE1ZDA0Zg==");
            //try
            //{
            //    Stream st = request.GetRequestStream();
            //    st.Write(body, 0, body.Length);
            //    st.Close();
            //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //    ShowCallbackMessage("Response status code: " + response.StatusCode, MessageType.Result);
            //    if (response.StatusCode == HttpStatusCode.OK)
            //    {
            //        resStream = response.GetResponseStream();
            //    }
            //    response.Close();
            //}
            //catch (Exception ex)
            //{
            //    ShowCallbackMessage(ex.Message, MessageType.Error);
            //}
            for (int i = 0; i < ROBOTS.Count; i++)
            {
                int x, y, r;
                ROBOTS[i].Cam.GetLastFrame(SnapType.Staff, out x, out y, out r);
                int w = ROBOTS[i].Cam.GetWidth();
                int h = ROBOTS[i].Cam.GetHeight();
                if (x != 0 && y != 0 && r != 0)
                {
                    cavMark.Children.Clear();
                    double cx = cavMark.Width * x / w;
                    double cy = cavMark.Height * y / h;
                    double cr = cavMark.Width * r / w;

                    Line line1 = new Line();
                    line1.StrokeThickness = 1;
                    line1.Stroke = new SolidColorBrush(Colors.Red);
                    line1.X1 = cx - 5;
                    line1.X2 = cx + 5;
                    line1.Y1 = line1.Y2 = cy;
                    Line line2 = new Line();
                    line2.StrokeThickness = 1;
                    line2.Stroke = new SolidColorBrush(Colors.Red);
                    line2.X1 = line2.X2 = cx;
                    line2.Y1 = cy - 5;
                    line2.Y2 = cy + 5;
                    Ellipse elp = new Ellipse();
                    elp.StrokeThickness = 1;
                    elp.Stroke = new SolidColorBrush(Colors.Blue);
                    elp.Margin = new Thickness(cx - cr, cy - cr, 0, 0);
                    elp.Width = elp.Height = cr * 2;
                    cavMark.Children.Add(line1);
                    cavMark.Children.Add(line2);
                    cavMark.Children.Add(elp);
                }
                else
                {
                    ShowCallbackMessage("staff find error", MessageType.Error);
                }
                break;
            }
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            int str = FMS.GetAgvStation("8");
            ShowCallbackMessage(str.ToString(), MessageType.Result);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
