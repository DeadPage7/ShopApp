using ShopApp.Models;
using System.Collections.Generic;

namespace ShopApp.Pages
{
    public partial class ProductsPage : ContentPage
    {
        private readonly Client _client; // Добавлен экземпляр клиента для хранения данных о пользователе
        private string _token;
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }

        public ProductsPage(int selectedCategoryId, Client client, string token)
        {
            InitializeComponent();
            _client = client;
            _token = token;

            Categories = new List<Category>();
            Products = new List<Product>();

            LabelClientName.Text = _client.FullName;

            // Загружаем категории и продукты
            LoadCategories();
            LoadProducts(selectedCategoryId);

            // Устанавливаем обработчик выбора категории
            CategoryCollectionView.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection.Count > 0)
                {
                    var selectedCategory = e.CurrentSelection[0] as Category;
                    if (selectedCategory != null)
                    {
                        await DisplayProductsByCategory(selectedCategory.Id);
                    }
                }
            };
        }

        private async void LoadCategories()
        {
            try
            {
                var productService = new ProductService(_token);
                Categories = await productService.GetCategoriesAsync();
                CategoryCollectionView.ItemsSource = Categories;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить категории: {ex.Message}", "ОК");
            }
        }

        private async void LoadProducts(int categoryId)
        {
            try
            {
                var productService = new ProductService(_token);
                Products = categoryId == 0
                    ? await productService.GetProductsAsync()
                    : await productService.GetProductsByCategoryAsync(categoryId);
                ProductsCollectionView.ItemsSource = Products;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить продукты: {ex.Message}", "ОК");
            }
        }

        private async Task DisplayProductsByCategory(int categoryId)
        {
            try
            {
                var productService = new ProductService(_token);
                Products = await productService.GetProductsByCategoryAsync(categoryId);
                ProductsCollectionView.ItemsSource = Products;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить продукты: {ex.Message}", "ОК");
            }
        }

        private async void OnDetailsProductButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var selectedProduct = button?.BindingContext as Product;

            if (selectedProduct != null)
            {
                // Переход на страницу деталей товара
                await Navigation.PushAsync(new DetailsProductPage(selectedProduct, _client, _token));
            }
        }
        private async void OnHomeButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HomePage(_client, _token)); // 0 для всех продуктов
        }
        // Обработчик кнопки Каталог
        private async void OnCatalogButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductsPage(0, _client, _token)); // 0 для всех продуктов
        }

        // Обработчик кнопки Профиль
        private async void OnProfileButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(_client, _token));
        }

        private async void OnCartButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartPage(_client, _token));
        }
    }
}
