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


        //user authentication and creation from youtube mainly but improvised
        private static int? _currentUserId;
        public static int? CurrentUserId
        {
            get => _currentUserId;
            set => _currentUserId = value;
        }

        // gets the currently logged in user
        public static User GetCurrentUser()
        {
            if (_currentUserId == null) return null;
            return Connection.Table<User>().FirstOrDefault(u => u.Id == _currentUserId);
        }

        // validates login credentials and returns user if valid
        public static User ValidateLogin(string email, string password)
        {
            var user = Connection.Table<User>()
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                CurrentUserId = user.Id; // sets the current logged in user
            }

            return user;
        }

        // checks if email already exists (for registration)
        public static bool EmailExists(string email)
        {
            return Connection.Table<User>().Any(u => u.Email == email);
        }

        // creates a new user account
        public static User CreateNewUser(string name, string email, string phoneNumber, string location, string restriction, string password)
        {
            var newUser = new User
            {
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                Location = location,
                Restriction = restriction,
                Password = password
            };

            Connection.Insert(newUser);
            CurrentUserId = newUser.Id; //automatically log in the new user
            return newUser;
        }

        // logs out the current user
        public static void Logout()
        {
            CurrentUserId = null;
        }


        // creates new user or edits existing user data
        public static void SaveUser(User user)
        {
            if (user.Id > 0)
                Connection.Update(user);
            else
                Connection.Insert(user);
        }

        //updates profile image path for current user
        public static void UpdateUserProfileImage(int userId, string imagePath)
        {
            var user = Connection.Table<User>().FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.ImageFilePath = imagePath;
                Connection.Update(user);
            }
        }

        // updates user name for current user 
        public static void UpdateUserName(int userId, string name)
        {
            var user = Connection.Table<User>().FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Name = name;
                Connection.Update(user);
            }
        }

        // updates user bio/location for current user
        public static void UpdateUserBio(int userId, string bio)
        {
            var user = Connection.Table<User>().FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Location = bio;
                Connection.Update(user);
            }
        }

        // restaurants data returned as a list as closest "SELECT * FROM ... ORDER BY DistanceKM" - SQL version
        public static List<Restaurant> GetNearbyRestaurants()
        {
            return Connection.Table<Restaurant>().OrderBy(r => r.DistanceKm).ToList();
        }

        // reviews returned in a list as newest first or null if none -- updated to get it for each user as in different ones
        public static Review GetLastReviewForUser(int userID)
        {
            return Connection.Table<Review>().Where(r => r.UserId == userID).OrderByDescending(r => r.ReviewDate).FirstOrDefault();
        }

        // events returned as the earliest one first, "ORDER BY DATE" - SQL version and loads invited friends for each event in a complicated way
        public static List<Event> GetUpcomingEventsForUser(int userID)
        {
            var events = Connection.Table<Event>().Where(e => e.HostUserId == userID && e.EventDate >= DateTime.Today).OrderBy(e => e.EventDate).ToList();
            foreach (var evt in events)
            {
                evt.InvitedFriends = GetEventAttendees(evt.Id);
            }
            return events;
        }


        //gets the review counts for specific user based on their id
        public static int GetUserReviewCount(int userId)
        {
            return Connection.Table<Review>().Count(r => r.UserId == userId);
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
            // ensure the event has the current user's ID as host
            if (evt.HostUserId == 0 && CurrentUserId.HasValue)
            {
                evt.HostUserId = CurrentUserId.Value;
            }
            if (evt.Id > 0)
                Connection.Update(evt);
            else
                Connection.Insert(evt);
        }

        // for past events storage
        public static List<Event> GetPastEvents(int userID)
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

       

        //creates new reviews from users 
        public static void SaveReview(Review review)
        {
            // ensure the review has the current user's ID
            if (review.UserId == 0 && CurrentUserId.HasValue)
            {
                review.UserId = CurrentUserId.Value;
            }

            if (review.Id > 0)
                Connection.Update(review);
            else
                Connection.Insert(review);

        }


        //reviews returned as the earliest same as the events date situation for each user id
        public static List<Review>GetUserReviews(int userId)
        {
            return Connection.Table<Review>().Where(r => r.UserId == userId).OrderByDescending(r => r.ReviewDate).ToList();
        }

        // sample data only adds when tables are empty which they are rn
        private static void PseudoData()
        {
            if (Connection.Table<Restaurant>().Count() == 0)
            {
                // Mario's Italian 
                Connection.Insert(new Restaurant
                {
                    Name = "Mario's Italian",
                    Cuisine = "Italian",
                    Price = "$$",
                    Rating = 4.5,
                    DistanceKm = 0.5,
                    Latitude = -32.9283,     
                    Longitude = 151.7817
                });

                // Sushi Palace 
                Connection.Insert(new Restaurant
                {
                    Name = "Sushi Palace",
                    Cuisine = "Japanese",
                    Price = "$$$",
                    Rating = 4.8,
                    DistanceKm = 0.8,
                    Latitude = -32.939086,
                    Longitude = 151.7543986
                });

                // Lana's Bistro 
                Connection.Insert(new Restaurant
                {
                    Name = "Lana's Bistro",
                    Cuisine = "Modern Italian",
                    Price = "$$$",
                    Rating = 4.7,
                    DistanceKm = 9.5,
                    Latitude = -32.9272946,
                    Longitude = 151.5968302
                });

                // Tom's Pub 
                Connection.Insert(new Restaurant
                {
                    Name = "Tom's Pub",
                    Cuisine = "Pub Food",
                    Price = "$$",
                    Rating = 4.3,
                    DistanceKm = 18,
                    Latitude = -32.7545675,
                    Longitude = 151.6306717
                });

                // Ray's Motels and Things 
                Connection.Insert(new Restaurant
                {
                    Name = "Ray's Motels and Things",
                    Cuisine = "American",
                    Price = "$$",
                    Rating = 4.1,
                    DistanceKm = 20,
                    Latitude = -32.7403304,
                    Longitude = 151.597709
                });

                // Jewells Tavern
                Connection.Insert(new Restaurant
                {
                    Name = "Jewells Tavern",
                    Cuisine = "Asian",
                    Price = "$",
                    Rating = 4.0,
                    DistanceKm = 12,
                    Latitude = -33.0110654,
                    Longitude = 151.6801538
                });
            }

            if (Connection.Table<User>().Count() == 0)
            {
                Connection.Insert(new User
                {
                    Name = "Orpi",
                    Email = "user@biite.com",
                    PhoneNumber = "123456789",
                    Location = "Newcastle",
                    Restriction = "Vegetarian",
                    Password = "magicpass"
                });
            }

            if (Connection.Table<Review>().Count() == 0)
            {
                Connection.Insert(new Review { RestaurantName = "Mario's Italian", ReviewSnippet = "Amazing pasta! Cozy vibe.", Stars = 4.0, ReviewDate = DateTime.Now.AddDays(-2), UserId =1,
                    Latitude = -32.9283,      
                    Longitude = 151.7817
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
