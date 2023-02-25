using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.OS;
using Android.Support.V7.App;
using Android.Content.PM;
using Android.Widget;
using Android.Locations;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;

namespace GpsApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        static readonly int RC_LAST_LOCATION_PERMISSION_CHECK = 1000;
        static readonly int RC_LOCATION_UPDATES_PERMISSION_CHECK = 1100;
        
        const long ONE_MINUTE = 60 * 1000;
        const long FIVE_MINUTES = 5 * ONE_MINUTE;
        const long TWO_MINUTES = 2 * ONE_MINUTE;
        
        bool isGooglePlayServicesInstalled;
        bool isRequestingLocationUpdates;
        
        LocationCallback locationCallback;
        LocationRequest locationRequest;
        
        FusedLocationProviderClient fusedLocationProviderClient;
        
        public EditText txtLatitude;
        public EditText txtLongitude;
        private Button btnGetGPS;
        
        Location currentLocation;  
        LocationManager locationManager;  
        string locationProvider;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            
            isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();
            
            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            
            txtLatitude = FindViewById<EditText>(Resource.Id.latitude);
            txtLongitude = FindViewById<EditText>(Resource.Id.longitude);
            btnGetGPS = FindViewById<Button>(Resource.Id.button);
            
            if (isGooglePlayServicesInstalled)
            {
                locationRequest = new LocationRequest()
                    .SetPriority(LocationRequest.PriorityHighAccuracy)
                    .SetInterval(FIVE_MINUTES)
                    .SetFastestInterval(TWO_MINUTES);
                
                locationCallback = new FusedLocationProviderCallback(this);

                fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
                btnGetGPS.Click += btnGetGPSOnClick;
            }
            else
            {
                throw new Exception("missing_googleplayservices_terminating");
            }
        }
        
        protected override void OnPause()
        {
            StopRequestLocationUpdates();
            base.OnPause();
        }

        async void StopRequestLocationUpdates()
        {
            txtLatitude.Text = string.Empty;
            txtLongitude.Text = string.Empty;

            if (isRequestingLocationUpdates)
            {
                await fusedLocationProviderClient.RemoveLocationUpdatesAsync(locationCallback);
            }
        }
        
        private async void btnGetGPSOnClick(object sender, EventArgs eventArgs)
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                await GetLastLocationFromDevice();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new[] {Manifest.Permission.AccessFineLocation}, RC_LAST_LOCATION_PERMISSION_CHECK);
            }
        }
        
        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK || requestCode == RC_LOCATION_UPDATES_PERMISSION_CHECK)
            {
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK)
                    {
                        await GetLastLocationFromDevice();
                    }
                    else
                    {
                        await fusedLocationProviderClient.RequestLocationUpdatesAsync(locationRequest, locationCallback);
                        isRequestingLocationUpdates = true;
                    }
                }
                else
                {
                    throw new Exception("permission_not_granted_termininating_app");
                }
            }
            else
            {
                Log.Debug("FusedLocationProviderSample", "Don't know how to handle requestCode " + requestCode);
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        
        async Task GetLastLocationFromDevice()
        {
            // This method assumes that the necessary run-time permission checks have succeeded.
            Location location = await fusedLocationProviderClient.GetLastLocationAsync();

            if (location == null)
            {
                // Seldom happens, but should code that handles this scenario
                txtLatitude.Text = "error when receiving coordiants";
            }
            else
            {
                // Do something with the location 
                txtLatitude.Text = location.Latitude.ToString();
                txtLongitude.Text = location.Longitude.ToString();
            }
        }
        
        bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}",
                    queryResult, errorString);

                // Alternately, display the error to the user.
            }

            return false;
        }
    }
}