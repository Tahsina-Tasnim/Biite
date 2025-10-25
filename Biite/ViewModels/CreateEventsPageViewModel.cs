using Biite.Models;
using Biite.Services;
using SQLite;
using Biite.Services.PartialMethods;

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

        public void SaveEvent(string eventTitle, DateTime eventDate, TimeSpan eventTime, string location, List<int> selectedFriendIds, double latitude, double longitude)
        {
            var eventDateTime = eventDate.Date + eventTime;

            var newEvent = new Event
            {
                Title = eventTitle,
                EventDate = eventDateTime,
                Location = location,
                HostUserId = DatabaseService.CurrentUserId.Value, //updated from 1 to use logged in user id
                IsPastEvent = false,
                Latitude = latitude,     
                Longitude = longitude
            };

            /* if (newEvent.Id > 0)
                 connection.Update(newEvent);
             else
                 connection.Insert(newEvent); */

            DatabaseService.SaveEvent(newEvent);

            DatabaseService.SaveEventAttendees(newEvent.Id, selectedFriendIds);

        
            ScheduleEventNotification(newEvent);
        }

        private void ScheduleEventNotification(Event eventItem)
        {
            // calculates the notification time to be 1 hour before event
            DateTime notificationTime = eventItem.EventDate.AddHours(-1);

            // only schedules if notification time is in the future
            if (notificationTime > DateTime.Now)
            {
                string title = "Event Reminder";
                string message = $"{eventItem.Title} starts in 1 hour!";

                NotificationService.SendNotification(title, message, notificationTime);
            }
            else if (eventItem.EventDate > DateTime.Now)
            {
                // If event is less than 1 hour away but still in the future
                // Send notification 10 seconds from now
                DateTime immediateTime = DateTime.Now.AddSeconds(10);

                string title = "Event Starting Soon!";
                string message = $"{eventItem.Title} is starting soon!";

                NotificationService.SendNotification(title, message, immediateTime);

            }
        }
    }
}