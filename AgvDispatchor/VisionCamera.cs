using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uEye;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows;

namespace AgvDispatchor
{
    internal class VisionCamera
    {
        public VisionCamera(RenderImage RendImg)
        {
            ShowImage = RendImg;
        }

        public bool CameraInit()
        {
            bool result = false;
            Cam = new Camera();
            uEye.Defines.Status statusRet = Cam.Init(0);
            if (statusRet == uEye.Defines.Status.Success)
            {
                // Set color mode
                statusRet = Cam.PixelFormat.Set(uEye.Defines.ColorMode.Mono8);
                if (statusRet == uEye.Defines.Status.Success)
                {
                    Cam.Timing.PixelClock.Set(20);
                    Cam.Timing.Exposure.Set(30);
                    Cam.Timing.Framerate.Set(50);
                    int x, y;
                    Cam.Size.AOI.Get(out x, out y, out Width, out Height);

                    // Allocate Memory
                    statusRet = AllocImageMems();
                    if (statusRet != uEye.Defines.Status.Success)
                    {
                    }

                    statusRet = InitSequence();
                    if (statusRet != uEye.Defines.Status.Success)
                    {
                    }

                    // Start Live Video
                    statusRet = Cam.Acquisition.Capture();
                    if (statusRet == uEye.Defines.Status.Success)
                    {
                        // Connect Event
                        Cam.EventFrame += CAMERA_EventFrame;
                        result = true;
                    }


                }
            }
            return result;
        }

        public void CameraClose()
        {
            if (Cam != null)
            {
                Cam.Acquisition.Stop();
                Cam.Exit();
            }
        }

        private uEye.Defines.Status AllocImageMems()
        {
            uEye.Defines.Status statusRet = uEye.Defines.Status.SUCCESS;
            for (int i = 0; i < 3; i++)
            {
                statusRet = Cam.Memory.Allocate();
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
            if (Cam.Memory.IsOpened)
            {
                statusRet = Cam.Memory.GetList(out idList);
                if (uEye.Defines.Status.SUCCESS == statusRet)
                {
                    foreach (int nMemID in idList)
                    {
                        do
                        {
                            statusRet = Cam.Memory.Free(nMemID);
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
            uEye.Defines.Status statusRet = Cam.Memory.GetList(out idList);
            if (uEye.Defines.Status.SUCCESS == statusRet)
            {
                statusRet = Cam.Memory.Sequence.Add(idList);
                if (uEye.Defines.Status.SUCCESS != statusRet)
                {
                    ClearSequence();
                }
            }
            return statusRet;
        }

        private uEye.Defines.Status ClearSequence()
        {
            if (Cam != null && Cam.Memory != null && Cam.Memory.Sequence != null && Cam.Memory.Sequence.IsOpened)
            {
                return Cam.Memory.Sequence.Clear();
            }
            return uEye.Defines.Status.NoSuccess;
        }

        public void GetLastFrame(out int coordX, out int coordY)
        {
            int s32MemID;
            coordX = 0;
            coordY = 0;
            uEye.Defines.Status statusRet = Cam.Memory.GetLast(out s32MemID);
            if ((uEye.Defines.Status.SUCCESS == statusRet) && (0 < s32MemID))
            {
                if (uEye.Defines.Status.SUCCESS == Cam.Memory.Lock(s32MemID))
                {
                    int w, h;
                    IntPtr frame;
                    Cam.Memory.GetWidth(s32MemID, out w);
                    Cam.Memory.GetHeight(s32MemID, out h);
                    Cam.Memory.GetLast(out frame);
                    Cam.Memory.Unlock(s32MemID);
                    DLL.FindStaff(null, out coordX, out coordY, w, h, frame);
                }
            }
        }

        private void CAMERA_EventFrame(object sender, EventArgs e)
        {
            Camera Camera = sender as Camera;
            IntPtr frame;
            Camera.Memory.GetLast(out frame);
            System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Gray8;
            int stride = (Width * pf.BitsPerPixel + 7) / 8;

            BitmapSource bs = BitmapSource.Create(Width, Height, 72, 72, pf, null, frame, Width * Height, stride);
            IntPtr img = Marshal.AllocHGlobal(Width * Height);
            bs.CopyPixels(new Int32Rect(0, 0, Width, Height), img, Width * Height, stride);

            try
            {
                ShowImage(Width, Height, 72, 72, pf, img, Width * Height, stride);
            }
            catch (Exception)
            {
            }
            finally
            {
                Marshal.FreeHGlobal(img);
            }
        }

        private Camera Cam;
        private int Width, Height;
        private RenderImage ShowImage;
    }
}
