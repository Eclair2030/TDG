using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using uEye;

namespace TDG_Vision
{
    internal class CameraControl
    {
        public delegate void CameraViewCallback(int width, int height, int dpiX, int dpiY, System.Windows.Media.PixelFormat pf, IntPtr data, int size, int stride);
        //public delegate void CameraViewCallback(IntPtr frame);

        public CameraViewCallback ShowImage;
        private int IMG_WIDTH, IMG_HEIGHT;                      //原图尺寸
        private int IMG_W, IMG_H;                                           //像框内的原图尺寸
        private double SHOW_WIDTH, SHOW_HEIGHT;     //像框尺寸
        public int IMG_LT_X, IMG_LT_Y;                                 //像框左上角在原图上的位置
        private ScaleSize SCALE;                                                //当前缩放档位
        private Camera CAMERA_LEFT, CAMERA_RIGHT;
        private bool LIVE_LEFT, LIVE_RIGHT;
        private const int NUMBER_SEQ_BUFFERS = 3;
        public double CURR_RATIO;

        public CameraControl()
        {
            LIVE_LEFT = false;
            LIVE_RIGHT = false;
            SCALE = ScaleSize.None;
            IMG_LT_X = IMG_LT_Y = 0;
            CURR_RATIO = 0;
        }

        public int InitCamera(CameraViewCallback callBack, double width, double height)
        {
            int result = 0;
            ShowImage = new CameraViewCallback(callBack);
            SHOW_WIDTH = width;
            SHOW_HEIGHT = height;

            CAMERA_LEFT = new Camera();
            uEye.Defines.Status statusRet = 0;

            // Open Camera
            statusRet = CAMERA_LEFT.Init(0);
            if (statusRet != uEye.Defines.Status.Success)
            {
                return 2;
            }

            // Set color mode
            statusRet = CAMERA_LEFT.PixelFormat.Set(uEye.Defines.ColorMode.Mono8);
            if (statusRet != uEye.Defines.Status.Success)
            {
                return 2;
            }

            CAMERA_LEFT.Timing.PixelClock.Set(60);
            //CAMERA_LEFT.Timing.Exposure.Set(300);
            //CAMERA_LEFT.Timing.Framerate.Set(28);
            int x, y;
            CAMERA_LEFT.Size.AOI.Get(out x, out y, out IMG_WIDTH, out IMG_HEIGHT);
            CURR_RATIO = SetScale(ScaleSize.None, new Point(0, 0));

            // Allocate Memory
            statusRet = AllocImageMems();
            if (statusRet != uEye.Defines.Status.Success)
            {
                return 2;
            }

            statusRet = InitSequence();
            if (statusRet != uEye.Defines.Status.Success)
            {
                return 2;
            }

            // Start Live Video
            statusRet = CAMERA_LEFT.Acquisition.Capture();
            if (statusRet != uEye.Defines.Status.Success)
            {
                return 2;
            }
            else
            {
                LIVE_LEFT = true;
            }

            // Connect Event
            CAMERA_LEFT.EventFrame += CAMERA_LEFT_EventFrame;

            //CB_Auto_Gain_Balance.Enabled = CAMERA_LEFT.AutoFeatures.Software.Gain.Supported;
            //CB_Auto_White_Balance.Enabled = CAMERA_LEFT.AutoFeatures.Software.WhiteBalance.Supported;
            return result;
        }

        public int ReleaseCamera()
        {
            int result = 0;
            CAMERA_LEFT.Acquisition.Stop();
            //ClearSequence();
            //FreeImageMems();
            //CAMERA_LEFT.Exit();

            return result;
        }

