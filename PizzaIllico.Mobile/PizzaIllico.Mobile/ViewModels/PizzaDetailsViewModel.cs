using System;
using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Services;
using Plugin.Settings;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class PizzaDetailsViewModel : ViewModelBase
    {
        private string _shopId;
        private string _name;
        private string _description;
        private double _price;
        private string _urlimage;
        private PizzaItem _pizzaItem;
        private string _accessToken;
        public ICommand MakeAnOrderCommand { get; }
        private List<PizzaItem> _createOrderRequest;

        public Command HomeCommand { get; }
        public List<PizzaItem> CreateOrderRequest
        {
            get { return _createOrderRequest; }
            set { 
                SetProperty(ref _createOrderRequest, value); }
        }
        public string AccessToken
        {
            get { return _accessToken; }
            set { SetProperty(ref _accessToken, value); }
        }
        [NavigationParameter]
        public string ShopId
        {
            get { return _shopId; }
            set { SetProperty(ref _shopId, value); }
        }
        [NavigationParameter]
        public PizzaItem PizzaItem
        {
            get { return _pizzaItem; }
            set { SetProperty(ref _pizzaItem, value); }
        }
        public string UrlImage
        {
            get => _urlimage;
            set => SetProperty(ref _urlimage, value);
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);

            AccessToken = JsonConvert.DeserializeObject<string>(CrossSettings.Current.GetValueOrDefault("AccessToken", string.Empty));
            ShopId = GetNavigationParameter<string>("shopId");
            PizzaItem = GetNavigationParameter<PizzaItem>("pizzaItem");

            Name = PizzaItem.Name;
            Price = PizzaItem.Price;
            Description = PizzaItem.Description;
            UrlImage = "https://pizza.julienmialon.ovh/" + Urls.GET_IMAGE.Replace("{shopId}", ShopId).Replace("{pizzaId}", PizzaItem.Id.ToString());

            CreateOrderRequest = JsonConvert.DeserializeObject<List<PizzaItem>>(CrossSettings.Current.GetValueOrDefault("Commands", string.Empty));
        }
        public PizzaDetailsViewModel()
        {
            MakeAnOrderCommand = new Command(MakeAnOrderAction);
            HomeCommand = new Command(HomeAction);
        }
        public async void HomeAction()
        {
            await NavigationService.PopAsync();
            await NavigationService.PopAsync();
            await NavigationService.PopAsync();
        }
        public async void MakeAnOrderAction()
        {
            if (CreateOrderRequest == null)
            {
                CreateOrderRequest = new List<PizzaItem>();
            }
            CreateOrderRequest.Add(PizzaItem);

            if (PizzaItem.OutOfStock)
            {
                await Application.Current.MainPage.DisplayAlert("Ohh no ", "It's out of stock !", "OK");
            }
            else
            {
                CrossSettings.Current.AddOrUpdateValue("Commands", JsonConvert.SerializeObject(CreateOrderRequest));
                CrossSettings.Current.AddOrUpdateValue("ShopId", JsonConvert.SerializeObject(ShopId));
                await Application.Current.MainPage.DisplayAlert("Nice !", "Pizza has been added to your cart !", "OK");
                await NavigationService.PopAsync();
            }
        }
    }
}