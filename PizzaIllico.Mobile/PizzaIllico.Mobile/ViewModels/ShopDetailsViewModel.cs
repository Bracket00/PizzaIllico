using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Pages;
using PizzaIllico.Mobile.Services;
using Plugin.Settings;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class ShopDetailsViewModel : ViewModelBase
    {
        private ObservableCollection<PizzaItem> _pizzas;
        private PizzaItem _selectedPizza;
        private string _shopId;
        private string _accessToken;
        private List<PizzaItem> _createOrderRequest;
        private ShopItem _shopItem;

        [NavigationParameter]
        public ShopItem ShopItem
        {
            get { return _shopItem; }
            set
            {
                SetProperty(ref _shopItem, value);
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
        public ICommand SelectedPizzaCommand { get; }
        public ICommand MyCartCommand { get; }
        public Command HomeCommand { get; }
        public ObservableCollection<PizzaItem> Pizzas
        {
            get => _pizzas;
            set => SetProperty(ref _pizzas, value);
        }

        public string AccessToken
        {
            get { return _accessToken; }
            set { SetProperty<string>(ref _accessToken, value); }
        }

        [NavigationParameter]
        public string ShopId
        {
            get { return _shopId; }
            set { SetProperty<string>(ref _shopId, value); }
        }
        public PizzaItem SelectedPizza
        {
            get => _selectedPizza;
            set
            {
                if (SetProperty(ref _selectedPizza, value) && value != null)
                {
                    _selectedPizza = value;
                    SelectedPizzaAction(value);
                    
                }
            }
        }
        public async void HomeAction()
        {
            await NavigationService.PopAsync();
            await NavigationService.PopAsync();
        }
        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            AccessToken = JsonConvert.DeserializeObject<string>(CrossSettings.Current.GetValueOrDefault("AccessToken", string.Empty));
            ShopId = GetNavigationParameter<string>("shopId");

            CreateOrderRequest = JsonConvert.DeserializeObject<List<PizzaItem>>(CrossSettings.Current.GetValueOrDefault("Commands", string.Empty));
            GetShopDetails();
        }
        public ShopDetailsViewModel()
        {
            SelectedPizzaCommand = new Command<PizzaItem>(SelectedPizzaAction);
            HomeCommand = new Command(HomeAction);
            MyCartCommand = new Command(MyCartAction);
        }


        public async void GetShopDetails()
        {
            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();
            Response<List<PizzaItem>> response = await service.ListPizzas(ShopId) ;
            if (response.IsSuccess)
            {
                Pizzas = new ObservableCollection<PizzaItem>(response.Data.OrderBy(PizzaItem => PizzaItem.Price));

            }
        }

        private async void SelectedPizzaAction(PizzaItem obj)
        {
            await NavigationService.PushAsync<PizzaDetailsPage>(
                new Dictionary<string, object>()
                {
                    { "accessToken", AccessToken},
                    {"shopId", ShopId },
                    {"pizzaItem", obj }
                }
                );
        }
        private async void MyCartAction()
        {
            await NavigationService.PushAsync<MyCartPage>();
        }
    }
}