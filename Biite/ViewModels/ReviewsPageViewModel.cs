using Biite.Models;
using Biite.Services;
using SQLite;
using System.Collections.ObjectModel;

namespace Biite.ViewModels
{
    internal class ReviewsPageViewModel : ObservableObject
    {
        public static ReviewsPageViewModel Current { get; set; }
        SQLiteConnection connection;

        public ReviewsPageViewModel()
        {
            Current = this;
            connection = DatabaseService.Connection;
        }

        public List<Review> UserReviews
        {
            get
            {
                var currentUser = DatabaseService.GetCurrentUser();
                if (currentUser == null)
                    return new List<Review>();

                // uses the user specific method from DatabaseService
                return DatabaseService.GetUserReviews(currentUser.Id);
            }
        }

        public void RefreshData()
        {
            OnPropertyChanged(nameof(UserReviews));
        }
    }
}