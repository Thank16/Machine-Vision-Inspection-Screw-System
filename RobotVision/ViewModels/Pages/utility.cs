
using OpenCvSharp;

namespace RobotVision.ViewModels.Pages
{
    public class utility
    {
        public static (ushort, ushort) FloatToHex(float f)
        {
            // Convert floating-point number to binary representation
            byte[] byteArray = BitConverter.GetBytes(f);

            // Split the binary representation into front and back parts
            ushort frontHex = BitConverter.ToUInt16(byteArray, 0);
            ushort backHex = BitConverter.ToUInt16(byteArray, 2);

            return (frontHex, backHex);
        }

        public static float HexToFloat(ushort frontHex, ushort backHex)
        {
            // Combine front and back parts to form the binary representation
            byte[] byteArray = new byte[4];
            BitConverter.GetBytes(frontHex).CopyTo(byteArray, 0);
            BitConverter.GetBytes(backHex).CopyTo(byteArray, 2);

            // Convert binary representation back to floating-point number
            float floatValue = BitConverter.ToSingle(byteArray, 0);

            return floatValue;
        }

        public Mat CreateBitmapSource(byte[] rgbData, int width, int height)
        {
            // Create Mat object from BayerBG8 image data
            Mat bayerImage = new Mat(height, width, MatType.CV_8UC1);
            bayerImage.SetArray(rgbData);

            // Convert BayerBG8 to BGR
            Mat bgrImage = new Mat();
            Cv2.CvtColor(bayerImage, bgrImage, ColorConversionCodes.BayerBG2BGR);

            return bgrImage;
        }

        

        public static List<(float, float)> Swap(List<(float, float)> numbers, int index1, int index2)
        {
            (float, float) first = numbers[index1]; // Storing the value of the first element in a temporary variable
            numbers[index1] = numbers[index2]; // Assigning the value of the last element to the first element
            numbers[index2] = first; // Assigning the value of the temporary variable to the last element

            return numbers; // Returning the modified array
        }

        public static int difer(int a, int b)
        {
            int dif = (int)(Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2)));

            return dif; // Returning the modified array
        }

        private static Mat cropimage(Mat image, OpenCvSharp.Rect cropRect)
        {
            Mat croppedImage = image.SubMat(cropRect);
            return croppedImage;
        }

        public static VideoCapture cap(int index, int w, int h)
        {
            VideoCapture cap = new VideoCapture(index, VideoCaptureAPIs.DSHOW);
            cap.Set(VideoCaptureProperties.FrameWidth, w);
            cap.Set(VideoCaptureProperties.FrameHeight, h);
            cap.Set(VideoCaptureProperties.Fps, 16.0);
            cap.Set(VideoCaptureProperties.FourCC, FourCC.MJPG);
            return cap;
        }

        public Mat ifrotate(Mat normalFrame, string degree)
        {
            if ("90 degree" == degree)
            {
                Cv2.Rotate(normalFrame, normalFrame, RotateFlags.Rotate90Clockwise);
            }
            else if ("180 degree" == degree)
            {
                Cv2.Rotate(normalFrame, normalFrame, RotateFlags.Rotate180);
            }
            else if ("270 degree" == degree)
            {
                Cv2.Rotate(normalFrame, normalFrame, RotateFlags.Rotate90Counterclockwise);
            }
            return normalFrame;
        }

        public void OnOpenCustomMessageBox(string title, string content)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = title,
                    Content =
                    content,
                    CloseButtonText = "OK",
                };

                var result = await uiMessageBox.ShowDialogAsync();
            });
        }
    }
}