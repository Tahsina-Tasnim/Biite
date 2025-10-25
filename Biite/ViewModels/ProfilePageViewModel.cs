using Biite.Models;
using Biite.Services;
using System.Collections.ObjectModel;

namespace Biite.ViewModels
{
    public class ProfilePageViewModel : ObservableObject
    {
        public static ProfilePageViewModel Current { get; set; }
        private User currentUser;
        private string userName;
        private string userBio;
        private string dietaryRestriction;
        private string profileImagePath;
        private Review lastReview;
        private int reviewCount;

        public ProfilePageViewModel()
        {
            Current = this;
            LoadUserData();
        }

        // loads data for the currently logged-in user
        private void LoadUserData()
        {
            currentUser = DatabaseService.GetCurrentUser();

            if (currentUser != null)
            {
                UserName = currentUser.Name;
                UserBio = $"{currentUser.Location}"; // location as bio
                DietaryRestriction = string.IsNullOrEmpty(currentUser.Restriction) ? "None" : currentUser.Restriction;
                ProfileImagePath = currentUser.ImageFilePath;

                // load user specific review data
                ReviewCount = DatabaseService.GetUserReviewCount(currentUser.Id);
                LastReview = DatabaseService.GetLastReviewForUser(currentUser.Id);

                
            }
        }

        // refreshes all user data (called when page appears)
        public void RefreshData()
        {
            LoadUserData();
        }

        // properties with change notification
        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged();
            }
        }

        public string UserBio
        {
            get => userBio;
            set
            {
                userBio = value;
                OnPropertyChanged();
            }
        }

        public string DietaryRestriction
        {
            get => dietaryRestriction;
            set
            {
                dietaryRestriction = value;
                OnPropertyChanged();
            }
        }

        public string ProfileImagePath
        {
            get => profileImagePath;
            set
            {
                profileImagePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasProfileImage));
            }
        }
        // fix for the UI issue for new users without reviews
        // true if lastReview exists
        public bool HasRecentReview => LastReview != null;

        

        public Review LastReview
        {
            get => lastReview;
            set
            {
                lastReview = value;
                OnPropertyChanged();

                OnPropertyChanged(nameof(HasRecentReview));
              
            }
        }

        public int ReviewCount
        {
            get => reviewCount;
            set
            {
                reviewCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ReviewCountText));
            }
        }

        // computed properties
        public string ReviewCountText => $"{ReviewCount} {(ReviewCount == 1 ? "review" : "reviews")}";

        public bool HasProfileImage => !string.IsNullOrEmpty(ProfileImagePath) && File.Exists(ProfileImagePath);

        // updates profile image and saves to database
        public void UpdateProfileImage(string imagePath)
        {
            if (currentUser != null)
            {
                ProfileImagePath = imagePath;
                DatabaseService.UpdateUserProfileImage(currentUser.Id, imagePath);
            }
        }

        

    }
}