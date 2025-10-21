using Biite.ViewModels;

namespace Biite.Pages
{
    public partial class HomePage : ContentPage
    {
        private HomePageViewModel viewModel;
        public HomePage()
        {

            InitializeComponent();
            viewModel = new HomePageViewModel();

            BindingContext = viewModel;
        }

        private async void CreateEvent_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CreateEventsPage));
        }

        private async void FindFriends_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//tabs/socials");
        }

        private async void AddReview_Clicked(object sender, EventArgs e) //got rid of find friends not needed since there is already a Socials page so
        {
            await Shell.Current.GoToAsync(nameof(AddReviewPage));
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.RefreshData();
        }
    }
}



