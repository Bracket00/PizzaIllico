using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Pages;
using Plugin.Settings;
using Storm.Mvvm;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class ShopListViewModel : ViewModelBase
    {
		private ShopItem _selectedShopItem;
		private string _accessToken;
		private string _shopId;
		public string ShopId
		{
			get { return _shopId; }
			set { SetProperty(ref _shopId, value); }
		}
		private List<PizzaItem> _createOrderRequest;
		public List<PizzaItem> CreateOrderRequest
		{
			get { return _createOrderRequest; }
			set
			{
                SetProperty(ref _createOrderRequest, value);
			}
		}
		public ShopItem SelectedShopItem
		{
			get => _selectedShopItem;
			set
			{
				if (SetProperty(ref _selectedShopItem, value) && value != null)
				{
					_selectedShopItem = value;
					SelectedAction(value);
				}
			}
		}
		public string AccessToken
		{
			get { return _accessToken; }
			set { SetProperty(ref _accessToken, value); }
		}
		private ObservableCollection<ShopItem> _shops;
		public ObservableCollection<ShopItem> Shops
		{
			get => _shops;
			set => SetProperty(ref _shops, value);
		}
		static public double Distance(double x1, double y1, double x2, double y2)
		{
			var R = 6378.137;
			var dLat = x2 * Math.PI / 180 - x1 * Math.PI / 180;
			var dLon = y2 * Math.PI / 180 - y1 * Math.PI / 180;
			var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
			Math.Cos(x1 * Math.PI / 180) * Math.Cos(x2 * Math.PI / 180) *
			Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			var d = R * c;
            if (d*1000 < 1000)
            {
				return Math.Round(d * 1000, 3);
            }
            else
            {
				return Math.Round(d);
			}
		}
		public ICommand SelectedCommand { get; }
		public ICommand HomeCommand { get; }
		public ShopListViewModel()
	    {
			AccessToken = JsonConvert.DeserializeObject<string>(CrossSettings.Current.GetValueOrDefault("AccessToken", string.Empty));
			CreateOrderRequest = new List<PizzaItem>();
			CrossSettings.Current.AddOrUpdateValue("Commands", JsonConvert.SerializeObject(CreateOrderRequest));
			SelectedCommand = new Command<ShopItem>(SelectedAction);
			HomeCommand = new Command(HomeAction);
	    }
	    private async void SelectedAction(ShopItem obj)
	    {
			CreateOrderRequest = new List<PizzaItem>();
			CrossSettings.Current.AddOrUpdateValue("Commands", JsonConvert.SerializeObject(CreateOrderRequest));

			ShopId = obj.Id.ToString();
			await NavigationService.PushAsync<ShopDetailsPage>(
				new Dictionary<string, object>()
				{
					{"shopId", ShopId }
				}
				);
		}
		public async void HomeAction()
		{
			await NavigationService.PopAsync();
		}
		public override async Task OnResume()
        {
	        await base.OnResume();
			var location = await Geolocation.GetLastKnownLocationAsync();

			List<ShopItem> myShops = JsonConvert.DeserializeObject<List<ShopItem>>(CrossSettings.Current.GetValueOrDefault("Shops", string.Empty));
			Shops = new ObservableCollection<ShopItem>(myShops.OrderBy(ShopItem => Distance(ShopItem.Latitude, ShopItem.Longitude, location.Latitude, location.Longitude)));
			foreach (ShopItem element in Shops)
			{
				element.Distance = Distance(element.Latitude, element.Longitude, location.Latitude, location.Longitude) + " km";
			}
        }
    }
}