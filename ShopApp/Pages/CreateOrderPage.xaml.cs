using ShopApp.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text;

namespace ShopApp.Pages
{
    public partial class CreateOrderPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private Client _client;
        private readonly string _token;
        private Address _selectedAddress; // Хранит выбранный адрес

        public ObservableCollection<Address> Addresses { get; set; } = new ObservableCollection<Address>();

        public CreateOrderPage(Client client, string token)
        {
            InitializeComponent();
            _client = client;
            _token = token;
            AddressesCollectionView.ItemsSource = Addresses;
            BindingContext = this;

            LoadAddresses();
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
                    await DisplayAlert("Ошибка", $"Код: {response.StatusCode}, Ответ: {errorContent}", "ОК");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "ОК");
            }
        }

        private void OnAddressSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Address selectedAddress)
            {
                _selectedAddress = selectedAddress;
                SelectedAddressLabel.Text = $"Выбран: {_selectedAddress.City}, {_selectedAddress.Street}, {_selectedAddress.House}";
            }
        }

        private async void OnPaymentButton(object sender, EventArgs e)
        {
            if (_selectedAddress == null)
            {
                await DisplayAlert("Ошибка", "Выберите адрес перед оформлением заказа.", "ОК");
                return;
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                var orderData = new { address_id = _selectedAddress.Id };
                var jsonContent = new StringContent(JsonSerializer.Serialize(orderData), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("http://course-project-4/api/orders", jsonContent);

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
