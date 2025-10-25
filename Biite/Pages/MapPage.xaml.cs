using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Biite.Services;      
using Biite.Models;
using System.Globalization;

namespace Biite.Pages
{
    [QueryProperty(nameof(IsLocationPicker), "IsLocationPicker")]
    public partial class MapPage : ContentPage
    {
        private Pin selectedPin;
        private Location selectedLocation;
        private bool isLocationPickerMode = false;

        public string IsLocationPicker
        {
            set
            {
                isLocationPickerMode = value == "true";
                ConfigureMapMode();
            }
        }

        public MapPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Center map on user location
            await CenterMapOnUserLocation();

            // Add restaurant pins
            AddRestaurantPins();
        }

        private void ConfigureMapMode()
        {
            if (isLocationPickerMode)
            {
                // Location picker mode
                Title = "Choose Event Location";
                SearchBar.IsVisible = false;
                EventLocation.IsVisible = true;
                ConfirmButton.IsVisible = true;
            }
            else
            {
                // Restaurant discovery mode
                Title = "Find Restaurants";
                SearchBar.IsVisible = true;
                EventLocation.IsVisible = false;
                ConfirmButton.IsVisible = false;
            }
        }

        private async Task CenterMapOnUserLocation()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(10)
                });

                if (location != null)
                {
                    MapSpan mapSpan = MapSpan.FromCenterAndRadius(
                        location,
                        Distance.FromKilometers(5));
                    map.MoveToRegion(mapSpan);
                }
            }
            catch
            {
                // Default to Newcastle if location fails
                var newcastleLocation = new Location(-32.9272, 151.7820);
                MapSpan mapSpan = MapSpan.FromCenterAndRadius(
                    newcastleLocation,
                    Distance.FromKilometers(10));
                map.MoveToRegion(mapSpan);
            }
        }

        private void AddRestaurantPins()
        {
            var connection = DatabaseService.Connection;
            var restaurants = connection.Table<Restaurant>().ToList();

            foreach (var restaurant in restaurants)
            {
                var pin = new Pin
                {
                    Label = restaurant.Name,
                    Address = $"{restaurant.Cuisine} • {restaurant.Price} • {restaurant.Rating}⭐",
                    Type = PinType.Place,
                    Location = new Location(restaurant.Latitude, restaurant.Longitude)
                };

                pin.MarkerClicked += (s, args) =>
                {
                    args.HideInfoWindow = false;
                    if (isLocationPickerMode)
                    {
                        // Remove previous selected pin if exists
                        if (selectedPin != null)
                        {
                            map.Pins.Remove(selectedPin);
                        }

                        // Set the restaurant location as selected
                        selectedLocation = pin.Location;

                        // Create a new pin to show selection
                        selectedPin = new Pin
                        {
                            Label = "Event Location",
                            Address = $"Selected: {pin.Label}",
                            Type = PinType.SavedPin,
                            Location = selectedLocation
                        };

                        map.Pins.Add(selectedPin);
                        ConfirmButton.IsEnabled = true;

                        args.HideInfoWindow = true; // Hide the restaurant info window
                    }
                };

                map.Pins.Add(pin);
            }
        }

        private void OnMapClicked(object sender, MapClickedEventArgs e)
        {
            if (isLocationPickerMode)
            {
                // for location picker mode places a pin where user clicks
                if (selectedPin != null)
                {
                    map.Pins.Remove(selectedPin);
                }

                selectedLocation = e.Location;
                selectedPin = new Pin
                {
                    Label = "Event Location",
                    Address = $"{selectedLocation.Latitude:F4}, {selectedLocation.Longitude:F4}",
                    Type = PinType.SavedPin,
                    Location = selectedLocation
                };

                map.Pins.Add(selectedPin);
                ConfirmButton.IsEnabled = true;
            }
            else
            {
                // Remove previous temporary pin if exists
                if (selectedPin != null)
                {
                    map.Pins.Remove(selectedPin);
                }

                // Create temporary pin at tapped location
                selectedPin = new Pin
                {
                    Label = "📍 Tapped Location",
                    Address = $"Lat: {e.Location.Latitude:F6}, Lon: {e.Location.Longitude:F6}",
                    Type = PinType.Place,
                    Location = e.Location
                };

                map.Pins.Add(selectedPin);

                // for restaurant mode
                System.Diagnostics.Debug.WriteLine($"Map clicked at: {e.Location.Latitude}, {e.Location.Longitude}");
            }
        }

        //  method for confirm button
        private async void OnConfirmClicked(object sender, EventArgs e)
        {
            if (selectedLocation != null)
            {
                // passes the location back to CreateEventsPage
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Latitude", selectedLocation.Latitude.ToString(CultureInfo.InvariantCulture) },
                    { "Longitude", selectedLocation.Longitude.ToString(CultureInfo.InvariantCulture) },
                    { "LocationName", $"📍 ({selectedLocation.Latitude:F4}, {selectedLocation.Longitude:F4})" }
                };

                await Shell.Current.GoToAsync("..", navigationParameter);
            }
        }
    }
}