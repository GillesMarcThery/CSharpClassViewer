using Microsoft.Win32;
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

namespace CSharpClassViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog odlgTextFile = new()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "csv",
                DereferenceLinks = true,
                Filter = "CS (*.cs)|*.cs|" + "All files (*.*)|*.*",
                Multiselect = false,
                RestoreDirectory = true,
                ShowReadOnly = false,
                Title = "Select a file to open", //Todo
                ValidateNames = true
            };

            if (odlgTextFile.ShowDialog() == false)
                return;

            CSharpFile csClass = new CSharpFile();
            csClass.Load(odlgTextFile.FileName);
            csClass.Parse();
        }
    }
}