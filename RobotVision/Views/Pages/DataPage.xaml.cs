// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using RobotVision.ViewModels.Pages;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Wpf.Ui.Controls;

namespace RobotVision.Views.Pages
{
    public partial class DataPage : INavigableView<DataViewModel>
    {
        public DataViewModel ViewModel { get; }

        public DataPage(DataViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
            ScrollToBottom();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                e.Handled = true;
            }catch (Exception ex) { }
        }

        private void ScrollToBottom()
        {
            if (dataGrid.Items.Count > 0)
            {
                dataGrid.ScrollIntoView(dataGrid.Items[dataGrid.Items.Count - 1]);
            }
        }
    }
}