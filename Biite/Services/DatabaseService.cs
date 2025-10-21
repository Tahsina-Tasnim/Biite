using SQLite;
using Biite.Models;

//the structure is a mess fix it up later so it makes sense instead of putting it where you though of at the time
                                         //FIX !!!!!! everything looks a mess and complicated
namespace Biite.Services
{
    public static class DatabaseService
    {
        private static string _databaseFile;
        private static string DatabaseFile
        {
            get
            {
                if (_databaseFile == null)
                {
                    string databaseDir = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "data");
                    System.IO.Directory.CreateDirectory(databaseDir);
                    _databaseFile = Path.Combine(databaseDir, "biite_data.sqlite");
                }
                return _databaseFile;
            }
        }
        private static SQLiteConnection _connection;
        public static SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SQLiteConnection(DatabaseFile);
                    _connection.CreateTable<User>();
                    _connection.CreateTable<Restaurant>();
                    _connection.CreateTable<Review>();
                    _connection.CreateTable<Event>();
                    _connection.CreateTable<Friend>();
                    _connection.CreateTable<EventAttendee>();
                    PseudoData();
                }
                return _connection;
            }
        }
        
        // restaurants data returned as a list as closest "SELECT * FROM ... ORDER BY DistanceKM" - SQL version
        public static List<Restaurant> GetNearbyRestaurants()
        {
            return Connection.Table<Restaurant>().OrderBy(r => r.DistanceKm).ToList();
        }

        // reviews returned in a list as newest first or null if none
        public static Review GetLastReview()
        {
            return Connection.Table<Review>().OrderByDescending(r => r.ReviewDate).FirstOrDefault();
        }

        // events returned as the earliest one first, "ORDER BY DATE" - SQL version and loads invited friends for each event in a complicated way
        public static List<Event> GetUpcomingEvents()
        {
            var events = Connection.Table<Event>().Where(e => e.EventDate >= DateTime.Today).OrderBy(e => e.EventDate).ToList();
            foreach (var evt in events)
            {
                evt.InvitedFriends = GetEventAttendees(evt.Id);
            }
            return events;
        }

        //to store the friends "invited" to the events created so can call back to it later
        public static List<Friend> GetEventAttendees(int eventId)
        {
            var attendeeIds = Connection.Table<EventAttendee>().Where(ea => ea.EventId == eventId).Select(ea => ea.FriendId).ToList();

            return Connection.Table<Friend>().Where(f => attendeeIds.Contains(f.Id)).ToList();
        }

        //creates new events same as save user and save reviews
        public static void SaveEvent(Event evt)
        {
            if (evt.Id > 0)
                Connection.Update(evt);
            else
                Connection.Insert(evt);
        }

        // for past events storage
        public static List<Event> GetPastEvents()
        {
            return Connection.Table<Event>().Where(e => e.IsPastEvent || e.EventDate < DateTime.Today).OrderByDescending(e => e.EventDate).ToList();
        }

        //friends returned as a list no date needed
        public static List<Friend> GetAllFriends()
        {
            return Connection.Table<Friend>().Where(f => f.IsConnected).ToList();
        }

        public static void SaveEventAttendees(int eventId, List<int> friendIds)
        {
            // clears duplicating attendees using SQL command, check if correct  
            Connection.Execute("DELETE FROM event_attendees WHERE EventId = ?", eventId);

            // loops through each friend selected and makes a new event attendee record to link event to each friend
            foreach (var friendId in friendIds)
            {
                var friend = Connection.Table<Friend>().FirstOrDefault(f => f.Id == friendId);
                if (friend != null)
                {
                    Connection.Insert(new EventAttendee
                    {
                        EventId = eventId,
                        FriendId = friendId,
                        FriendName = friend.Name
                    });
                }
            }

        }

        //to show the details of events including name, date, time, location, friends invited
        public static string GetEventDetails(int eventId)
        {
            //gets the event record from database
            var evt = Connection.Table<Event>().FirstOrDefault(e => e.Id == eventId);
            if (evt == null) return "Event not found";
            //gets list of friends that are invited
            var attendees = Connection.Table<EventAttendee>().Where(ea => ea.EventId == eventId).Select(ea => ea.FriendName).ToList();
            
            //joins the friends with a comma using the command or shows default no friends invited message
            var friendsList = attendees.Any() ? string.Join(", ", attendees) : "No friends invited";

            return $"Event: {evt.Title}\nDate: {evt.DateOnly}\nTime: {evt.TimeOnly}\nLocation: {evt.Location}\nInvited Friends: {friendsList}";
        }

        // creates new user or edits existing user data
        public static void SaveUser(User user)
        {
            if (user.Id > 0)
                Connection.Update(user);
            else
                Connection.Insert(user);
        }

        //creates new reviews from users 
        public static void SaveReview(Review review)
        {
            if (review.Id > 0)
                Connection.Update(review);
            else
                Connection.Insert(review);
        }


        //reviews returned as the earliest same as the events date situation
        public static List<Review>GetUserReviews(int userId)
        {
            return Connection.Table<Review>().Where(r => r.UserId == userId).OrderByDescending(r => r.ReviewDate).ToList();
        }


        // sample data only adds when tables are empty which they are rn
        private static void PseudoData()
        {
            
            if (Connection.Table<Restaurant>().Count() == 0)
            {
                Connection.Insert(new Restaurant { Name = "Mario's Italian", Cuisine = "Italian", Price = "$$", Rating = 4.5, DistanceKm = 0.5 });
                Connection.Insert(new Restaurant { Name = "Sushi Palace", Cuisine = "Japanese", Price = "$$$", Rating = 4.8, DistanceKm = 0.8 });
            }
            
            if (Connection.Table<User>().Count() == 0)
            {
                Connection.Insert(new User
                {
                    Name = "Your Name",
                    Email = "user@biite.com",
                    PhoneNumber = "123456789",
                    Location = "Newcastle"
                });
            }

            if (Connection.Table<Review>().Count() == 0)
            {
                Connection.Insert(new Review { RestaurantName = "Mario's Italian", ReviewSnippet = "Amazing pasta! Cozy vibe.", Stars = 4.0, ReviewDate = DateTime.Now.AddDays(-2), UserId =1,
                    Latitude = -33.8688,
                    Longitude = 151.2093
                });
            }

            if (Connection.Table<Friend>().Count() == 0)
            {
                Connection.Insert(new Friend { Name = "Lana Cossettini", Email = "lana@example.com", IsConnected = true });
                Connection.Insert(new Friend { Name = "Ben Davis", Email = "ben@example.com", IsConnected = true });
                Connection.Insert(new Friend { Name = "Nathan Thong", Email = "nathan@example.com", IsConnected = true });
                Connection.Insert(new Friend { Name = "Thomas Adams", Email = "tom@example.com", IsConnected = true });
                Connection.Insert(new Friend { Name = "Dominic Wallace", Email = "dom@example.com", IsConnected = true });
            }
            if (Connection.Table<Event>().Count() == 0)
            {
                Connection.Insert(new Event { Title = "Team Lunch", EventDate = DateTime.Today.AddDays(1).AddHours(12.5), People = 6, Location = "Downtown Cafe", HostUserId = 1, IsPastEvent = false });
                Connection.Insert(new Event { Title = "Birthday Dinner", EventDate = DateTime.Today.AddDays(-7).AddHours(19), Location = "Italian Bistro", HostUserId = 1, IsPastEvent = true });
            }
        }
    }
}
