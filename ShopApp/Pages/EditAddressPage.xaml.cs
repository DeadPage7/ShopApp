using ShopApp.Models;
using System.Text;
using System.Text.Json;

namespace ShopApp.Pages;

public partial class EditAddressPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();  // HTTP клиент для выполнения запросов
    private Client _client;  // Данные клиента
    private Address _address;  // Адрес, который мы редактируем
    private readonly string _token;  // Токен для авторизации

    // Конструктор страницы редактирования адреса
    public EditAddressPage(Address address, Client client, string token)
    {
        InitializeComponent();  // Инициализация компонентов на странице
        _address = address;  // Сохранение переданного адреса
        _client = client;  // Сохранение данных клиента
        _token = token;  // Сохранение токена для авторизации

        // Заполнение полей ввода данными из переданного адреса
        CityEntry.Text = _address.City;
        StreetEntry.Text = _address.Street;
        HouseEntry.Text = _address.House;
        FloorEntry.Text = _address.Floor?.ToString();
        ApartmentOrOfficeEntry.Text = _address.ApartmentOrOffice;
        EntranceEntry.Text = _address.Entrance;
        IntercomEntry.Text = _address.Intercom;
        CommentEditor.Text = _address.Comment;
    }

    // Обработчик нажатия кнопки "Сохранить изменения"
    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        // Обновление объекта адреса новыми данными из полей ввода
        _address.City = CityEntry.Text;
        _address.Street = StreetEntry.Text;
        _address.House = HouseEntry.Text;
        _address.Floor = int.TryParse(FloorEntry.Text, out int floor) ? floor : null;  // Преобразование этажа в число
        _address.ApartmentOrOffice = ApartmentOrOfficeEntry.Text;
        _address.Entrance = EntranceEntry.Text;
        _address.Intercom = IntercomEntry.Text;
        _address.Comment = CommentEditor.Text;

        try
        {
            // Установка заголовка авторизации с токеном
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            // Подготовка данных для отправки в формате JSON
            var jsonContent = new StringContent(JsonSerializer.Serialize(_address), Encoding.UTF8, "application/json");

            // Отправка PUT-запроса на сервер для обновления адреса
            var response = await _httpClient.PutAsync($"http://course-project-4/api/addresses/{_address.Id}", jsonContent);

            // Проверка успешности ответа от сервера
            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Успех", "Адрес обновлен", "ОК");
                await Navigation.PopAsync();  // Переход обратно на предыдущую страницу
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();  // Чтение ошибки, если запрос не удался
                await DisplayAlert("Ошибка", $"Код: {response.StatusCode}, Ответ: {errorContent}", "ОК");
            }
        }
        catch (Exception ex)  // Обработка исключений (например, проблемы с интернет-соединением)
        {
            await DisplayAlert("Ошибка", ex.Message, "ОК");
        }
    }
}
