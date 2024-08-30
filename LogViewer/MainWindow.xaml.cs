using LogViewer.Parsers;
using Microsoft.Win32;
using System.IO;
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

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogParser _logParser;

        public MainWindow()
        {
            _logParser = new LogParser();

            InitializeComponent();
        }

        private async void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Log Files (*.log)|*.log|Text Files (*.txt)|*.txt|JSON Files (*.json)|*.json",
                Multiselect = false
            };

            var result = openFileDialog.ShowDialog();
            if (result != true) return;

            using var reader = new StreamReader(openFileDialog.FileName);
            var text = await reader.ReadToEndAsync();

            await _logParser.Parse(text);
        }
    }
}