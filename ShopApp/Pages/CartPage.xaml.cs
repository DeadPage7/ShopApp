using ShopApp.Models; // Подключение моделей, которые содержат классы для Cart и CartProduct
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShopApp.Pages;

public partial class CartPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient(); // Инициализация HTTP-клиента для запросов
    private Client _client; // Клиент, переданный в конструктор
    private readonly string _token; // Токен авторизации, переданный в конструктор
    public ObservableCollection<CartProduct> Cartes { get; set; } = new ObservableCollection<CartProduct>(); // Коллекция для привязки товаров корзины

    // Конструктор страницы, принимает клиента и токен для авторизации
    public CartPage(Client client, string token)
    {
        InitializeComponent();
        _client = client;
        _token = token;

        BindingContext = this; // Устанавливаем привязку данных для страницы

        LoadCartData(); // Загружаем данные корзины
    }

    // Метод для обработки клика по кнопке "Оформить заказ"
    private async void OnOrderButtonClicked(object sender, EventArgs e)
    {
        // Переходим на страницу оформления заказа
        await Navigation.PushAsync(new CreateOrderPage(_client, _token));
    }

    // Метод для загрузки данных корзины
    private async void LoadCartData()
    {
        try
        {
            // Устанавливаем заголовок авторизации с токеном
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            // Отправляем GET-запрос на сервер для получения данных корзины
            var response = await _httpClient.GetAsync("http://course-project-4/api/cart");

            // Проверка успешности запроса
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(); // Получаем данные как строку
                var cart = JsonSerializer.Deserialize<Cart>(content); // Десериализуем данные в модель Cart

                if (cart != null && cart.Products != null)
                {
                    // Очищаем текущую корзину
                    Cartes.Clear();

                    // Заполняем коллекцию товаров в корзине
                    foreach (var product in cart.Products)
                    {
                        // Проверяем корректность URL для изображения товара
                        if (!Uri.IsWellFormedUriString(product.Photo, UriKind.Absolute))
                        {
                            product.Photo = $"http://course-project-4/storage/{product.Photo}"; // Корректируем путь к изображению
                        }

                        // Добавляем товар в корзину
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

                    // Обновляем общую стоимость корзины
                    TotalCostLabel.Text = $"Общая стоимость: {cart.TotalCost:C}"; // Форматируем стоимость в валюту
                    OrderButton.IsVisible = true; // Показываем кнопку оформления заказа
                }
                else
                {
                    // Если корзина пуста или не удалось получить данные
                    await DisplayAlert("Ошибка", "Корзина пуста или данные не найдены.", "OK");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Если корзина не найдена на сервере
                await DisplayAlert("Ошибка", "Корзина пуста.", "OK");
            }
            else
            {
                // В случае других ошибок
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", $"Не удалось загрузить корзину: {errorContent}", "OK");
            }
        }
        catch (Exception ex)
        {
            // Если произошла ошибка при запросе
            await DisplayAlert("Ошибка сети", ex.Message, "OK");
        }
    }
}
