// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Configuration;

namespace RobotVision.Models
{
    internal class AppConfig : ConfigurationSection
    {
        //data

        //public string AppPropertiesFileName { get; set; }
        //data/
        //save
        [ConfigurationProperty("Rotate", DefaultValue = "0 degree")]
        public string Rotate
        {
            //get; set;
            get { return (string)this["Rotate"]; }
            set { this["Rotate"] = value; ; }
        }

        [ConfigurationProperty("Width", DefaultValue = "1280")]
        public string Width
        {
            get { return (string)this["Width"]; }
            set { this["Width"] = value; ; }
        }

        [ConfigurationProperty("Height", DefaultValue = "720")]
        public string Height
        {
            get { return (string)this["Height"]; }
            set { this["Height"] = value; ; }
        }

        [ConfigurationProperty("conf", DefaultValue = "0.5")]
        public string conf
        {
            get { return (string)this["conf"]; }
            set { this["conf"] = value; ; }
        }

        [ConfigurationProperty("Camera")]
        public string Camera
        {
            get { return (string)this["Camera"]; }
            set { this["Camera"] = value; ; }
        }

        [ConfigurationProperty("Model")]
        public string Model
        {
            get { return (string)this["Model"]; }
            set { this["Model"] = value; ; }
        }

        [ConfigurationProperty("OK")]
        public string OK
        {
            get { return (string)this["OK"]; }
            set { this["OK"] = value; ; }
        }

        [ConfigurationProperty("NG")]
        public string NG
        {
            get { return (string)this["NG"]; }
            set { this["NG"] = value; ; }
        }

        [ConfigurationProperty("checkbox1")]
        public bool checkbox1
        {
            get { return (bool)this["checkbox1"]; }
            set { this["checkbox1"] = value; ; }
        }

        [ConfigurationProperty("checkbox2")]
        public bool checkbox2
        {
            get { return (bool)this["checkbox2"]; }
            set { this["checkbox2"] = value; ; }
        }

        [ConfigurationProperty("checkbox3")]
        public bool checkbox3
        {
            get { return (bool)this["checkbox3"]; }
            set { this["checkbox3"] = value; ; }
        }

        [ConfigurationProperty("Light", DefaultValue = false)]
        public bool Light
        {
            get { return (bool)this["Light"]; }
            set { this["Light"] = value; ; }
        }

        [ConfigurationProperty("Dark", DefaultValue = false)]
        public bool Dark
        {
            get { return (bool)this["Dark"]; }
            set { this["Dark"] = value; ; }
        }

        [ConfigurationProperty("Default", DefaultValue = true)]
        public bool Default
        {
            get { return (bool)this["Default"]; }
            set { this["Default"] = value; ; }
        }

        [ConfigurationProperty("Cap", DefaultValue = false)]
        public bool Cap
        {
            get { return (bool)this["Cap"]; }
            set { this["Cap"] = value; ; }
        }

        [ConfigurationProperty("Oca", DefaultValue = false)]
        public bool Oca
        {
            get { return (bool)this["Oca"]; }
            set { this["Oca"] = value; ; }
        }

        [ConfigurationProperty("Mute", DefaultValue = true)]
        public bool Mute
        {
            get { return (bool)this["Mute"]; }
            set { this["Mute"] = value; ; }
        }

        [ConfigurationProperty("Delay")]
        public string Delay
        {
            get { return (string)this["Delay"]; }
            set { this["Delay"] = value; ; }
        }

        [ConfigurationProperty("Point")]
        public int Point
        {
            get { return (int)this["Point"]; }
            set { this["Point"] = value; ; }
        }

        [ConfigurationProperty("ptm")]
        public string ptm
        {
            get { return (string)this["ptm"]; }
            set { this["ptm"] = value; ; }
        }

        [ConfigurationProperty("IP")]
        public string IP
        {
            get { return (string)this["IP"]; }
            set { this["IP"] = value; ; }
        }

        [ConfigurationProperty("Port")]
        public string Port
        {
            get { return (string)this["Port"]; }
            set { this["Port"] = value; ; }
        }

        [ConfigurationProperty("pythonpath")]
        public string pythonpath
        {
            get { return (string)this["pythonpath"]; }
            set { this["pythonpath"] = value; ; }
        }

        [ConfigurationProperty("mousep")]
        public string mousep
        {
            get { return (String)this["mousep"]; }
            set { this["mousep"] = value; ; }
        }

        [ConfigurationProperty("D1", DefaultValue = "0")]
        public string sD1
        {
            get { return (String)this["D1"]; }
            set { this["D1"] = value; ; }
        }

        [ConfigurationProperty("D2", DefaultValue = "0")]
        public string sD2
        {
            get { return (String)this["D2"]; }
            set { this["D2"] = value; ; }
        }

        [ConfigurationProperty("C1", DefaultValue = "0")]
        public string sC1
        {
            get { return (String)this["C1"]; }
            set { this["C1"] = value; ; }
        }

        [ConfigurationProperty("C2", DefaultValue = "0")]
        public string sC2
        {
            get { return (String)this["C2"]; }
            set { this["C2"] = value; ; }
            //save/
        }

        [ConfigurationProperty("left", DefaultValue = "0")]
        public string left
        {
            get { return (String)this["left"]; }
            set { this["left"] = value; ; }
        }

        [ConfigurationProperty("mid", DefaultValue = "0")]
        public string mid
        {
            get { return (String)this["mid"]; }
            set { this["mid"] = value; ; }
        }

        [ConfigurationProperty("rigth", DefaultValue = "0")]
        public string rigth
        {
            get { return (String)this["rigth"]; }
            set { this["rigth"] = value; ; }
        }
    }
}