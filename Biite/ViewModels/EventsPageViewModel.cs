using Biite.Models;
using Biite.Services;
using SQLite;

namespace Biite.ViewModels
{
    internal class EventsPageViewModel : ObservableObject
    {
        public static EventsPageViewModel Current { get; set; }
        SQLiteConnection connection;

        public EventsPageViewModel()
        {
            Current = this;
            connection = DatabaseService.Connection;
        }

        public List<Event> UpcomingEvents
        {
            get
            {
                var currentUser = DatabaseService.GetCurrentUser();
                if (currentUser == null)
                    return new List<Event>();

                // updated added WHERE to filter by HostUserId
                var events = connection.Table<Event>().Where(e => e.HostUserId == currentUser.Id && e.EventDate >= DateTime.Today && !e.IsPastEvent).OrderBy(e => e.EventDate).ToList();

                // loads invited friends for each event 
                foreach (var evt in events)
                {
                    evt.InvitedFriends = DatabaseService.GetEventAttendees(evt.Id);
                }
                return events;
            }
        }

        public List<Event> PastEvents
        {
            get
            {
                var currentUser = DatabaseService.GetCurrentUser();
                if (currentUser == null)
                    return new List<Event>();

                // updated added WHERE to filter by HostUserId
                return connection.Table<Event>().Where(e => e.HostUserId == currentUser.Id && (e.IsPastEvent || e.EventDate < DateTime.Today)).OrderByDescending(e => e.EventDate).ToList();
            }
        }

        public void RefreshEvents()
        {
            OnPropertyChanged(nameof(UpcomingEvents));
            OnPropertyChanged(nameof(PastEvents));
        }
    }
}