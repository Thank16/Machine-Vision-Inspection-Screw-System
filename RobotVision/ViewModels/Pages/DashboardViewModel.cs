// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.
using NModbus;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Python.Runtime;
using RobotVision.Models;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Common;
using static OpenCvSharp.Stitcher;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Application = System.Windows.Application;
using Configuration = System.Configuration.Configuration;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;
using Size = OpenCvSharp.Size;

namespace RobotVision.ViewModels.Pages

{
    public partial class DashboardViewModel : ObservableObject
    {
        /// <summary>
        [ObservableProperty]
        public BitmapSource _img;

        [ObservableProperty]
        public SolidColorBrush _statuscon;

        [ObservableProperty]
        private bool _boolstart;

        [ObservableProperty]
        private bool _boolstop;

        [ObservableProperty]
        private string _model1;

        [ObservableProperty]
        private string _unit;

        [ObservableProperty]
        private string _symbol;

        [ObservableProperty]
        private string _symbolimg;

        [ObservableProperty]
        private string _proOK;

        [ObservableProperty]
        private string _proNG;

        [ObservableProperty]
        private string _statuscard;

        [ObservableProperty]
        private string _vis = "Hidden";

        [ObservableProperty]
        private string _labelNG = "0";

        [ObservableProperty]
        private string _labelOK = "0";

        [ObservableProperty]
        private string _mode;

        [ObservableProperty]
        private string _ct;

        /// </summary>
        private static IModbusMaster master;

        private static Mat frame1 = new Mat();
        private static Mat frame2 = new Mat();
        private static Mat frame3 = new Mat();
        private static bool v1 = false;
        private static bool v2 = false;
        private static bool v3 = false;
        private Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private Socket sendertcp;
        private string recvG50;
        private static bool video = true;
        private bool shocket = true;
        private utility utility = new utility();
        private DataViewModel DataViewModel = new DataViewModel();
        private int c = 0;
        private int c2 = 0;
        private int c3 = 0;

        public DashboardViewModel()
        {
            AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");
            Symbol = "Copy32";
            Symbolimg = "Copy32";
            ProOK = "0";
            ProNG = "0";
            Statuscard = "#FF2D2D2D";
            Model1 = "Model : ";
            Unit = "Screw : ";
            Boolstop = false;
            Boolstart = true;
            Runtime.PythonDLL = UISettingSection.pythonpath;
        }

        //camera

        private static void camera3(int indexleft, int indexmid, int indexrigth, int w, int h)
        {
            Thread left = new Thread(() => F1(indexleft, w, h));
            Thread mid = new Thread(() => F2(indexmid, w, h));
            Thread right = new Thread(() => F3(indexrigth, w, h));
            left.Start();
            mid.Start();
            right.Start();
            while (true)
            {
                Thread.Sleep(1000);
                if (v1 && v2 && v3)
                    break;
            }
        }

        private static void F1(int id, int w, int h)
        {
            var cap1 = utility.cap(id, w, h);

            while (video)
            {
                cap1.Read(frame1);
                v1 = true;
            }
            cap1.Dispose();
        }

        private static void F2(int id, int w, int h)
        {
            var cap2 = utility.cap(id, w, h);

            while (video)
            {
                cap2.Read(frame2);
                v2 = true;
            }
            cap2.Dispose();
        }

        private static void F3(int id, int w, int h)
        {
            var cap3 = utility.cap(id, w, h);

            while (video)
            {
                cap3.Read(frame3);
                v3 = true;
            }
            cap3.Dispose();
        }

        public static Mat EnlargeAndReplace(Mat image, Rect box, double scale = 5)
        {
            int height = image.Height;
            int width = image.Width;

            // Extract the region of interest (ROI) of the detected object
            Mat objectImg = new Mat(image, box);

            // Enlarge the object
            int objectHeight = objectImg.Rows;
            int objectWidth = objectImg.Cols;
            //int newHeight = (int)(objectHeight * scale);
            //int newWidth = (int)(objectWidth * scale);
            int newHeight = 100;
            int newWidth = 100;
            Mat enlargedObjectImg = new Mat();
            Cv2.Resize(objectImg, enlargedObjectImg, new OpenCvSharp.Size(newWidth, newHeight), 0, 0, InterpolationFlags.Linear);

            // Calculate new position for the enlarged object
            int centerX = (box.Left + box.Right) / 2;

            int centerY = ((box.Top + box.Bottom) / 2);
            if (centerY < 200)
            {
                centerY += 100;
            }
            else if (centerY >= 200)
            {
                centerY -= 100;
            }
            int newXmin = Math.Max(centerX - newWidth / 2, 0);
            int newYmin = Math.Max(centerY - newHeight / 2, 0);
            int newXmax = Math.Min(centerX + newWidth / 2, width);
            int newYmax = Math.Min(centerY + newHeight / 2, height);

            // Create an empty canvas to overlay the enlarged object
            Mat enlargedCanvas = Mat.Zeros(image.Size(), image.Type());
            int y1Offset = Math.Max(0, -newYmin);
            int x1Offset = Math.Max(0, -newXmin);
            int y2Offset = newHeight - Math.Max(0, newYmax - height);
            int x2Offset = newWidth - Math.Max(0, newXmax - width);

            // Create a copy of the original image to modify
            Mat outputImage = image.Clone();

            // Replace the region in the output image with the enlarged object
            Rect outputRoi = new Rect(newXmin, newYmin, newXmax - newXmin, newYmax - newYmin);
            enlargedObjectImg[new Rect(x1Offset, y1Offset, outputRoi.Width, outputRoi.Height)].CopyTo(outputImage[outputRoi]);
            Scalar greenColor = new Scalar(255, 255, 255);
            int thickness = 2;
            Cv2.Rectangle(outputImage, outputRoi, greenColor, thickness);
            return outputImage;
        }

