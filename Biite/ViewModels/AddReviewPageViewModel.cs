using Biite.Models;
using Biite.Services;
using SQLite;


namespace Biite.ViewModels
{
    internal class AddReviewPageViewModel : ObservableObject
    {
        public static AddReviewPageViewModel Current { get; set; }
        SQLiteConnection connection;

        private string restaurantName;
        private string reviewText;
        private int rating;
        private bool _isCheckingLocation = false;
        private CancellationTokenSource _cancelTokenSource;
        public AddReviewPageViewModel()
        {
            Current = this;
            connection = DatabaseService.Connection;
        }


        public string RestaurantName
        {
            get => restaurantName;
            set
            {
                restaurantName = value;
                OnPropertyChanged();
            }
        }

        public string ReviewText
        {
            get => reviewText;
            set
            {
                reviewText = value;
                OnPropertyChanged();
            }
        }

        public int Rating //change it to double
        {
            get => rating;
            set
            {
                rating = value;
                OnPropertyChanged();
            }
        }

       // public void SaveReview()
       public async Task SaveReview (string photoPath = null) //fixed using MAUI suggestion dunno why it works but no errors anymore
        {
            var currentUser = DatabaseService.GetCurrentUser();

            // safety check - should not happen if user is logged in
            if (currentUser == null)
            {
                return;
            }

            var location = await GetCurrentLocationAsync(); //something was wrong maui suggestion fixed it 
            var review = new Review
            {
                RestaurantName = RestaurantName,
                ReviewSnippet = string.IsNullOrWhiteSpace(ReviewText) ? "No review text" : ReviewText,
                Stars = Rating, //rating uses double so Review.Stars should be double; not anymore now its int because couldnt use toolkt
                ReviewDate = DateTime.Now,
                UserId = currentUser.Id,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                ImageFilePath = photoPath
            };

            DatabaseService.SaveReview(review);

            HomePageViewModel.Current?.RefreshData();
            ProfilePageViewModel.Current?.RefreshData();

            /*   if (review.Id > 0)
                   connection.Update(review);
               else
                   connection.Insert(review); */
        }


        private async Task<Location> GetCurrentLocationAsync() //something was wrong maui suggestion fixed it 
        {
            if (_isCheckingLocation)
                return new Location(-33.8688, 151.2093); // brothers location

            try
            {
                // Request permission like in the lab
                PermissionStatus status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    return new Location(0, 0); //default
                }

                _isCheckingLocation = true;

                
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                    return location;
                else
                    return new Location(0, 0); 
            }
            catch (Exception)
            {
                return new Location(0, 0);  
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }

    }
}