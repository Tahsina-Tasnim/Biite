using Biite.Models;
using Biite.Services;
using SQLite;


namespace Biite.ViewModels
{

    internal class ProfilePageViewModel : ObservableObject
    {
        public static ProfilePageViewModel Current { get; set; }
        SQLiteConnection connection;

        public ProfilePageViewModel()
        {
            Current = this;
            connection = DatabaseService.Connection;
        }

        public Review LastReview
        {
            get
            {
                return connection.Table<Review>().OrderByDescending(r => r.ReviewDate).FirstOrDefault();
            }
        }
        public int ReviewCount
        {
            get
            {
                return connection.Table<Review>().Count();
            }
        }

        // property for displaying review count with proper singular/plural
        public string ReviewCountText
        {
            get
            {
                int count = ReviewCount;
                if (count == 1)
                    return "1 review • 24 friends";
                else
                    return $"{count} reviews • 24 friends";
            }
        }

        // added property to get current users profile image if any
        public string ProfileImagePath
        {
            get
            {
                var user = connection.Table<User>().FirstOrDefault();
                return user?.ImageFilePath;
            }
        }

        // property to check if user has a profile image
        public bool HasProfileImage
        {
            get
            {
                return !string.IsNullOrEmpty(ProfileImagePath) && File.Exists(ProfileImagePath);
            }
        }

        // added new method to update users profile image
        public void UpdateProfileImage(string imagePath)
        {
            var user = connection.Table<User>().FirstOrDefault();
            if (user != null)
            {
                user.ImageFilePath = imagePath;
                connection.Update(user);
                OnPropertyChanged(nameof(ProfileImagePath));
                OnPropertyChanged(nameof(HasProfileImage));
            }
        }

        public void RefreshData()
        {
            OnPropertyChanged(nameof(LastReview));
            OnPropertyChanged(nameof(ReviewCount));
            OnPropertyChanged(nameof(ReviewCountText));
            OnPropertyChanged(nameof(ProfileImagePath));
            OnPropertyChanged(nameof(HasProfileImage));
        }
    }
}

