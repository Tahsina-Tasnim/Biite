using Biite.Pages;

namespace Biite
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {

            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(SetUpPage), typeof(SetUpPage));
            Routing.RegisterRoute(nameof(CreateEventsPage), typeof(CreateEventsPage));
            Routing.RegisterRoute(nameof(AddReviewPage), typeof(AddReviewPage));
            Routing.RegisterRoute(nameof(ReviewsPage), typeof(ReviewsPage));
            Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));

        }
    }
}
