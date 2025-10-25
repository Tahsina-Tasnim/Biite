namespace Biite.Pages;
using Biite.Services;

public partial class LoginPage : ContentPage
{
    public LoginPage() => InitializeComponent();
        protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetTabBarIsVisible(this, false);
        Shell.SetNavBarIsVisible(this, false);
    }

    //to not show the tabbar
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
       
    }

    //handling the login
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // gets the entered email and password
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;

        // validate that fields are not empty
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please enter both email and password", "OK");
            return;
        }

        // validate login credentials using DatabaseService
        var user = DatabaseService.ValidateLogin(email, password);

        if (user != null)
        {
            // goes to homepage if success
            await DisplayAlert("Welcome", $"Welcome back, {user.Name}!", "OK");
            await Shell.Current.GoToAsync("//tabs/home");
        }
        else
        {
            // shows error if not
            await DisplayAlert("Login Failed", "Invalid email or password. Please try again.", "OK");
        }
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        //go to setup page if signing in
        await Shell.Current.GoToAsync(nameof(SetUpPage));
    }
}
