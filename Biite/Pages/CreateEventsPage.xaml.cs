using Biite.Models;
using Biite.ViewModels;

namespace Biite.Pages
{
    public partial class CreateEventsPage : ContentPage
    {
        private CreateEventsPageViewModel viewModel;
        private List<int> selectedFriendIds = new List<int>();

        public CreateEventsPage()
        {
            InitializeComponent();
            viewModel = new CreateEventsPageViewModel();
            BindingContext = viewModel;

            // default value set
            EventDatePicker.Date = DateTime.Today.AddDays(1);
            EventTimePicker.Time = new TimeSpan(12, 0, 0);
        }

        private void OnFriendSelectionChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var friend = checkBox?.BindingContext as Friend;

            if (friend != null)
            {
                if (e.Value) // for checked
                {
                    if (!selectedFriendIds.Contains(friend.Id))
                        selectedFriendIds.Add(friend.Id);
                }
                else // for unchecked
                {
                    selectedFriendIds.Remove(friend.Id);
                }

                // updates selected count display
                SelectedCountLabel.Text = $"Selected: {selectedFriendIds.Count} friends";
            }
        }

        private async void OnCreateEventClicked(object sender, EventArgs e)
        {
            // make sure the event title isnt just spaces
            if (string.IsNullOrWhiteSpace(EventTitleEntry.Text))
            {
                await DisplayAlert("Error", "Please enter an event title", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(EventLocationEntry.Text))
            {
                await DisplayAlert("Error", "Please enter a location", "OK");
                return;
            }

            // save event with the data
            viewModel.SaveEvent(
                EventTitleEntry.Text,
                EventDatePicker.Date,
                EventTimePicker.Time,
                EventLocationEntry.Text,
                selectedFriendIds
            );

            await DisplayAlert("Success",
                $"Event '{EventTitleEntry.Text}' created successfully with {selectedFriendIds.Count} friends invited!", "OK");

            await Shell.Current.GoToAsync("..");
        }
    }
}