// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

using static RobotVision.ViewModels.Pages.AddViewmodel;
using ClosedXML.Excel;
using OxyPlot.Series;
using OxyPlot.Utilities;
using OxyPlot.Wpf;
using OxyPlot.Axes;
using DocumentFormat.OpenXml.Spreadsheet;
using OxyPlot;

namespace RobotVision.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private List<Datalist> _list = new List<Datalist>();

        [ObservableProperty]
        private PlotModel _plotModel;

        [ObservableProperty]
        private PlotModel _plotModel2;

        public DataViewModel()
        {
            read();
            CreatePlot();
        }

        private void CreatePlot()
        {
            // สร้างโมเดลกราฟวงกลม
            PlotModel = new PlotModel { Title = "OK/NG total", TitleColor = OxyColors.White };

            // สร้างซีรีส์สำหรับกราฟวงกลม
            var pieSeries = new PieSeries
            {
                //TextColor = OxyColors.White,
                StrokeThickness = 1,
                InsideLabelPosition = 0.5, // ตั้งตำแหน่ง label ภายใน
                Background = OxyColors.Transparent, // ตั้งค่าพื้นหลังโปร่งใส
                TextColor = OxyColors.White,
            };

            // เพิ่มข้อมูลให้กับ PieSeries
            pieSeries.Slices.Add(new PieSlice("OK", 100) { IsExploded = true });
            pieSeries.Slices.Add(new PieSlice("NG", 1)); // กุมภาพันธ์

            PlotModel.Series.Add(pieSeries);

            PlotModel2 = new PlotModel { Title = "OK/NG per month", TitleColor = OxyColors.White, SelectionColor = OxyColors.White, PlotAreaBorderColor = OxyColors.White };

            // ข้อมูล
            List<int> values1 = new List<int> { 100, 32, 12 }; // ค่าแรก
            List<int> values2 = new List<int> { 19002, 9000, 9000 }; // ค่าสอง

            // สร้าง Bar Series
            var barSeries1 = new BarSeries { XAxisKey = "Value", YAxisKey = "Category", FillColor = OxyColors.Red, TextColor = OxyColors.White };
            var barSeries2 = new BarSeries { XAxisKey = "Value", YAxisKey = "Category", FillColor = OxyColors.Green, TextColor = OxyColors.White };

            // เพิ่ม BarItems สำหรับค่าแรก
            for (int i = 0; i < values1.Count; i++)
            {
                barSeries1.Items.Add(new BarItem { Value = values1[i] });
            }

            // เพิ่ม BarItems สำหรับค่าสอง
            for (int i = 0; i < values2.Count; i++)
            {
                barSeries2.Items.Add(new BarItem { Value = values2[i] });
            }

            // เพิ่ม BarSeries ลงใน PlotModel
            PlotModel2.Series.Add(barSeries1);
            PlotModel2.Series.Add(barSeries2);

            // แกนประเภท
            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "Category",
                TextColor = OxyColors.White,  // สีตัวอักษรเป็นสีขาว

                TitleColor = OxyColors.White, // สีของชื่อแกนเป็นสีขาว
                TicklineColor = OxyColors.White // สีของเส้น Tick เป็นสีขาว
            };
            categoryAxis.Labels.Add("January");
            categoryAxis.Labels.Add("February");
            categoryAxis.Labels.Add("March");
            PlotModel2.Axes.Add(categoryAxis);

            // แกนค่า
            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Value",
                Title = "Values",
                TextColor = OxyColors.White, // สีตัวอักษรเป็นสีขาว
                TitleColor = OxyColors.White, // สีของชื่อแกนเป็นสีขาว
                TicklineColor = OxyColors.White, // สีของเส้น Tick เป็นสีขาว
                MajorGridlineColor = OxyColors.White, // สีของเส้นกริดหลักเป็นสีขาว
                MinorGridlineColor = OxyColors.White  // สีของเส้นกริดรองเป็นสีขาว
            };
            PlotModel2.Axes.Add(valueAxis);
        }

        private void read()
        {
            List = ListSaveManager1.LoadListFromFile("Data.xml");
        }

        public void Insert(string date, string model, string ok, string miss, string status, string path)
        //public void Insert()
        {
            Datalist newModel = new Datalist { Date = date, Model = model, OK = ok, Miss = miss, Status = status, Path = path };
            List.Add(newModel);

            //UISettingSection.Modellist=list;
            ListSaveManager1.SaveListToFile(List, "Data.xml");
            read();
        }

        [RelayCommand]
        public void exportdata()
        {
            OnSaveFile();
        }

        [RelayCommand]
        private void reff()
        {
            read();
        }

        public void OnSaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Data";
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                // Get the path of specified file
                string filePath = saveFileDialog.FileName;
                ExportListToExcel(List, filePath);
                MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportListToExcel(List<Datalist> list, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Data");

                // Add headers
                worksheet.Cell(1, 1).Value = "Date";
                worksheet.Cell(1, 2).Value = "Model";
                worksheet.Cell(1, 3).Value = "OK";
                worksheet.Cell(1, 4).Value = "Miss";
                worksheet.Cell(1, 5).Value = "Status";

                // Add values
                for (int i = 0; i < list.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = list[i].Date;
                    worksheet.Cell(i + 2, 2).Value = list[i].Model;
                    worksheet.Cell(i + 2, 3).Value = list[i].OK;
                    worksheet.Cell(i + 2, 4).Value = list[i].Miss;
                    worksheet.Cell(i + 2, 5).Value = list[i].Status;
                }

                // Save the Excel file

                workbook.SaveAs(@$"{filePath}");
            }
        }
    }

    public class Datalist
    {
        public string Date { get; set; }
        public string Model { get; set; }
        public string OK { get; set; }
        public string Miss { get; set; }
        public string Status { get; set; }
        public string Path { get; set; }
    }

    public class ListSaveManager1
    {
        public static void SaveListToFile(List<Datalist> people, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Datalist>));

            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, people);
            }
        }

        public static List<Datalist> LoadListFromFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Datalist>));

            using (TextReader reader = new StreamReader(filePath))
            {
                return (List<Datalist>)serializer.Deserialize(reader);
            }
        }
    }
}