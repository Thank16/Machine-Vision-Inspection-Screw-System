// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using RobotVision.Models;
using RobotVision.ViewModels.Pages;
using System.Configuration;
using System.Numerics;
using System.Windows.Input;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace RobotVision.Views.Pages
{
    public partial class SensorPage : INavigableView<SensorViewModel>
    {
        public SensorViewModel ViewModel { get; }

        public SensorPage(SensorViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        private static int mode = 0;
        private float x1 = 0f; private float y1 = 0f; private float x2 = 0f; private float y2 = 0f;

        /*public void image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(image);
            float Xmouse = (float)p.X / (float)image.ActualWidth;
            float Ymouse = (float)p.Y / (float)image.ActualHeight;
            Databackend.mode += 1;
            if (Databackend.mode == 1)
            {
                x1 = Xmouse; y1 = Ymouse;
                Databackend.X1 = x1; Databackend.Y1 = y1;
            }
            else if (Databackend.mode == 2)
            {
                x2 = Xmouse; y2 = Ymouse;
                //MessageBox.Show("INPUT");
                Databackend.roi.Add((x1, y1, x2, y2));
                Databackend.mode = 0;
            }
        }*/

        private void image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point p = e.GetPosition(image);
            float Xmouse = (float)p.X / (float)image.ActualWidth;
            float Ymouse = (float)p.Y / (float)image.ActualHeight;
            if (mode == 1)
            {
                D_1.Text = p.X.ToString();
            }
            Databackend.X2 = Xmouse;
            Databackend.Y2 = Ymouse;
            //UISettingSection.mousep = $"{Xmouse},{Ymouse}";
            //test.Content = $"{Xmouse},{Ymouse}";
        }

        private void D_1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("fsdf");
            mode = 1;
        }
    }
}