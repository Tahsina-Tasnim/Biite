using Biite.ViewModels;

namespace Biite.Pages
{
    public partial class SocialsPage : ContentPage
    {
        private SocialsPageViewModel viewModel;

        public SocialsPage()
        {
            InitializeComponent();
            viewModel = new SocialsPageViewModel();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.RefreshData();
        }

        private async void OnAcceptFriendRequestClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Info", "Accept friend functionality coming soon!", "OK");
        }

        private async void OnDeclineFriendRequestClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Info", "Decline friend functionality coming soon!", "OK");
        }
    }
}