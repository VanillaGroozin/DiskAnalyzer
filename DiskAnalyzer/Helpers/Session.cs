using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DiskAnalyzer
{

    public sealed class Session
    {
        private Session()
        {
            directory = new Directory();
            directories = new ObservableCollection<Directory>();
            pathHistory = new List<string>();
            directories.Add(directory);
        }
        public ObservableCollection<Directory> directories { get; set;
        }
        public Directory directory { get; set; }
        public int currentPath { get; set; }
        public List<string> pathHistory { get; set; }
        public bool isLoading { get; set; }
        private static Session instance = null;
        public static Session Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Session();
                }
                return instance;
            }
        }
        public void SetDirectories(ObservableCollection<Directory> directories)
        {
            this.directories = directories;
        }     
        private ObservableCollection<Directory> FindSubDirectoriesByPath(string path, ObservableCollection<Directory> directories = null)
        {
            directories = directories is null ? this.directories : directories;
            ObservableCollection<Directory> foundDirectory = null;
            foreach (var dir in directories)
            {
                if (!dir.IsFile)
                {
                    if (dir.Path != path)
                    {
                        foundDirectory = FindSubDirectoriesByPath(path, dir.SubDirectories);
                        if (foundDirectory != null) break;
                    }
                    else
                    {
                        foundDirectory = dir.SubDirectories;
                        break;
                    }
                }
            }
            return foundDirectory;
        }
        public ObservableCollection<Directory> GetSubDirectoriesByPath(string path)
        {
            return FindSubDirectoriesByPath(path);
        }
    }
}
