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
            ProductImage.Source = _product.Photo;
            ProductName.Text = _product.Name;
            ProductPrice.Text = $"Цена: {_product.Price:C}";
            ProductDescription.Text = _product.Description;
        }

        // Обработчик кнопки "+"
        private void OnIncreaseButtonClicked(object sender, EventArgs e)
        {
            _quantity++; // Увеличиваем количество
            QuantityLabel.Text = _quantity.ToString(); // Обновляем отображаемое количество
        }

        // Обработчик кнопки "-"
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
            string quantity = QuantityLabel.Text;
            var createData = new MultipartFormDataContent
    {
        { new StringContent(quantity), "quantity" },
    };

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                HttpResponseMessage response = await _httpClient.PostAsync($"http://course-project-4/api/cart/product/{_product.Id}", createData);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("?????", "????? ??????? ????????", "??");
                    await Navigation.PushAsync(new HomePage(_client, _token));
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // ?????? JSON ??? JsonDocument
                        using var document = JsonDocument.Parse(errorContent);
                        var root = document.RootElement;

                        var message = root.GetProperty("message").GetString();

                        if (root.TryGetProperty("errors", out var errors))
                        {
                            var errorMessages = new List<string>();

                            // ???????? ?? ???? ???????
                            foreach (var error in errors.EnumerateObject())
                            {
                                foreach (var msg in error.Value.EnumerateArray())
                                {
                                    errorMessages.Add(System.Text.RegularExpressions.Regex.Unescape(msg.GetString()));
                                }
                            }

                            var combinedErrors = string.Join("\n", errorMessages);
                            await DisplayAlert("?????? ?????????", combinedErrors, "??");
                        }
                        else
                        {
                            await DisplayAlert("??????", message ?? "????????? ?????? ?????????.", "??");
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("??????", $"?? ??????? ?????????? ?????: {ex.Message}\n\n?????: {errorContent}", "??");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("??????", $"??? ??????: {(int)response.StatusCode}\n{errorContent}", "??");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("?????? ????", ex.Message, "??");
            }
        }

        private async void OnCartButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartPage(_client, _token));
        }
    }
}
