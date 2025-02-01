using ShopApp.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShopApp.Pages;

public partial class DetailsOrderPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private Client _client;
    private readonly string _token;
    private int _orderId;

    public DetailsOrderPage(Client client, int orderId, string token)
    {
        InitializeComponent();
        _client = client;
        _orderId = orderId;
        _token = token;
        LoadOrderDetails();
    }

    private async void LoadOrderDetails()
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            HttpResponseMessage response = await _httpClient.GetAsync($"http://course-project-4/api/orders/{_orderId}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var order = JsonSerializer.Deserialize<Order>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (order != null)
                {
                    OrderIdLabel.Text = order.Id.ToString();
                    OrderDateLabel.Text = order.OrderDate.ToString("dd.MM.yyyy");
                    TotalCostLabel.Text = $"{order.TotalCost} ₽";

                    foreach (var item in order.Items)
                    {
                        if (!string.IsNullOrEmpty(item.Product.Photo) && !item.Product.Photo.StartsWith("http"))
                        {
                            item.Product.Photo = $"http://course-project-4/storage/{item.Product.Photo}"; // Исправьте путь на реальный
                        }
                    }
                    ItemsListView.ItemsSource = order.Items;
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить детали заказа", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Ошибка загрузки деталей заказа: {ex.Message}", "OK");
        }
    }
}
