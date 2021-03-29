using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Accounts;
using PizzaIllico.Mobile.Dtos.Authentications;
using PizzaIllico.Mobile.Dtos.Authentications.Credentials;
using PizzaIllico.Mobile.Dtos.Pizzas;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.Services
{
    public interface IPizzaApiService
    {
        Task<Response<List<ShopItem>>> ListShops();
        Task<Response<List<PizzaItem>>> ListPizzas(string shopId);
        Task<Response<UserProfileResponse>> CreateUser(string name, string firstName, string mail, string phone, string password);
        Task<Response<LoginResponse>> Login(string mail, string password);
        Task<Response<UserProfileResponse>> GetUserProfile(string access_token);
        Task<Response<UserProfileResponse>> SetUserProfile(SetUserProfileRequest updateProfileRequest, string access_token);
        Task<Response<UserProfileResponse>> SetPasswordProfile(SetPasswordRequest setPasswordRequest, string access_token);
        Task<Response<ShopItem>> PassAnOrder(CreateOrderRequest createOrderRequest, string accessToken, string shopId);
        Task<Response<List<OrderItem>>> GetOrders(string accessToken);

    }

    public class PizzaApiService : IPizzaApiService
    {
        private readonly IApiService _apiService;

        public PizzaApiService()
        {
            _apiService = DependencyService.Get<IApiService>();
        }

        public async Task<Response<List<ShopItem>>> ListShops()
        {
            return await _apiService.Get<Response<List<ShopItem>>>(Urls.LIST_SHOPS);
        }

        public async Task<Response<List<PizzaItem>>> ListPizzas(string shopId)
        {
            return await _apiService.Get<Response<List<PizzaItem>>>(Urls.LIST_PIZZA.Replace("{shopId}", shopId));
        }

        public async Task<Response<UserProfileResponse>> GetUserProfile(string acces_token)
        {
            return await _apiService.Get<Response<UserProfileResponse>>(Urls.USER_PROFILE, acces_token);
        }

        public async Task<Response<UserProfileResponse>> CreateUser(string name, string firstName, string mail, string phone, string password)
        {
            CreateUserRequest createUserRequest = new();
            createUserRequest.Email = mail;
            createUserRequest.LastName = name;
            createUserRequest.FirstName = firstName;
            createUserRequest.PhoneNumber = phone;
            createUserRequest.Password = password;
            createUserRequest.ClientId = "MOBILE";
            createUserRequest.ClientSecret = "UNIV";

            return await _apiService.Post<Response<UserProfileResponse>>(Urls.CREATE_USER, createUserRequest);
        }
        public async Task<Response<LoginResponse>> Login(string mail, string password)
        {
            return await _apiService.Post<Response<LoginResponse>>(Urls.LOGIN_WITH_CREDENTIALS, mail, password);
        }
    public async Task<Response<UserProfileResponse>> SetUserProfile(SetUserProfileRequest updateProfileRequest, string access_token)
        {
            string json = JsonConvert.SerializeObject(updateProfileRequest);
            return await _apiService.Patch<Response<UserProfileResponse>>(Urls.SET_USER_PROFILE, json, access_token);
        }
        public async Task<Response<UserProfileResponse>> SetPasswordProfile(SetPasswordRequest setPasswordRequest, string access_token)
        {
            string json = JsonConvert.SerializeObject(setPasswordRequest);
            return await _apiService.Patch<Response<UserProfileResponse>>(Urls.SET_PASSWORD, json, access_token);
        }
        public async Task<Response<ShopItem>> PassAnOrder(CreateOrderRequest createOrderRequest, string access_token, string shopId)
        {
            return await _apiService.Post<Response<ShopItem>>(Urls.DO_ORDER.Replace("{shopId}", shopId), createOrderRequest, access_token) ;
        }
        public async Task<Response<List<OrderItem>>> GetOrders(string accessToken)
        {
            return await _apiService.Get<Response<List<OrderItem>>>(Urls.LIST_ORDERS, accessToken);
        }



    }
}