using Biite.Models;
using Biite.ViewModels;
using System.Globalization;

namespace Biite.Pages
{

    [QueryProperty(nameof(Latitude), "Latitude")]
    [QueryProperty(nameof(Longitude), "Longitude")]
    [QueryProperty(nameof(LocationName), "LocationName")]
    public partial class CreateEventsPage : ContentPage
    {
        private CreateEventsPageViewModel viewModel;
        private List<int> selectedFriendIds = new List<int>();

        private double eventLatitude;
        private double eventLongitude;
        private string successMessage;

        public double Latitude
        {
            set
            {
              
                     eventLatitude = value;
                     //debug to see where the issue is DELETE once done EVERYWHERE
                     System.Diagnostics.Debug.WriteLine($"Latitude set to: {eventLatitude}");
                    
               
            }
        }

        public double Longitude
        {
            set
            {
              
                     eventLongitude = value;
                     System.Diagnostics.Debug.WriteLine($"Longitude set to: {eventLongitude}");
                    
                
            }
        }


        // shows the location picked from map
        public string LocationName
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && EventLocationEntry != null)
                {
                    EventLocationEntry.Text = value;
                }
            }
        }

        public CreateEventsPage()
        {
            InitializeComponent();
            viewModel = new CreateEventsPageViewModel();
            BindingContext = viewModel;

            // default value set
            EventDatePicker.Date = DateTime.Today.AddDays(1);
            EventTimePicker.Time = new TimeSpan(12, 0, 0);
        }


        // navigates to map page for location picking
        private async void OnPickLocationClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(MapPage)}?IsLocationPicker=true");
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

            if (eventLatitude == 0 || eventLongitude == 0)
            {
                await DisplayAlert("Error", "Please enter a location", "OK");
                return;
            }

            var eventDateTime = EventDatePicker.Date.Date + EventTimePicker.Time;

            //checks if event is in the past
            if (eventDateTime < DateTime.Now)
            {
                await DisplayAlert("Error", "Event time cannot be in the past", "OK");
                return;
            }

            // save event with the data
            viewModel.SaveEvent(
                EventTitleEntry.Text,
                EventDatePicker.Date,
                EventTimePicker.Time,
                EventLocationEntry.Text,
                selectedFriendIds,
                eventLatitude,     
                eventLongitude
            );

            DateTime reminderTime = eventDateTime.AddHours(-1);

            if (reminderTime > DateTime.Now)
            {
                // notification 1 hour before
                successMessage = $"Event '{EventTitleEntry.Text}' created with {selectedFriendIds.Count} friends invited!\n\n" +
                                $"Reminder set for:\n{reminderTime:ddd, MMM dd 'at' h:mm tt}";
            }
            else
            {
                // event is less than 1 hour away
                successMessage = $"Event '{EventTitleEntry.Text}' created with {selectedFriendIds.Count} friends invited!\n\n" +
                                $"Reminder will be sent in 10 seconds (event is starting soon!)";
            }

            await DisplayAlert("Success", successMessage, "OK");

            await Shell.Current.GoToAsync("..");
        }
    }
}