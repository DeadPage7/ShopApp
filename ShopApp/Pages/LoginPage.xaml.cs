using ShopApp.Models;
using ShopApp.Services;
using ShopApp.Pages;

namespace ShopApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly AuthService _authService;

        public LoginPage()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        // ��������� ������
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var login = LoginEntry.Text;
            var password = PasswordEntry.Text;

            var client = await _authService.LoginAsync(login, password);

            if (client != null)
            {
                // �������� ����, ������� ��������� ������� ��� ���������
                var productService = new ProductService();
                await Navigation.PushAsync(new HomePage(client, productService)); // �������� ������ ������� � ������ ���������
            }
            else
            {
                // ������ �����������
                await DisplayAlert("������", "�������� ����� ��� ������", "OK");
            }
        }

        private async void OnRegisterPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage()); // ��������� �������� �����������
        }
    }
}
