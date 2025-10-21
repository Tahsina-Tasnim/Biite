using Biite.ViewModels;
using Biite.Services;

namespace Biite.Pages
{
    public partial class EventsPage : ContentPage
    {
        private EventsPageViewModel viewModel;

        public EventsPage()
        {
            InitializeComponent();
            viewModel = new EventsPageViewModel();
            BindingContext = viewModel;
        }
        private async void CreateEvent_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CreateEventsPage));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.RefreshEvents();
        }

        private async void OnViewEventClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var evt = button?.BindingContext as Biite.Models.Event;

            if (evt != null)
            {
                var eventDetails = DatabaseService.GetEventDetails(evt.Id);
                await DisplayAlert("Event Details", eventDetails, "OK");
            }
        }
    }
}