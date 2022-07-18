using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Involution
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread _thAuto, _thUi;
        private UiSignal _signal;
        private Queue<Run> _inforArray;
        private int _inforShow, _inforMaxsize;
        private double _slidePara;
        private Timer _buffTimer;
        private bool inLoop;
        private Task _loopTask;

        public MainWindow()
        {
            inLoop = true;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _signal = UiSignal.None;
            _inforArray = new Queue<Run>(0);
            _inforShow = 12;
            _inforMaxsize = 52;
            _slidePara = svMsg.ScrollableHeight / (_inforMaxsize - _inforShow);
            btnRead.IsEnabled = false;
            _buffTimer = null;

            _thUi = new Thread(ConnectPLC_UI);
            _thUi.Start();
            _thAuto = new Thread(ConnectPLC);
            _thAuto.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lock ((object)inLoop)
            {
                inLoop = false;
            }
            HJF.FinishUi();
            HJF.FinishAuto();
        }

        private void ConnectPLC()
        {//600151
            try
            {
                int res = 1;
                while (res != 0 && inLoop)
                {
                    res = HJF.InitAuto(MsgShow);
                    while (res == 0 && inLoop)
                    {
                        Dispatcher.Invoke(new Action(() => {
                            dpTital.Background = Brushes.LightGreen;
                        }));
                        res = HJF.Trigger(MsgShow, FlushBuffer);
                        Thread.Sleep(2000);
                    }
                    IntPtr p = Marshal.StringToHGlobalAnsi("PLC connect fail, Code = " + res);
                    MsgShow(p, MessageType.Error);
                    this.Dispatcher.Invoke(new Action(() => {
                        dpTital.Background = Brushes.LightPink;
                        labMsg.Content = "PLC connect fail, Code = " + res;
                    }));
                    Thread.Sleep(5000);
                }
            }
            catch (Exception e)
            {
                if (inLoop)
                {
                    this.Dispatcher.Invoke(new Action(() => {
                        labMsg.Content = e.Message;
                    }));
                }
                
            }
        }

        private void ConnectPLC_UI()
        {
            try
            {
                int res = 1;
                while (res != 0 && inLoop)
                {
                    res = HJF.InitUi(MsgShow);
                    while (res == 0 && inLoop)
                    {
                        Dispatcher.Invoke(new Action(() => {
                            eLight.Fill = Brushes.PaleGreen;
                            btnRead.IsEnabled = true;
                        }));
                        lock ((object)_signal)
                        {
                            switch (_signal)
                            {
                                case UiSignal.None:
                                    break;
                                case UiSignal.Button_Read:
                                    res = HJF.GetAllPosTable(MsgShow);
                                    _signal = UiSignal.None;
                                    break;
                                case UiSignal.Button_Write:
                                    res = HJF.SetRecipe_ForTest(MsgShow);
                                    _signal = UiSignal.None;
                                    break;
                                case UiSignal.Button_Edit:
                                    int value = 0;
                                    this.Dispatcher.Invoke(new Action(() => {
                                        value = Convert.ToInt32(tbValue.Text);
                                    }));
                                    res = HJF.Edit_ForTest(MsgShow, value);
                                    _signal = UiSignal.None;
                                    break;
                                case UiSignal.Button_PLC_Trigger_On:
                                    res = HJF.Trigger_ForTest(MsgShow);
                                    _signal = UiSignal.None;
                                    break;
                                case UiSignal.Button_PLC_Trigger_Off:
                                    res = HJF.TriggerReset_ForTest(MsgShow);
                                    _signal = UiSignal.None;
                                    break;
                            }
                        }
                        Thread.Sleep(1000);
                    }
                    this.Dispatcher.Invoke(new Action(() => {
                        labMsg.Content = "UI PLC connect fail, Code = " + res;
                        eLight.Fill = Brushes.MistyRose;
                        btnRead.IsEnabled = false;
                    }));
                    Thread.Sleep(5000);
                }
            }
            catch (Exception e)
            {
                if (inLoop)
                {
                    this.Dispatcher.Invoke(new Action(() => {
                        labMsg.Content = e.Message;
                    }));
                }
            }
        }

        private void ElMin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ElClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void DpTital_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string str = btn.Name;
            lock ((object)_signal)
            {
                switch (str)
                {
                    case "btnRead":
                        _signal = UiSignal.Button_Read;
                        break;
                    case "btnWrite":
                        _signal = UiSignal.Button_Write;
                        break;
                    case "btnTrig":
                        _signal = UiSignal.Button_PLC_Trigger_On;
                        break;
                    case "btnEdit":
                        _signal = UiSignal.Button_Edit;
                        break;
                    case "btnTrigOff":
                        _signal = UiSignal.Button_PLC_Trigger_Off;
                        break;
                    case "btnTT":
                        Setting st = new Setting();
                        st.Show();
                        break;
                    default:
                        break;
                }
            }
        }

        private void MsgShow(IntPtr p, MessageType type)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Run r = new Run(Marshal.PtrToStringAnsi(p) + Environment.NewLine);
                r.Foreground = HJF.BRUSH_TYPE[type];
                _inforArray.Enqueue(r);
                lock (_inforArray)
                {
                    while (_inforArray.Count > _inforMaxsize)
                    {
                        _inforArray.Dequeue();
                    }
                }
                tbMessage.Inlines.Clear();
                foreach (Run r0 in _inforArray)
                {
                    tbMessage.Inlines.Add(r0);
                }
                svMsg.ScrollToVerticalOffset((_inforArray.Count - _inforShow) * _slidePara);

                labMsg.Content = Marshal.PtrToStringAnsi(p);
            }));
        }

        private void FlushBuffer(int iStart)
        {
            if (iStart == 1)
            {
                if (_buffTimer == null)
                {
                    _buffTimer = new Timer(WriteBuffer, null, 0, 60000);
                }
            }
            else if (iStart == 0)
            {
                if (_buffTimer != null)
                {
                    _buffTimer.Dispose();
                    _buffTimer = null;
                    IntPtr p = Marshal.StringToHGlobalAnsi("flush timer stopped.");
                    MsgShow(p, MessageType.FunctionResult);
                }
            }
        }

        private void WriteBuffer(object obj)
        {
            if (HJF.FlushRecordBuffer(MsgShow, FlushBuffer) == 2)
            {
                IntPtr p = Marshal.StringToHGlobalAnsi("data save error, close csv file.");
                MsgShow(p, MessageType.Error);
            }
        }
    }
}
