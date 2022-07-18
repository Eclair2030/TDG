using System;
using System.Collections.Generic;
using System.IO;
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

namespace CClinkTest
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        ModelPlasma _plasma;
        ModelAcf _acf;
        ModelPreBond _prb;
        ModelMainBond _main;

        Timer _timer;
        short _ret;
        public MainWindow()
        {
            InitializeComponent();

            _ret = 0;
        }

        private void RefreshMessage(OperateAction act)
        {
            string strInitMessage = string.Empty;
            switch (act)
            {
                case OperateAction.Start:
                    strInitMessage = "Connect to PLC result:" + _ret + Environment.NewLine;
                    break;
                case OperateAction.Stop:
                    strInitMessage = "DisConnect to PLC:" + _ret + ", action result:" + (_ret == 0 ? "Success" : "Fail") + Environment.NewLine;
                    break;
                case OperateAction.Read:
                    strInitMessage = "Read all position data from PLC:";
                    break;
                case OperateAction.Compare:
                    strInitMessage = "Compare current data to PLC:";
                    break;
                default:
                    strInitMessage = "Action type error.";
                    break;
            }
            tbMessage.Text = strInitMessage;
        }

        //Calling in new thread
        private void ReadAllPositionFromPLC()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                tbMessage.Text = "Reading position from PLC......" + Environment.NewLine;
            }));
            _plasma.ReadFromPLC();
            Dispatcher.Invoke(new Action(()=> 
            {
                tbMessage.Text = "Read Plasma position from PLC complete." + Environment.NewLine;
            }));
            _acf.ReadFromPLC();
            Dispatcher.Invoke(new Action(() =>
            {
                tbMessage.Text += "Read Acf position from PLC complete." + Environment.NewLine;
            }));
            _prb.ReadFromPLC();
            Dispatcher.Invoke(new Action(() =>
            {
                tbMessage.Text += "Read Pre bond position from PLC complete." + Environment.NewLine;
            }));
            _main.ReadFromPLC();
            Dispatcher.Invoke(new Action(() =>
            {
                tbMessage.Text += "Read Main bond position from PLC complete." + Environment.NewLine;
                tbMessage.Text += "Read all position from PLC complete." + Environment.NewLine;
                btnRead.IsEnabled = true;
            }));
            
        }

        private void PlcTriggerTimer(object obj)
        {
            int size = 2;
            short data = 0;
            int ret = Mary.MdReceiveEx((int)PlcDeviceType.Trigger, Mary._plcTriggerDeviceNumber, out size, out data);
            if (ret == 0 && data != 0)
            {
                size = 2;
                short reset = 0;
                ret = Mary.MdSendEx((int)PlcDeviceType.Trigger, Mary._plcTriggerDeviceNumber, out size, out reset);
                if (ret != 0)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        tbMessage.Text += "Reset trigger signal fail." + Environment.NewLine;
                    }));
                    return;
                }
                Mary._date = DateTime.Now;
                string path = Mary._filePath + Mary._date.Year + "\\" + Mary._date.Month + "\\";
                DirectoryInfo di = new DirectoryInfo(path);
                di.Create();
                di.Attributes = FileAttributes.Directory & FileAttributes.Normal;
                FileStream file = null;
                string content = string.Empty;
                try
                {
                    if (File.Exists(path + Mary._date.Day + Mary._fileExtendName))
                    {
                        file = File.Open(path + Mary._date.Day + Mary._fileExtendName, FileMode.Open);
                    }
                    else
                    {
                        file = File.Create(path + Mary._date.Day + Mary._fileExtendName);
                        content = Mary._posChangeTableHead;
                    }
                    file.Seek(0, SeekOrigin.End);
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        tbMessage.Text = ex.Message + Environment.NewLine;
                    }));
                }

                switch (data)
                {
                    case (short)PlcSavingTrigger.Plasma_Stage_X:
                    case (short)PlcSavingTrigger.Plasma_Stage_Y:
                    case (short)PlcSavingTrigger.Plasma_Stage_T1:
                    case (short)PlcSavingTrigger.Plasma_Stage_T2:
                    case (short)PlcSavingTrigger.Plasma_Stage_Ym:
                    case (short)PlcSavingTrigger.Plasma_Stage_X_1:
                    case (short)PlcSavingTrigger.Plasma_Stage_Y_1:
                    case (short)PlcSavingTrigger.Plasma_Stage_T1_1:
                    case (short)PlcSavingTrigger.Plasma_Stage_T2_1:
                    case (short)PlcSavingTrigger.Plasma_Stage_Ym_1:
                    case (short)PlcSavingTrigger.Plasma_Handler_Default:
                        _plasma.CompareWithPLC((PlcSavingTrigger)data);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            tbMessage.Text += "Plasma Save Operation Triggered:" + data + Environment.NewLine;
                        }));
                        break;
                    case (short)PlcSavingTrigger.Acf_Stage1_X:
                    case (short)PlcSavingTrigger.Acf_Stage1_Y:
                    case (short)PlcSavingTrigger.Acf_Stage1_T:
                    case (short)PlcSavingTrigger.Acf_Stage2_X:
                    case (short)PlcSavingTrigger.Acf_Stage2_Y:
                    case (short)PlcSavingTrigger.Acf_Stage2_T:
                    case (short)PlcSavingTrigger.Acf_Feeding_1:
                    case (short)PlcSavingTrigger.Acf_Feeding_2:
                    case (short)PlcSavingTrigger.Acf_Tool_1:
                    case (short)PlcSavingTrigger.Acf_Tool_2:
                    case (short)PlcSavingTrigger.Acf_Reel_1:
                    case (short)PlcSavingTrigger.Acf_Reel_2:
                    case (short)PlcSavingTrigger.Acf_Separator_1:
                    case (short)PlcSavingTrigger.Acf_Separator_2:
                    case (short)PlcSavingTrigger.Acf_Handler_Default:
                        _acf.CompareWithPLC((PlcSavingTrigger)data);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            tbMessage.Text += "Acf Save Operation Triggered:" + data + Environment.NewLine;
                        }));
                        break;
                    case (short)PlcSavingTrigger.Prb_Stage_X:
                    case (short)PlcSavingTrigger.Prb_Stage_Y:
                    case (short)PlcSavingTrigger.Prb_Stage_T1:
                    case (short)PlcSavingTrigger.Prb_Stage_T2:
                    case (short)PlcSavingTrigger.Prb_Stage_X_1:
                    case (short)PlcSavingTrigger.Prb_Stage_Y_1:
                    case (short)PlcSavingTrigger.Prb_Stage_T1_1:
                    case (short)PlcSavingTrigger.Prb_Stage_T2_1:
                    case (short)PlcSavingTrigger.Prb_Tool1_X:
                    case (short)PlcSavingTrigger.Prb_Tool1_Z:
                    case (short)PlcSavingTrigger.Prb_Tool1_T:
                    case (short)PlcSavingTrigger.Prb_Tool2_X:
                    case (short)PlcSavingTrigger.Prb_Tool2_Z:
                    case (short)PlcSavingTrigger.Prb_Tool2_T:
                    case (short)PlcSavingTrigger.Prb_Carrier1_X:
                    case (short)PlcSavingTrigger.Prb_Carrier1_Y:
                    case (short)PlcSavingTrigger.Prb_Carrier1_Z:
                    case (short)PlcSavingTrigger.Prb_Carrier1_X_1:
                    case (short)PlcSavingTrigger.Prb_Carrier1_Y_1:
                    case (short)PlcSavingTrigger.Prb_Carrier1_Z_1:
                    case (short)PlcSavingTrigger.Prb_Carrier1_T1:
                    case (short)PlcSavingTrigger.Prb_Carrier1_T2:
                    case (short)PlcSavingTrigger.Prb_Carrier2_X:
                    case (short)PlcSavingTrigger.Prb_Carrier2_Y:
                    case (short)PlcSavingTrigger.Prb_Carrier2_Z:
                    case (short)PlcSavingTrigger.Prb_Carrier2_T:
                    case (short)PlcSavingTrigger.Prb_Camera_X:
                    case (short)PlcSavingTrigger.Prb_Camera_Y:
                    case (short)PlcSavingTrigger.Prb_Handler_Default:
                    case (short)PlcSavingTrigger.Prb_USC_Default:
                    case (short)PlcSavingTrigger.IC_Shuttle1_X:
                    case (short)PlcSavingTrigger.IC_Shuttle1_Z:
                    case (short)PlcSavingTrigger.IC_Shuttle2_X:
                    case (short)PlcSavingTrigger.IC_Shuttle2_Z:
                    case (short)PlcSavingTrigger.IC_LoadUnload1:
                    case (short)PlcSavingTrigger.IC_LoadUnload2:
                    case (short)PlcSavingTrigger.IC_Camera_Y:
                    case (short)PlcSavingTrigger.IC_Camera_T:
                    case (short)PlcSavingTrigger.IC_Buffer:
                        content += _prb.CompareWithPLC((PlcSavingTrigger)data);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            tbMessage.Text += "Pre bond save operation triggered:" + data + Environment.NewLine;
                        }));
                        break;
                    case (short)PlcSavingTrigger.Fnb_Handler_Default:
                    case (short)PlcSavingTrigger.Fnb_Stage1_Stage1Y:
                    case (short)PlcSavingTrigger.Fnb_Stage1_Stage1T:
                    case (short)PlcSavingTrigger.Fnb_Stage1_Stage2Y:
                    case (short)PlcSavingTrigger.Fnb_Stage1_Stage2T:
                    case (short)PlcSavingTrigger.Fnb_Stage2_Stage3Y:
                    case (short)PlcSavingTrigger.Fnb_Stage2_Stage3T:
                    case (short)PlcSavingTrigger.Fnb_Stage2_Stage4Y:
                    case (short)PlcSavingTrigger.Fnb_Stage2_Stage4T:
                    case (short)PlcSavingTrigger.Fnb_Tool1_Z:
                    case (short)PlcSavingTrigger.Fnb_Tool2_Z:
                    case (short)PlcSavingTrigger.Fnb_Tool3_Z:
                    case (short)PlcSavingTrigger.Fnb_Tool4_Z:
                    case (short)PlcSavingTrigger.Fnb_Camera_Default:
                    case (short)PlcSavingTrigger.Fnb_SheetFeeding1_T:
                    case (short)PlcSavingTrigger.Fnb_SheetFeeding2_T:
                        content += _main.CompareWithPLC((PlcSavingTrigger)data);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            tbMessage.Text += "Main bond save operation triggered:" + data + Environment.NewLine;
                        }));
                        break;
                    default:
                        break;
                }
                try
                {
                    file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
                    file.Flush();
                    file.Close();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        tbMessage.Text = ex.Message + Environment.NewLine;
                    }));
                }
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _plasma = new ModelPlasma();
            _acf = new ModelAcf();
            _prb = new ModelPreBond();
            _main = new ModelMainBond();
            _ret = Mary.MdOpen();
            if (_ret == 0)
            {
                ellState.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;
                int size = 2;
                short reset = 0;
                int ret = Mary.MdSendEx((int)PlcDeviceType.Trigger, Mary._plcTriggerDeviceNumber, out size, out reset);
                if (ret != 0)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        tbMessage.Text += "Reset trigger signal fail." + Environment.NewLine;
                    }));
                    return;
                }
                btnRead.IsEnabled = false;
                Thread th = new Thread(ReadAllPositionFromPLC);
                th.Start();
            }
            else
            {
            }
            RefreshMessage(OperateAction.Start);
            _timer = new Timer(PlcTriggerTimer, null, 0, Mary._plcTriggerTimerPeriod);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Mary.MdClose();
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }

        

        

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            _ret = Mary.MdOpen();
            if (_ret == 0)
            {
                ellState.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;
            }
            else
            {
            }
            RefreshMessage(OperateAction.Start);
            _timer = new Timer(PlcTriggerTimer, null, 0, Mary._plcTriggerTimerPeriod);
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _ret = Mary.MdClose();
            if (_ret == 0)
            {
                ellState.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;
            }
            else
            {
            }
            RefreshMessage(OperateAction.Stop);
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }

        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            btnRead.IsEnabled = false;
            Thread th = new Thread(ReadAllPositionFromPLC);
            th.Start();
        }

        private void BtnCompare_Click(object sender, RoutedEventArgs e)
        {

        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //_data[0] = 2099;
            //_plcDeveiceNumber = 584650;
            //tbMessage.Text = "";
            //tbMessage.Text += "Write data result:" + Mary.mdSendEx(_path, _plcNetWorkNumber, _plcStationNumber, _plcDeveiceType, _plcDeveiceNumber, out _plcReadSize, out _data[0]) + Environment.NewLine;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //_plcDeveiceNumber = 584650;
            //tbMessage.Text = "";
            //tbMessage.Text += "Read data result:" + Mary.mdReceiveEx(_path, _plcNetWorkNumber, _plcStationNumber, _plcDeveiceType, _plcDeveiceNumber, out _plcReadSize, out _data[0]) + Environment.NewLine;
            //tbMessage.Text += _data[0] + Environment.NewLine;
        }
    }
}
