using DiskAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskAnalyzer.ViewModels
{
    class ProgramsViewModel : ObservableObject
    {
        public ProgramsViewModel()
        {
            Programs = new ObservableCollection<Program>();
        }

        private ObservableCollection<Program> _programs;
        public ObservableCollection<Program> Programs
        {
            get { return _programs; }
            set { SetProperty(ref _programs, value); }
        }
    }

    
}
