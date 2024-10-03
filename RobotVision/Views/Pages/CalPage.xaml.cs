// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using RobotVision.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace RobotVision.Views.Pages
{
    public partial class CalPage : INavigableView<CalViewmodel>
    {
        public CalViewmodel ViewModel { get; }

        public CalPage(CalViewmodel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}