using Biite.ViewModels;
using CommunityToolkit.Maui.Views;

namespace Biite.Pages
{
    public partial class AddReviewPage : ContentPage
    {
        private AddReviewPageViewModel viewModel;
        private int selectedRating = 0;
        private string selectedPhotoPath = null;
        public AddReviewPage()
        {
            InitializeComponent();
            viewModel = new AddReviewPageViewModel();
            BindingContext = viewModel;
        }

        private void OnStarClicked(object sender, EventArgs e)
        {
            var clickedButton = sender as Button;

            // determines which star was clicked
            if (clickedButton == Star1) selectedRating = 1;
            else if (clickedButton == Star2) selectedRating = 2;
            else if (clickedButton == Star3) selectedRating = 3;
            else if (clickedButton == Star4) selectedRating = 4;
            else if (clickedButton == Star5) selectedRating = 5;

            // updates star colors
            UpdateStarDisplay();

            // updates rating label
            RatingLabel.Text = $"Rating: {selectedRating} stars";

            // updates ViewModel to show
            viewModel.Rating = selectedRating;
        }
        //could not use the toolkit for some reason so making the stars manually,
        //the unicode was picked up from the net and then added and somehow it worked without throwing errors
        //do want to get rid of this system and use toolkit ask for help
        private void UpdateStarDisplay()
        {
            var stars = new[] { Star1, Star2, Star3, Star4, Star5 };

            for (int i = 0; i < stars.Length; i++)
            {
                // colors the stars to the selected rating in orange, rest is in gray
                stars[i].TextColor = i < selectedRating ? Colors.Orange : Colors.Gray;
            }
        }

        // photo handler specifically for android
        private async void OnAddPhotoClicked(object sender, EventArgs e)
        {
            try
            {
                FileResult photo = null;

                // Show choice for users camera or gallery
                var choice = await DisplayActionSheet("Add Food Photo", "Cancel", null, "Take Photo", "Choose from Gallery");

                if (choice == "Take Photo")
                {
                    // checks camera permission
                    var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.Camera>();
                    }

                    if (status == PermissionStatus.Granted)
                    {
                        photo = await MediaPicker.CapturePhotoAsync();
                    }
                    else
                    {
                        await DisplayAlert("Permission Denied", "Camera permission is required to take photos.", "OK");
                        return;
                    }
                }
                else if (choice == "Choose from Gallery")
                {
                    // gallery doesn't need special permissions 
                    photo = await MediaPicker.PickPhotoAsync();
                }
                else
                {
                    // user cancelled
                    return;
                }

                // user selected a photo
                if (photo != null)
                {
                    //creates review_images directory
                    string imagesDir = Path.Combine(FileSystem.Current.AppDataDirectory, "review_images");
                    Directory.CreateDirectory(imagesDir);

                    // creates unique filename with timestamp idk if needed but used
                    string fileName = $"review_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(photo.FileName)}";
                    var newFile = Path.Combine(imagesDir, fileName);

                    // copy the photo to app directory
                    using (var stream = await photo.OpenReadAsync())
                    using (var newStream = File.OpenWrite(newFile))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    // stores the path and show preview
                    selectedPhotoPath = newFile;
                    PhotoPreview.Source = ImageSource.FromFile(newFile);
                    PhotoPreviewFrame.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load photo: {ex.Message}", "OK");
            }
        }

        // remove photo handler
        private void OnRemovePhotoClicked(object sender, EventArgs e)
        {
            selectedPhotoPath = null;
            PhotoPreview.Source = null;
            PhotoPreviewFrame.IsVisible = false;
        }
        private async void OnSaveReviewClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.RestaurantName)) //to ensure name isnt just a space so no not NullOrEmpty
            {
                await DisplayAlert("Error", "Please enter a restaurant name", "OK");
                return;
            }

            if (viewModel.Rating == 0) //to make sure that user cannot leave a review without star rating, no review text is fine tho
            {
                await DisplayAlert("Error", "Please enter a star rating for review", "OK"); //to show a pop up used DisplayAlert 
                return;
            }


            await viewModel.SaveReview(selectedPhotoPath);
            await DisplayAlert("Success", "Review saved successfully!", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}