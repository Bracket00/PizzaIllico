using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos.Accounts;
using PizzaIllico.Mobile.Dtos.Pizzas;

namespace PizzaIllico.Mobile.Services
{
    public interface IApiService
    {
        Task<TResponse> Get<TResponse>(string url);
        Task<TResponse> Get<TResponse>(string url, string access_token);
        Task<TResponse>  Post<TResponse>(string createUser, CreateUserRequest createUserRequest);
        Task<TResponse> Post<TResponse>(string url, string mail, string password);
        Task<TResponse> Post<TResponse>(string url, CreateOrderRequest createOrderRequest, string accessToken);
        Task<TResponse> Patch<TResponse>(string url, string myJson, string access_token);
    }
    
    public class ApiService : IApiService
    {
	    private const string HOST = "https://pizza.julienmialon.ovh/";
        private readonly HttpClient _client = new();
        
        public async Task<TResponse> Get<TResponse>(string url)
        {
            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, HOST + url);
            HttpResponseMessage response = await _client.SendAsync(request);

	        string content = await response.Content.ReadAsStringAsync();
#if DEBUG
            Console.WriteLine($"CODE ERREUR + {response.StatusCode}");
            Console.WriteLine($"CODE ERREUR + {content}");
#endif          

            return JsonConvert.DeserializeObject<TResponse>(content);
        }
        public async Task<TResponse> Get<TResponse>(string url, string access_token)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, HOST + url);
            request.Headers.Add("Authorization", "Bearer " + access_token);
            HttpResponseMessage response = await _client.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();
#if DEBUG
            Console.WriteLine($"CODE ERREUR + {response.StatusCode}");
            Console.WriteLine($"CODE ERREUR + {content}");
#endif

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<TResponse> Post<TResponse>(string url, CreateUserRequest createUserRequest)
        {
            
            HttpRequestMessage request = new(HttpMethod.Post, HOST + url);
            string json = JsonConvert.SerializeObject(createUserRequest);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(request.RequestUri, content);
            string contentRequest = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(contentRequest);
        }

        public async Task<TResponse> Post<TResponse>(string url, CreateOrderRequest createOrderRequest, string accessToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string json = JsonConvert.SerializeObject(createOrderRequest);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync(HOST + url, content);
            string contentRequest = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(contentRequest);
        }

        public async Task<TResponse> Post<TResponse>(string url, string mail, string password)
        {

            HttpRequestMessage request = new(HttpMethod.Post, HOST + url);
            var myJson = new
            {
                login = mail,
                password = password,
                client_id = "MOBILE",
                client_secret = "UNIV"
            };
            string json = JsonConvert.SerializeObject(myJson);
            StringContent contentJson = new(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync(request.RequestUri, contentJson);
            


            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(content);    
        }

        public async Task<TResponse> Patch<TResponse>(string url, string myJson, string access_token)
        {
            StringContent contentJson = new(myJson, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new(new HttpMethod("PATCH"), HOST + url)
            {
                Content = contentJson
            };
            request.Headers.Add("Authorization", "Bearer " + access_token);
            HttpResponseMessage response = await _client.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(content);
        }

    }
}