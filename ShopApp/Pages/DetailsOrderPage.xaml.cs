using ShopApp.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShopApp.Pages;

public partial class DetailsOrderPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient(); // Инициализируем HttpClient для отправки запросов
    private Client _client; // Объект клиента
    private readonly string _token; // Токен для авторизации
    private int _orderId; // Идентификатор заказа

    // Конструктор, инициализирует клиент, заказ и токен, затем загружает детали заказа
    public DetailsOrderPage(Client client, int orderId, string token)
    {
        InitializeComponent();
        _client = client;
        _orderId = orderId; // Присваиваем идентификатор заказа
        _token = token; // Присваиваем токен для авторизации
        LoadOrderDetails(); // Загружаем детали заказа
    }

    // Метод для загрузки деталей заказа с сервера
    private async void LoadOrderDetails()
    {
        try
        {
            // Устанавливаем заголовок авторизации для запроса
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            // Отправляем GET-запрос на получение данных заказа по его ID
            HttpResponseMessage response = await _httpClient.GetAsync($"http://course-project-4/api/orders/{_orderId}");

            if (response.IsSuccessStatusCode) // Если запрос успешен
            {
                string json = await response.Content.ReadAsStringAsync(); // Читаем ответ в формате JSON
                // Десериализуем JSON в объект Order
                var order = JsonSerializer.Deserialize<Order>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (order != null) // Если заказ найден
                {
                    // Отображаем информацию о заказе на экране
                    OrderIdLabel.Text = order.Id.ToString();
                    OrderDateLabel.Text = order.OrderDate.ToString("dd.MM.yyyy");
                    TotalCostLabel.Text = $"{order.TotalCost} ₽";

                    // Проходим по каждому товару в заказе и исправляем путь к изображению, если это необходимо
                    foreach (var item in order.Items)
                    {
                        // Проверяем, что изображение не пустое и не начинается с "http", чтобы скорректировать путь
                        if (!string.IsNullOrEmpty(item.Product.Photo) && !item.Product.Photo.StartsWith("http"))
                        {
                            item.Product.Photo = $"http://course-project-4/storage/{item.Product.Photo}"; // Обновляем путь к фото
                        }
                    }

                    // Присваиваем список товаров в заказе элементу ListView
                    ItemsListView.ItemsSource = order.Items;
                }
            }
            else
            {
                // Если запрос не успешен, показываем ошибку
                await DisplayAlert("Ошибка", "Не удалось загрузить детали заказа", "OK");
            }
        }
        catch (Exception ex) // Обработка ошибок при выполнении запроса
        {
            // Показываем сообщение об ошибке при возникновении исключений
            await DisplayAlert("Ошибка", $"Ошибка загрузки деталей заказа: {ex.Message}", "OK");
        }
    }
}
