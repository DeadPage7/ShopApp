using ShopApp.Models; // Подключение моделей
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShopApp.Pages;

public partial class CartPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private Client _client;
    private readonly string _token;
    public ObservableCollection<CartProduct> Cartes { get; set; } = new ObservableCollection<CartProduct>();

    public CartPage(Client client, string token)
    {
        InitializeComponent();
        _client = client;
        _token = token;

        // Устанавливаем BindingContext для привязки
        BindingContext = this;

        LoadCartData(); // Загрузка данных корзины
    }

    private async void LoadCartData()
    {
        try
        {
            // Устанавливаем токен авторизации
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            // Отправляем GET-запрос на сервер
            var response = await _httpClient.GetAsync("http://course-project-4/api/cart");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var cart = JsonSerializer.Deserialize<Cart>(content); // Десериализация в модель Cart

                if (cart != null && cart.Products != null)
                {
                    // Заполняем коллекцию товаров
                    Cartes.Clear();
                    foreach (var product in cart.Products)
                    {
                        if (!Uri.IsWellFormedUriString(product.Photo, UriKind.Absolute))
                        {
                            product.Photo = $"http://course-project-4/storage/{product.Photo}";
                        }
                        Cartes.Add(new CartProduct
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Photo = product.Photo,
                            Description = product.Description,
                            Quantity = product.Quantity
                        });
                    }


                    // Обновляем общую стоимость
                    TotalCostLabel.Text = $"Общая стоимость: {cart.TotalCost:C}";
                    OrderButton.IsVisible = true;
                }
                else
                {
                    await DisplayAlert("Ошибка", "Корзина пуста или данные не найдены.", "OK");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await DisplayAlert("Ошибка", "Корзина пуста.", "OK");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", $"Не удалось загрузить корзину: {errorContent}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка сети", ex.Message, "OK");
        }
    }

    private async void OnOrderButtonClicked(object sender, EventArgs e)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync("http://course-project-4/api/orders", null);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Заказ", "Ваш заказ оформлен!", "OK");
                await Navigation.PushAsync(new HomePage(_client, _token));
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", $"Описание: {(int)response.StatusCode}\n{errorContent}", "ОК");
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.Message, "ОК");
        }
    }
}
