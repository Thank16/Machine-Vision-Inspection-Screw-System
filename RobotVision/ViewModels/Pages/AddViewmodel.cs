using System.IO;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace RobotVision.ViewModels.Pages
{
    public partial class AddViewmodel : ObservableObject
    {
        //run
        public AddViewmodel()
        {
            read();
        }

        private void read()
        {
            List = ListSaveManager.LoadListFromFile("Modelpath.xml");
        }

        //run/
        //data binding
        [ObservableProperty]
        private string _path;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private List<Modellist> _list = new List<Modellist>();

        [ObservableProperty]
        private int _selectedIndex;

        //data binding/
        //relay
        [RelayCommand]
        private void openflie()
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".pt"; // Default file extension
            dialog.Filter = "Model (pt)|*.pt"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                Path = filename;
            }
        }

        [RelayCommand]
        private void Insert()
        {
            Modellist newModel = new Modellist { Models = Name, Paths = Path };
            List.Add(newModel);

            //UISettingSection.Modellist=list;
            ListSaveManager.SaveListToFile(List, "Modelpath.xml");
            read();
        }

        [RelayCommand]
        private void Delete()
        {
            try
            {
                List.RemoveAt(SelectedIndex);
                //MessageBox.Show(list[0].ToString());
                //UISettingSection.Modellist=list;
                ListSaveManager.SaveListToFile(List, "Modelpath.xml");
                read();
            }
            catch { }
        }

        //relay/
    }

    public class Modellist
    {
        public string Models { get; set; }
        public string Paths { get; set; }
    }

    public class ListSaveManager
    {
        public static void SaveListToFile(List<Modellist> people, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Modellist>));

            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, people);
            }
        }

        public static List<Modellist> LoadListFromFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Modellist>));

            using (TextReader reader = new StreamReader(filePath))
            {
                return (List<Modellist>)serializer.Deserialize(reader);
            }
        }
    }
}