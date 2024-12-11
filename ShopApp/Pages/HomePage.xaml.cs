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

        public List<Product> Products { get; set; } // Добавляем свойство для привязки продуктов
        public List<Category> Categories { get; set; } // Добавляем свойство для привязки категорий

        public HomePage(Client client, ProductService productService)
        {
            InitializeComponent();
            _client = client;
            _productService = productService;
            Products = new List<Product>();
            Categories = new List<Category>();

            LabelClientName.Text = _client.FullName;

            // Добавляем обработчик для выбора категории
            CategoryCollectionView.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection.Count > 0)
                {
                    var selectedCategory = e.CurrentSelection[0] as Category;
                    if (selectedCategory != null)
                    {
                        // Навигация на страницу с продуктами выбранной категории
                        await Navigation.PushAsync(new ProductsPage(selectedCategory.Id));
                    }
                }
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Плавное появление элементов
            var animation = new Animation(v => CategoryCollectionView.Opacity = v, 0, 1);
            animation.Commit(this, "FadeInAnimation", length: 1000, easing: Easing.Linear);

            // Загрузка категорий
            var categories = await _productService.GetCategoriesAsync();
            if (categories != null)
            {
                Categories = categories;
                CategoryCollectionView.ItemsSource = Categories;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить категории", "OK");
            }

            // Анимация для продуктов (например, увеличение масштаба)
            var productAnimation = new Animation(v => ProductCarouselView.Scale = v, 0, 1);
            productAnimation.Commit(this, "ScaleAnimation", length: 1000, easing: Easing.Linear);

            // Получаем список продуктов
            var products = await _productService.GetProductsAsync();

            if (products != null)
            {
                // Логируем пути к изображению для проверки
                foreach (var product in products)
                {
                    Console.WriteLine($"Product Image: {product.Photo}");
                }

                Products = products; // Привязываем продукты
                ProductCarouselView.ItemsSource = Products; // Обновляем источник данных
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить продукты", "OK");
            }
        }
        // Обработчик кнопки Каталог
        private async void OnCatalogButtonClicked(object sender, EventArgs e)
        {
            // Переход на страницу каталога (ProductsPage) с дефолтным selectedCategoryId (например, 0)
            await Navigation.PushAsync(new ProductsPage(0));
        }

        // Обработчик кнопки Профиль
        private async void OnProfileButtonClicked(object sender, EventArgs e)
        {
            // Переход на страницу профиля (ProfilePage)
            await Navigation.PushAsync(new ProfilePage());
        }
        private async void OnHomeButtonClicked(object sender, EventArgs e)
        {
            
        }

    }
}
