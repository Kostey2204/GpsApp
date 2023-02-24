using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Locations;

namespace GpsApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ILocationListener
    {
        private EditText txtLatitude;
        private EditText txtLongitude;
        
        Location currentLocation;  
        LocationManager locationManager;  
        string locationProvider;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            
            txtLatitude = FindViewById<EditText>(Resource.Id.latitude);
            txtLongitude = FindViewById<EditText>(Resource.Id.longitude);
            
            InitializeLocationManager();
        }
        private void InitializeLocationManager()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);

            var criteriaForLocationService = new Criteria {Accuracy = Accuracy.Fine};
            
            try
            {
                locationProvider = locationManager.GetBestProvider(criteriaForLocationService, true);
                if (locationManager.IsProviderEnabled(locationProvider))
                {
                    locationManager.RequestLocationUpdates(locationProvider, 2000, 1, this);
                }
                else
                {
                    throw new Exception("error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
        
        public void OnLocationChanged(Location location)  
        {  
            currentLocation = location;  
            if (currentLocation == null)  
            {  
                txtLatitude.Text = "error";
                txtLongitude.Text = "error";  
            }
            else   
            {  
                txtLatitude.Text = currentLocation.Latitude.ToString();  
                txtLongitude.Text = currentLocation.Longitude.ToString();  
            }  
        }

        public void OnProviderDisabled(string? provider)
        {
            
        }

        public void OnProviderEnabled(string? provider)
        {
            
        }

        public void OnStatusChanged(string? provider, Availability status, Bundle? extras)
        {
            
        }

        protected override void OnResume()  
        {  
            base.OnResume();  
            //locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);  
        }  
        protected override void OnPause()  
        {  
            base.OnPause();  
            locationManager.RemoveUpdates(this);  
        }
    }
}