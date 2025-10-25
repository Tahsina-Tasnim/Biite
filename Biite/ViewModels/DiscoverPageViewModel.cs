using Biite.Models;
using Biite.Services;
using SQLite;
using System.Collections.ObjectModel;

namespace Biite.ViewModels
{
    internal class DiscoverPageViewModel : ObservableObject
    {
        public static DiscoverPageViewModel Current { get; set; }
        SQLiteConnection connection;

        public DiscoverPageViewModel()
        {
            Current = this;
            connection = DatabaseService.Connection;
        }

        public List<Restaurant> Nearby
        {
            get
            {
                return connection.Table<Restaurant>().OrderBy(r => r.DistanceKm).ToList();
            }
        }

        public List<Restaurant> TopRatedRestaurants
        {
            get
            {
                return connection.Table<Restaurant>().OrderByDescending(r => r.Rating).Take(5).ToList();
            }
        }

        public void RefreshData()
        {
            OnPropertyChanged(nameof(TopRatedRestaurants));
            OnPropertyChanged(nameof(Nearby));
        }
    }
}