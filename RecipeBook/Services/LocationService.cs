using System;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace RecipeBook.Services
{
    public class LocationService
    {
        public async Task<string> GetCurrentLocationAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    location = await Geolocation.GetLocationAsync(request);
                }

                if (location != null)
                {
                    // Try to get the placemark (address) from coordinates
                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = placemarks?.FirstOrDefault();

                    if (placemark != null)
                    {
                        // Format the address
                        return $"{placemark.Locality}, {placemark.AdminArea}, {placemark.CountryName}";
                    }
                    else
                    {
                        // Return coordinates if placemark is not available
                        return $"Lat: {location.Latitude:F2}, Long: {location.Longitude:F2}";
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to get location: {ex.Message}");
                return string.Empty;
            }
        }
    }
}