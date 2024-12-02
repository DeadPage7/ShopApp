using ShopApp.Models;
using ShopApp.Services;
using System;
using System.Collections.Generic;

namespace ShopApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly ProductService _productService;
        private readonly Client _client;

        public List<Product> Products { get; set; } // Добавляем свойство для привязки продуктов

        public HomePage(Client client, ProductService productService)
        {
            InitializeComponent();
            _client = client;
            _productService = productService;
            Products = new List<Product>(); // Инициализация списка продуктов

            // Привязываем только имя клиента
            LabelClientName.Text = _client.FullName;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Получаем список продуктов
            var products = await _productService.GetProductsAsync();

            if (products != null)
            {
                // Логируем пути к изображению для проверки
                foreach (var product in products)
                {
                    Console.WriteLine($"Product Image: {product.Photo}");
                }

                Products = products; // Привязываем продукты
                ProductCarouselView.ItemsSource = Products; // Обновляем источник данных
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить продукты", "OK");
            }
        }
    }
}
