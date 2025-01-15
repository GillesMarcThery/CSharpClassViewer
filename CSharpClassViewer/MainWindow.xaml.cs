using Microsoft.Win32;
using System.Diagnostics;
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
using System.Xml.Linq;

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

            CSharpFile csf = new CSharpFile();
            csf.Load(odlgTextFile.FileName);
            csf.Parse();

            // RAZ treeview
            treeView.Items.Clear();

            // Création de la racine du TreeView
            TreeViewItem TV_root = new();
            TV_root.Header = odlgTextFile.FileName.Split('\\').Last();
            //TV_root.MouseRightButtonDown += TV_Item_MouseRightButtonDown;
            treeView.Items.Add(TV_root);

            // Et on contruit l'arbre
            Buildtreeview(TV_root, csf.myClasses);
        }
        private void Buildtreeview(TreeViewItem TV_Item, List<CSharpClass> myClasses)
        {
            foreach (CSharpClass csc in myClasses)
            {
                TreeViewItem item = new();
                item.Header = csc.access + " " + csc.name + " in " + csc.myNamespace;
                TV_Item.Items.Add(item);
                if (csc.fields.Count > 0)
                {
                    TreeViewItem tv_fields = new();
                    tv_fields.Header = "fields";
                    item.Items.Add(tv_fields);
                    foreach (Field f in csc.fields)
                    {
                        TreeViewItem tvi = new();
                        tvi.Header = f.access + " " + f.type + " " + f.name;
                        tv_fields.Items.Add(tvi);
                    }
                }
                if (csc.properties.Count > 0)
                {
                    TreeViewItem tv_property = new();
                    tv_property.Header = "properties";
                    item.Items.Add(tv_property);
                    foreach (Property p in csc.properties)
                    {
                        TreeViewItem tvi = new();
                        tvi.Header = p.access + " " + p.type + " " + p.name;
                        tv_property.Items.Add(tvi);
                    }
                }
                if (csc.constructor.name != null)
                {
                    TreeViewItem tv_constructor = new();
                    tv_constructor.Header = "constructor";
                    item.Items.Add(tv_constructor);
                    TreeViewItem tvi = new();
                    tvi.Header = csc.constructor.access + " " + csc.constructor.name;
                    tv_constructor.Items.Add(tvi);
                }
                if (csc.methods.Count > 0)
                {
                    TreeViewItem tv_method = new();
                    tv_method.Header = "methods";
                    item.Items.Add(tv_method);
                    foreach (Method m in csc.methods)
                    {
                        TreeViewItem tvi = new();
                        tvi.Header = m.access + " " + m.type + " " + m.name;
                        tv_method.Items.Add(tvi);
                    }
                }
                //if (n.Nodes.Count > 0)
                //    Buildtreeview(item, n);
            }
        }

        private void OpenDirectory_Click(object sender, RoutedEventArgs e)
        {
            string[] allfiles;
            List<CSharpFile> mycSharpFiles = [];

            var folderDialog = new OpenFolderDialog
            {
                Title = "Select Folder",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                //MessageBox.Show($"You picked ${folderName}!");
                allfiles = Directory.GetFiles(folderName, "*.cs", SearchOption.AllDirectories);
                foreach (string filename in allfiles)
                {
                    Debug.WriteLine(filename);
                    CSharpFile csf = new();
                    csf.Load(filename);
                    csf.Parse();
                    mycSharpFiles.Add(csf);
                }
                // RAZ treeview
                treeView.Items.Clear();

                // Création de la racine du TreeView
                TreeViewItem TV_root = new();
                TV_root.Header = folderName;
                treeView.Items.Add(TV_root);

                foreach (CSharpFile csf in mycSharpFiles)
                {
                    // Création de la racine du TreeView
                    TreeViewItem TV_File = new();
                    TV_File.Header = csf.filename.Split('\\').Last();
                    //TV_root.MouseRightButtonDown += TV_Item_MouseRightButtonDown;
                    TV_root.Items.Add(TV_File);
                    // Et on contruit l'arbre
                    Buildtreeview(TV_File, csf.myClasses);
                }
            }
        }
    }
}