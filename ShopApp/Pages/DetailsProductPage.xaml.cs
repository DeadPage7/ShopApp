using ShopApp.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShopApp.Pages
{
    public partial class DetailsProductPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private Product _product;
        private Client _client;
        private string _token;
        private int _quantity = 1; // Переменная для отслеживания количества товара

        // Передаем продукт и клиента в конструктор
        public DetailsProductPage(Product product, Client client, string token)
        {
            InitializeComponent();
            _product = product;
            _client = client; // Присваиваем клиента
            _token = token;
            BindProductDetails(); // Заполняем элементы значениями продукта
            LabelClientName.Text = _client.FullName; // Отображаем имя пользователя

            // Инициализируем количество на старте
            QuantityLabel.Text = _quantity.ToString();
        }

        // Заполнение элементов страницы данными продукта
        private void BindProductDetails()
        {
            ProductImage.Source = _product.Photo; // Устанавливаем изображение продукта
            ProductName.Text = _product.Name; // Устанавливаем название продукта
            ProductPrice.Text = $"Цена: {_product.Price:C}"; // Отображаем цену с форматированием
            ProductDescription.Text = _product.Description; // Отображаем описание продукта
        }

        // Обработчик кнопки "+" для увеличения количества
        private void OnIncreaseButtonClicked(object sender, EventArgs e)
        {
            _quantity++; // Увеличиваем количество
            QuantityLabel.Text = _quantity.ToString(); // Обновляем отображаемое количество
        }

        // Обработчик кнопки "-" для уменьшения количества
        private void OnDecreaseButtonClicked(object sender, EventArgs e)
        {
            if (_quantity > 1) // Проверяем, что количество не меньше 1
            {
                _quantity--; // Уменьшаем количество
                QuantityLabel.Text = _quantity.ToString(); // Обновляем отображаемое количество
            }
        }

        // Обработчик кнопки "Купить"
        private async void OnBuyButtonClicked(object sender, EventArgs e)
        {
            string quantity = QuantityLabel.Text; // Получаем количество товара
            var createData = new MultipartFormDataContent
            {
                { new StringContent(quantity), "quantity" }, // Отправляем количество товара в запросе
            };

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token); // Добавляем заголовок с токеном для авторизации

                // Отправляем POST-запрос для добавления товара в корзину
                HttpResponseMessage response = await _httpClient.PostAsync($"http://course-project-4/api/cart/product/{_product.Id}", createData);

                if (response.IsSuccessStatusCode) // Если запрос успешен
                {
                    await DisplayAlert("Успех", "Товар добавлен в корзину", "ОК");
                    await Navigation.PushAsync(new HomePage(_client, _token)); // Переход на главную страницу
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity) // Обработка ошибки, если запрос не может быть обработан
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // Парсим JSON для получения подробностей об ошибке
                        using var document = JsonDocument.Parse(errorContent);
                        var root = document.RootElement;

                        var message = root.GetProperty("message").GetString(); // Получаем сообщение об ошибке

                        if (root.TryGetProperty("errors", out var errors)) // Если есть дополнительные ошибки
                        {
                            var errorMessages = new List<string>();

                            // Перебираем ошибки и добавляем их в список
                            foreach (var error in errors.EnumerateObject())
                            {
                                foreach (var msg in error.Value.EnumerateArray())
                                {
                                    errorMessages.Add(System.Text.RegularExpressions.Regex.Unescape(msg.GetString()));
                                }
                            }

                            var combinedErrors = string.Join("\n", errorMessages); // Объединяем ошибки в одну строку
                            await DisplayAlert("Ошибка валидации", combinedErrors, "ОК");
                        }
                        else
                        {
                            await DisplayAlert("Ошибка", message ?? "Неизвестная ошибка при добавлении товара.", "ОК");
                        }
                    }
                    catch (Exception ex) // Обработка исключений при разборе JSON
                    {
                        await DisplayAlert("Ошибка", $"Не удалось обработать ошибку: {ex.Message}\n\nОтвет: {errorContent}", "ОК");
                    }
                }
                else
                {
                    // Обработка других ошибок (например, если сервер вернул код ошибки)
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Ошибка", $"Ошибка {response.StatusCode}: {errorContent}", "ОК");
                }
            }
            catch (Exception ex) // Обработка исключений при запросе
            {
                await DisplayAlert("Ошибка сети", ex.Message, "ОК");
            }
        }

        // Обработчик кнопки для перехода в корзину
        private async void OnCartButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartPage(_client, _token)); // Переход на страницу корзины
        }
    }
}
