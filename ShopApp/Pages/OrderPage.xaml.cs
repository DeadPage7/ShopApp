using ShopApp.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShopApp.Pages;

public partial class OrderPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient(); // Создание HttpClient для работы с API
    private readonly Client _client; // Экземпляр клиента для хранения информации о пользователе
    private readonly string _token; // Токен авторизации для запросов

    // Конструктор страницы, получает клиента и токен
    public OrderPage(Client client, string token)
    {
        InitializeComponent();
        _client = client;
        _token = token;
        LoadOrders(); // Загружаем заказы при инициализации страницы
    }

    // Метод для загрузки заказов с сервера
    private async void LoadOrders()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token); // Добавляем токен в заголовок запроса

            HttpResponseMessage response = await _httpClient.GetAsync($"http://course-project-4/api/orders"); // Отправляем запрос на сервер

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync(); // Читаем ответ от сервера как строку
                var orders = JsonSerializer.Deserialize<List<Order>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); // Десериализуем в список заказов

                // Если заказы найдены, отображаем их, иначе показываем сообщение, что заказов нет
                if (orders != null && orders.Count > 0)
                {
                    OrdersListView.ItemsSource = orders; // Привязываем данные заказов к элементу интерфейса
                    NoOrdersLabel.IsVisible = false; // Скрываем сообщение о том, что заказов нет
                }
                else
                {
                    NoOrdersLabel.IsVisible = true; // Показываем сообщение, что нет заказов
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить заказы", "OK"); // Ошибка при запросе
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Ошибка загрузки заказов: {ex.Message}", "OK"); // Обработка ошибок
        }
    }

    // Метод для удаления заказа
    private async void DeleteOrder_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int orderId) // Получаем ID заказа из параметра кнопки
        {
            bool confirm = await DisplayAlert("Подтверждение", "Вы действительно хотите удалить заказ?", "Да", "Нет"); // Подтверждаем удаление
            if (!confirm) return; // Если не подтвердили, выходим из метода

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token); // Добавляем токен в заголовок запроса

                HttpResponseMessage response = await _httpClient.DeleteAsync($"http://course-project-4/api/orders/{orderId}"); // Отправляем запрос на удаление заказа

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Успех", "Заказ удален", "OK"); // Уведомляем о успешном удалении
                    LoadOrders(); // Обновляем список заказов
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось удалить заказ", "OK"); // Ошибка при удалении
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка удаления заказа: {ex.Message}", "OK"); // Обработка ошибок при удалении
            }
        }
    }

    // Метод для просмотра деталей заказа
    private async void ViewOrder_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int orderId) // Получаем ID заказа из параметра кнопки
        {
            await Navigation.PushAsync(new DetailsOrderPage(_client, orderId, _token)); // Переход к странице с деталями заказа
        }
    }
}
