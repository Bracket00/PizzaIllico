using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Pages;
using PizzaIllico.Mobile.Services;
using Plugin.Settings;
using Storm.Mvvm;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class MyCartViewModel : ViewModelBase
    {
        private ObservableCollection<PizzaItem> _pizzas;
        private double _total;
        private CreateOrderRequest createOrderRequest;
        public ICommand PassAnOrderCommand { get; }
        public Command HomeCommand { get; }
        public ICommand ClearCartCommand { get; }
        public string AccessToken { get; private set; }
        public string ShopId { get; private set; }
        public double Total
        {
            get { return _total; }
            set
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
            }
        }
        public ObservableCollection<PizzaItem> Pizzas
        {
            get => _pizzas;
            set => SetProperty(ref _pizzas, value);
        }
        public CreateOrderRequest CreateOrderRequest { get => createOrderRequest; set => createOrderRequest = value; }
        public MyCartViewModel()
        {
            PassAnOrderCommand = new Command(PassAnOrderAction);
            HomeCommand = new Command(HomeAction);
            ClearCartCommand = new Command(ClearCartAction);
            AccessToken = JsonConvert.DeserializeObject<string>(CrossSettings.Current.GetValueOrDefault("AccessToken", string.Empty));
            ShopId = JsonConvert.DeserializeObject<string>(CrossSettings.Current.GetValueOrDefault("ShopId", string.Empty));
            Total = 0;
            CreateOrderRequest = new CreateOrderRequest
            {
                PizzaIds = new List<long>()
            };            
        }
        public async void HomeAction()
        {
            await NavigationService.PopAsync();
        }
        public async void ClearCartAction()
        {
            if (Pizzas.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Wait !", "Your cart is empty !", "OK");
                MessagingCenter.Send(this, "Hi", "0");
            }
            else
            {
                CrossSettings.Current.AddOrUpdateValue("Commands", JsonConvert.SerializeObject(""));
                CrossSettings.Current.AddOrUpdateValue("ShopId", JsonConvert.SerializeObject(""));
                await Application.Current.MainPage.DisplayAlert("Your cart is clear !", "Have a good lunch !", "OK");
                await NavigationService.PopAsync();
            }
        }
        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            Pizzas = new ObservableCollection<PizzaItem>();
            string _shopId = JsonConvert.DeserializeObject<string>(CrossSettings.Current.GetValueOrDefault("ShopId", string.Empty));
            if (!string.IsNullOrEmpty(_shopId))
            {
                
                Pizzas = new ObservableCollection<PizzaItem>(JsonConvert.DeserializeObject<List<PizzaItem>>(CrossSettings.Current.GetValueOrDefault("Commands", string.Empty)).OrderBy(PizzaItem => PizzaItem.Price));
                foreach (PizzaItem i in Pizzas)
                    {
                        CreateOrderRequest.PizzaIds.Add(i.Id);
                        Total += i.Price;
                    }
            }
        }
        public async void PassAnOrderAction()
        {
            if (ShopId != "" && AccessToken != "" && Pizzas.Count != 0)
            {
                Response<ShopItem> response = await DependencyService.Get<IPizzaApiService>().PassAnOrder(CreateOrderRequest, AccessToken, ShopId);
                if (response.IsSuccess)
                {
                    await Application.Current.MainPage.DisplayAlert("Well done !", "Your order is confirmed !", "OK");
                    await NavigationService.PopAsync();
                    CrossSettings.Current.AddOrUpdateValue("Commands", JsonConvert.SerializeObject(new List<PizzaItem>()));
                    CrossSettings.Current.AddOrUpdateValue("ShopId", JsonConvert.SerializeObject(""));
                    Total = 0;
                    MessagingCenter.Send(this, "Hi", "0");

                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Wait !", "Your cart is empty !", "OK");
            }
            
        }

    }
}