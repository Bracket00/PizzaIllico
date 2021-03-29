using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Services;
using Plugin.Settings;
using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;
using Newtonsoft.Json;

namespace PizzaIllico.Mobile.ViewModels
{
    public class OrdersViewModel : ViewModelBase
    {

        public OrdersViewModel()
        {
            
        }

        public ObservableCollection<OrderItem> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        public override async Task OnResume()
        {
            await base.OnResume();
            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();
            var _accessToken = JsonConvert.DeserializeObject<string>(CrossSettings.Current.GetValueOrDefault("AccessToken", string.Empty));
            var response = await service.GetOrders(_accessToken);
            Console.WriteLine($"CODE ERREUR + {_accessToken}");

            if (response.IsSuccess)
            {
                Orders = new ObservableCollection<OrderItem>(response.Data);
            }
        }

        private ICommand homeCommand;
        private ObservableCollection<OrderItem> _orders;

        public ICommand HomeCommand => homeCommand ??= new Command(Home);

        private async void Home()
        {
            await NavigationService.PopAsync();
        }
    }
}