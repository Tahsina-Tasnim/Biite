using Biite.ViewModels;
using Biite.Services;   
namespace Biite.Pages;

public partial class SetUpPage : ContentPage
{
    private SetUpPageViewModel viewModel;
    public SetUpPage()
    {
        InitializeComponent();
        viewModel = new SetUpPageViewModel();
        BindingContext = viewModel;

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetTabBarIsVisible(this, false);
        Shell.SetNavBarIsVisible(this, false);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // validates that all fields are filled no space or blank
        if (string.IsNullOrWhiteSpace(viewModel.Name) ||
            string.IsNullOrWhiteSpace(viewModel.Email) ||
            string.IsNullOrWhiteSpace(viewModel.PhoneNumber) ||
            string.IsNullOrWhiteSpace(viewModel.Location) ||
            string.IsNullOrWhiteSpace(viewModel.Password))
        {
            await DisplayAlert("Error", "Please fill in all required fields (Name, Email, Phone, Location, and Password)", "OK");
            return;
        }

        // checks if email already exists
        if (DatabaseService.EmailExists(viewModel.Email))
        {
            await DisplayAlert("Error", "An account with this email already exists. Please login instead.", "OK");
            return;
        }

        // creates the new user
        var newUser = DatabaseService.CreateNewUser(
            viewModel.Name,
            viewModel.Email,
            viewModel.PhoneNumber,
            viewModel.Location,
            viewModel.Restriction ?? "None", // defaults to "None" if empty
            viewModel.Password
        );

        if (newUser != null)
        {
            await DisplayAlert("Success", $"Welcome to Biite, {newUser.Name}!", "OK");
            // jump into the TabBar at the Home tab now the bar is visible
            await Shell.Current.GoToAsync("//tabs/home");
        }
        else
        {
            await DisplayAlert("Error", "Failed to create account. Please try again.", "OK");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}