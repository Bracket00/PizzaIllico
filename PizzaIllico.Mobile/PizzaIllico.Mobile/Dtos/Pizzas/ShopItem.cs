using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PizzaIllico.Mobile.Dtos.Pizzas
{
    public class ShopItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("address")]
        public string Address { get; set; }
        
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        
        [JsonProperty("minutes_per_kilometer")]
        public double MinutesPerKilometer { get; set; }

        [JsonProperty("distance")]
        public string Distance { get; set; }

        internal IEnumerable<OrderItem> OrderBy(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}