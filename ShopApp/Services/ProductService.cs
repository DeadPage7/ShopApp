﻿using Newtonsoft.Json;
using ShopApp.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShopApp.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "http://course-project-4/api/product";

        public ProductService()
        {
            _httpClient = new HttpClient();
        }

        // Метод для получения списка продуктов
        public async Task<List<Product>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync(ApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(responseData);

                // Преобразуем относительный путь к фото в полный
                foreach (var product in products)
                {
                    // Добавляем базовый URL, если путь является относительным
                    if (!Uri.IsWellFormedUriString(product.Photo, UriKind.Absolute))
                    {
                        product.Photo = $"http://course-project-4/storage/{product.Photo}";
                    }
                }

                return products;
            }

            return null;
        }
    }
}