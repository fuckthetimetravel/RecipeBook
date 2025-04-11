using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Maui.Devices.Sensors;

namespace RecipeBook.Services
{
    public class LocationService
    {
        // Retrieves the current location as a formatted string.
        // Attempts to use the last known location, and if unavailable, requests a new location.
        // If placemark (address) data is available, returns a formatted address; otherwise, returns coordinates.
        public async Task<string> GetCurrentLocationAsync()
        {
            try
            {
                // Attempt to obtain the last known location
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    // Request a new location if the last known location is not available
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    location = await Geolocation.GetLocationAsync(request);
                }

                if (location != null)
                {
                    // Attempt to retrieve placemark information using the current coordinates
                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = placemarks?.FirstOrDefault();

                    if (placemark != null)
                    {
                        // Return a formatted address string
                        return $"{placemark.Locality}, {placemark.AdminArea}, {placemark.CountryName}";
                    }
                    else
                    {
                        // Return the coordinates if no placemark data is found
                        return $"Lat: {location.Latitude:F2}, Long: {location.Longitude:F2}";
                    }
                }

                // Return an empty string if the location cannot be determined
                return string.Empty;
            }
            catch (Exception ex)
            {
                // Log the exception message and return an empty string
                Console.WriteLine($"Unable to get location: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
