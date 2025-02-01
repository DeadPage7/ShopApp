using ShopApp.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShopApp.Pages;

public partial class OrderPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly Client _client;
    private readonly string _token;

    public OrderPage(Client client, string token)
    {
        InitializeComponent();
        _client = client;
        _token = token;
        LoadOrders(); // Загружаем заказы
    }

    private async void LoadOrders()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            HttpResponseMessage response = await _httpClient.GetAsync($"http://course-project-4/api/orders");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var orders = JsonSerializer.Deserialize<List<Order>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (orders != null && orders.Count > 0)
                {
                    OrdersListView.ItemsSource = orders;
                    NoOrdersLabel.IsVisible = false;
                }
                else
                {
                    NoOrdersLabel.IsVisible = true;
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить заказы", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Ошибка загрузки заказов: {ex.Message}", "OK");
        }
    }

    private async void DeleteOrder_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int orderId)
        {
            bool confirm = await DisplayAlert("Подтверждение", "Вы действительно хотите удалить заказ?", "Да", "Нет");
            if (!confirm) return;

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                HttpResponseMessage response = await _httpClient.DeleteAsync($"http://course-project-4/api/orders/{orderId}");

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Успех", "Заказ удален", "OK");
                    LoadOrders(); // Обновляем список заказов
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось удалить заказ", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка удаления заказа: {ex.Message}", "OK");
            }
        }
    }

    private async void ViewOrder_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int orderId)
        {
            await Navigation.PushAsync(new DetailsOrderPage(_client, orderId, _token));
        }
    }

}
