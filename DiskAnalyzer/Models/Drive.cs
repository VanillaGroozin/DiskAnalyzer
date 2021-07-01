using DiskAnalyzer.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiskAnalyzer.Models
{
    class Drive
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public long TotalSize { get; set; }
        public string Label { get; set; }
        public bool IsSystemDrive { get; set; } = false;
        public string NameForCard
        {
            get { return $"{Name} ({Label})"; }
            set { }
        }
        private bool IsSorted { get; set; } = false;
        public long AvailableFreeSpace { get; set; }
        public bool IsBeingAnalyzed { get; set; }

        public List<File> Files = new List<File>();
        private long FilesSize { get; set; } = 0;

        enum RecycleFlags : int
        {
            SHRB_NOCONFIRMATION = 0x00000001, // Don't ask for confirmation
            SHRB_NOPROGRESSUI = 0x00000001, // Don't show progress
            SHRB_NOSOUND = 0x00000004 // Don't make sound when the action is executed
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        private struct SHQUERYRBINFO
        {
            public Int32 cbSize;
            public UInt64 i64Size;
            public UInt64 i64NumItems;
        }
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlags dwFlags);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHQueryRecycleBin(string pszRootPath, ref SHQUERYRBINFO pSHQueryRBInfo);



        public Dictionary<string, long> FilesByCategorySize = new Dictionary<string, long>();
        public string SizeComparison
        {
            get { return Helper.FormatSize(this.AvailableFreeSpace) + " / " + Helper.FormatSize(this.TotalSize); }
            set { }
        }

        public int Percentage
        {
            get { return 100 - (int)(((double)this.AvailableFreeSpace/ (double)this.TotalSize) * 100); }
            set { }
        }
        private long GetCurrentFilesSize()
        {
            return Files.Sum(x => x.Size);
        }
        private int GetFilesLoadingPercentage()
        {
            return (int)Math.Ceiling(((float)FilesSize / (float)(TotalSize - AvailableFreeSpace)) * 100);
        }
        private string LoadingInfoToString()
        {
            var totalFileSize = (TotalSize - AvailableFreeSpace);
            return $"Files: {Files.Count}, {Helper.FormatSize(FilesSize > totalFileSize ? totalFileSize : FilesSize)}/{Helper.FormatSize(totalFileSize)}";
        }

        public void FillDiskFiles(BackgroundWorker sender, string path = null)
        {
            DirectoryInfo di = new DirectoryInfo(path ?? Name);
            try
            {
                if (di.GetFiles().Length > 0)
                {                 
                    foreach (var file in di.GetFiles())
                    {
                        var newFile = new File(file.Name, file.Extension, Helper.GetFilePurposeDescription(file.Name), file.Length, file.FullName);
                        this.FilesSize += file.Length;
                        AddFileToCategory(newFile);
                        this.Files.Add(newFile);
                    }
                }
                if (di.GetDirectories().Length > 0)
                {
                    Parallel.ForEach(di.GetDirectories(), (dir) =>
                    {
                        try
                        {
                            FillDiskFiles(sender, dir.FullName);
                        }
                        catch { }
                    });
                }
            }
            catch { }
            if (sender != null) sender.ReportProgress(this.GetFilesLoadingPercentage(), this.LoadingInfoToString());
        }

        private void AddFileToCategory(File file)
        {
            if (!FilesByCategorySize.ContainsKey(file.ExtensionDescription))
                FilesByCategorySize.Add(file.ExtensionDescription, file.Size);
            else
                FilesByCategorySize[file.ExtensionDescription] += file.Size;
        }

        public void SortFilesByFileSize()
        {
            if (this.IsSorted) return;
            this.Files = this.Files.OrderByDescending(x => x.Size).ToList();
            this.IsSorted = true;
        }

        public void RemoveNullFiles()
        {
            this.Files.RemoveAll(x => x == null);
        }

        public void RefreshFilesByCategorySize()
        {
            FilesByCategorySize.Clear();
            foreach (var file in this.Files)
                if (file != null) AddFileToCategory(file);
        }

        public void Clear()
        {
            FilesByCategorySize.Clear();
            Files.Clear();
            FilesSize = 0;
            IsSorted = false;
        }

        public string DeleteFile(File fileToDelete)
        {
            try
            {  
                if (System.IO.File.Exists(fileToDelete.Path))
                { 
                    System.IO.File.Delete(fileToDelete.Path);
                    Files.Remove(fileToDelete);
                    return "File deleted";
                }
                else throw new Exception("File not found");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public long CleanRecycleBin()
        {
            try
            {
                var recycleBinSize = GetRecycleBinSize();
                SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlags.SHRB_NOCONFIRMATION);
                return recycleBinSize;
            }
            catch {
                return 0;
            }  
        }

        private long GetRecycleBinSize()
        {
            SHQUERYRBINFO query = new SHQUERYRBINFO();
            query.cbSize = Marshal.SizeOf(typeof(SHQUERYRBINFO));
            try
            {
                int result = SHQueryRecycleBin(null, ref query);
                if (result == 0)
                {
                    return (long)query.i64Size;
                }
                else
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            catch
            {
                return 0;
            }
        }

        public long CleanTempFiles()
        {
            long tempSize = 0;
            try
            {
                DirectoryInfo dir = new DirectoryInfo($"{this.Name}temp");
                foreach (var file in dir.GetFiles())
                {
                    tempSize += file.Length;
                    System.IO.File.Delete(file.FullName);
                }
                return tempSize;
            }
            catch {
                return 0;
            }
        }
    }
    class File
    {
        public File(string Name, string Extension, string ExtensionDescription, long Size, string Path)
        {
            this.Name = Name;
            this.Extension = Extension;
            this.ExtensionDescription = ExtensionDescription;
            this.Size = Size;
            this.Path = Path;
        }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
        public string ExtensionDescription { get; set; }
        public long Size { get; set; }
        public string SizeToString
        {
            get { return Helper.FormatSize(Size); }
            set { }
        }

    }
}
