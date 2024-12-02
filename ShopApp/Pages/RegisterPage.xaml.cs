using ShopApp.Models;
using ShopApp.Services;
using System;

namespace ShopApp.Pages
{
    public partial class RegisterPage : ContentPage
    {
        private readonly AuthService _authService;

        public RegisterPage()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        // ����� ��� �����������
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // �������� ������ �� �����
            var login = RegisterLoginEntry.Text;
            var password = RegisterPasswordEntry.Text;
            var passwordConfirmation = RegisterPasswordConfirmationEntry.Text;
            var fullName = RegisterFullNameEntry.Text;
            var email = RegisterEmailEntry.Text;
            var birth = RegisterBirthEntry.Date;
            var telephone = RegisterTelephoneEntry.Text;

            // ��������� ������ �� �����������
            var client = await _authService.RegisterAsync(login, password, passwordConfirmation, fullName, email, birth, telephone);

            // ���������, ��� ����������� ������ �������
            if (client != null)
            {
                // ������� �� ������� �������� � �������� ������ ������� � ������� ��� ���������
                var productService = new ProductService();
                await Navigation.PushAsync(new HomePage(client, productService)); // �������� ������ Client � ������ ��� ���������
            }
            else
            {
                // �������� ��������� �� ������, ���� ����������� �� �������
                await DisplayAlert("������", "�� ������� ���������������� ������������. ���������� �����.", "OK");
            }
        }

        // ������� �� �������� �����������
        private async void OnLoginPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage()); // ������� �� �������� ������
        }
    }
}
