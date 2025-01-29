using ShopApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ShopApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly ProductService _productService;
        private readonly Client _client; // Экземпляр клиента
        private readonly string _token;

        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }

        public HomePage(Client client, string token)
        {
            InitializeComponent();
            _client = client;
            _token = token;

            // Инициализация ProductService с передачей токена
            _productService = new ProductService(token);

            Products = new List<Product>();
            Categories = new List<Category>();

            // Устанавливаем имя пользователя в Label
            LabelClientName.Text = client.FullName;

            // Загрузка данных категорий
            LoadCategories();
            LoadProducts();

            CategoryCollectionView.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection.Count > 0)
                {
                    var selectedCategory = e.CurrentSelection[0] as Category;
                    if (selectedCategory != null)
                    {
                        await Navigation.PushAsync(new ProductsPage(selectedCategory.Id, _client, _token));
                    }
                }
            };
        }

        private async void LoadCategories()
        {
            try
            {
                Categories = await _productService.GetCategoriesAsync();
                CategoryCollectionView.ItemsSource = Categories;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить категории: {ex.Message}", "ОК");
            }
        }
        private async void LoadProducts()
        {
            try
            {
                Products = await _productService.GetProductsAsync();
                ProductCarouselView.ItemsSource = Products;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить категории: {ex.Message}", "ОК");
            }
        }


        // Обработчик кнопки Каталог
        private async void OnCatalogButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductsPage(0, _client, _token)); // 0 для всех продуктов
        }

        // Обработчик кнопки Профиль
        private async void OnProfileButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(_client, _token));
        }

        private async void OnDetailsProductButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var selectedProduct = button?.BindingContext as Product;

            if (selectedProduct != null)
            {
                await Navigation.PushAsync(new DetailsProductPage(selectedProduct, _client, _token));
            }
        }

        private async void OnCartButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartPage(_client, _token));
        }
    }

    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "http://course-project-4/api/product";
        private const string CategoryApiUrl = "http://course-project-4/api/categories";

        public ProductService(string token)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://course-project-4/")
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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

        // Метод для получения списка продуктов по категории
        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await GetAsync<List<Product>>($"api/category/{categoryId}/products");

            // Проверяем и преобразуем пути к фото
            if (products != null)
            {
                foreach (var product in products)
                {
                    if (!Uri.IsWellFormedUriString(product.Photo, UriKind.Absolute))
                    {
                        product.Photo = $"http://course-project-4/storage/{product.Photo}";
                    }
                }
            }

            return products;
        }


        // Метод для получения списка категорий
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await GetAsync<List<Category>>(CategoryApiUrl);
        }

        // Общий метод для выполнения GET-запросов
        private async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка запроса: {response.StatusCode}");
            }

            var responseData = await response.Content.ReadAsStringAsync();

            try
            {
                return JsonConvert.DeserializeObject<T>(responseData);
            }
            catch (JsonException)
            {
                throw new Exception("Ошибка обработки данных сервера.");
            }
        }
    }
}
