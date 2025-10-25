using Biite.ViewModels;

namespace Biite.Pages
{
    public partial class DiscoverPage : ContentPage
    {
        private DiscoverPageViewModel viewModel;

        public DiscoverPage()
        {
            InitializeComponent();
            viewModel = new DiscoverPageViewModel();
            BindingContext = viewModel;
        }

        private async void OnCardTapped(object sender, EventArgs e)
        {
            var tappedEventArgs = e as TappedEventArgs;
            if (tappedEventArgs?.Parameter is string url)
            {
                await OpenUrl(url);
            }
        }

        private async Task OpenUrl(string url)
        {
            try
            {
                await Launcher.Default.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                // Handle the case where the browser cannot be opened
                await DisplayAlert("Error", "Unable to open browser: " + ex.Message, "OK");
            }
        }

        // handles the search bar tap
        private async void OnSearchBarTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MapPage));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.RefreshData();
        }
    }
}