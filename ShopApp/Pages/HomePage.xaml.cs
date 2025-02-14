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
        private readonly ProductService _productService; // Сервис для работы с продуктами
        private readonly Client _client; // Экземпляр клиента
        private readonly string _token; // Токен для авторизации

        public List<Product> Products { get; set; } // Список продуктов
        public List<Category> Categories { get; set; } // Список категорий

        // Конструктор страницы
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

            // Загружаем категории и продукты
            LoadCategories();
            LoadProducts();

            // Обработчик выбора категории
            CategoryCollectionView.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection.Count > 0)
                {
                    var selectedCategory = e.CurrentSelection[0] as Category;
                    if (selectedCategory != null)
                    {
                        await Navigation.PushAsync(new ProductsPage(selectedCategory.Id, _client, _token)); // Переход на страницу с продуктами выбранной категории
                    }
                }
            };
        }

        // Метод для загрузки категорий
        private async void LoadCategories()
        {
            try
            {
                Categories = await _productService.GetCategoriesAsync();
                CategoryCollectionView.ItemsSource = Categories; // Устанавливаем список категорий в CollectionView
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить категории: {ex.Message}", "ОК"); // Обработка ошибки
            }
        }

        // Метод для загрузки продуктов
        private async void LoadProducts()
        {
            try
            {
                Products = await _productService.GetProductsAsync();
                //ProductCarouselView.ItemsSource = Products; // Продукты можно отобразить в CarouselView
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить продукты: {ex.Message}", "ОК"); // Обработка ошибки
            }
        }

        // Обработчик кнопки "Каталог"
        private async void OnCatalogButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductsPage(0, _client, _token)); // Переход на страницу с продуктами (0 — все продукты)
        }

        // Обработчик кнопки "Профиль"
        private async void OnProfileButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(_client, _token)); // Переход на страницу профиля
        }

        // Обработчик кнопки "Детали продукта"
        private async void OnDetailsProductButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var selectedProduct = button?.BindingContext as Product;

            if (selectedProduct != null)
            {
                await Navigation.PushAsync(new DetailsProductPage(selectedProduct, _client, _token)); // Переход на страницу с деталями выбранного продукта
            }
        }

        // Обработчик кнопки "Корзина"
        private async void OnCartButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartPage(_client, _token)); // Переход на страницу корзины
        }
    }

    // Сервис для работы с продуктами
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "http://course-project-4/api/product";
        private const string CategoryApiUrl = "http://course-project-4/api/categories";

        // Конструктор, принимающий токен для авторизации
        public ProductService(string token)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://course-project-4/") // Устанавливаем базовый URL
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); // Добавляем авторизационный заголовок
        }

        // Метод для получения списка всех продуктов
        public async Task<List<Product>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync(ApiUrl); // Выполняем GET-запрос для получения всех продуктов

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(responseData); // Десериализация JSON в список продуктов

                // Преобразуем относительные пути к фото в абсолютные
                foreach (var product in products)
                {
                    if (!Uri.IsWellFormedUriString(product.Photo, UriKind.Absolute))
                    {
                        product.Photo = $"http://course-project-4/storage/{product.Photo}"; // Добавляем базовый URL к фото
                    }
                }

                return products;
            }

            return null; // Если ошибка, возвращаем null
        }

        // Метод для получения списка продуктов по категории
        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await GetAsync<List<Product>>($"api/category/{categoryId}/products"); // Запрос на сервер для получения продуктов по категории

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

            return products; // Возвращаем список продуктов
        }

        // Метод для получения списка категорий
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await GetAsync<List<Category>>(CategoryApiUrl); // Запрос на сервер для получения категорий
        }

        // Общий метод для выполнения GET-запросов
        private async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url); // Выполняем GET-запрос

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка запроса: {response.StatusCode}"); // Если ошибка, выбрасываем исключение
            }

            var responseData = await response.Content.ReadAsStringAsync();

            try
            {
                return JsonConvert.DeserializeObject<T>(responseData); // Десериализация JSON
            }
            catch (JsonException)
            {
                throw new Exception("Ошибка обработки данных сервера."); // Если ошибка десериализации
            }
        }
    }
}
