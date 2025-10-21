using SQLite;

namespace Biite.Models
{
    [Table("event_attendees")]
    public class EventAttendee : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int EventId { get; set; }
        public int FriendId { get; set; }
        public string FriendName { get; set; } //  easier display
    }
}
