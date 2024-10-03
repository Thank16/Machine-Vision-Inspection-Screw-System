// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace RobotVision.Models
{
    public static class Databackend
    {
        public static float X1 { get; set; }
        public static float Y1 { get; set; }
        public static float X2 { get; set; }
        public static float Y2 { get; set; }
        public static int mode { get; set; }
        public static List<(float, float, float, float)> roi = new List<(float, float, float, float)>();
    }
}