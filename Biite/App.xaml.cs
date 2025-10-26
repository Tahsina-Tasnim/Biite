using Biite.Services;
using Biite.Models;
namespace Biite
{
    public partial class App : Application
    {
        //public static DatabaseService Database { get; private set; }
        public App()
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            InitializeComponent();
            
            //Database = new DatabaseService();

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            //first screen is always login unless I add authentication feature ? maybe later?
            window.Page.Dispatcher.Dispatch(async () =>
            {
                await Shell.Current.GoToAsync(nameof(Biite.Pages.LoginPage));
            });

            return window;

        }
    }
}
