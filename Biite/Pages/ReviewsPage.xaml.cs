using Biite.ViewModels;

namespace Biite.Pages
{

    public partial class ReviewsPage : ContentPage
    {
        private ReviewsPageViewModel viewModel;
        public ReviewsPage()
        {
            InitializeComponent();
            viewModel = new ReviewsPageViewModel();
            BindingContext = viewModel;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.RefreshData();
        }
    }
}