using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskAnalyzer.ViewModels
{
    class DirectoryViewModel : ObservableObject
    {
        public DirectoryViewModel()
        {
            Directories = new ObservableCollection<Directory>();
        }

        private ObservableCollection<Directory> _directories;
        public ObservableCollection<Directory> Directories
        {
            get { return _directories; }
            set { SetProperty(ref _directories, value); }
        }
    }
}
