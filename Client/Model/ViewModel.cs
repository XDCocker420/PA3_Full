using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    internal class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<City> _searchCities;
        public ObservableCollection<City> SearchCities
        {
            get => _searchCities;
            set
            {
                _searchCities = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchCities)));
            }
        }

        private ObservableCollection<City> _pathCities;
        public ObservableCollection<City> PathCities
        {
            get => _pathCities;
            set
            {
                _pathCities = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PathCities)));
            }
        }

        public ViewModel()
        {
            SearchCities = new ObservableCollection<City>();
            PathCities = new ObservableCollection<City>();
        }
    }
}
