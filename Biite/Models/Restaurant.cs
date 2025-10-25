using SQLite;

using System.Security.Cryptography.X509Certificates;

namespace Biite.Models

{

[Table("restaurants")]
	

public class Restaurant : ObservableObject
{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Cuisine { get; set; }
		public string Price { get; set; }
		public string Address { get; set; }
		public double Rating { get; set; }
		public double DistanceKm { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	
	}
}