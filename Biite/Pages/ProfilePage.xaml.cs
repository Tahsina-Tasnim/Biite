using Biite.ViewModels;

namespace Biite.Pages

{
    public partial class ProfilePage : ContentPage
    {
        private ProfilePageViewModel viewModel;
        public ProfilePage()
        {
            InitializeComponent();
            viewModel = new ProfilePageViewModel();

            BindingContext = viewModel;
            UpdateProfileImageVisibility();
        }
        private async void OnProfileImageTapped(object sender, EventArgs e)
        {
            await LoadProfileImage();
        }

        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Edit Profile", "Cancel", null, "Change Profile Picture", "Edit Name", "Edit Bio");

            if (action == "Change Profile Picture")
            {
                await LoadProfileImage();
            }
            else if (action == "Edit Name")
            {
                await DisplayAlert("Info", "Edit name functionality coming soon!", "OK");
            }
            else if (action == "Edit Bio")
            {
                await DisplayAlert("Info", "Edit bio functionality coming soon!", "OK");
            }
        }
        private async Task LoadProfileImage()
        {
            FileResult photo = null;

            if (MediaPicker.Default.IsCaptureSupported && DeviceInfo.Platform != DevicePlatform.WinUI)
            {
                PermissionStatus status = await GetCameraPermission();
                if (status == PermissionStatus.Granted)
                {
                    var choice = await DisplayActionSheet("Select Photo Source", "Cancel", null, "Take Photo", "Choose from Gallery");

                    if (choice == "Take Photo")
                    {
                        photo = await MediaPicker.CapturePhotoAsync();
                    }
                    else if (choice == "Choose from Gallery")
                    {
                        photo = await MediaPicker.PickPhotoAsync();
                    }
                }
            }
            else
            {
                photo = await MediaPicker.PickPhotoAsync();
            }

            if (photo != null)
            {
                string imagesDir = Path.Combine(FileSystem.Current.AppDataDirectory, "images");
                Directory.CreateDirectory(imagesDir);

                var newFile = Path.Combine(imagesDir, photo.FileName);

                using (var stream = await photo.OpenReadAsync())
                using (var newStream = File.OpenWrite(newFile))
                {
                    await stream.CopyToAsync(newStream);
                }

                viewModel.UpdateProfileImage(newFile);
                viewModel.RefreshData();
                UpdateProfileImageVisibility(); 
            }
        }

        private async Task<PermissionStatus> GetCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission Denied", "Camera permission is required to take photos.", "OK");
            }

            return status;
        }

        // manually control visibility 
        private void UpdateProfileImageVisibility()
        {
            bool hasImage = viewModel.HasProfileImage;
            ProfileImage.IsVisible = hasImage;
            DefaultProfileLabel.IsVisible = !hasImage;
        }

        private async void OnViewReviewsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ReviewsPage));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.RefreshData();
            UpdateProfileImageVisibility();
        }
    }
}


    
