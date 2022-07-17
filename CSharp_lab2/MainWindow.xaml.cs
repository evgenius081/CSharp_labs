using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ExtensionMethods;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MessageBox = System.Windows.MessageBox;
using TreeView = System.Windows.Forms.TreeView;

namespace ExtensionMethods
{
    public static class FileSystemInfoExtension
    { 
        public static string Rash(this FileSystemInfo fsi)
        { 
            StringBuilder ret = new StringBuilder("----");
            ret[0] = ((fsi.Attributes & FileAttributes.ReadOnly) != 0) ? 'r' : '-'; 
            ret[1] = ((fsi.Attributes & FileAttributes.Archive) != 0) ? 'a' : '-';
            ret[2] = ((fsi.Attributes & FileAttributes.System) != 0) ? 's' : '-';
            ret[3] = ((fsi.Attributes & FileAttributes.Hidden) != 0) ? 'h' : '-';
            return ret.ToString();
        }
    }
}

namespace PT_lab8
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private string _rootFolderName = "C:\\Users\\gulev\\RiderProjects";
        string folderPath = "";
        TreeViewItem cur = null;
        TreeViewItem root = new TreeViewItem
        {
            Header = "RiderProjects",
            Tag = "C:\\Users\\gulev\\RiderProjects"
        };
        public MainWindow()
        {
            InitializeComponent();
            BuildTree("C:\\Users\\gulev\\RiderProjects", root);
            root.ContextMenu = new ContextMenu();
            System.Windows.Controls.MenuItem create = new System.Windows.Controls.MenuItem();
            create.Header = "Create";
            create.Click += CreateFolderClick;
            root.ContextMenu.Items.Add(create);
            System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
            delete.Header = "Delete";
            delete.Click += DeleteFolderClick;
            root.ContextMenu.Items.Add(delete);
            TreeView1.Items.Add(root);
            TreeView1.SelectedItemChanged += Rash;
        }

        private void Rash(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)TreeView1.SelectedItem;
            DirectoryInfo di = new DirectoryInfo(tvi.Tag.ToString());
            TextBlock2.Text = di.Rash();
        }
        
        private void OpenClick(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to open", SelectedPath = "C:\\Users\\gulev\\RiderProjects"};
            DialogResult result = dlg.ShowDialog();
            if( result == System.Windows.Forms.DialogResult.OK )
            {
                _rootFolderName = dlg.SelectedPath;
                string[] path = _rootFolderName.Split('\\');
                root.Header = path[path.Length - 1];
                root.Tag = _rootFolderName;
                root.ContextMenu = new ContextMenu();
                root.Items.Clear();
                root.ContextMenu = new ContextMenu();
                System.Windows.Controls.MenuItem create = new System.Windows.Controls.MenuItem();
                create.Header = "Create";
                create.Click += CreateFolderClick;
                root.ContextMenu.Items.Add(create);
                System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
                delete.Header = "Delete";
                delete.Click += DeleteFolderClick;
                root.ContextMenu.Items.Add(delete);
                BuildTree(_rootFolderName, root);
                TreeView1.Items.Clear();
                TreeView1.Items.Add(root);
            }
        }
        
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem mnu = sender as System.Windows.Controls.MenuItem;
            TreeViewItem tvi = ((ContextMenu)mnu.Parent).PlacementTarget as TreeViewItem;
            string text = File.ReadAllText(tvi.Tag.ToString());
            TextBlock1.Text = text;
        }

        private void DeleteFileClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem mnu = sender as System.Windows.Controls.MenuItem;
            TreeViewItem tvi = ((ContextMenu) mnu.Parent).PlacementTarget as TreeViewItem;
            ((TreeViewItem)(tvi.Parent)).Items.Remove(tvi);
            File.Delete(tvi.Tag.ToString());
        }

        private void DeleteFolder(string path)
        {
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                DeleteFolder(dir);
            }

            Directory.Delete(path, true);
        }
        
        private void DeleteFolderClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem mnu = sender as System.Windows.Controls.MenuItem;
            TreeViewItem tvi = ((ContextMenu) mnu.Parent).PlacementTarget as TreeViewItem;
            if (Object.ReferenceEquals(TreeView1.GetType(), tvi.Parent.GetType()))
            {
                TreeView1.Items.Remove(tvi);
            }
            else
            {
                ((TreeViewItem)(tvi.Parent)).Items.Remove(tvi);
            }
            
            DeleteFolder(tvi.Tag.ToString());
        }
        
        private void CreateFolderClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem mnu = sender as System.Windows.Controls.MenuItem;
            TreeViewItem tvi = ((ContextMenu) mnu.Parent).PlacementTarget as TreeViewItem;
            FolderWindow fWindow = new FolderWindow();
            folderPath = tvi.Tag.ToString();
            cur = tvi;
            fWindow.RaiseCustomEvent += new EventHandler<CustomEventArgs>(newWindow_RaiseCustomEvent);
            fWindow.Show();
        }

        void newWindow_RaiseCustomEvent(object sender, CustomEventArgs e)
        {
            if(e.Message != "none")
            {
                string[] info = e.Message.Split('|');
                string path = folderPath + "/" + info[1];
                var item = new TreeViewItem { Header = info[1], Tag = path };
                item.ContextMenu = new ContextMenu();
                if (info[0] == "file")
                {
                    if (!File.Exists(path ))
                    {
                        File.Create(path);
                        FileAttributes attributes = File.GetAttributes(path);
                        string[] perm = info[2].Split(';');
                        if(perm[0] == "r") File.SetAttributes(path, attributes | FileAttributes.ReadOnly);
                        if (perm[1] == "a") File.SetAttributes(path, attributes | FileAttributes.Archive);
                        if (perm[2] == "s") File.SetAttributes(path, attributes | FileAttributes.System);
                        if (perm[3] == "h") File.SetAttributes(path, attributes | FileAttributes.Hidden);
                        System.Windows.Controls.MenuItem open = new System.Windows.Controls.MenuItem();
                        open.Header = "Open";
                        open.Click += OpenFileClick;
                        item.ContextMenu.Items.Add(open);
                        System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
                        delete.Header = "Delete";
                        delete.Click += DeleteFileClick;
                        item.ContextMenu.Items.Add(delete);
                    }
                    else
                    {
                        MessageBox.Show("Already exists!");
                    }
                }
                else if(info[0] == "folder")
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        string[] perm = info[2].Split(';');
                        var di = new DirectoryInfo(path);
                        if ( perm[0] == "r") di.Attributes |= FileAttributes.ReadOnly;
                        if ( perm[1] == "a") di.Attributes |= FileAttributes.Archive;
                        if ( perm[2] == "s") di.Attributes |= FileAttributes.System;
                        if ( perm[3] == "h") di.Attributes |= FileAttributes.Hidden;
                        System.Windows.Controls.MenuItem create = new System.Windows.Controls.MenuItem();
                        create.Header = "Create";
                        create.Click += CreateFolderClick;
                        item.ContextMenu.Items.Add(create);
                        System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
                        delete.Header = "Delete";
                        delete.Click += DeleteFolderClick;
                        item.ContextMenu.Items.Add(delete);
                    }
                    
                }
                cur.Items.Add(item);
            }
        }

        private void BuildTree(string rootPath, TreeViewItem root)
        {
            DirectoryInfo di = new DirectoryInfo(rootPath);
            var dirs = Directory.GetDirectories(rootPath);
            foreach (var dir in dirs){
                string[] dirName = dir.Split('\\');
                Console.WriteLine(dirName[dirName.Length-1]);
                var item = new TreeViewItem {Header = dirName[dirName.Length-1], Tag = dir}; 
                item.ContextMenu = new ContextMenu();
                System.Windows.Controls.MenuItem create = new System.Windows.Controls.MenuItem();
                create.Header = "Create";
                create.Click += CreateFolderClick;
                item.ContextMenu.Items.Add(create);
                System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
                delete.Header = "Delete";
                delete.Click += DeleteFolderClick;
                item.ContextMenu.Items.Add(delete);
                BuildTree(dir, item);
                root.Items.Add(item);
            }
            
            var files = Directory.GetFiles(rootPath);
            foreach (var file in files){
                string[] fileName = file.Split('\\');
                var item = new TreeViewItem {Header = fileName[fileName.Length-1], Tag = file};
                item.ContextMenu = new ContextMenu();
                System.Windows.Controls.MenuItem open = new System.Windows.Controls.MenuItem();
                open.Header = "Open";
                open.Click += OpenFileClick;
                item.ContextMenu.Items.Add(open);
                System.Windows.Controls.MenuItem delete = new System.Windows.Controls.MenuItem();
                delete.Header = "Delete";
                delete.Click += DeleteFileClick;
                item.ContextMenu.Items.Add(delete);
                root.Items.Add(item);
            }
        }
    }
}