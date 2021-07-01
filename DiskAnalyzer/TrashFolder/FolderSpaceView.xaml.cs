using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace DiskAnalyzer
{
    /// <summary>
    /// Interaction logic for FolderSpaceControl.xaml
    /// </summary>
    public partial class FolderSpaceControl : UserControl
    {
        Session session = Session.Instance;
        public FolderSpaceControl()
        {

            InitializeComponent();
            //MainGrid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Pixel);
            //this.DataContext = new DirectoryViewModel();
        }

        //private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        //{
        //    string path = (string)e.Argument;
        //    GetDirectoryInfo(path, (BackgroundWorker)sender);
        //}
        //private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    SearchProgress.IsIndeterminate = true;
        //    ((DirectoryViewModel)DataContext).Directories.Add((Directory)e.UserState);
        //}
        //private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    treeView1.ItemsSource = session.directories;
        //    SearchProgress.IsIndeterminate = false;
        //    MainGrid.ColumnDefinitions[1].Width = new GridLength(190, GridUnitType.Pixel); ;
        //}
        //private ObservableCollection<Directory> GetDirectoryInfo(string path, BackgroundWorker sender)
        //{
        //    DirectoryInfo di = new DirectoryInfo(path);
        //    var dir = new Directory(di.Name, string.Empty, 0, di.FullName, false, true, new ObservableCollection<Directory>());
        //    dir = GatherDirectoriesInside(dir, di, sender);
        //    session.SetDirectories(dir.SubDirectories);
        //    return dir.SubDirectories;
        //}

        //public Directory GatherDirectoriesInside(Directory currentDirectory, DirectoryInfo currentDirectoryInfo, BackgroundWorker sender)
        //{
        //    try
        //    {
        //        if (currentDirectoryInfo.GetFiles().Length > 0)
        //        {
        //            foreach (var file in currentDirectoryInfo.GetFiles())
        //            {
        //                var newFile = new Directory(file.Name, Directory.FormatSize(file.Length), file.Length, file.FullName, true, false, null);
        //                //session.directories.Add(newFile);
        //                //session.directories.Add(new Directory(file.Name, Directory.FormatSize(file.Length), file.Length, file.FullName, true, false, null));
        //                currentDirectory.SubDirectories.Add(newFile);
        //            }
        //        }
        //        if (currentDirectoryInfo.GetDirectories().Length > 0)
        //        {
        //            foreach (var subDir in currentDirectoryInfo.GetDirectories())
        //            {
        //                var newFolder = GatherDirectoriesInside(new Directory(subDir.Name, "", 0, subDir.FullName, false, true, new ObservableCollection<Directory>()),
        //                    new DirectoryInfo(subDir.FullName), sender);
        //                //session.directories.Add(
        //                //    GatherDirectoriesInside(new Directory(subDir.Name, "", 0, subDir.FullName, false, true, new ObservableCollection<Directory>()),
        //                //    new DirectoryInfo(subDir.FullName), sender)
        //                //    );
        //                currentDirectory.SubDirectories.Add(newFolder);
        //            }
        //        }


        //        currentDirectory.FileLength += currentDirectory.SubDirectories.Sum(x => x.FileLength);
        //        currentDirectory.Size = Directory.FormatSize(currentDirectory.FileLength);
        //        currentDirectory.IsLoading = false;
        //    }
        //    catch { }

        //    if (sender != null) sender.ReportProgress(0, currentDirectory);
        //    return currentDirectory;
        //}

        //void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    SearchProgress.Value = 0;
        //    var item = ((FrameworkElement)e.OriginalSource).DataContext as Directory;
        //    if (item != null)
        //    {
        //        if (!item.IsFile)
        //        {
        //            SearchTb.Text = item.Path;
        //            gridView1.ItemsSource = session.GetSubDirectoriesByPath(item.Path);

        //            session.pathHistory.Add(item.Path);
        //            session.currentPath++;
        //        }
        //    }
        //}

        //private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    using (var fbd = new FolderBrowserDialog())
        //    {
        //        DialogResult result = fbd.ShowDialog();

        //        if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
        //        {
        //            //this.DataContext = new DirectoryViewModel();
        //            string path = fbd.SelectedPath;
        //            SearchTb.Text = fbd.SelectedPath;
        //            session.currentPath = 1;
        //            session.pathHistory = new List<string> { fbd.SelectedPath };
        //            BackgroundWorker backgroundWorker = new BackgroundWorker()
        //            {
        //                WorkerReportsProgress = true,
        //                WorkerSupportsCancellation = true
        //            };
        //            backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
        //            backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
        //            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        //            backgroundWorker.RunWorkerAsync(argument: path);
        //        }
        //    }
        //}



        //private void Window_ContentRendered(object sender, EventArgs e)
        //{
        //    //BackgroundWorker worker = new BackgroundWorker();
        //    //worker.WorkerReportsProgress = true;
        //    //worker.DoWork += worker_DoWork;
        //    //worker.ProgressChanged += worker_ProgressChanged;

        //    //worker.RunWorkerAsync();
        //}

        //private void Button_Click_Back(object sender, RoutedEventArgs e)
        //{
        //    if (session.currentPath <= session.pathHistory.Count && session.currentPath != 1)
        //    {
        //        string nextPath = session.pathHistory[session.currentPath - 2];
        //        SearchTb.Text = nextPath;
        //        gridView1.ItemsSource = session.GetSubDirectoriesByPath(nextPath);
        //        session.currentPath--;
        //    }
        //}

        //private void Button_Click_Forward(object sender, RoutedEventArgs e)
        //{
        //    if (session.currentPath < session.pathHistory.Count)
        //    {
        //        string nextPath = session.pathHistory[session.currentPath];
        //        SearchTb.Text = nextPath;
        //        gridView1.ItemsSource = session.GetSubDirectoriesByPath(nextPath);
        //        session.currentPath++;
        //    }
        //}

        //private void DataGridTextColumn_ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        //{

        //}


    }
}
