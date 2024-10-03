using DocumentFormat.OpenXml.Office2010.Excel;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using OpenCvSharp.WpfExtensions;
using RobotVision.Models;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Point = OpenCvSharp.Point;

namespace RobotVision.ViewModels.Pages
{
    public partial class SensorViewModel : ObservableObject
    {
        private System.Configuration.Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private Dictionary dictionary;

        //data binding
        [ObservableProperty]
        private string _d1;

        [ObservableProperty]
        private string _d2;

        [ObservableProperty]
        private string _c1;

        [ObservableProperty]
        private string _c2;

        [ObservableProperty]
        private string _xp;

        [ObservableProperty]
        private string _yp;

        [ObservableProperty]
        private float _test1;

        [ObservableProperty]
        private float _test2;

        [ObservableProperty]
        private BitmapSource _img;

        private static double pixeltommj;
        private bool break1;

        [ObservableProperty]
        private bool _change = true;

        [ObservableProperty]
        private bool _save1 = false;

        //data binding/
        //fx
        private static bool run;

        private static Mat frame1 = new Mat();
        private static Mat frame2 = new Mat();
        private static Mat frame3 = new Mat();
        private static bool v1 = false;
        private static bool v2 = false;
        private static bool v3 = false;
        private utility utility = new utility();

        public SensorViewModel()
        {
            AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");
            D1 = UISettingSection.sD1;
            D2 = UISettingSection.sD2;
            C1 = UISettingSection.sC1;
            C2 = UISettingSection.sC2;

            /*Thread thread = new Thread(new ThreadStart(YOLO));
            thread.Start();*/
        }

