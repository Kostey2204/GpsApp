using System.Linq;

using Android.Gms.Location;
using Android.Util;

namespace GpsApp
{
    public class FusedLocationProviderCallback : LocationCallback
    {
        readonly MainActivity activity;

        public FusedLocationProviderCallback(MainActivity activity)
        {
            this.activity = activity;
        }

        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            Log.Debug("FusedLocationProviderSample", "IsLocationAvailable: {0}",locationAvailability.IsLocationAvailable);
        }


        public override void OnLocationResult(LocationResult result)
        {
            if (result.Locations.Any())
            {
                var location = result.Locations.First();
                activity.txtLatitude.Text = location.Latitude.ToString();
                activity.txtLongitude.Text = location.Longitude.ToString();
            }
            else
            {
                activity.txtLatitude.Text = "unavail";
                activity.txtLongitude.Text = "unavail";
            }
        }        
    }
}