using ShopApp.Models;
using System.Text.Json;
using System.Text;

namespace ShopApp.Pages;

public partial class CreateAddressPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient(); // HTTP клиент для отправки запросов
    private Client _client; // Хранит информацию о клиенте
    private readonly string _token; // Токен для авторизации пользователя

    // Конструктор страницы, в который передаются клиент и токен
    public CreateAddressPage(Client client, string token)
    {
        InitializeComponent();
        _client = client; // Инициализация клиента
        _token = token; // Инициализация токена
    }

    // Обработчик кнопки для сохранения нового адреса
    private async void OnSaveAddressClicked(object sender, EventArgs e)
    {
        // Создание нового объекта Address, который будет отправлен на сервер
        var newAddress = new Address
        {
            City = CityEntry.Text, // Заполнение города
            Street = StreetEntry.Text, // Заполнение улицы
            House = HouseEntry.Text, // Заполнение дома
            Floor = int.TryParse(FloorEntry.Text, out int floor) ? floor : null, // Заполнение этажа (если возможно)
            ApartmentOrOffice = ApartmentOrOfficeEntry.Text, // Заполнение квартиры или офиса
            Entrance = EntranceEntry.Text, // Заполнение подъезда
            Intercom = IntercomEntry.Text, // Заполнение домофона
            Comment = CommentEditor.Text // Заполнение комментария
        };

        try
        {
            // Установка заголовка авторизации с Bearer токеном
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            // Сериализация объекта newAddress в JSON
            var jsonContent = JsonSerializer.Serialize(newAddress);
            // Создание тела запроса с типом "application/json"
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Отправка POST-запроса на сервер для добавления нового адреса
            var response = await _httpClient.PostAsync("http://course-project-4/api/addresses", content);

            // Обработка успешного ответа от сервера
            if (response.IsSuccessStatusCode)
            {
                // Показать сообщение об успешном добавлении адреса
                await DisplayAlert("Успех", "Адрес успешно добавлен!", "ОК");
                await Navigation.PopAsync(); // Возвращаемся на предыдущую страницу
            }
            else
            {
                // Если ошибка, выводим сообщение об ошибке с кодом и содержимым ответа
                var errorContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Ошибка", $"Код: {response.StatusCode}\n{errorContent}", "ОК");
            }
        }
        catch (Exception ex)
        {
            // Обработка исключений, если произошла ошибка при отправке запроса
            await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "ОК");
        }
    }
}
