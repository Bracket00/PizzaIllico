using System;
using System.Windows.Input;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Accounts;
using PizzaIllico.Mobile.Dtos.Authentications.Credentials;
using PizzaIllico.Mobile.Services;
using Plugin.Settings;
using Storm.Mvvm;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class EditProfileViewModel : ViewModelBase
    {
        private string _name;
        private string _firstname;
        private string _mail;
        private string _phone;
        private string _oldPassword;
        private string _newPassword;
        private string _accessToken;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Firstname
        {
            get { return _firstname; }
            set
            {
                _firstname = value;
                OnPropertyChanged(nameof(Firstname)); 
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
        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone)); 
            }
        }
        public string OldPassword
        {
            get { return _oldPassword; }
            set
            {
                _oldPassword = value;
                OnPropertyChanged(nameof(OldPassword)); 
            }
        }
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword)); 
            }
        }

        public async void HomeAction()
        {
            await NavigationService.PopAsync();
        }

        public string AccessToken
        {
            get { return _accessToken; }
            set { SetProperty<string>(ref _accessToken, value); }
        }

        public ICommand EditProfileCommand { get; }
        public ICommand ModifyPasswordCommand { get; }
        public ICommand HomeCommand { get; }
        

        public EditProfileViewModel()
        {
            AccessToken = JsonConvert.DeserializeObject<string>(CrossSettings.Current.GetValueOrDefault("AccessToken", string.Empty));
            EditProfileCommand = new Command(SetUserProfileAction);
            ModifyPasswordCommand = new Command(SetPasswordAction);
            HomeCommand = new Command(HomeAction);
            GetUserProfile();

        }

        public async void GetUserProfile()
        {
            Response<UserProfileResponse> response = await DependencyService.Get<IPizzaApiService>().GetUserProfile(AccessToken);
            Name = response.Data.LastName;
            Firstname = response.Data.FirstName;
            Mail = response.Data.Email;
            Phone = response.Data.PhoneNumber;
           
        }
        public async void SetUserProfileAction()
        {
            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

            SetUserProfileRequest setUserProfileRequest = new();

            setUserProfileRequest.Email = Mail;
            setUserProfileRequest.PhoneNumber = Phone;
            setUserProfileRequest.LastName = Name;
            setUserProfileRequest.FirstName = Firstname;


            Response<UserProfileResponse> response = await service.SetUserProfile(setUserProfileRequest, AccessToken);

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Informations not valid", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Great !", "Your profile has been changed", "OK");
                await NavigationService.PopAsync();
            }
        }

        public async void SetPasswordAction()
        {
            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();
            SetPasswordRequest setPasswordRequest = new()
            {
                OldPassword = OldPassword,
                NewPassword = NewPassword
            };
            Response<UserProfileResponse> response = await service.SetPasswordProfile(setPasswordRequest, AccessToken);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Password not valid", "OK");
            }
            else
            {
                CrossSettings.Current.AddOrUpdateValue("password", JsonConvert.SerializeObject(NewPassword));
                await Application.Current.MainPage.DisplayAlert("Great !", "Password changed successfully", "OK");
                await NavigationService.PopAsync();
            }
            
        }

    }
}