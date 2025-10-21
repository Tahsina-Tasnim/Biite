using Biite.Models;
using Biite.Services;
using SQLite;


namespace Biite.ViewModels
{

    internal class HomePageViewModel : ObservableObject
    {
        public static HomePageViewModel Current { get; set; }
        SQLiteConnection connection;

        public HomePageViewModel()
        {
            Current = this;
            connection = DatabaseService.Connection;
        }

        public Review LastReview
        {
            get
            {
                return connection.Table<Review>().OrderByDescending(r => r.ReviewDate).FirstOrDefault();
            }
        }

        public List<Restaurant> Nearby
        {
            get
            {
                return connection.Table<Restaurant>().OrderBy(r => r.DistanceKm).ToList();
            }
        }

        public List<Event> Upcoming
        {
            get
            {
                var events = connection.Table<Event>().Where(e => e.EventDate >= DateTime.Today).OrderBy(e => e.EventDate).ToList();
                // loads invited friends for each event 
                foreach (var evt in events)
                {
                    evt.InvitedFriends = DatabaseService.GetEventAttendees(evt.Id);
                }

                return events;
            }
        }

        public void RefreshData()
        {
            OnPropertyChanged(nameof(LastReview));
            OnPropertyChanged(nameof(Nearby));
            OnPropertyChanged(nameof(Upcoming));
        }
    }
}

