using SQLite;

using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Biite.Models

{
[Table("events")]
	
public class Event : ObservableObject
{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Title {  get; set; }
		public DateTime EventDate { get; set; }
		public int People {  get; set; }
		public string Location { get; set; }
		public int HostUserId { get; set; }
        public bool IsPastEvent { get; set; } = false;

        
        public string When => EventDate.ToString("dddd, MMMM dd, h:mm tt"); //formatted like Day, Month, Date Hour:Mins PM/AM
        public string DateOnly => EventDate.ToString("MMM dd, yyyy");
        public string TimeOnly => EventDate.ToString("h:mm tt");

        [Ignore]
        public List<Friend> InvitedFriends { get; set; } = new List<Friend>();

        [Ignore]
        public int TotalPeople => InvitedFriends?.Count + 1 ?? 1; //+1 for myself as the host for now

    }
}