using Biite.Models;
using Biite.Services;
using SQLite;

namespace Biite.ViewModels
{
    internal class SocialsPageViewModel : ObservableObject
    {
        public static SocialsPageViewModel Current { get; set; }
        SQLiteConnection connection;

        private string searchText;

        public SocialsPageViewModel()
        {
            Current = this;
            connection = DatabaseService.Connection;
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConnectedFriends)); //  friends when search changes
            }
        }

        public List<Friend> ConnectedFriends
        {
            get
            {
                var friends = connection.Table<Friend>().Where(f => f.IsConnected).ToList();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    friends = friends.Where(f => f.Name.ToLower().Contains(SearchText.ToLower())).ToList();
                }

                return friends;
            }
        }

        public void RefreshData()
        {
            OnPropertyChanged(nameof(ConnectedFriends));
        }
    }
}