using Biite.Models;
using Biite.Services;
using SQLite;


namespace Biite.ViewModels
{

    internal class HomePageViewModel : ObservableObject
    {
        public static HomePageViewModel Current { get; set; }
        SQLiteConnection connection;
        public bool HasRecentReview => LastReview != null;

        public HomePageViewModel()
        {
            Current = this;
            connection = DatabaseService.Connection;
        }

        // updated to show only the current users last review CHECK OUT LATER

        
        public Review LastReview
        {
            get
            {
                var currentUser = DatabaseService.GetCurrentUser();
                System.Diagnostics.Debug.WriteLine($"[LastReview] CurrentUser: {currentUser?.Name ?? "NULL"} (ID: {currentUser?.Id ?? 0})");
                if (currentUser == null)
                    return null;


              
                return DatabaseService.GetLastReviewForUser(currentUser.Id); 
            }
        }

        public List<Restaurant> Nearby
        {
            get
            {
                return connection.Table<Restaurant>().OrderBy(r => r.DistanceKm).ToList();
            }
        }


        // updated to show current users upcoming events CHECK OUT LATER
        public List<Event> Upcoming
        {
            get
            {
                var currentUser = DatabaseService.GetCurrentUser();

                if (currentUser == null)
                {
                    return new List<Event>();
                }

                var events = DatabaseService.GetUpcomingEventsForUser(currentUser.Id);

                // The GetUpcomingEventsForUser already loads invited friends
                // but keeping this for safety if you bypass the method
                foreach (var evt in events)
                {
                    if (evt.InvitedFriends == null || evt.InvitedFriends.Count == 0)
                    {
                        evt.InvitedFriends = DatabaseService.GetEventAttendees(evt.Id);
                    }
                }

                return events;
            }
        }

        // ui fix for Homepage when there are no reviews theres a huge white card to get rid of
        // check to see if review exists
       

        
        public void RefreshData()
        {
            System.Diagnostics.Debug.WriteLine($"[RefreshData] Called");
            OnPropertyChanged(nameof(LastReview));
            OnPropertyChanged(nameof(Nearby));
            OnPropertyChanged(nameof(Upcoming));
            OnPropertyChanged(nameof(HasRecentReview));


        }
    }
}

