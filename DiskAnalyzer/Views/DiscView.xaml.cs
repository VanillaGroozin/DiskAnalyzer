using DiskAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using LiveCharts;
using LiveCharts.Wpf;
using DiskAnalyzer.Helpers;
using System.Runtime.InteropServices;
using MaterialDesignThemes.Wpf;
using System.Diagnostics;

namespace DiskAnalyzer.Views
{
    /// <summary>
    /// Interaction logic for DiscView.xaml
    /// </summary>
    public partial class DiscView : UserControl
    {
        Session session = Session.Instance;
        Drive currentDrive;
        DateTime analyzeTime;
        public DiscView()
        {
            InitializeComponent();
            FillDriveCards();

            DirectoryInfo Dir = new DirectoryInfo(System.IO.Path.GetTempPath());

            FileInfo[] Files = Dir.GetFiles();
            var listView1 = new List<string>();
            foreach (FileInfo file in Files)
            {
                listView1.Add(file.Name);
            }
        }

        public void FillDriveCards()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            ObservableCollection<Drive> Drives = new ObservableCollection<Drive>();
            foreach (var drive in allDrives.Where(x => x.IsReady))
            {
                try
                {
                    Drives.Add(new Drive()
                    {
                        Name = drive.Name,
                        Type = drive.DriveType.ToString(),
                        Format = drive.DriveFormat.ToString(),
                        AvailableFreeSpace = drive.AvailableFreeSpace,
                        TotalSize = drive.TotalSize,
                        IsSystemDrive = System.IO.Directory.Exists($"{drive.Name}Windows\\System32"),
                        Label = drive.VolumeLabel,
                        IsBeingAnalyzed = false
                    });
                }
                catch { }
            }
            Drivers.ItemsSource = Drives;
        }

