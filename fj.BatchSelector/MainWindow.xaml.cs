using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace fj.BatchSelector
{
    public partial class MainWindow : Window
    {
        private readonly XDocument XmlData = XDocument.Load("Files.xml");

        public MainWindow()
        {
            InitializeComponent();
            BatchPB.Visibility = System.Windows.Visibility.Hidden;
            ProgressL.Visibility = System.Windows.Visibility.Hidden;
            var items = getBatchItems();
            BatchLB.ItemsSource = getBatchItems();
            updateUIFromXml();
            this.Height = items.Count() * (55 + 10) + 108;
        }

        private void updateUIFromXml()
        {
            var settings = (from itemHeader in XmlData.Descendants("FileList")
                         select new
                         {
                             title = itemHeader.Attribute("Title").Value,
                             loaderMsg = itemHeader.Attribute("LoaderMessage").Value
                         }).First();

            this.Title = settings.title;
            this.ProgressL.Content = settings.loaderMsg;
        }

        private IEnumerable<Item> getBatchItems()
        {
            var items = from item in XmlData.Descendants("File")
                        select new Item
                        {
                            Label = item.Attribute("Label").Value,
                            FileName = item.Attribute("FileName").Value
                        };
            return items;
        }

        private void exit() { this.Close(); }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var item = (Item)(e.OriginalSource as FrameworkElement).DataContext;
            var fileName = item.FileName;

            TaskScheduler bgScheduler = TaskScheduler.Default;
            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            var ui = Task.Factory.StartNew(
                () => { disableUI(); }, CancellationToken.None, TaskCreationOptions.None, uiScheduler)
            .ContinueWith(delegate { executeFile(fileName); }, bgScheduler)
            .ContinueWith(delegate { exit(); }, uiScheduler);
        }

        private static void executeFile(string fileName)
        {
            var startInfo = new ProcessStartInfo(fileName) { WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden };
            var process = System.Diagnostics.Process.Start(startInfo);
            process.WaitForExit();
        }

        private void disableUI()
        {
            BatchLB.IsEnabled = false;
            BatchPB.Visibility = System.Windows.Visibility.Visible;
            ProgressL.Visibility = System.Windows.Visibility.Visible;
        }
    }

    class Item
    {
        public String Label { get; set; }
        public String FileName { get; set; }
    }

}
