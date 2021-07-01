using DiskAnalyzer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskAnalyzer
{
    public class Directory : ObservableObject
    {
        public Directory() { }
        public Directory(string name, string size, long fileLength, string path, string extension, bool isFile, bool isLoading, ObservableCollection<Directory> subDirectories)
        {
            this.Name = name;
            this.Size = size;
            this.FileLength = fileLength;
            this.Path = path;
            this.Extension = extension;
            this.IsFile = isFile;
            this.IsLoading = isLoading;
            this.SubDirectories = subDirectories;
        }
        #region PROPS
        private string _name;
        public string Name {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _size;
        public string Size
        {
            get { return _size; }
            set { SetProperty(ref _size, value); }
        }

        private long _fileLength;
        public long FileLength
        {
            get { return _fileLength; }
            set { SetProperty(ref _fileLength, value); }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        private string _extension;
        public string Extension
        {
            get { return _extension; }
            set { SetProperty(ref _extension, value); }
        }

        private bool _isFile;
        public bool IsFile
        {
            get { return _isFile; }
            set { SetProperty(ref _isFile, value); }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
        private ObservableCollection<Directory> _subDirectories;
        public ObservableCollection<Directory> SubDirectories
        {
            get { return _subDirectories; }
            set { SetProperty(ref _subDirectories, value); }
        }
        #endregion
 
        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            try
            {
                FileInfo[] fis = d.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    size += fi.Length;
                }
                // Add subdirectory sizes.
                DirectoryInfo[] dis = d.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    size += DirSize(di);
                }
            }
            catch { }
            return size;
        }

        public static ObservableCollection<Directory> GetDirectoryInfo(string path, BackgroundWorker sender)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            var dir = new Directory(di.Name, string.Empty, 0, di.FullName, di.Extension, false, true, new ObservableCollection<Directory>());
            var rootDir = new ObservableCollection<Directory>();
            dir = GatherDirectoriesInside(dir, di, sender);
            rootDir.Add(dir);
            return rootDir;
        }

        public string Delete()
        {
            try
            {
                if (System.IO.File.Exists(this.Path))
                {
                    System.IO.File.Delete(this.Path);
                    return "Deleted";
                }
                else throw new Exception("Not found");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private static Directory GatherDirectoriesInside(Directory currentDirectory, DirectoryInfo currentDirectoryInfo, BackgroundWorker sender)
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
    } 
}
