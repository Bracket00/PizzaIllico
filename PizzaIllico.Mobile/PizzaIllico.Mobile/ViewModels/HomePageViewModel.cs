using System;
using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Accounts;
using PizzaIllico.Mobile.Dtos.Authentications;
using PizzaIllico.Mobile.Pages;
using PizzaIllico.Mobile.Services;
using Plugin.Settings;
using Storm.Mvvm;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private string _mail;
        private string _password;
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                OnPropertyChanged(nameof(Mail));
            }
        }
        

        public HomePageViewModel()
        {
            LoginCommand = new Command(LoginAction);
            RegisterCommand = new Command(GoToRegisterAction);
            string _password = CrossSettings.Current.GetValueOrDefault("password", string.Empty);
            string _mail = CrossSettings.Current.GetValueOrDefault("mail", string.Empty);
            if (!_mail.Equals("") && !_password.Equals(""))
            {
                Mail = JsonConvert.DeserializeObject<string>(_mail);
                Password = JsonConvert.DeserializeObject<string>(_password);
                LoginAction();
            }          
        }
        public async void LoginAction()
        {
            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();
            Response <LoginResponse> response = await service.Login(Mail, Password);
            if (!response.IsSuccess)
                await Application.Current.MainPage.DisplayAlert("Error", "Authentification error", "OK");
            else
            {
                CrossSettings.Current.AddOrUpdateValue("AccessToken", JsonConvert.SerializeObject(response.Data.AccessToken));
                CrossSettings.Current.AddOrUpdateValue("mail", JsonConvert.SerializeObject(Mail));
                CrossSettings.Current.AddOrUpdateValue("password", JsonConvert.SerializeObject(Password));

                await NavigationService.PushAsync<LoginPage>();
                Mail = "";
                Password = "";
            }
            
        }
        public async void GoToRegisterAction()
        {
            await NavigationService.PushAsync<RegisterPage>();
            Mail = "";
            Password = "";
        }
    }
}