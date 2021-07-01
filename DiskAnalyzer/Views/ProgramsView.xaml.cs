using DiskAnalyzer.Models;
using DiskAnalyzer.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
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

namespace DiskAnalyzer.Views
{
    public partial class ProgramsView : UserControl
    {
        ObservableCollection<Program> Programs = new ObservableCollection<Program>();
        public ProgramsView()
        {
            InitializeComponent();
            GetInstalledApps();
            this.DataContext = new ProgramsViewModel();
        }


        public void GetInstalledApps()
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        try
                        {
                            var displayVersion = subkey.GetValue("DisplayVersion");
                            var installDate = subkey.GetValue("InstallDate");
                            DateTime? emptyDate = null;

                            if (subkey.GetValue("DisplayName") != null && subkey.GetValue("UninstallString") != null && subkey.GetValue("UninstallString") != string.Empty)
                            {
                                Programs.Add(
                                    new Program(subkey.GetValue("DisplayName").ToString(),
                                    subkey.GetValue("UninstallString").ToString(),
                                    installDate == null ? emptyDate : DateTime.ParseExact(subkey.GetValue("InstallDate").ToString(), "yyyyMMdd", CultureInfo.InvariantCulture),
                                    displayVersion == null ? string.Empty : displayVersion.ToString()
                                ));
                            }
                        }
                        catch { }                     
                    }
                }
            }
            InstalledPrograms.ItemsSource = Programs;
        }


        private void UninstallBtn_Click(object sender, RoutedEventArgs e)
        {
            var message = string.Empty;
            if (Uninstall(((InstalledPrograms.SelectedItem) as Program).UninstallPath))
            {
                message = "Uninstallation started...";
            }
            else
            {
                message = "Uninstallation failed...";
            }

            SnackbarOne.MessageQueue?.Enqueue(
                message,
                null,
                null,
                null,
                false,
                true,
                TimeSpan.FromSeconds(3));

        }
        public bool Uninstall(string uninstallCommand)
        {
            bool uninstalled;
            try
            {
                uninstallCommand = uninstallCommand.Replace("\"", "");

                string uninstallArguments = null;
                string uninstallAssembly = null;
                if (!uninstallCommand.Contains("/"))
                {
                    uninstallAssembly = uninstallCommand;
                }
                else
                {
                    string[] uninstallArgumentsArray = uninstallCommand.Split(new string[] { " /" }, StringSplitOptions.RemoveEmptyEntries);
                    if (uninstallArgumentsArray.Count() > 1) // If 
                    {
                        for (int count = 1; count < uninstallArgumentsArray.Count(); count++)
                        {
                            uninstallArguments = "/" + uninstallArgumentsArray[count];
                        }
                    }
                    uninstallAssembly = uninstallArgumentsArray[0];
                }

                if (!string.IsNullOrWhiteSpace(uninstallAssembly))
                {
                    Process uninstallProcess = new Process();
                    uninstallProcess.StartInfo = new ProcessStartInfo();
                    uninstallProcess.StartInfo.FileName = uninstallAssembly;
                    uninstallProcess.StartInfo.Arguments = uninstallArguments;
                    uninstallProcess.Start();
                    uninstalled = true;
                }
                else
                {
                    uninstalled = false;
                }
            }
            catch (Exception)
            {
                uninstalled = false;
            }
            return uninstalled;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text != string.Empty)
                InstalledPrograms.ItemsSource = Programs.Where(x => x.Name.ToLower().Contains(SearchBox.Text.ToLower()));
            else
                InstalledPrograms.ItemsSource = Programs;
        }

        private void InstalledPrograms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UninstallBtn.IsEnabled = true;
        }

        private void InstalledPrograms_Unselected(object sender, RoutedEventArgs e)
        {
            UninstallBtn.IsEnabled = false;
        }
    }
}