        private void YOLO()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Vis = "Visible";
                });
                System.Configuration.Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");
                List<Modellist> listmodel = new List<Modellist>();
                List<string> Model = new List<string>();
                List<string> Paths = new List<string>();
                listmodel = ListSaveManager.LoadListFromFile("Modelpath.xml");
                foreach (var x in listmodel.Select(x => x.Models))
                {
                    Model.Add(x);
                }
                foreach (var x in listmodel.Select(x => x.Paths))
                {
                    Paths.Add(x);
                }
                int select = int.Parse(UISettingSection.Camera);
                int width = int.Parse(UISettingSection.Width);
                int height = int.Parse(UISettingSection.Height);
                int d1 = int.Parse(UISettingSection.sD1);
                int d2 = int.Parse(UISettingSection.sD2);
                int cc1 = int.Parse(UISettingSection.sC1);
                int cc2 = int.Parse(UISettingSection.sC2);
                int left = int.Parse(UISettingSection.left);
                int mid = int.Parse(UISettingSection.mid);
                int rigth = int.Parse(UISettingSection.rigth);
                int Point = UISettingSection.Point;
                float conf = float.Parse(UISettingSection.conf);
                float convert = float.Parse(UISettingSection.ptm);
                bool Oca = UISettingSection.Oca;
                bool cap = UISettingSection.Cap;
                float delay = float.Parse(UISettingSection.Delay);
                int fineModelIndex = Model.IndexOf(UISettingSection.Model);
                var degree = UISettingSection.Rotate;
                string Path = Paths[fineModelIndex];
                string Modell = Model[fineModelIndex];
                PythonEngine.Initialize();
                PythonEngine.RunSimpleString(@"
import sys
from io import StringIO
memory_buffer = StringIO()
sys.stdout = memory_buffer");
                dynamic cv2 = Py.Import("cv2");
                dynamic io = Py.Import("io");
                dynamic np = Py.Import("numpy");
                dynamic ultralytics = Py.Import("ultralytics");
                dynamic PIL = Py.Import("PIL");
                dynamic base64 = Py.Import("base64");

                c = 0;
                c2 = 0;
                c3 = 0;
                dynamic model = ultralytics.YOLO(Path);
                List<(float, float)> Point3 = new List<(float, float)>();
                List<(float, float)> Point21 = new List<(float, float)>();
                Stopwatch sw = new Stopwatch();
                //camera3(left, mid, rigth, width, height);
                sendVideo(null);
                Vis = "Hidden";
                Model1 = "Model : " + Modell;
                Unit = "Screw : " + Point + " unit";

                Boolstop = true;

                using (Py.GIL())
                {
                    while (video)
                    {
                        try
                        {
                            if (frame1 != null && frame2 != null && frame3 != null || true)
                            {
                                var combinedImage = new Mat();

                                Cv2.HConcat(frame1, frame2, combinedImage);
                                Cv2.HConcat(combinedImage, frame3, combinedImage);

                                //Mat normalFrame = combinedImage;
                                Mat normalFrame = Cv2.ImRead(@"C:\Users\tanak\Desktop\Check screw\New folder\image14.jpg");
                                // normalFrame.ConvertTo(normalFrame, -1, 1.2, 15);
                                Mat RAW = normalFrame.Clone();
                                byte[]? imageInBytes = RAW.ToBytes(".bmp");
                                dynamic image = PIL.Image.open(io.BytesIO(imageInBytes));
                                dynamic results = model(image, device: 0);
                                dynamic boxes = results[0].boxes;
                                dynamic cls = results[0].boxes.cls;
                                int widthf = normalFrame.Cols / 2;
                                int heightf = normalFrame.Rows / 2;
                                int NGc = 0;
                                int OKc = 0;
                                int Coutyolo = 0;
                                for (int i = 0; i < (int)(cls.size()[0]); i++)
                                {
                                    //MessageBox.Show(cls.size()[0]);
                                    float confi = (float)(results[0].boxes.conf[i]);
                                    int nameint = (int)(cls[i].item());
                                    dynamic names = results[0].names;
                                    string name = (string)(names[nameint]);
                                    HersheyFonts fonts = HersheyFonts.Italic;

                                    float fsize = 1f;
                                    int thi = 2;
                                    if (confi >= conf)
                                    {
                                        int x = (int)(boxes.xyxy[i][0].item());
                                        int y = (int)(boxes.xyxy[i][1].item());
                                        int width1 = (int)(boxes.xyxy[i][2].item()) - x;
                                        int height1 = (int)(boxes.xyxy[i][3].item()) - y;
                                        int cx = x + (width1 / 2);
                                        int cy = y + (height1 / 2);
                                        int r = ((width1 / 2) + (height1 / 2)) / 2;
                                        OpenCvSharp.Point center = new OpenCvSharp.Point(cx, cy);
                                        Rect box = new Rect(x, y, width1, height1);
                                        normalFrame = EnlargeAndReplace(normalFrame, box);
                                        if (name == "FG")
                                        {
                                            Coutyolo++;
                                            OKc++;
                                            Cv2.Circle(normalFrame, center, r * 2, Scalar.GreenYellow, 5);
                                            DrawTextWithBackground(normalFrame, $"{confi.ToString("F2")}", new Point((x + width1 * 2), (y + height1 + 10)),
                                                fonts, fsize, Scalar.Black, thi, Scalar.LightGreen);
                                            DrawTextWithBackground(normalFrame, $"OK", new Point((x + width1 * 2), (y)),
                                                fonts, fsize, Scalar.Black, thi, Scalar.LightGreen);
                                            //Cv2.PutText(normalFrame, $"{confi.ToString("F2")}", new Point((x + width1 * 2), (y + height1 + 10)), fonts, 1, Scalar.DarkGreen, thi);
                                            //Cv2.PutText(normalFrame, $"OK", new Point((x + width1 * 2), (y)), fonts, 1, Scalar.DarkGreen, thi);
                                        }
                                        else if (name == "NG")
                                        {
                                            NGc++;
                                            Cv2.Circle(normalFrame, center, r * 2, Scalar.Red, 5);
                                            Coutyolo++;
                                            DrawTextWithBackground(normalFrame, $"{confi.ToString("F2")}", new Point((x + width1 * 2), (y + height1 + 10)),
                                                fonts, fsize, Scalar.Black, thi, Scalar.LightPink);
                                            DrawTextWithBackground(normalFrame, $"NG", new Point((x + width1 * 2), (y)),
                                                fonts, fsize, Scalar.Black, thi, Scalar.LightPink);
                                        }
                                    }
                                }

                                if (Coutyolo > 1)
                                {
                                    if (c == 0)
                                    {
                                        sw.Start();

                                        c = 1;
                                    }

                                    if (sw.ElapsedMilliseconds / 1000 >= delay && Coutyolo == Point)
                                    {
                                        if (c2 == 0)
                                        {
                                            if (NGc > 0)//NG
                                            {
                                                string currentDirectory = Environment.CurrentDirectory;
                                                Symbol = "Dismiss12";
                                                Statuscard = "#FFF40000";
                                                LabelNG = NGc.ToString();
                                                LabelOK = OKc.ToString();
                                                ProNG = ((int)((NGc * 100) / Point)).ToString();
                                                ProOK = ((int)((OKc * 100) / Point)).ToString();
                                                //MessageBox.Show(ProNG.ToString(), ProOK);
                                                DataViewModel.Insert(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), Modell, LabelOK, LabelNG, "NG",
                                                    @$"{currentDirectory}/picNG/{DateTime.Now.ToString("ddMMyyyy,HH-mm")} NG.png");
                                                sendVideo(normalFrame);
                                                normalFrame.SaveImage(@$"{currentDirectory}/picNG/{DateTime.Now.ToString("ddMMyyyy,HH-mm")} NG.png");
                                                RAW.SaveImage(@$"{currentDirectory}/picRAW/{DateTime.Now.ToString("ddMMyyyy,HH-mm")} NG.png");
                                            }
                                            else if (NGc == 0)//OK
                                            {
                                                Symbol = "Checkmark12";
                                                Statuscard = "#FF30E354";
                                                LabelNG = NGc.ToString();
                                                LabelOK = OKc.ToString();
                                                ProNG = ((int)((NGc * 100) / Point)).ToString();
                                                ProOK = ((int)((OKc * 100) / Point)).ToString();
                                                DataViewModel.Insert(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), Modell, LabelOK, LabelNG, "OK", "");

                                                sendVideo(normalFrame);
                                            }
                                            c2 = 1;
                                        }
                                        sw.Stop();

                                        //sendVideo(normalFrame);
                                    }
                                    else if (sw.ElapsedMilliseconds / 1000 >= delay && Coutyolo != Point && c2 == 0)
                                    {
                                        sendVideo(normalFrame);
                                        //Symbolimg = "EyeTrackingOff20";
                                        sw.Stop();
                                    }
                                }
                                else
                                {
                                    sendVideo(null);

                                    Symbol = "Copy32";

                                    LabelNG = "0";
                                    LabelOK = "0";
                                    ProNG = "0";
                                    ProOK = "0";

                                    Statuscard = "#0FFFFFFF";
                                    Symbolimg = "Copy32";
                                    Thread.Sleep(500);
                                    Symbolimg = "ArrowStepInRight16";
                                    Thread.Sleep(500);
                                    sw.Reset();
                                    c = 0;
                                    c2 = 0;
                                    c3 = 0;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            utility.OnOpenCustomMessageBox("Home", ex.ToString());
                            PythonEngine.Shutdown();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                utility.OnOpenCustomMessageBox("Home", ex.ToString());
            }
            try
            {
                PythonEngine.Shutdown();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Boolstart = true;
                });
            }
            catch { }
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

        private static void DrawTextWithBackground(Mat image, string text, Point org, HersheyFonts font, double fontScale, Scalar textColor, int thickness, Scalar backgroundColor)
        {
            // คำนวณขนาดของข้อความ
            Size textSize = Cv2.GetTextSize(text, font, fontScale, thickness, out int baseline);

            // ตำแหน่งของพื้นหลังสี่เหลี่ยม
            Point backgroundOrg = new Point(org.X, org.Y - textSize.Height - baseline);

            // ขนาดของพื้นหลังสี่เหลี่ยม
            Size backgroundSize = new Size(textSize.Width, textSize.Height + baseline);

            // วาดพื้นหลังสี่เหลี่ยม
            Cv2.Rectangle(image, new Rect(backgroundOrg, backgroundSize), backgroundColor, -1);

            // วาดข้อความบนพื้นหลังสี่เหลี่ยม
            Cv2.PutText(image, text, org, font, fontScale, textColor, thickness);
        }

        //camera/

        //tcp
        private void runconnect()
        {
            Thread thread = new Thread(new ThreadStart(connecttcpAsync));
            thread.Start();
        }

        private void connecttcpAsync()
        {
            try
            {
                byte slaveAddress = 1;
                ushort startAddress = 100;
                ushort numOfPoints = 2;
                System.Configuration.Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");
                shocket = true;
                //tcp();
                string ipAddress = UISettingSection.IP;
                int port = int.Parse(UISettingSection.Port);
                TcpClient client = new TcpClient(ipAddress, port);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Statuscon = new BrushConverter().ConvertFromString("#FF55B82A") as SolidColorBrush;
                });
                // Create Modbus TCP master
                var factory = new ModbusFactory();
                master = factory.CreateMaster(client);

                while (shocket)
                {
                    ushort[] registers = master.ReadHoldingRegisters(slaveAddress, startAddress, numOfPoints);
                    recvG50 = utility.HexToFloat(registers[0], registers[1]).ToString();
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Statuscon = new BrushConverter().ConvertFromString("#FFC82020") as SolidColorBrush;
                });
                client.Close();
            }
            catch
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Statuscon = new BrushConverter().ConvertFromString("#FFC82020") as SolidColorBrush;
                });
                runconnect();
            }
        }

        private void sendtcp(float GV51, float GV52, float GV53)
        {
            byte slaveAddress = 1;
            ushort startAddress = 102;
            var (AD100, AD101) = utility.FloatToHex(GV51);
            var (AD102, AD103) = utility.FloatToHex(GV52);
            var (AD104, AD105) = utility.FloatToHex(GV53);
            ushort[] registers1 = new ushort[] { AD100, AD101, AD102, AD103, AD104, AD105 };
            master.WriteMultipleRegisters(slaveAddress, startAddress, registers1);
        }

        //tcp/
        //relay
        [RelayCommand]
        private void start()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Statuscon = new BrushConverter().ConvertFromString("#FF80B9EE") as SolidColorBrush;
            });
            runconnect();
            sendVideo(null);

            Symbol = "Copy32";

            LabelNG = "0";
            LabelOK = "0";
            ProNG = "0";
            ProOK = "0";

            Statuscard = "#0FFFFFFF";
            Boolstart = false;
            //Boolstop = true;
            video = true;
            Thread thread = new Thread(new ThreadStart(YOLO));
            thread.Start();
        }

        [RelayCommand]
        private void stop()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Model1 = "Stop";
            });
            shocket = false;
            Boolstop = false;
            video = false;
        }

        //relay/
    }
}