using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopApp.Models
{
    public class Order
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("client_id")]
        public int ClientId { get; set; }
        [JsonPropertyName("address_id")]
        public int AddressId { get; set; }
        [JsonPropertyName("order_date")]
        public DateTime OrderDate { get; set; }
        [JsonPropertyName("total_cost")]
        public decimal TotalCost { get; set; }
        [JsonPropertyName("status_id")]
        public int StatusId { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("status")]
        public Status Status { get; set; }
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }
    }
}
