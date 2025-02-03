using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShopApp.Models
{
    public class Address
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("city")]
        public string City { get; set; }
        [JsonPropertyName("street")]
        public string Street { get; set; }
        [JsonPropertyName("house")]
        public string House { get; set; }
        [JsonPropertyName("floor")]
        public int? Floor { get; set; }
        [JsonPropertyName("apartment_or_office")]
        public string? ApartmentOrOffice { get; set; }
        [JsonPropertyName("entrance")]
        public string? Entrance { get; set; }
        [JsonPropertyName("intercom")]
        public string? Intercom { get; set; }
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }
}
