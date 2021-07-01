using DiskAnalyzer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiskAnalyzer.Views
{
    /// <summary>
    /// Interaction logic for DirectoryView.xaml
    /// </summary>
    public partial class DirectoryView : System.Windows.Controls.UserControl
    {
        Session session = Session.Instance;
        public DirectoryView()
        {
            InitializeComponent();
            MainGrid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Pixel);
            this.DataContext = new DirectoryViewModel();
        }


        private Directory GatherDirectoriesInside(Directory currentDirectory, DirectoryInfo currentDirectoryInfo, BackgroundWorker sender)
        {
            try
            {
                if (currentDirectoryInfo.GetFiles().Length > 0)
                {
                    foreach (var file in currentDirectoryInfo.GetFiles())
                    {
                        var newFile = new Directory(file.Name, Helper.FormatSize(file.Length), file.Length, file.FullName, file.Extension, true, false, null);
                        currentDirectory.SubDirectories.Add(newFile);
                    }
                }
                if (currentDirectoryInfo.GetDirectories().Length > 0)
                {
                    foreach (var subDir in currentDirectoryInfo.GetDirectories())
                    {
                        var newFolder = GatherDirectoriesInside(new Directory(subDir.Name, "", 0, subDir.FullName, subDir.Extension, false, true, new ObservableCollection<Directory>()),
                            new DirectoryInfo(subDir.FullName), sender);
                        currentDirectory.SubDirectories.Add(newFolder);
                    }
                }
                currentDirectory.FileLength += currentDirectory.SubDirectories.Sum(x => x.FileLength);
                currentDirectory.Size = Helper.FormatSize(currentDirectory.FileLength);
                currentDirectory.IsLoading = false;
            }
            catch { }

            if (sender != null) sender.ReportProgress(0, currentDirectory);
            return currentDirectory;
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            string path = (string)e.Argument;
            session.SetDirectories(Directory.GetDirectoryInfo(path, (BackgroundWorker)sender));
        }
        private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SearchProgress.IsIndeterminate = true;
            ((DirectoryViewModel)DataContext).Directories.Add((Directory)e.UserState);
        }
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            treeView1.ItemsSource = session.directories;
            ((DirectoryViewModel)DataContext).Directories = session.directories;
            SearchProgress.IsIndeterminate = false;
            MainGrid.ColumnDefinitions[0].Width = new GridLength(190, GridUnitType.Pixel); ;
        }
        


        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Directory;
            if (item != null)
            {
                if (!item.IsFile)
                {
                    SearchTb.Text = item.Path;
                    gridView1.ItemsSource = session.GetSubDirectoriesByPath(item.Path);

                    session.pathHistory.Add(item.Path);
                    session.currentPath++;
                }
            }
        }
        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartInfo.Visibility = Visibility.Collapsed;
            gridView1.Visibility = Visibility.Visible;
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string path = fbd.SelectedPath;
                    SearchTb.Text = fbd.SelectedPath;
                    session.currentPath = 1;
                    session.pathHistory = new List<string> { fbd.SelectedPath };
                    BackgroundWorker backgroundWorker = new BackgroundWorker()
                    {
                        WorkerReportsProgress = true,
                        WorkerSupportsCancellation = true
                    };
                    backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
                    backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
                    backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
                    backgroundWorker.RunWorkerAsync(argument: path);
                }
            }
        }
        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            if (session.currentPath <= session.pathHistory.Count && session.currentPath != 1 && session.currentPath != 0)
            {
                string nextPath = session.pathHistory[session.currentPath - 2];
                SearchTb.Text = nextPath;
                gridView1.ItemsSource = session.GetSubDirectoriesByPath(nextPath);
                session.currentPath--;
            }
        }
        private void Button_Click_Forward(object sender, RoutedEventArgs e)
        {
            if (session.currentPath < session.pathHistory.Count)
            {
                string nextPath = session.pathHistory[session.currentPath];
                SearchTb.Text = nextPath;
                gridView1.ItemsSource = session.GetSubDirectoriesByPath(nextPath);
                session.currentPath++;
            }
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var fileToOpen = (sender as FrameworkElement).DataContext as Directory;

            var messageBoxText = string.Empty;

            try
            {
                Process.Start(fileToOpen.Path);
                messageBoxText = "File opened";
            }
            catch (Exception ex)
            {
                messageBoxText = ex.Message;
            }

            SnackbarOne.MessageQueue?.Enqueue(
                        messageBoxText,
                        null,
                        null,
                        null,
                        false,
                        true,
                        TimeSpan.FromSeconds(3));
        }
        private void DeleteFileClick(object sender, RoutedEventArgs e)
        {
            var fileToDelete = (sender as FrameworkElement).DataContext as Directory;

            string messageBoxText = $"Do you want to delete {fileToDelete.Name}?";
            string caption = "Deleting file";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            MessageBoxResult result = System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);


            switch (result)
            {
                case MessageBoxResult.Yes:

                    messageBoxText = fileToDelete.Delete();
                    SnackbarOne.MessageQueue?.Enqueue(
                        messageBoxText,
                        null,
                        null,
                        null,
                        false,
                        true,
                        TimeSpan.FromSeconds(3));

                    break;
                case MessageBoxResult.No:
                    // Do nothing
                    break;
            }
        }
    }
}
