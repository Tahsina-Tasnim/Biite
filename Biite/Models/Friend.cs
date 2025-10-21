using SQLite;


namespace Biite.Models
{
	[Table("friends")]

	public class Friend : ObservableObject
	{
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; } //if mediapicker used just in case
        public bool IsConnected { get; set; } = true;
        //public string DietaryRestriction { get; set; } not yet maybe later?
        
    }
}