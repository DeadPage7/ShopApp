using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopApp.Models
{
    public class Status
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
