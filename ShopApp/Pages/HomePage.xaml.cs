using ShopApp.Models;
using ShopApp.Services;
using System;
using System.Collections.Generic;

namespace ShopApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private readonly ProductService _productService;
        private readonly Client _client;

        public List<Product> Products { get; set; } // ��������� �������� ��� �������� ���������

        public HomePage(Client client, ProductService productService)
        {
            InitializeComponent();
            _client = client;
            _productService = productService;
            Products = new List<Product>(); // ������������� ������ ���������

            // ����������� ������ ��� �������
            LabelClientName.Text = _client.FullName;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // �������� ������ ���������
            var products = await _productService.GetProductsAsync();

            if (products != null)
            {
                // �������� ���� � ����������� ��� ��������
                foreach (var product in products)
                {
                    Console.WriteLine($"Product Image: {product.Photo}");
                }

                Products = products; // ����������� ��������
                ProductCarouselView.ItemsSource = Products; // ��������� �������� ������
            }
            else
            {
                await DisplayAlert("������", "�� ������� ��������� ��������", "OK");
            }
        }
    }
}
