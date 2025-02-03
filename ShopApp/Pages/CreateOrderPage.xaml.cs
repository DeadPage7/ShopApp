using ShopApp.Models;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ShopApp.Pages
{
    public partial class CreateOrderPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private Client _client;
        private readonly string _token;

        public ObservableCollection<Address> Addresses { get; set; } = new ObservableCollection<Address>();

        public CreateOrderPage(Client client, string token)
        {
            InitializeComponent();
            _client = client;
            _token = token;
            AddressesCollectionView.ItemsSource = Addresses;
            BindingContext = this;

            LoadAddresses(); // Загружаем адреса
        }

        private async void LoadAddresses()
        {
            try
            {

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                var response = await _httpClient.GetAsync("http://course-project-4/api/addresses");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var addresses = JsonSerializer.Deserialize<List<Address>>(content);

                    Addresses.Clear();
                    foreach (var address in addresses)
                    {
                        Addresses.Add(address);
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Îøèáêà", $"Êîä: {response.StatusCode}, Îòâåò: {errorContent}", "ÎÊ");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Îøèáêà", $"Ïðîèçîøëà îøèáêà: {ex.Message}", "ÎÊ");
            }
        }

        private async void OnSelectAddressClicked(object sender, EventArgs e)
        {
            var selectedAddress = (sender as Button)?.BindingContext as Address;
            if (selectedAddress != null)
            {
                await DisplayAlert("Адрес выбран", $"Вы выбрали: {selectedAddress.City}, {selectedAddress.Street}", "OK");
            }
        }

        private async void OnPaymentButton(object sender, EventArgs e)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
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
}
