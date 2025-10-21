namespace Biite.Pages;

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

    private async void OnLoginClicked(object sender, EventArgs e)
    {

        //go straight to homepage if login
        await Shell.Current.GoToAsync("//tabs/home");
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        //go to setup page if signing in
        await Shell.Current.GoToAsync(nameof(SetUpPage));
    }
}
