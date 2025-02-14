using ShopApp.Models;
using System.Collections.Generic;

namespace ShopApp.Pages
{
    public partial class ProductsPage : ContentPage
    {
        private readonly Client _client; // Экземпляр клиента
        private string _token; // Токен аутентификации
        public List<Product> Products { get; set; } // Список продуктов
        public List<Category> Categories { get; set; } // Список категорий

        // Конструктор с параметрами
        public ProductsPage(int selectedCategoryId, Client client, string token)
        {
            InitializeComponent();
            _client = client;
            _token = token;

            Categories = new List<Category>();
            Products = new List<Product>();

            // Устанавливаем имя клиента в верхней части страницы
            LabelClientName.Text = _client.FullName;

            LoadCategories(); // Загрузка категорий
            LoadProducts(selectedCategoryId); // Загрузка продуктов

            // Обработчик выбора категории
            CategoryCollectionView.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection.Count > 0)
                {
                    var selectedCategory = e.CurrentSelection[0] as Category;
                    if (selectedCategory != null)
                    {
                        await DisplayProductsByCategory(selectedCategory.Id); // Загружаем продукты выбранной категории
                    }
                }
            };
        }

        // Загрузка категорий с сервера
        private async void LoadCategories()
        {
            try
            {
                var productService = new ProductService(_token);
                Categories = await productService.GetCategoriesAsync();
                CategoryCollectionView.ItemsSource = Categories; // Привязываем категории
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить категории: {ex.Message}", "ОК");
            }
        }

        // Загрузка продуктов по выбранной категории
        private async void LoadProducts(int categoryId)
        {
            try
            {
                var productService = new ProductService(_token);
                Products = categoryId == 0
                    ? await productService.GetProductsAsync()
                    : await productService.GetProductsByCategoryAsync(categoryId);
                ProductsCollectionView.ItemsSource = Products; // Привязываем продукты
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить продукты: {ex.Message}", "ОК");
            }
        }

        // Метод для отображения продуктов по категории
        private async Task DisplayProductsByCategory(int categoryId)
        {
            try
            {
                var productService = new ProductService(_token);
                Products = await productService.GetProductsByCategoryAsync(categoryId);
                ProductsCollectionView.ItemsSource = Products; // Привязываем продукты
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить продукты: {ex.Message}", "ОК");
            }
        }

        // Обработчик кнопки "Подробнее"
        private async void OnDetailsProductButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var selectedProduct = button?.BindingContext as Product;

            if (selectedProduct != null)
            {
                // Переход на страницу с деталями товара
                await Navigation.PushAsync(new DetailsProductPage(selectedProduct, _client, _token));
            }
        }

        // Обработчик кнопки "Главная"
        private async void OnHomeButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HomePage(_client, _token));
        }

        // Обработчик кнопки "Каталог"
        private async void OnCatalogButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductsPage(0, _client, _token)); // Переход на страницу всех продуктов
        }

        // Обработчик кнопки "Профиль"
        private async void OnProfileButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(_client, _token)); // Переход на страницу профиля
        }

        // Обработчик кнопки "Корзина"
        private async void OnCartButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartPage(_client, _token)); // Переход в корзину покупок
        }
    }
}
