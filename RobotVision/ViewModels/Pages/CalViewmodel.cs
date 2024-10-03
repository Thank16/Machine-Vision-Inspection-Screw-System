using OpenCvSharp;
using OpenCvSharp.Aruco;
using OpenCvSharp.WpfExtensions;
using RobotVision.Models;
using System.Configuration;
using System.Windows.Media.Imaging;
using Rect = OpenCvSharp.Rect;

namespace RobotVision.ViewModels.Pages
{
    public partial class CalViewmodel : ObservableObject
    {
        private System.Configuration.Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private static bool run;
        private Dictionary dictionary;

        //data binding
        [ObservableProperty]
        private string _d1;

        [ObservableProperty]
        private string _d2;

        [ObservableProperty]
        private string _l;

        [ObservableProperty]
        private string _m;

        [ObservableProperty]
        private string _r;

        [ObservableProperty]
        private bool _ostatus = true;

        [ObservableProperty]
        private bool _sstatus = false;

        [ObservableProperty]
        private int _cout;

        [ObservableProperty]
        private string _path;

        [ObservableProperty]
        private BitmapSource _img;

        private static double pixeltommj;
        private static Mat frame1 = new Mat();
        private static Mat frame2 = new Mat();
        private static Mat frame3 = new Mat();
        private static Mat normalFrame = new Mat();
        private static bool v1 = false;
        private static bool v2 = false;
        private static bool v3 = false;

        //data binding/
        //fx
        private utility utility = new utility();

        public CalViewmodel()
        {
            AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");
            L = UISettingSection.left;
            M = UISettingSection.mid;
            R = UISettingSection.rigth;
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

            while (run)
            {
                cap1.Read(frame1);
                v1 = true;
            }
            cap1.Dispose();
        }

        private static void F2(int id, int w, int h)
        {
            var cap2 = utility.cap(id, w, h);

            while (run)
            {
                cap2.Read(frame2);
                v2 = true;
            }
            cap2.Dispose();
        }

        private static void F3(int id, int w, int h)
        {
            var cap3 = utility.cap(id, w, h);

            while (run)
            {
                cap3.Read(frame3);
                v3 = true;
            }
            cap3.Dispose();
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
                camera3(left, mid, rigth, width, height);
                Sstatus = true;
                while (run)
                {
                    try
                    {
                        if (frame1 != null && frame2 != null && frame3 != null)
                        {
                            {
                                // Access the image data
                                var combinedImage = new Mat();

                                Cv2.HConcat(frame1, frame2, combinedImage);
                                Cv2.HConcat(combinedImage, frame3, combinedImage);
                                //Cv2.Resize(combinedImage, combinedImage, new OpenCvSharp.Size(combinedImage.Width * 0.8, combinedImage.Height * 0.8));

                                normalFrame = combinedImage;
                                sendVideo(normalFrame);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Ostatus = true;
                        utility.OnOpenCustomMessageBox("Camera", ex.ToString());
                        break;
                    }
                }

                Ostatus = true;
            }
            catch (Exception ex)
            {
                Ostatus = true;
                utility.OnOpenCustomMessageBox("Camera", ex.ToString());
            }
        }

        //fx/
        //relay command
        [RelayCommand]
        private void capture()
        {
            try
            {
                run = true;
                Ostatus = false;
                Thread thread = new Thread(new ThreadStart(YOLO));
                thread.Start();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        [RelayCommand]
        private void Stop()
        {
            run = false;
            sendVideo(null);
            Sstatus = false;
        }

        [RelayCommand]
        private void folder()
        {
            // Configure open folder dialog box
            Microsoft.Win32.OpenFolderDialog dialog = new();

            dialog.Multiselect = false;
            dialog.Title = "Select a folder";

            // Show open folder dialog box
            bool? result = dialog.ShowDialog();

            // Process open folder dialog box results
            if (result == true)
            {
                // Get the selected folder
                Path = dialog.FolderName;
                string folderNameOnly = dialog.SafeFolderName;
            }
        }

        [RelayCommand]
        private void Reset()
        {
            Cout = 0;
        }

        [RelayCommand]
        private void Save()
        {
            AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");
            normalFrame.SaveImage(@$"{Path}\image{Cout}.jpg");
            Cout++;

            //utility.OnOpenCustomMessageBox("Calibrate", $"Save values {Ptm}");
            //UISettingSection.ptm = Ptm.ToString();
            AppConfig.Save();
        }

        [RelayCommand]
        private void Save2()
        {
            AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");

            //utility.OnOpenCustomMessageBox("Calibrate", $"Save values {Ptm}");
            UISettingSection.left = L;
            UISettingSection.mid = M;
            UISettingSection.rigth = R;
            AppConfig.Save();
            utility.OnOpenCustomMessageBox("Camera", "saved successfully");
        }

        //relay command/
    }
}