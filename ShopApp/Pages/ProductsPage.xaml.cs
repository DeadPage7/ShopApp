using ShopApp.Models;
using ShopApp.Services;
using System.Collections.Generic;

namespace ShopApp.Pages
{
    public partial class ProductsPage : ContentPage
    {
        private readonly ProductService _productService;
        private readonly Client _client; // Добавлен экземпляр клиента для хранения данных о пользователе
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }

        public ProductsPage(int selectedCategoryId, Client client) // Передаем клиента при создании страницы
        {
            InitializeComponent();
            _productService = new ProductService();
            _client = client; // Присваиваем клиента
            Categories = new List<Category>();
            Products = new List<Product>();

            // Устанавливаем имя клиента в Label
            LabelClientName.Text = _client.FullName;

            // Загружаем категории и продукты
            LoadCategories();
            LoadProducts(selectedCategoryId);
        }

        // Метод для загрузки категорий
        private async void LoadCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            if (categories != null)
            {
                // Добавляем "Все товары" в начало списка категорий
                Categories.Clear();
                Categories.Add(new Category { Id = 0, Name = "Все товары" });
                Categories.AddRange(categories);

                CategoryCollectionView.ItemsSource = Categories; // Привязываем категории к CollectionView
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить категории", "OK");
            }
        }

        // Метод для загрузки продуктов
        private async void LoadProducts(int categoryId)
        {
            List<Product> products;

            if (categoryId == 0)
            {
                // Если выбран "Все товары", загружаем все продукты
                products = await _productService.GetProductsAsync();
            }
            else
            {
                // Загружаем продукты для выбранной категории
                products = await _productService.GetProductsByCategoryAsync(categoryId);
            }

            if (products != null)
            {
                // Обновляем список продуктов
                Products.Clear();
                Products.AddRange(products);

                // Перезаписываем привязку для обновления данных в UI
                ProductsCollectionView.ItemsSource = null; // Очищаем текущую привязку
                ProductsCollectionView.ItemsSource = Products; // Устанавливаем заново
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить продукты", "OK");
            }
        }

        private async void OnDetailsProductButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var selectedProduct = button?.BindingContext as Product;

            if (selectedProduct != null)
            {
                // Переход на страницу деталей товара
                await Navigation.PushAsync(new DetailsProductPage(selectedProduct, _client));
            }
        }

        // Обработчик для изменения выбранной категории
        private void OnCategorySelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                // Получаем выбранную категорию
                var selectedCategory = e.CurrentSelection[0] as Category;
                if (selectedCategory != null)
                {
                    // Загружаем продукты для выбранной категории
                    LoadProducts(selectedCategory.Id);

                    // Снимаем выделение, чтобы избежать повторных загрузок
                    ((CollectionView)sender).SelectedItem = null;
                }
            }
        }
    }
}