        public void ShowSnap(CameraViewCallback Show)
        {
            int s32MemID;
            uEye.Defines.Status statusRet = CAMERA_LEFT.Memory.GetLast(out s32MemID);
            if ((uEye.Defines.Status.SUCCESS == statusRet) && (0 < s32MemID))
            {
                if (uEye.Defines.Status.SUCCESS == CAMERA_LEFT.Memory.Lock(s32MemID))
                {
                    int w, h;
                    byte[] data;
                    CAMERA_LEFT.Memory.GetWidth(s32MemID, out w);
                    CAMERA_LEFT.Memory.GetHeight(s32MemID, out h);
                    CAMERA_LEFT.Memory.CopyToArray(s32MemID, out data);
                    System.Windows.Media.PixelFormat pf = PixelFormats.Gray8;
                    int stride = (w * pf.BitsPerPixel + 7) / 8;
                    try
                    {
                        //Show(w, h, 72, 72, pf, data, stride);
                    }
                    catch (Exception)
                    {
                    }
                    CAMERA_LEFT.Memory.Unlock(s32MemID);
                }
            }
        }

        public int ClipMark(string path, Int32Rect rect)
        {
            int result = -1;
            int s32MemID;
            uEye.Defines.Status statusRet = CAMERA_LEFT.Memory.GetLast(out s32MemID);
            if ((uEye.Defines.Status.SUCCESS == statusRet) && (0 < s32MemID))
            {
                if (uEye.Defines.Status.SUCCESS == CAMERA_LEFT.Memory.Lock(s32MemID))
                {
                    int w, h;
                    byte[] data;
                    CAMERA_LEFT.Memory.GetWidth(s32MemID, out w);
                    CAMERA_LEFT.Memory.GetHeight(s32MemID, out h);
                    CAMERA_LEFT.Memory.CopyToArray(s32MemID, out data);
                    System.Windows.Media.PixelFormat pf = PixelFormats.Gray8;
                    int stride = (w * pf.BitsPerPixel + 7) / 8;

                    try
                    {
                        BitmapSource bs = BitmapSource.Create(w, h, 72, 72, pf, null, data, stride);
                        Int32Rect rectInSource = new Int32Rect(Convert.ToInt32(rect.X / SHOW_WIDTH * IMG_W) + IMG_LT_X, Convert.ToInt32(rect.Y / SHOW_HEIGHT * IMG_H) + IMG_LT_Y,
                            Convert.ToInt32(rect.Width / SHOW_WIDTH * IMG_W), Convert.ToInt32(rect.Height / SHOW_HEIGHT * IMG_H));
                        CroppedBitmap cb = new CroppedBitmap(bs, rectInSource);
                        PngBitmapEncoder be = new PngBitmapEncoder();
                        be.Frames.Add(BitmapFrame.Create(cb));
                        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                        be.Save(fs);
                        fs.Close();
                        result = 0;
                    }
                    catch (Exception)
                    {
                        result = 2;
                    }
                    CAMERA_LEFT.Memory.Unlock(s32MemID);
                }
            }
            return result;
        }

