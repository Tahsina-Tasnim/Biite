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
                await EditName();
            }
            else if (action == "Edit Bio")
            {
                await EditBio();
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

        // to upadte user name
        private async Task EditName()
        {
            string currentName = viewModel.UserName;

            string newName = await DisplayPromptAsync(
                "Edit Name",
                "Enter your name:",
                initialValue: currentName,
                maxLength: 50,
                keyboard: Keyboard.Text,
                placeholder: "Your name"
            );

            if (!string.IsNullOrWhiteSpace(newName) && newName != currentName)
            {
                viewModel.UpdateUserName(newName);
                viewModel.RefreshData();
                UpdateProfileImageVisibility(); // refresh the initial letter if no profile pic
                await DisplayAlert("Success", "Name updated successfully!", "OK");
            }
        }

        // to update user bio
        private async Task EditBio()
        {
            string currentBio = viewModel.UserBio;

            string newBio = await DisplayPromptAsync(
                "Edit Bio",
                "Enter your location/bio:",
                initialValue: currentBio,
                maxLength: 100,
                keyboard: Keyboard.Text,
                placeholder: " Tell us about yourself "
            );

            if (!string.IsNullOrWhiteSpace(newBio) && newBio != currentBio)
            {
                viewModel.UpdateUserBio(newBio);
                viewModel.RefreshData(); 
                await DisplayAlert("Success", "Bio updated successfully!", "OK");
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


    
