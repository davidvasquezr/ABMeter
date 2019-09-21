using ABMeter.Views.NavPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace ABMeter.Views.Home
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Home : ContentPage
	{
        bool isStart;
        public DateTime TimeStart { get; set; }

        public Home()
		{
            CustomNavigationPage.SetTitleMargin(this, new Thickness(0, 0, 0, 0));
            InitializeComponent ();

            map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;

            //Sets the title position to end
            CustomNavigationPage.SetTitlePosition(this, CustomNavigationPage.TitleAlignment.Center);

            //Sets shadow for bar bottom
            CustomNavigationPage.SetHasShadow(this, true);

            //Sets the title text font to Micro
            CustomNavigationPage.SetTitleFont(this, Font.SystemFontOfSize(NamedSize.Large));

            //Sets the title color
            CustomNavigationPage.SetTitleColor(this, Color.White);

            //Sets bar background opacity
            //CustomNavigationPage.SetBarBackgroundOpacity(this, 0.6f);

            CustomNavigationPage.SetGradientColors(this, new Tuple<Color, Color>(Color.FromHex("#00838f"), Color.FromHex("#00838f")));
            //CustomNavigationPage.SetGradientColors(this, new Tuple<Color, Color>(Color.FromHex("#00838f"), Color.FromHex("#4fb3bf")));
            CustomNavigationPage.SetGradientDirection(this, CustomNavigationPage.GradientDirection.LeftToRight);

            // Map Long clicked
            map.MapLongClicked += (sender, e) =>
            {
                var lat = e.Point.Latitude.ToString("0.000000");
                var lng = e.Point.Longitude.ToString("0.000000");
                //this.DisplayAlert("MapLongClicked", $"{lat}/{lng}", "CLOSE");
                latDestino.Text = lat;
                lonDestino.Text = lng;

                var pin = new Pin()
                {
                    Label = "Destino",
                    Position = new Position(e.Point.Latitude, e.Point.Longitude)
                };

                map.Pins.Clear();
                map.Pins.Add(pin);
            };

            //Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            //{
            //    // Do something
            //    CalculateLocation();

            //    return llego; // True = Repeat again, False = Stop the timer
            //});
        }

        
        private async void CalculateLocation()
        {
            if (isStart)
            {   
                try
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    var location = await Geolocation.GetLocationAsync(request);

                    if (location != null)
                    {
                        //Obtiene cordenadas
                        latOrigen.Text = location.Latitude.ToString();
                        lonOrigen.Text = location.Longitude.ToString();
                        Location origin = new Location(location.Latitude, location.Longitude);
                        Location destination = new Location(Convert.ToDouble(latDestino.Text), Convert.ToDouble(lonDestino.Text));

                        //Calcula las cordenadas
                        double kms = Location.CalculateDistance(origin, destination, DistanceUnits.Kilometers);
                        lblDistance.Text = kms.ToString("0.000");
                        if (kms < 0.01)
                        {
                            //Quiere decir que llego lo mas cercano posible al punto
                            isStart = false;
                            await this.DisplayAlert("Llegaste", "Que bien perro. " + DateTime.Now.TimeOfDay, "Cerrar");
                        }
                    }
                }
                catch (FeatureNotSupportedException fnsEx)
                {
                    // Handle not supported on device exception
                }
                catch (FeatureNotEnabledException fneEx)
                {
                    // Handle not enabled on device exception
                }
                catch (PermissionException pEx)
                {
                    // Handle permission exception
                }
                catch (Exception ex)
                {
                    // Unable to get location
                }
            }

        }

        private void Btn_Clicked(object sender, EventArgs e)
        {
            isStart = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                // Do something
                CalculateLocation();

                return isStart; // True = Repeat again, False = Stop the timer
            });

            //var location = new Location(Convert.ToDouble("7.1062345"), Convert.ToDouble("-73.1051232"));
            //var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };

            //Map.OpenAsync(location, options);

            var location = new Location(Convert.ToDouble(latDestino.Text), Convert.ToDouble(lonDestino.Text));
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };

            Xamarin.Essentials.Map.OpenAsync(location, options);

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //your code here;
            Position position = await GetLocation();
            latOrigen.Text = position.Latitude.ToString("0.000000");
            lonOrigen.Text = position.Longitude.ToString("0.000000");
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMeters(500)));
        }

        private async Task<Position> GetLocation()
        {
            Position position = new Position(0,0);
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    //Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    position = new Position(location.Latitude, location.Longitude);
                }
            }
            catch (Exception ex)
            {
            }

            return position;
        }

    }
}