        private Drive GetAnalyzingDrive()
        {
            foreach (Drive driver in Drivers.Items)
            {
                if (driver.IsBeingAnalyzed) return driver;
            }
            return null;
        } 

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartInfo.Visibility = Visibility.Collapsed;
            LoadingBar.Visibility = Visibility.Visible;
            Drive driveToAnalyze = (Drive)(sender as FrameworkElement).DataContext;
            var analyzingDrive = GetAnalyzingDrive();
            if (analyzingDrive != null)
            {
                MessageBox.Show($"Driver {analyzingDrive.NameForCard} is analyzing, please wait", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ((sender as FrameworkElement).DataContext as Drive).IsBeingAnalyzed = true;
            string messageBoxText = $"Do you want to analyze disk {driveToAnalyze.Name}?";
            string caption = $"{driveToAnalyze.Name} analyze";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);


            BackgroundWorker backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
            backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            
            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:
                    FilesSubPanel.IsEnabled = diskPieChart.IsEnabled = false;
                    backgroundWorker.RunWorkerAsync(argument: driveToAnalyze);
                    break;
                case MessageBoxResult.No:
                    // User pressed No button
                    // ...
                    break;
                case MessageBoxResult.Cancel:
                    // User pressed Cancel button
                    // ...
                    break;
            }
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            currentDrive = (Drive)e.Argument;
            currentDrive.Clear();
            analyzeTime = DateTime.Now;
            currentDrive.FillDiskFiles((BackgroundWorker)sender);
        }
        private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            loadingFilesPB.Value = e.ProgressPercentage;
            loadingFilesLbl.Text = e.UserState.ToString();
        }
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadingFilesPB.Value = 100;
            TimeSpan span = DateTime.Now - analyzeTime;
            double seconds  = span.TotalSeconds;
            loadingFilesLbl.Text = $"Seconds taken {seconds}";
            currentDrive.RemoveNullFiles();
            currentDrive.SortFilesByFileSize();
            currentDrive.IsBeingAnalyzed = false;
            FillFilesDiagram();
            FilesSubPanel.IsEnabled = diskPieChart.IsEnabled = true;
        }

        private void FillFilesDiagram()
        {
            diskPieChart.Series.Clear();
            //var sortedDictList = (from dict in currentDrive.FilesByCategorySize orderby dict.Value descending select dict).ToList(); //.ToDictionary(x => x.Key, x => x.Value)

            var sortedList = currentDrive.FilesByCategorySize.ToList();
            sortedList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            var sortedDict = sortedList.ToDictionary(x => x.Key, x => x.Value);
            currentDrive.FilesByCategorySize = sortedDict;
            for (int i = 0; i < (sortedDict.Count < 15 ? sortedDict.Count : 15); i++)
            {
                var fileCategory = sortedDict.ElementAt(i);
                diskPieChart.Series.Add(new PieSeries
                {
                    Title = $"{fileCategory.Key} ({Helper.FormatSize(fileCategory.Value)})",
                    Values = new ChartValues<double> { (double)fileCategory.Value },
                    PushOut = 3
                });
            }
            if (sortedDict.Count > 15)
            {
                long otherFilesSize = 0;
                for (int i = 15; i < sortedDict.Count; i++)
                {
                    otherFilesSize += sortedDict.ElementAt(i).Value;
                }
                diskPieChart.Series.Add(new PieSeries
                {
                    Title = $"Остальное ({Helper.FormatSize(otherFilesSize)})",
                    Values = new ChartValues<double> { (double)otherFilesSize },
                    PushOut = 3
                });
            }
        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            for (int i = 0; i < chart.Series.Count; i++)
            {
                if (chart.Series[i] == (PieSeries)chartpoint.SeriesView)
                {
                    ShowFileListOfSelectedChart(i);
                    break;
                }
            }

            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }

        private void SwitchPageView()
        {
            if (AnalyzeDiskTopPanel.Visibility == Visibility.Visible) {
                AnalyzeDiskTopPanel.Visibility = MyScrollViewer.Visibility = diskPieChart.Visibility = Visibility.Hidden;
                CategoryFilesListTopPanel.Visibility = FilesList.Visibility = Visibility.Visible;
            }
            else
            {
                AnalyzeDiskTopPanel.Visibility = MyScrollViewer.Visibility = diskPieChart.Visibility = Visibility.Visible;
                CategoryFilesListTopPanel.Visibility = FilesList.Visibility = Visibility.Hidden;
            }
        }

        private void ShowFileListOfSelectedChart(int i)
        {
            SwitchPageView();

            List<Models.File> filesSelected = new List<Models.File>();
            var categorySelected = currentDrive.FilesByCategorySize.ElementAt(i).Key;

            foreach (var file in currentDrive.Files)
            {
                if (file != null) {
                    if (file.ExtensionDescription == categorySelected)
                        filesSelected.Add(file);
                }
            }
            FilesList.ItemsSource = new ObservableCollection<Models.File>(filesSelected);
        }

        private void BackToDiskClick(object sender, RoutedEventArgs e)
        {
            SwitchPageView();
        }
        private void BiggestFilesSubPanelSelect(object sender, RoutedEventArgs e)
        {
            BiggestFilesSubPanel.Visibility = Visibility.Visible;
            CleanFilesSubPanel.Visibility = Visibility.Collapsed;
        }
        private void CleanFilesSubPanelSelect(object sender, RoutedEventArgs e)
        {
            CleanFilesSubPanel.Visibility = Visibility.Visible;
            CleanFilesSubPanel.IsEnabled = currentDrive.IsSystemDrive;
            BiggestFilesSubPanel.Visibility = Visibility.Collapsed;
        }

        private void BiggestFilesSubPanelShow(object sender, RoutedEventArgs e)
        {
            SwitchPageView();
            
            List<Models.File> filesSelected = new List<Models.File>();
            for (int i = 0; i < (currentDrive.Files.Count < (int)FilesCountSlider.Value ? currentDrive.Files.Count : (int)FilesCountSlider.Value); i++)
            {
                if (currentDrive.Files[i] != null)
                    filesSelected.Add(currentDrive.Files[i]);
            }
            FilesList.ItemsSource = new ObservableCollection<Models.File>(filesSelected);
        }

        private void FilesList_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        

        private void FilesCountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FilesCountTb.Text = $"Files: {(int)FilesCountSlider.Value}";
        }

        private void CleanDiscClick(object sender, RoutedEventArgs e)
        {
            if (currentDrive.IsSystemDrive)
            {
                long totalSizeCleared = 0;
                if ((bool)RecBinToggleBtn.IsChecked)
                    totalSizeCleared += currentDrive.CleanRecycleBin();
                if ((bool)TempToggleBtn.IsChecked)
                    totalSizeCleared += currentDrive.CleanTempFiles();
                var caption = "Cleared";
                var button = MessageBoxButton.OK;
                var icon = MessageBoxImage.Information;
                var result = MessageBox.Show($"Space cleared: {Helper.FormatSize(totalSizeCleared)}", caption, button, icon);
            }            
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var fileToOpen = (Models.File)(sender as FrameworkElement).DataContext;

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
            var fileToDelete = (Models.File)(sender as FrameworkElement).DataContext;

            string messageBoxText = $"Do you want to delete {fileToDelete.Name}({fileToDelete.SizeToString}) from disk {currentDrive.Name}?";
            string caption = "Deleting file";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);


            switch (result)
            {
                case MessageBoxResult.Yes:

                    messageBoxText = currentDrive.DeleteFile(fileToDelete);
                    SnackbarOne.MessageQueue?.Enqueue(
                        messageBoxText,
                        null,
                        null,
                        null,
                        false,
                        true,
                        TimeSpan.FromSeconds(3));

                    FilesList.ItemsSource = new ObservableCollection<Models.File>(currentDrive.Files);
                    currentDrive.RefreshFilesByCategorySize();
                    FillFilesDiagram();
                    break;
                case MessageBoxResult.No:
                    // Do nothing
                    break;
            }
        }
    }
}
