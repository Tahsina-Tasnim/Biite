using SQLite;


namespace Biite.Models

{

[Table("users")]
public class User : ObservableObject
{

	[PrimaryKey, AutoIncrement] 
	public int Id { get; set; }

		[MaxLength(260)]
		public string Name { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string Password { get; set; }
		public string Location { get; set; }

		//added to store profile image paths
        public string ImageFilePath { get; set; }


    }

}