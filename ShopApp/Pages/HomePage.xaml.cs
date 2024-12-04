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
        public List<Category> Categories { get; set; } // ��������� �������� ��� �������� ���������

        public HomePage(Client client, ProductService productService)
        {
            InitializeComponent();
            _client = client;
            _productService = productService;
            Products = new List<Product>(); // ������������� ������ ���������
            Categories = new List<Category>(); // ������������� ������ ���������

            // ����������� ������ ��� �������
            LabelClientName.Text = _client.FullName;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // ������� ��������� ���������
            var animation = new Animation(v => CategoryCollectionView.Opacity = v, 0, 1);
            animation.Commit(this, "FadeInAnimation", length: 1000, easing: Easing.Linear);

            // �������� ���������
            var categories = await _productService.GetCategoriesAsync();
            if (categories != null)
            {
                Categories = categories;
                CategoryCollectionView.ItemsSource = Categories;
            }
            else
            {
                await DisplayAlert("������", "�� ������� ��������� ���������", "OK");
            }

            // �������� ��� ��������� (��������, ���������� ��������)
            var productAnimation = new Animation(v => ProductCarouselView.Scale = v, 0, 1);
            productAnimation.Commit(this, "ScaleAnimation", length: 1000, easing: Easing.Linear);

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
