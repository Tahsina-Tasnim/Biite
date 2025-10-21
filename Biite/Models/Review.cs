using SQLite;


namespace Biite.Models
{

[Table("reviews")]
	
public class Review : ObservableObject
{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int UserId { get; set; }
		public string RestaurantName { get; set; }
		public string ReviewSnippet { get; set; }
		public double Stars {  get; set; } //okay alrady double
		public DateTime ReviewDate { get; set; }
        
		//for location coordinates, maybe no need to get exact location?
		public double Latitude { get; set; }
        public double Longitude { get; set; }
		 public string ImageFilePath { get; set; }



        public string StarsDisplay
		{
			get
			{
				int starCount = (int)Stars;
				return new string('★', starCount) + new string('☆', 5 - starCount);
            }
		}
		public string LocationDisplay => $"Lat: {Latitude}, Long: {Longitude:F4}"; //surely a better way to actually get the location rather than coordinates? Not shown in lab yet so
                                                                                   // look into it later !!!!
        //new property to check if review has a photo
		public bool HasPhoto => !string.IsNullOrEmpty(ImageFilePath) && File.Exists(ImageFilePath);

    }
}