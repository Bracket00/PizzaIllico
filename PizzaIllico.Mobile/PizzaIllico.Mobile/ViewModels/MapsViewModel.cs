using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos.Pizzas;
using Plugin.Geolocator;
using Plugin.Settings;
using Storm.Mvvm;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace PizzaIllico.Mobile.ViewModels
{
    public class MapsViewModel : ViewModelBase
    {
        public Xamarin.Forms.Maps.Map MainMap { get; private set; }
        public MapsViewModel()
        {
            Visible = false;
            MainMap = new Xamarin.Forms.Maps.Map()
            {
                IsShowingUser = true
            };
            
        }
        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            
            GetCurrentLocation();
            LoadPins();
            

        }
            public async void GetCurrentLocation()
        {
            var position = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(1000));
            MainMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude),
                                                         Distance.FromMiles(1)));
            Visible = true;
        }
        private void LoadPins()
        {
            foreach (var shop in JsonConvert.DeserializeObject<List<ShopItem>>(CrossSettings.Current.GetValueOrDefault("Shops", string.Empty)))
            {
                MainMap.Pins.Add(
                    new Pin
                    {
                        Type = PinType.SavedPin,
                        Position = new Position(shop.Latitude, shop.Longitude),
                        Label = shop.Name
                    }
                    );
            }
        }

        private bool visible;

        public bool Visible { get => visible; set => SetProperty(ref visible, value); }
    }
}