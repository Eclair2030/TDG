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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace TDG_Vision
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer SYS_TIMER;
        Timer UPSYS_SIGNAL_TIMER;
        Thread CONN_UPSYS;
        Thread CAMERA_INIT;
        bool IS_CONNECTED;                                          //与上位机连接状态
        bool IS_AMPLIFY;                                                 //像框操作是否处于放大状态
        bool IS_MARKING;                                                //是否正在录入Mark
        RecipeModel PBD_TOOL, PBD_STAGE;
        CameraControl CAM_CON, CAM_CON_RIGHT;
        int MAX_LINES;

        ToolShape TS;
        Point PT_LAST;


        public MainWindow()
        {
            InitializeComponent();
            IS_CONNECTED = false;
            IS_AMPLIFY = false;
            IS_MARKING = false;
            MAX_LINES = 40;
            TS = new ToolShape();
            PT_LAST = new Point();
        }

        private void SetConnectStatus(bool b)
        {
            lock ((object)IS_CONNECTED)
            {
                IS_CONNECTED = b;
            }
        }

        private void AddMessage(IntPtr msg)
        {
            string str = Marshal.PtrToStringAnsi(msg) + Environment.NewLine;
            Dispatcher.Invoke(() =>
            {
                if (tbMessage.Inlines.Count >= MAX_LINES)
                {
                    tbMessage.Inlines.Remove(tbMessage.Inlines.First());
                }
                tbMessage.Inlines.Add(str);
            });
        }

        private void IsFileExist(Image img, ref string path)
        {
            img.Source = null;
            if (File.Exists(path))
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                Stream st = new MemoryStream(File.ReadAllBytes(path));
                bi.StreamSource = st;
                bi.EndInit();
                bi.Freeze();

                //WriteableBitmap writeableBitmap = new WriteableBitmap(bi);
                //byte[] ColorData = { 0, 0, 255, 0 }; // B G R
                //for (int i = 0; i < 100; i++)
                //{
                //    for (int j = 0; j < 100; j++)
                //    {
                //        Int32Rect rect = new Int32Rect(
                //        10 + i,
                //        10 + j,
                //        1,
                //        1);

                //        writeableBitmap.WritePixels(rect, ColorData, 4, 0);
                //    }
                //}
                
                img.Source = bi;
            }
            else
            {
                path = string.Empty;
            }
        }

        private void SystemTimer(object sender, EventArgs e)
        {
            labDatetime.Content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 重新增加Recipe信息，并使底层C++模块重新加载mark
        /// </summary>
        /// <param name="AddMessage"></param>
        private void RefreshRecipeList(VisionDll.SHOWMESSAGE AddMessage)
        {
            Thread recipeLoader = new Thread(() =>
            {
                PBD_TOOL = new RecipeModel();
                PBD_STAGE = new RecipeModel();
                List<RecipeListItem> list = Recipes.LoadAllRecipe();
                string pathToolL1 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_TOOL_CODE_VALUE + @"\" + Recipes.CAMERA_CODE1 + @"\" + Recipes.MARK_NAME1;
                string pathToolL2 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_TOOL_CODE_VALUE + @"\" + Recipes.CAMERA_CODE1 + @"\" + Recipes.MARK_NAME2;
                string pathToolL3 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_TOOL_CODE_VALUE + @"\" + Recipes.CAMERA_CODE1 + @"\" + Recipes.MARK_NAME3;
                string pathToolR1 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_TOOL_CODE_VALUE + @"\" + Recipes.CAMERA_CODE2 + @"\" + Recipes.MARK_NAME1;
                string pathToolR2 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_TOOL_CODE_VALUE + @"\" + Recipes.CAMERA_CODE2 + @"\" + Recipes.MARK_NAME2;
                string pathToolR3 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_TOOL_CODE_VALUE + @"\" + Recipes.CAMERA_CODE2 + @"\" + Recipes.MARK_NAME3;
                string pathStageL1 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_STAGE_CODE_VALUE + @"\" + Recipes.CAMERA_CODE1 + @"\" + Recipes.MARK_NAME1;
                string pathStageL2 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_STAGE_CODE_VALUE + @"\" + Recipes.CAMERA_CODE1 + @"\" + Recipes.MARK_NAME2;
                string pathStageL3 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_STAGE_CODE_VALUE + @"\" + Recipes.CAMERA_CODE1 + @"\" + Recipes.MARK_NAME3;
                string pathStageR1 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_STAGE_CODE_VALUE + @"\" + Recipes.CAMERA_CODE2 + @"\" + Recipes.MARK_NAME1;
                string pathStageR2 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_STAGE_CODE_VALUE + @"\" + Recipes.CAMERA_CODE2 + @"\" + Recipes.MARK_NAME2;
                string pathStageR3 = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_STAGE_CODE_VALUE + @"\" + Recipes.CAMERA_CODE2 + @"\" + Recipes.MARK_NAME3;
                Dispatcher.Invoke(() =>
                {
                    labRecipe.Content = "no recipe infor";
                    lvRecipe.Items.Clear();
                    for (int i = 0; i < list.Count; i++)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Content = list[i];
                        if (list[i].Inuse == Recipes.INUSE_VALUE)
                        {
                            item.Background = new SolidColorBrush(Color.FromRgb(220, 150, 60));
                            labRecipe.Content = list[i].Code + " - " + list[i].Name;
                            PBD_TOOL = Recipes.GetRecipe(list[i].Code, Recipes.PRB_TOOL_CODE_VALUE);
                            PBD_STAGE = Recipes.GetRecipe(list[i].Code, Recipes.PRB_STAGE_CODE_VALUE);
                            if (PBD_TOOL != null && PBD_STAGE != null)
                            {
                                tbxDelay.Text = PBD_TOOL.DELAY.ToString();
                                tbxOfsetX.Text = PBD_TOOL.OFFSET_X.ToString();
                                tbxOfsetY.Text = PBD_TOOL.OFFSET_Y.ToString();
                                tbxOfsetT.Text = PBD_TOOL.OFFSET_T.ToString();
                                tbxSpecX.Text = PBD_TOOL.SPEC_X.ToString();
                                tbxSpecY.Text = PBD_TOOL.SPEC_Y.ToString();
                                tbxSpecT.Text = PBD_TOOL.SPEC_T.ToString();
                                tbxCalibX.Text = PBD_TOOL.CALIB_X.ToString();
                                tbxCalibY.Text = PBD_TOOL.CALIB_Y.ToString();
                                tbxCalibT.Text = PBD_TOOL.CALIB_T.ToString();
                                tbxYmap.Text = PBD_TOOL.Y_MAP.ToString();
                                cbYmap.IsChecked = PBD_TOOL.USE_Y_MAP;
                                labToolCalibData.Content = "LX res: " + PBD_TOOL.L_CAMERA_RECIPE.Resolution_X + Environment.NewLine;
                                labToolCalibData.Content += "LY res: " + PBD_TOOL.L_CAMERA_RECIPE.Resolution_Y + Environment.NewLine;
                                labToolCalibData.Content += "LX center: " + PBD_TOOL.L_CAMERA_RECIPE.RotateCenter_X + Environment.NewLine;
                                labToolCalibData.Content += "LY center: " + PBD_TOOL.L_CAMERA_RECIPE.RotateCenter_Y + Environment.NewLine + Environment.NewLine;
                                labToolCalibData.Content += "RX res: " + PBD_TOOL.R_CAMERA_RECIPE.Resolution_X + Environment.NewLine;
                                labToolCalibData.Content += "RY res: " + PBD_TOOL.R_CAMERA_RECIPE.Resolution_Y + Environment.NewLine;
                                labToolCalibData.Content += "RX center: " + PBD_TOOL.R_CAMERA_RECIPE.RotateCenter_X + Environment.NewLine;
                                labToolCalibData.Content += "RY center: " + PBD_TOOL.R_CAMERA_RECIPE.RotateCenter_Y;
                                labStageCalibData.Content = "X res: " + PBD_STAGE.L_CAMERA_RECIPE.Resolution_X;
                                IsFileExist(imgToolLmark1, ref pathToolL1);
                                IsFileExist(imgToolLmark2, ref pathToolL2);
                                IsFileExist(imgToolLmark3, ref pathToolL3);
                                IsFileExist(imgToolRmark1, ref pathToolR1);
                                IsFileExist(imgToolRmark2, ref pathToolR2);
                                IsFileExist(imgToolRmark3, ref pathToolR3);
                                IsFileExist(imgStageLmark1, ref pathStageL1);
                                IsFileExist(imgStageLmark2, ref pathStageL2);
                                IsFileExist(imgStageLmark3, ref pathStageL3);
                                IsFileExist(imgStageRmark1, ref pathStageR1);
                                IsFileExist(imgStageRmark2, ref pathStageR2);
                                IsFileExist(imgStageRmark3, ref pathStageR3);
                            }
                            else
                            {
                                AddMessage(Marshal.StringToHGlobalAnsi("Load model recipe infor fail."));
                            }
                        }
                        lvRecipe.Items.Add(item);
                    }
                });
                string msg = "Recipe data read complete.";
                AddMessage(Marshal.StringToHGlobalAnsi(msg));

                VisionDll.SetToolParameter(int.Parse(PBD_TOOL.MODEL_CODE), PBD_TOOL.DELAY, PBD_TOOL.OFFSET_X, PBD_TOOL.OFFSET_Y,
                    PBD_TOOL.OFFSET_T, PBD_TOOL.SPEC_X, PBD_TOOL.SPEC_Y, PBD_TOOL.SPEC_T, PBD_TOOL.Y_MAP,
                    PBD_TOOL.USE_Y_MAP, PBD_TOOL.L_CAMERA_RECIPE.MARK_ENABLE_LIST,
                    PBD_TOOL.L_CAMERA_RECIPE.MARK_CEN,
                    Marshal.StringToHGlobalAnsi(pathToolL1),
                    Marshal.StringToHGlobalAnsi(pathToolL2),
                    Marshal.StringToHGlobalAnsi(pathToolL3),
                    PBD_TOOL.L_CAMERA_RECIPE.RotateCenter_X, PBD_TOOL.L_CAMERA_RECIPE.RotateCenter_Y,
                    PBD_TOOL.L_CAMERA_RECIPE.Resolution_X, PBD_TOOL.L_CAMERA_RECIPE.Resolution_Y,
                    PBD_TOOL.R_CAMERA_RECIPE.MARK_ENABLE_LIST,
                    PBD_TOOL.R_CAMERA_RECIPE.MARK_CEN,
                    Marshal.StringToHGlobalAnsi(pathToolR1),
                    Marshal.StringToHGlobalAnsi(pathToolR2),
                    Marshal.StringToHGlobalAnsi(pathToolR3),
                    PBD_TOOL.R_CAMERA_RECIPE.RotateCenter_X, PBD_TOOL.R_CAMERA_RECIPE.RotateCenter_Y,
                    PBD_TOOL.R_CAMERA_RECIPE.Resolution_X, PBD_TOOL.R_CAMERA_RECIPE.Resolution_Y, AddMessage);
                VisionDll.SetStageParameter(int.Parse(PBD_STAGE.MODEL_CODE), PBD_STAGE.DELAY, PBD_STAGE.OFFSET_X, PBD_STAGE.OFFSET_Y,
                    PBD_STAGE.OFFSET_T, PBD_STAGE.SPEC_X, PBD_STAGE.SPEC_Y, PBD_STAGE.SPEC_T, PBD_STAGE.Y_MAP,
                    PBD_STAGE.USE_Y_MAP, PBD_STAGE.L_CAMERA_RECIPE.MARK_ENABLE_LIST,
                    PBD_STAGE.L_CAMERA_RECIPE.MARK_CEN,
                    Marshal.StringToHGlobalAnsi(pathStageL1),
                    Marshal.StringToHGlobalAnsi(pathStageL2),
                    Marshal.StringToHGlobalAnsi(pathStageL3),
                    PBD_STAGE.L_CAMERA_RECIPE.RotateCenter_X, PBD_STAGE.L_CAMERA_RECIPE.RotateCenter_Y,
                    PBD_STAGE.L_CAMERA_RECIPE.Resolution_X, PBD_STAGE.L_CAMERA_RECIPE.Resolution_Y,
                    PBD_STAGE.R_CAMERA_RECIPE.MARK_ENABLE_LIST,
                    PBD_STAGE.R_CAMERA_RECIPE.MARK_CEN,
                    Marshal.StringToHGlobalAnsi(pathStageR1),
                    Marshal.StringToHGlobalAnsi(pathStageR2),
                    Marshal.StringToHGlobalAnsi(pathStageR3),
                    PBD_STAGE.R_CAMERA_RECIPE.RotateCenter_X, PBD_STAGE.R_CAMERA_RECIPE.RotateCenter_Y,
                    PBD_STAGE.R_CAMERA_RECIPE.Resolution_X, PBD_STAGE.R_CAMERA_RECIPE.Resolution_Y, AddMessage);
            });
            recipeLoader.Start();
        }

        private void ConnSys(object ojb)
        {
            while (true)
            {
                if (IS_CONNECTED)
                {
                    continue;
                }
                string ip = "192.168.1.39";
                int port = 8500;
                int type = 2;

                int result = VisionDll.ConnectToUpSystem(type, Marshal.StringToHGlobalAnsi(ip), port, AddMessage);

                if (result == 0)
                {
                    SetConnectStatus(true);
                    UPSYS_SIGNAL_TIMER = new Timer(SignalCallback, null, 0, 1000);
                    Dispatcher.Invoke(() =>
                    {
                        dpTitle.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    });
                    while (IS_CONNECTED)
                    {
                        HeartBeat();
                        Thread.Sleep(4000);
                    }
                }
                else
                {
                    SetConnectStatus(false);
                    Dispatcher.Invoke(() =>
                    {
                        dpTitle.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    });
                }
                Thread.Sleep(5000);
            }
        }

        private void InitCameras(object ojb)
        {
            CAM_CON = new CameraControl();
            CAM_CON_RIGHT = new CameraControl();
            double w = 0, h = 0;
            Dispatcher.Invoke(() =>
            {
                w = imgLeft.Width;
                h = imgLeft.Height;
            });
            if (CAM_CON.InitCamera(ShowCameraImage, w, h) == 0 && CAM_CON_RIGHT.InitCamera(ShowRightCameraImage, w, h) == 0)
            {
                AddMessage(Marshal.StringToHGlobalAnsi("Camera start success."));
            }
            else
            {
                AddMessage(Marshal.StringToHGlobalAnsi("Camera start fail."));
            }
        }

        private void SignalCallback(object state)
        {
            string msg = string.Empty;
            if (!IS_CONNECTED && UPSYS_SIGNAL_TIMER != null)
            {
                msg = "signal not connect";
                UPSYS_SIGNAL_TIMER.Dispose();
                AddMessage(Marshal.StringToHGlobalAnsi(msg));
            }
            int command, subcommand, code;
            int result = VisionDll.GetSignalFromUpSystem(AddMessage, out command, out subcommand, out code);
            if (result != 0)
            {
                msg = "get up system signal fail.";
                AddMessage(Marshal.StringToHGlobalAnsi(msg));
            }
            else
            {
                int srcWidth, srcHeight;
                int markWidth, markHeight;
                int markPointX, markPointY;
                IntPtr frame = new IntPtr(0);
                switch (command)
                {
                    case 1:     //Snap
                        CAM_CON.GetLastFrame(out srcWidth, out srcHeight, out frame);
                        result = VisionDll.SequenceShot(subcommand, AddMessage, out markWidth, out markHeight, out markPointX, out markPointY, srcWidth, srcHeight, frame);
                        break;
                    case 2:     //Tool Calibration
                        break;
                    case 3:     //Stage Calibration
                        break;
                    case 4:     //Recipe
                        switch (subcommand)
                        {
                            case 1:
                                //Recipes.CreateRecipe("");
                                break;
                            case 2:
                                if (Recipes.CopyRecipe(code.ToString()) == 0)
                                {
                                    AddMessage(Marshal.StringToHGlobalAnsi("Up system recipe " + code + " copyed complete."));
                                    RefreshRecipeList(AddMessage);
                                }
                                else
                                {
                                    AddMessage(Marshal.StringToHGlobalAnsi("Up system recipe " + code + " copyed fail."));
                                }
                                break;
                            case 3:
                                if (Recipes.ChangeRecipe(code.ToString()) == 0)
                                {
                                    AddMessage(Marshal.StringToHGlobalAnsi("Up system recipe " + code + " actived complete."));
                                    RefreshRecipeList(AddMessage);
                                }
                                else
                                {
                                    AddMessage(Marshal.StringToHGlobalAnsi("Up system recipe " + code + " actived fail."));
                                }
                                break;
                            case 4:
                                if (Recipes.DeleteRecipe(code.ToString()) == 0)
                                {
                                    AddMessage(Marshal.StringToHGlobalAnsi("Up system recipe " + code + " delete complete."));
                                    RefreshRecipeList(AddMessage);
                                }
                                else
                                {
                                    AddMessage(Marshal.StringToHGlobalAnsi("Up system recipe " + code + " delete fail."));
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void ShowCameraImage(int width, int height, int dpiX, int dpiY, PixelFormat pf, IntPtr data, int size, int stride)
        //private void ShowCameraImage(IntPtr frame)
        {
            Dispatcher.Invoke(() =>
            {
                imgLeft.Source = BitmapSource.Create(width, height, dpiX, dpiY, pf, null, data, size, stride);
                //imgLeft.Source = Imaging.CreateBitmapSourceFromHBitmap(frame, IntPtr.Zero, new Int32Rect(0, 0, 1600, 1200),
                //        BitmapSizeOptions.FromEmptyOptions());
            });
        }

        private void ShowRightCameraImage(int width, int height, int dpiX, int dpiY, PixelFormat pf, IntPtr data, int size, int stride)
        {
            Dispatcher.Invoke(() =>
            {
                imgRight.Source = BitmapSource.Create(width, height, dpiX, dpiY, pf, null, data, size, stride);
            });
        }

        private void HeartBeat()
        {
            int result = VisionDll.HeartBeatOnce(AddMessage);
            switch (result)
            {
                case 0:
                    SetConnectStatus(true);
                    Dispatcher.Invoke(() =>
                    {
                        tbMessage.Background = new SolidColorBrush(Color.FromRgb(200, 230, 160));
                    });
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() =>
                    {                        
                        tbMessage.Background = new SolidColorBrush(Color.FromRgb(220, 220, 220));
                    });
                    break;
                case 2:
                    Dispatcher.Invoke(() =>
                    {
                        tbMessage.Background = new SolidColorBrush(Color.FromRgb(250, 240, 190));
                    });
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() =>
                    {
                        tbMessage.Background = new SolidColorBrush(Color.FromRgb(220, 220, 220));
                    });
                    break;
                case 1:
                case 3:
                case -1:
                case -2:
                default:
                    Dispatcher.Invoke(() =>
                    {
                        tbMessage.Background = new SolidColorBrush(Color.FromRgb(245, 205, 205));
                    });
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() =>
                    {
                        tbMessage.Background = new SolidColorBrush(Color.FromRgb(220, 220, 220));
                    });
                    SetConnectStatus(false);
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CAMERA_INIT != null)
            {
                CAMERA_INIT.Abort();
            }
            if (CAM_CON != null)
            {
                CAM_CON.ReleaseCamera();
            }
            if (CAM_CON_RIGHT != null)
            {
                CAM_CON_RIGHT.ReleaseCamera();
            }
            if (UPSYS_SIGNAL_TIMER != null)
            {
                UPSYS_SIGNAL_TIMER.Dispose();
            }
            if (CONN_UPSYS != null)
            {
                CONN_UPSYS.Abort();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SYS_TIMER = new DispatcherTimer();
            SYS_TIMER.Tick += new EventHandler(SystemTimer);
            SYS_TIMER.Interval = new TimeSpan(0, 0, 0, 1, 0);
            SYS_TIMER.Start();

            //RECONNECT_TIMER = new Timer(ConnSys, null, 0, 5000);
            RefreshRecipeList(AddMessage);
            CONN_UPSYS = new Thread(ConnSys);
            CONN_UPSYS.Start();

            CAMERA_INIT = new Thread(InitCameras);
            CAMERA_INIT.Start();
        }


        private void dpTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        
        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }        

        private void lvRecipe_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void ButtonActive_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            RecipeListItem item = btn.DataContext as RecipeListItem;
            if (Recipes.ChangeRecipe(item.Code) == 0)
            {
                AddMessage(Marshal.StringToHGlobalAnsi("Active recipe changed to " + item.Code + " complete."));
                RefreshRecipeList(AddMessage);
            }
            else
            {
                AddMessage(Marshal.StringToHGlobalAnsi("Active recipe changed to " + item.Code + " fail."));
            }
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            RecipeListItem item = btn.DataContext as RecipeListItem;
            if (Recipes.CopyRecipe(item.Code) == 0)
            {
                AddMessage(Marshal.StringToHGlobalAnsi("Copy recipe complete."));
                RefreshRecipeList(AddMessage);
            }
            else
            {
                AddMessage(Marshal.StringToHGlobalAnsi("Copy recipe fail."));
            }
        }
        
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            RecipeListItem item = btn.DataContext as RecipeListItem;
            if (Recipes.DeleteRecipe(item.Code) == 0)
            {
                AddMessage(Marshal.StringToHGlobalAnsi("Recipe delete complete."));
                RefreshRecipeList(AddMessage);
            }
            else
            {
                AddMessage(Marshal.StringToHGlobalAnsi("Recipe delete fail."));
            }
        }
        
        private void btnCreateRecipe_Click(object sender, RoutedEventArgs e)
        {
            int result = Recipes.CreateRecipe(null, tbxRecipeName.Text.Trim());
            if (result == 0)
            {
                string msg = "Recipe " + tbxRecipeName.Text.Trim() + " created complete.";
                AddMessage(Marshal.StringToHGlobalAnsi(msg));
                RefreshRecipeList(AddMessage);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            double.TryParse(tbxDelay.Text.Trim(), out PBD_TOOL.DELAY);
            double.TryParse(tbxYmap.Text.Trim(), out PBD_TOOL.Y_MAP);
            PBD_TOOL.USE_Y_MAP = (bool)cbYmap.IsChecked;
            double.TryParse(tbxOfsetX.Text.Trim(), out PBD_TOOL.OFFSET_X);
            double.TryParse(tbxOfsetY.Text.Trim(), out PBD_TOOL.OFFSET_Y);
            double.TryParse(tbxOfsetT.Text.Trim(), out PBD_TOOL.OFFSET_T);
            double.TryParse(tbxSpecX.Text.Trim(), out PBD_TOOL.SPEC_X);
            double.TryParse(tbxSpecY.Text.Trim(), out PBD_TOOL.SPEC_Y);
            double.TryParse(tbxSpecT.Text.Trim(), out PBD_TOOL.SPEC_T);
            double.TryParse(tbxCalibX.Text.Trim(), out PBD_TOOL.CALIB_X);
            double.TryParse(tbxCalibY.Text.Trim(), out PBD_TOOL.CALIB_Y);
            double.TryParse(tbxCalibT.Text.Trim(), out PBD_TOOL.CALIB_T);

            int result = Recipes.EditRecipe(Recipes.RECIPE_CODE, PBD_TOOL);
            string msg = string.Empty;
            if (result == 0)
            {
                msg = "Recipe " + Recipes.RECIPE_CODE + " saved complete.";
                RefreshRecipeList(AddMessage);
            }
            else
            {
                msg = "Recipe " + Recipes.RECIPE_CODE + " saved fail.";
            }
            AddMessage(Marshal.StringToHGlobalAnsi(msg));
        }

        private void rect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            rect.Visibility = Visibility.Hidden;
        }

        private void img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string strName = "rect" + ((Image)sender).Name.Substring(3);
            Rectangle rect = FindName(strName) as Rectangle;
            rect.Visibility = Visibility.Visible;
            
        }

        private void labTool_MouseEnter(object sender, MouseEventArgs e)
        {
            labToolCalibData.Visibility = Visibility.Visible;
        }

        private void labTool_MouseLeave(object sender, MouseEventArgs e)
        {
            labToolCalibData.Visibility = Visibility.Hidden;
        }

        private void btnMarkCreate_Click(object sender, RoutedEventArgs e)
        {
            int result = -1;
            for (int i = 1; i < 4; i++)
            {
                string name = "rectToolLmark" + i;
                int modelCode = 0, cameraCode = 0;
                Rectangle rect = FindName(name) as Rectangle;
                if (rect != null && rect.Visibility == Visibility.Visible && int.TryParse(PBD_TOOL.MODEL_CODE, out modelCode) && int.TryParse(Recipes.CAMERA_CODE1, out cameraCode))
                {
                    if (!IS_MARKING)
                    {
                        TS.ShowDefaultRect(canvLeft, CAM_CON.CURR_RATIO);
                        IS_MARKING = true;
                    }
                    else
                    {
                        string path = Recipes.MARK_PATH + Recipes.RECIPE_CODE + @"\" + Recipes.PRB_TOOL_CODE_VALUE + @"\" + Recipes.CAMERA_CODE1 + @"\Mark" + i + @".bmp";
                        result = CAM_CON.ClipMark(path, TS.GetRect());
                        Point ptCenter = new Point(TS.GetMarkCenter().X * CAM_CON.CURR_RATIO, TS.GetMarkCenter().Y * CAM_CON.CURR_RATIO);
                        if (Recipes.SetMarkCenterData(modelCode.ToString(), cameraCode.ToString(), i.ToString(), ptCenter.X.ToString(), ptCenter.Y.ToString()) == 0)
                        {
                            AddMessage(Marshal.StringToHGlobalAnsi("Mark add complete."));
                        }
                        else
                        {
                            //Mark Center信息写入Recipe失败
                            AddMessage(Marshal.StringToHGlobalAnsi("Mark center write fail."));
                        }
                        TS.RemoveRect(canvLeft);
                        IS_MARKING = false;
                        RefreshRecipeList(AddMessage);
                    }
                    break;
                }

                name = "rectToolRmark" + i;
                rect = FindName(name) as Rectangle;
                if (rect != null && rect.Visibility == Visibility.Visible)
                {
                    break;
                }

                name = "rectStageLmark" + i;
                rect = FindName(name) as Rectangle;
                if (rect != null && rect.Visibility == Visibility.Visible)
                {
                    break;
                }

                name = "rectStageRmark" + i;
                rect = FindName(name) as Rectangle;
                if (rect != null && rect.Visibility == Visibility.Visible)
                {
                    break;
                }
            }
        }

        private void btnMarkEnable_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnMarkDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMarkTest_Click(object sender, RoutedEventArgs e)
        {
            int w, h;
            int cx, cy;
            int srcW, srcH;
            IntPtr src;
            TS.ClearMarkResult(canvLeft);
            CAM_CON.GetLastFrame(out srcW, out srcH, out src);
            if (VisionDll.SequenceShot(1, AddMessage, out w, out h, out cx, out cy, srcW, srcH, src) == 0)
            {
                AddMessage(Marshal.StringToHGlobalAnsi("Mark match at x: " + (int)cx + ", y: " + (int)cy));
                TS.DrawMarkResult(canvLeft, new Point(CAM_CON.IMG_LT_X, CAM_CON.IMG_LT_Y), new Point((int)cx, (int)cy), CAM_CON.CURR_RATIO, w, h);
            }
            else
            {
                AddMessage(Marshal.StringToHGlobalAnsi("Mark test fail."));
            }
        }

        private void btnLarge_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnAmplify_Click(object sender, RoutedEventArgs e)
        {
            IS_AMPLIFY = !IS_AMPLIFY;
            if (IS_AMPLIFY)
            {
                btnAmplify.Background = new SolidColorBrush(Color.FromArgb(255, 190, 240, 140));
            }
            else
            {
                btnAmplify.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                CAM_CON.SetScale(ScaleSize.None, new Point(0, 0));
                TS.SetRect(new Point(0, 0), CAM_CON.CURR_RATIO);
            }
        }

        private void canvLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PT_LAST = e.GetPosition(this);
        }

        private void canvLeft_MouseEnter(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(imgLeft);
            LineSelect ls = TS.PointOnLine(pt);
            TS.ChangeSelect(ls);
        }

        private void canvLeft_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                TS.ChangeSelect(LineSelect.None);
            }
        }

        private void imgLeft_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IS_AMPLIFY)
            {
                Point pt = e.GetPosition(imgLeft);
                CAM_CON.Amplify(pt);
                TS.SetRect(new Point(CAM_CON.IMG_LT_X, CAM_CON.IMG_LT_Y), CAM_CON.CURR_RATIO);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point pt = e.GetPosition(this);
                TS.UpdateRect(canvLeft, pt.X - PT_LAST.X, pt.Y - PT_LAST.Y, new Point(CAM_CON.IMG_LT_X, CAM_CON.IMG_LT_Y), CAM_CON.CURR_RATIO);
                PT_LAST = pt;
            }
        }
    }
}
