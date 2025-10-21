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
                return connection.Table<Review>().Where(r => r.UserId == 1).OrderByDescending(r => r.ReviewDate).ToList();
            }
        }

        public void RefreshData()
        {
            OnPropertyChanged(nameof(UserReviews));
        }
    }
}