        private void sendVideo(Mat mat)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (mat != null)
                {
                    Img = mat.ToBitmapSource();
                }
                else
                {
                    Img = null;
                }
            });
        }

        private static (string, string, string, string, string) read()
        {
            System.Configuration.Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");
            return (UISettingSection.mousep, UISettingSection.sD1, UISettingSection.sD2, UISettingSection.sC1, UISettingSection.sC2);
        }

        private int flotoint(float u1, int u2)
        {
            return (int)(u1 * u2);
        }

        private static void camera3(int indexleft, int indexmid, int indexrigth, int w, int h)
        {
            var cap1 = utility.cap(indexleft, w, h);
            var cap2 = utility.cap(indexmid, w, h);
            var cap3 = utility.cap(indexrigth, w, h);
            cap1.Read(frame1);
            cap2.Read(frame2);
            cap3.Read(frame3);
        }

        private void YOLO()
        {
            try
            {
                System.Configuration.Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");
                int select = int.Parse(UISettingSection.Camera);
                int width = int.Parse(UISettingSection.Width);
                int height = int.Parse(UISettingSection.Height);
                string degree = UISettingSection.Rotate;
                int left = int.Parse(UISettingSection.left);
                int mid = int.Parse(UISettingSection.mid);
                int rigth = int.Parse(UISettingSection.rigth);
                //camera3(left, mid, rigth, width, height);
                var cap1 = utility.cap(left, width, height);
                var cap2 = utility.cap(mid, width, height);
                var cap3 = utility.cap(rigth, width, height);
                Thread.Sleep(1000);
                cap1.Read(frame1);
                cap2.Read(frame2);
                cap3.Read(frame3);
                cap1.Dispose();
                cap2.Dispose();
                cap3.Dispose();
                int c10 = 0;
                try
                {
                    // Access the image data

                    break1 = false;

                    void senddata()
                    {
                        System.Configuration.Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");
                        //if (frame1 != null && frame2 != null && frame3 != null)
                        {
                            // Access the image data
                            var combinedImage = new Mat();

                            Cv2.HConcat(frame1, frame2, combinedImage);
                            Cv2.HConcat(combinedImage, frame3, combinedImage);
                            //Cv2.Resize(combinedImage, combinedImage, new OpenCvSharp.Size(combinedImage.Width * 0.8, combinedImage.Height * 0.8));

                            Mat normalFrame = combinedImage;
                            normalFrame = utility.ifrotate(normalFrame, degree);
                            int width1 = normalFrame.Cols;
                            int height1 = normalFrame.Rows;
                            int mode = Databackend.mode;
                            int dd1 = int.Parse(UISettingSection.sD1);
                            int dd2 = int.Parse(UISettingSection.sD2);
                            int cc1 = int.Parse(UISettingSection.sC1);
                            int cc2 = int.Parse(UISettingSection.sC2);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Xp = "X : " + ((int)(Databackend.X2 * width1)).ToString();
                                Yp = "Y : " + ((int)(Databackend.Y2 * height)).ToString();
                            });
                            /*if (mode == 1)
                            {
                                Point topLeft = new Point(flotoint(Databackend.X1, width1), flotoint(Databackend.Y1, height1));
                                Point bottomRight = new Point(flotoint(Databackend.X2, width1), flotoint(Databackend.Y2, height1));
                                Cv2.Rectangle(normalFrame, topLeft, bottomRight, Scalar.Red, 3);
                            }
                            if (Databackend.roi != null)
                            {
                                foreach ((float, float, float, float) roi in Databackend.roi)
                                {
                                    (float x1, float y1, float x2, float y2) = roi;
                                    Point topLeft = new Point(flotoint(x1, width1), flotoint(y1, height1));
                                    Point bottomRight = new Point(flotoint(x2, width1), flotoint(y2, height1));
                                    Cv2.Rectangle(normalFrame, topLeft, bottomRight, Scalar.Red, 3);
                                }
                            }*/
                            //Cv2.Circle(normalFrame, new OpenCvSharp.Point(Xp1, Yp1), 10, Scalar.SpringGreen, 9);
                            Cv2.Line(normalFrame, new OpenCvSharp.Point(dd1, 0), new OpenCvSharp.Point(dd1, height1), Scalar.Red, 3);
                            Cv2.Line(normalFrame, new OpenCvSharp.Point(dd2, 0), new OpenCvSharp.Point(dd2, height1), Scalar.Red, 3);
                            Cv2.PutText(normalFrame, "S1", new OpenCvSharp.Point(dd1 + 20, height1 / 2), HersheyFonts.Italic, 2, Scalar.White, 2);
                            Cv2.PutText(normalFrame, "S2", new OpenCvSharp.Point(dd2 + 20, height1 / 2), HersheyFonts.Italic, 2, Scalar.White, 2);
                            Cv2.Line(normalFrame, new OpenCvSharp.Point(cc1, 0), new OpenCvSharp.Point(cc1, height1), Scalar.Blue, 3);
                            Cv2.Line(normalFrame, new OpenCvSharp.Point(cc2, 0), new OpenCvSharp.Point(cc2, height1), Scalar.Blue, 3);
                            Cv2.PutText(normalFrame, "Crop1", new OpenCvSharp.Point(cc1 + 20, height1 / 2), HersheyFonts.Italic, 2, Scalar.White, 2);
                            Cv2.PutText(normalFrame, "Crop2", new OpenCvSharp.Point(cc2 + 20, height1 / 2), HersheyFonts.Italic, 2, Scalar.White, 2);
                            sendVideo(normalFrame);
                        }
                    }

                    while (true)
                    {
                        senddata();
                        if (break1)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                senddata();
                            }

                            break;
                        }
                    }
                    //sentVideo(normalFrame);
                }
                catch (Exception ex) { utility.OnOpenCustomMessageBox("Sensor", ex.ToString()); }
            }
            catch (Exception ex) { utility.OnOpenCustomMessageBox("Sensor", ex.ToString()); }
        }

        //fx/
        //relay command
        [RelayCommand]
        private void capture()
        {
            //AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");
            //YOLO();
            Change = false;
            Save1 = true;
            Thread thread = new Thread(new ThreadStart(YOLO));
            thread.Start();
        }

        [RelayCommand]
        private void Save()
        {
            System.Configuration.Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");

            //utility.OnOpenCustomMessageBox("Calibrate", $"Save values {pixeltommj}");
            UISettingSection.sD1 = D1.ToString();
            UISettingSection.sD2 = D2.ToString();
            UISettingSection.sC1 = C1.ToString();
            UISettingSection.sC2 = C2.ToString();
            AppConfig.Save();
            break1 = true;
            Change = true;
            Save1 = false;
        }

        //relay command/
    }
}