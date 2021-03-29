using System.Collections.Generic;
using System;
using System.Windows.Input;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Pages;
using PizzaIllico.Mobile.Services;
using Plugin.Settings;
using Storm.Mvvm;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace PizzaIllico.Mobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private string _mail;
        private string _accessToken;
        private List<PizzaItem> _createOrderRequest;
        private bool visible;
        private string _numberOfPizza;

        public bool Visible { get => visible; set => SetProperty(ref visible, value); }
        public string NumberOfPizza
        {
            get => _numberOfPizza;
            set
            {
                _numberOfPizza = value;
                OnPropertyChanged(nameof(NumberOfPizza));
            }
        }

        public List<PizzaItem> CreateOrderRequest
        {
            get { return _createOrderRequest; }
            set
            {
                SetProperty(ref _createOrderRequest, value);
            }
        }
        public ICommand EditProfileCommand { get; }
        public ICommand ListRestaurantsCommand { get; }
        public ICommand MyCartCommand { get; }
        public ICommand MapCommand { get; }
        public ICommand MyOrdersCommand { get; }
        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                OnPropertyChanged(nameof(Mail));
            }
        }
        public string AccessToken
        {
            get { return _accessToken; }
            set { SetProperty(ref _accessToken, value); }
        }
        public LoginPageViewModel()
        {
            EditProfileCommand = new Command(EditProfileAction);
            ListRestaurantsCommand = new Command(ListRestaurantsAction);
            MyCartCommand = new Command(MyCartAction);
            MapCommand = new Command(MapAction);
            MyOrdersCommand = new Command(OrdersAction);
            Mail = JsonConvert.DeserializeObject<string>(CrossSettings.Current.GetValueOrDefault("mail", string.Empty));

            GetListShops();
            Visible = true;
            var _shopId = JsonConvert.DeserializeObject<string>(CrossSettings.Current.GetValueOrDefault("ShopId", string.Empty));
            if (!string.IsNullOrEmpty(_shopId))
            {
                var _commands = JsonConvert.DeserializeObject<List<PizzaItem>>(CrossSettings.Current.GetValueOrDefault("Commands", string.Empty));
                NumberOfPizza = _commands.Count.ToString();
            }
            MessagingCenter.Subscribe<MyCartViewModel, string>(this, "Hi", (sender, arg) =>
            {
                NumberOfPizza = arg;
            });
        }
        public async void MapAction()
        {
            await NavigationService.PushAsync<Maps>();
        }
        public async void OrdersAction()
        {
            await NavigationService.PushAsync<OrdersPage>();
        }

        public async void EditProfileAction()
        {
            await NavigationService.PushAsync<EditProfilePage>();
        }
        public async void MyCartAction()
        {
            await NavigationService.PushAsync<MyCartPage>();

        }
        public async void ListRestaurantsAction()
        {
            await NavigationService.PushAsync<ShopListPage>();
        }
        public async void GetListShops()
        {
            Response<List<ShopItem>> response = await DependencyService.Get<IPizzaApiService>().ListShops();
            CrossSettings.Current.AddOrUpdateValue("Shops", JsonConvert.SerializeObject(response.Data)) ;
        }
        //OnResume ne fonctionne pas..
        public async override Task OnResume()
        {
            await base.OnResume();
            Visible = true;
            var _shopId = JsonConvert.DeserializeObject<string>(CrossSettings.Current.GetValueOrDefault("ShopId", string.Empty));
            if (!string.IsNullOrEmpty(_shopId))
            {
                var _commands = JsonConvert.DeserializeObject<List<PizzaItem>>(CrossSettings.Current.GetValueOrDefault("Commands", string.Empty));
                NumberOfPizza = _commands.Count.ToString();
                
            }
        }

    }
}