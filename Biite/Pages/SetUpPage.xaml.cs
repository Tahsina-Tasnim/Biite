using Biite.ViewModels;

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
        // Jump into the TabBar at the Home tab (now the bar is visible)
        await Shell.Current.GoToAsync("//tabs/home");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}