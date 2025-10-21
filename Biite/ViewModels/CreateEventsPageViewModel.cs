using Biite.Models;
using Biite.Services;
using SQLite;

namespace Biite.ViewModels
{
    internal class CreateEventsPageViewModel : ObservableObject
    {
        public static CreateEventsPageViewModel Current { get; set; }
        SQLiteConnection connection;

        public CreateEventsPageViewModel()
        {
            Current = this;
            connection = DatabaseService.Connection;
        }

        public List<Friend> AvailableFriends
        {
            get
            {
                return connection.Table<Friend>().Where(f => f.IsConnected).ToList();
            }
        }

        public void SaveEvent(string eventTitle, DateTime eventDate, TimeSpan eventTime, string location, List<int> selectedFriendIds)
        {
            var eventDateTime = eventDate.Date + eventTime;

            var newEvent = new Event
            {
                Title = eventTitle,
                EventDate = eventDateTime,
                Location = location,
                HostUserId = 1,
                IsPastEvent = false
            };

            if (newEvent.Id > 0)
                connection.Update(newEvent);
            else
                connection.Insert(newEvent);

            DatabaseService.SaveEventAttendees(newEvent.Id, selectedFriendIds);
        }
    }
}