// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using RobotVision.ViewModels.Windows;
using System.Diagnostics;
using Wpf.Ui.Controls;

namespace RobotVision.Views.Windows
{
    public partial class MainWindow
    {
        public MainWindowViewModel ViewModel { get; }

        public MainWindow(
            MainWindowViewModel viewModel,
            INavigationService navigationService,
            IServiceProvider serviceProvider,
            ISnackbarService snackbarService,
            IContentDialogService contentDialogService
        )
        {
            Wpf.Ui.Appearance.Watcher.Watch(this);

            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

            navigationService.SetNavigationControl(NavigationView);
            snackbarService.SetSnackbarPresenter(SnackbarPresenter);
            contentDialogService.SetContentPresenter(RootContentDialog);

            NavigationView.SetServiceProvider(serviceProvider);
        }

        private void FluentWindow_Closed(object sender, EventArgs e)
        {

            string programName = "RobotVision.exe"; // Replace with the name of the program you want to shut down

            // Check if the program is currently running
            Process[] processes = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(programName));

            if (processes.Length > 0)
            {
                // Close all instances of the program
                foreach (var process in processes)
                {
                    process.CloseMainWindow(); // Close the main window (if possible)


                    if (!process.HasExited)
                    {
                        // If the process did not exit, kill it forcefully
                        process.Kill();
                    }
                }

            }
        }
    }
}
