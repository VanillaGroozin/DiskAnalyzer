using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskAnalyzer.Models
{
    class Program
    {
        public Program(string name, string uninstallPath, DateTime? installDate, string version)
        {
            this.Name = name;
            this.UninstallPath = uninstallPath;
            this.InstallDate = installDate;
            this.Version = version;
        }
        public string Name { get; set; }
        public string UninstallPath { get; set; }
        public DateTime? InstallDate { get; set; }

        public string Version { get; set; }
    }
}