        public void GetLastFrame(out int width, out int height, out IntPtr frame)
        {
            int s32MemID;
            width = 0;
            height = 0;
            frame = IntPtr.Zero;
            uEye.Defines.Status statusRet = CAMERA_LEFT.Memory.GetLast(out s32MemID);
            if ((uEye.Defines.Status.SUCCESS == statusRet) && (0 < s32MemID))
            {
                if (uEye.Defines.Status.SUCCESS == CAMERA_LEFT.Memory.Lock(s32MemID))
                {
                    CAMERA_LEFT.Memory.GetWidth(s32MemID, out width);
                    CAMERA_LEFT.Memory.GetHeight(s32MemID, out height);
                    CAMERA_LEFT.Memory.GetLast(out frame);
                    CAMERA_LEFT.Memory.Unlock(s32MemID);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="ptClick">图像框中的鼠标点击位置坐标</param>
        /// <returns></returns>
        public double SetScale(ScaleSize ss, Point ptClick)
        {
            Point ptClickOnSource = new Point();
            ptClickOnSource.X = Convert.ToInt32(ptClick.X / SHOW_WIDTH * IMG_W) + IMG_LT_X;
            ptClickOnSource.Y = Convert.ToInt32(ptClick.Y / SHOW_HEIGHT * IMG_H) + IMG_LT_Y;
            SCALE = ss;
            switch (ss)
            {
                case ScaleSize.None:
                    IMG_W = IMG_WIDTH;
                    IMG_H = IMG_HEIGHT;
                    IMG_LT_X = 0;
                    IMG_LT_Y = 0;
                    break;
                case ScaleSize.Large1:
                    IMG_W = IMG_WIDTH * 3 / 4;
                    IMG_H = IMG_HEIGHT * 3 / 4;
                    IMG_LT_X = Convert.ToInt32(ptClickOnSource.X) - Convert.ToInt32(IMG_W / 2) > 0 ? 
                        (Convert.ToInt32(ptClickOnSource.X) - Convert.ToInt32(IMG_W / 2) < IMG_WIDTH - IMG_W ?
                        Convert.ToInt32(ptClickOnSource.X) - Convert.ToInt32(IMG_W / 2) : IMG_WIDTH - IMG_W)
                        : 0;
                    IMG_LT_Y = Convert.ToInt32(ptClickOnSource.Y) - Convert.ToInt32(IMG_H / 2) > 0 ? 
                        (Convert.ToInt32(ptClickOnSource.Y) - Convert.ToInt32(IMG_H / 2) < IMG_HEIGHT - IMG_H ?
                        Convert.ToInt32(ptClickOnSource.Y) - Convert.ToInt32(IMG_H / 2) : IMG_HEIGHT - IMG_H)
                        : 0;
                    break;
                case ScaleSize.Large2:
                    IMG_W = IMG_WIDTH / 2;
                    IMG_H = IMG_HEIGHT / 2;
                    IMG_LT_X = Convert.ToInt32(ptClickOnSource.X) - Convert.ToInt32(IMG_W / 2) > 0 ?
                        (Convert.ToInt32(ptClickOnSource.X) - Convert.ToInt32(IMG_W / 2) < IMG_WIDTH - IMG_W ?
                        Convert.ToInt32(ptClickOnSource.X) - Convert.ToInt32(IMG_W / 2) : IMG_WIDTH - IMG_W)
                        : 0;
                    IMG_LT_Y = Convert.ToInt32(ptClickOnSource.Y) - Convert.ToInt32(IMG_H / 2) > 0 ?
                        (Convert.ToInt32(ptClickOnSource.Y) - Convert.ToInt32(IMG_H / 2) < IMG_HEIGHT - IMG_H ?
                        Convert.ToInt32(ptClickOnSource.Y) - Convert.ToInt32(IMG_H / 2) : IMG_HEIGHT - IMG_H)
                        : 0;
                    break;
                case ScaleSize.LargeMax:
                    IMG_W = IMG_WIDTH / 4;
                    IMG_H = IMG_HEIGHT / 4;
                    IMG_LT_X = Convert.ToInt32(ptClickOnSource.X) - Convert.ToInt32(IMG_W / 2) > 0 ?
                        (Convert.ToInt32(ptClickOnSource.X) - Convert.ToInt32(IMG_W / 2) < IMG_WIDTH - IMG_W ?
                        Convert.ToInt32(ptClickOnSource.X) - Convert.ToInt32(IMG_W / 2) : IMG_WIDTH - IMG_W)
                        : 0;
                    IMG_LT_Y = Convert.ToInt32(ptClickOnSource.Y) - Convert.ToInt32(IMG_H / 2) > 0 ?
                        (Convert.ToInt32(ptClickOnSource.Y) - Convert.ToInt32(IMG_H / 2) < IMG_HEIGHT - IMG_H ?
                        Convert.ToInt32(ptClickOnSource.Y) - Convert.ToInt32(IMG_H / 2) : IMG_HEIGHT - IMG_H)
                        : 0;
                    break;
                default:
                    break;
            }
            CURR_RATIO = IMG_W / SHOW_WIDTH;
            return CURR_RATIO;
        }

        public ScaleSize GetScale()
        {
            return SCALE;
        }

        public Point GetLT()
        {
            return new Point();
        }

        public void Amplify(Point ptClick)
        {
            if (SCALE != ScaleSize.LargeMax)
            {
                SCALE += 1;
                SetScale(SCALE, ptClick);
            }
        }

        private void CAMERA_LEFT_EventFrame(object sender, EventArgs e)
        {
            Camera Camera = sender as Camera;
            IntPtr frame;
            Camera.Memory.GetLast(out frame);
            System.Windows.Media.PixelFormat pf = PixelFormats.Gray8;
            int stride = (IMG_WIDTH * pf.BitsPerPixel + 7) / 8;

            BitmapSource bs = BitmapSource.Create(IMG_WIDTH, IMG_HEIGHT, 72, 72, pf, null, frame, IMG_WIDTH * IMG_HEIGHT, stride);
            IntPtr img = Marshal.AllocHGlobal(IMG_WIDTH * IMG_HEIGHT);
            bs.CopyPixels(new Int32Rect(IMG_LT_X, IMG_LT_Y, IMG_W, IMG_H), img, IMG_WIDTH * IMG_HEIGHT, stride);

            try
            {
                ShowImage(IMG_W, IMG_H, 72, 72, pf, img, IMG_WIDTH * IMG_HEIGHT, stride);
            }
            catch (Exception)
            {
            }
            finally
            {
                Marshal.FreeHGlobal(img);
            }
        }

        private uEye.Defines.Status AllocImageMems()
        {
            uEye.Defines.Status statusRet = uEye.Defines.Status.SUCCESS;
            for (int i = 0; i < NUMBER_SEQ_BUFFERS; i++)
            {
                statusRet = CAMERA_LEFT.Memory.Allocate();
                if (statusRet != uEye.Defines.Status.SUCCESS)
                {
                    FreeImageMems();
                }
            }
            return statusRet;
        }

        private uEye.Defines.Status FreeImageMems()
        {
            int[] idList;
            uEye.Defines.Status statusRet = uEye.Defines.Status.NoSuccess;
            if (CAMERA_LEFT.Memory.IsOpened)
            {
                statusRet = CAMERA_LEFT.Memory.GetList(out idList);
                if (uEye.Defines.Status.SUCCESS == statusRet)
                {
                    foreach (int nMemID in idList)
                    {
                        do
                        {
                            statusRet = CAMERA_LEFT.Memory.Free(nMemID);
                            if (uEye.Defines.Status.SEQ_BUFFER_IS_LOCKED == statusRet)
                            {
                                Thread.Sleep(1);
                                continue;
                            }
                            break;
                        }
                        while (true);
                    }
                }
            }            
            return statusRet;
        }

        private uEye.Defines.Status InitSequence()
        {
            int[] idList;
            uEye.Defines.Status statusRet = CAMERA_LEFT.Memory.GetList(out idList);
            if (uEye.Defines.Status.SUCCESS == statusRet)
            {
                statusRet = CAMERA_LEFT.Memory.Sequence.Add(idList);
                if (uEye.Defines.Status.SUCCESS != statusRet)
                {
                    ClearSequence();
                }
            }
            return statusRet;
        }

        private uEye.Defines.Status ClearSequence()
        {
            if (CAMERA_LEFT != null && CAMERA_LEFT.Memory != null && CAMERA_LEFT.Memory.Sequence != null && CAMERA_LEFT.Memory.Sequence.IsOpened)
            {
                return CAMERA_LEFT.Memory.Sequence.Clear();
            }
            return uEye.Defines.Status.NoSuccess;
        }

    }

    public enum ScaleSize
    {
        None = 0,
        Large1 = 1,
        Large2 = 2,
        LargeMax = 3
    }
}
