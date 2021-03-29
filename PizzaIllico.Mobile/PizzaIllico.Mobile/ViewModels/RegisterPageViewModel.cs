using System;
using System.Windows.Input;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Accounts;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class RegisterPageViewModel : ViewModelBase
    {
        private string _name;
        private string _firstname;
        private string _mail;
        private string _phone;
        private string _password;
        private string _confirmedPassword;

        public string Name { get => _name; set => _name = value; }
        public string Firstname { get => _firstname; set => _firstname = value; }
        public string Mail { get => _mail; set => _mail = value; }
        public string Phone { get => _phone; set => _phone = value; }
        public string Password { get => _password; set => _password = value; }
        public string ConfirmedPassword { get => _confirmedPassword; set => _confirmedPassword = value; }

        public ICommand RegisterCommand { get; }
        public RegisterPageViewModel()
        {
            RegisterCommand = new Command(RegisterAction);
        }
        public async void RegisterAction()
        {
            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

            if (!ConfirmedPassword.Equals(Password))
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match", "OK");
            else
            {
                Response<UserProfileResponse> response = await service.CreateUser(Name, Firstname, Mail, Phone, Password);
                if (!response.IsSuccess)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Informations not valid", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Succes", "Your account has been created successfully !", "OK");
                    await NavigationService.PopAsync();
                }
            }
        }
    }
}