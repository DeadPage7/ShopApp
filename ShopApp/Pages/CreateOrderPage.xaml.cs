using ShopApp.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text;

namespace ShopApp.Pages
{
    public partial class CreateOrderPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient(); // Инициализируем HttpClient для отправки запросов
        private Client _client; // Объект клиента
        private readonly string _token; // Токен для авторизации
        private Address _selectedAddress; // Хранит выбранный адрес для заказа

        public ObservableCollection<Address> Addresses { get; set; } = new ObservableCollection<Address>(); // Коллекция адресов клиента

        // Конструктор для инициализации страницы
        public CreateOrderPage(Client client, string token)
        {
            InitializeComponent();
            _client = client; // Присваиваем клиента
            _token = token; // Присваиваем токен для авторизации
            AddressesCollectionView.ItemsSource = Addresses; // Привязываем коллекцию адресов к CollectionView
            BindingContext = this; // Устанавливаем BindingContext для привязки данных

            LoadAddresses(); // Загружаем адреса клиента
        }

        // Метод для загрузки адресов с сервера
        private async void LoadAddresses()
        {
            try
            {
                // Устанавливаем заголовок авторизации
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                // Отправляем GET-запрос для получения списка адресов
                var response = await _httpClient.GetAsync("http://course-project-4/api/addresses");

                if (response.IsSuccessStatusCode) // Если запрос успешен
                {
                    var content = await response.Content.ReadAsStringAsync(); // Читаем ответ
                    // Десериализуем JSON в список адресов
                    var addresses = JsonSerializer.Deserialize<List<Address>>(content);

                    Addresses.Clear(); // Очищаем текущий список адресов
                    foreach (var address in addresses) // Добавляем адреса в ObservableCollection
                    {
                        Addresses.Add(address);
                    }
                }
                else
                {
                    // В случае ошибки выводим сообщение с кодом ошибки и содержимым ответа
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Ошибка", $"Код: {response.StatusCode}, Ответ: {errorContent}", "ОК");
                }
            }
            catch (Exception ex) // Обработка исключений
            {
                // Показываем ошибку в случае проблем с подключением
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "ОК");
            }
        }

        // Обработчик события выбора адреса
        private void OnAddressSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Address selectedAddress) // Проверяем, что выбран адрес
            {
                _selectedAddress = selectedAddress; // Сохраняем выбранный адрес
                SelectedAddressLabel.Text = $"Выбран: {_selectedAddress.City}, {_selectedAddress.Street}, {_selectedAddress.House}"; // Отображаем адрес
            }
        }

        // Обработчик кнопки "Оформить заказ"
        private async void OnPaymentButton(object sender, EventArgs e)
        {
            if (_selectedAddress == null) // Если адрес не выбран
            {
                await DisplayAlert("Ошибка", "Выберите адрес перед оформлением заказа.", "ОК");
                return; // Выход из метода, если адрес не выбран
            }

            try
            {
                // Устанавливаем заголовок авторизации
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                // Создаем данные для заказа
                var orderData = new { address_id = _selectedAddress.Id };
                var jsonContent = new StringContent(JsonSerializer.Serialize(orderData), Encoding.UTF8, "application/json");

                // Отправляем POST-запрос для создания нового заказа
                HttpResponseMessage response = await _httpClient.PostAsync("http://course-project-4/api/orders", jsonContent);

                if (response.IsSuccessStatusCode) // Если запрос успешен
                {
                    await DisplayAlert("Заказ", "Ваш заказ оформлен!", "OK");
                    // Перенаправляем пользователя на главную страницу
                    await Navigation.PushAsync(new HomePage(_client, _token));
                }
                else
                {
                    // В случае ошибки отображаем сообщение с кодом ошибки и описанием
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Ошибка", $"Описание: {(int)response.StatusCode}\n{errorContent}", "ОК");
                }
            }
            catch (Exception ex) // Обработка исключений
            {
                // Показываем ошибку в случае проблем с подключением
                await DisplayAlert("Ошибка", ex.Message, "ОК");
            }
        }
    }
}
