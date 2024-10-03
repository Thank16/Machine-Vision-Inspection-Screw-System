// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using RobotVision.Models;
using System.Collections.ObjectModel;
using System.Configuration;
using Wpf.Ui.Controls;
using Configuration = System.Configuration.Configuration;

namespace RobotVision.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        //binding var
        [ObservableProperty]
        private string _camera;

        [ObservableProperty]
        private string[] _rotatelist = new string[] { "0 degree", "90 degree", "180 degree", "270 degree" };

        [ObservableProperty]
        private string _rotate;

        [ObservableProperty]
        private string _width;

        [ObservableProperty]
        private string _heigth;

        [ObservableProperty]
        private string _model;

        [ObservableProperty]
        private string _conf;

        [ObservableProperty]
        private int _point;

        [ObservableProperty]
        private string _delay;

        [ObservableProperty]
        private bool _default;

        [ObservableProperty]
        private bool _cap;

        [ObservableProperty]
        private bool _oca;

        [ObservableProperty]
        private bool _light;

        [ObservableProperty]
        private bool _dark;

        [ObservableProperty]
        private string _model1;

        [ObservableProperty]
        private string _ip;

        [ObservableProperty]
        private string _port;

        [ObservableProperty]
        private string _pythonpath;

        [ObservableProperty]
        private List<string> _modelcombo;

        //binding/
        //main()
        private utility utility = new utility();

        public SettingsViewModel()
        {
            if (AppConfig.Sections["UISettings"] is null)
            {
                AppConfig.Sections.Add("UISettings", new AppConfig());
            }
            AppConfig.Save(ConfigurationSaveMode.Modified);
            reff();
        }

        private void selectmodel()
        {
            List<Modellist> list = new List<Modellist>();
            List<string> model = new List<string>();
            list = ListSaveManager.LoadListFromFile("Modelpath.xml");

            foreach (var x in list.Select(x => x.Models))
            {
                model.Add(x);
            }
            Modelcombo = model;
        }

        private void reff()
        {
            try
            {
                selectmodel();
                AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");

                Width = UISettingSection.Width;
                Heigth = UISettingSection.Height;
                Conf = UISettingSection.conf;
                Point = UISettingSection.Point;
                Delay = UISettingSection.Delay;
                Rotate = UISettingSection.Rotate;
                Camera = UISettingSection.Camera;
                Model = UISettingSection.Model;
                Ip = UISettingSection.IP;
                Port = UISettingSection.Port;
                Default = UISettingSection.Default;
                Cap = UISettingSection.Cap;
                Oca = UISettingSection.Oca;
                Pythonpath = UISettingSection.pythonpath;
                Light = UISettingSection.Light;
                Dark = UISettingSection.Dark;
            }
            catch (Exception ex) { }
        }

        //main/
        //relay
        [RelayCommand]
        private void openflie()
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Python.dll"; // Default file name
            dialog.DefaultExt = ".dll"; // Default file extension
            dialog.Filter = "Python (dll)|*.dll"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                Pythonpath = filename;
            }
        }

        [RelayCommand]
        private void refresh()
        {
            reff();
        }

        [RelayCommand]
        private void savevalues()
        {
            try
            {
                utility.OnOpenCustomMessageBox("Setting", "Save values complete");
                AppConfig UISettingSection = (AppConfig)AppConfig.GetSection("UISettings");
                UISettingSection.Width = Width;
                UISettingSection.Height = Heigth;
                UISettingSection.conf = Conf;
                UISettingSection.Point = Point;
                UISettingSection.Delay = Delay;
                UISettingSection.Rotate = Rotate;
                UISettingSection.Camera = Camera;
                UISettingSection.Model = Model;
                UISettingSection.IP = Ip;
                UISettingSection.Port = Port;
                UISettingSection.Default = Default;
                UISettingSection.Oca = Oca;
                UISettingSection.Cap = Cap;
                UISettingSection.pythonpath = Pythonpath;
                UISettingSection.Light = Light;
                UISettingSection.Dark = Dark;
                AppConfig.Save();
            }
            catch (Exception ex) { }
        }

        //relay/

        //site1
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private Wpf.Ui.Appearance.ThemeType _currentTheme = Wpf.Ui.Appearance.ThemeType.Unknown;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        { }

        private void InitializeViewModel()
        {
            CurrentTheme = Wpf.Ui.Appearance.Theme.GetAppTheme();
            AppVersion = $"RobotVision - {GetAssemblyVersion()}";

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == Wpf.Ui.Appearance.ThemeType.Light)
                        break;

                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Light);
                    CurrentTheme = Wpf.Ui.Appearance.ThemeType.Light;

                    break;

                default:
                    if (CurrentTheme == Wpf.Ui.Appearance.ThemeType.Dark)
                        break;

                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Dark);
                    CurrentTheme = Wpf.Ui.Appearance.ThemeType.Dark;

                    break;
            }
        }

        //site1
    }
}