﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Collections.ObjectModel;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace RobotVision.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "Vision Inspection Screw";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = "Data",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.DataPage)
            },

            new NavigationViewItem()
            {
                Content = "Model",
                Icon = new SymbolIcon { Symbol = SymbolRegular.AddSquare20 },
                TargetPageType = typeof(Views.Pages.AddPage)
            },
            new NavigationViewItem()
            {
                Content = "Camera",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Camera16 },
                TargetPageType = typeof(Views.Pages.CalPage)
            },
            /*new NavigationViewItem()
            {
                Content = "Sensor",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Options16 },
                TargetPageType = typeof(Views.Pages.SensorPage)
            }*/
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}