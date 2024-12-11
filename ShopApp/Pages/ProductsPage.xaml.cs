using ShopApp.Models;
using ShopApp.Services;
using System.Collections.Generic;

namespace ShopApp.Pages
{
    public partial class ProductsPage : ContentPage
    {
        private readonly ProductService _productService;
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }

        // Конструктор с параметром selectedCategoryId
        public ProductsPage(int selectedCategoryId)
        {
            InitializeComponent();
            _productService = new ProductService();
            Categories = new List<Category>();
            Products = new List<Product>();

            // Загружаем категории и продукты для выбранной категории
            LoadCategories();
            LoadProducts(selectedCategoryId); // Загружаем продукты для выбранной категории или все продукты (если selectedCategoryId == 0)
        }

        // Метод для загрузки категорий
        private async void LoadCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            if (categories != null)
            {
                Categories = categories;
                CategoryCollectionView.ItemsSource = Categories; // Привязываем категории к CollectionView
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить категории", "OK");
            }
        }

        // Метод для загрузки продуктов по выбранной категории
        private async void LoadProducts(int categoryId)
        {
            List<Product> products = null;

            // Если categoryId == 0, то загружаем все продукты
            if (categoryId == 0)
            {
                products = await _productService.GetProductsAsync(); // Получаем все продукты
            }
            else
            {
                products = await _productService.GetProductsByCategoryAsync(categoryId); // Получаем продукты для выбранной категории
            }

            if (products != null)
            {
                // Обновляем список продуктов
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }

                // Перезаписываем привязку для обновления данных в UI
                ProductsCollectionView.ItemsSource = null; // Очищаем текущую привязку
                ProductsCollectionView.ItemsSource = Products; // Устанавливаем заново
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить продукты для этой категории", "OK");
            }
        }

        // Обработчик для изменения выбранной категории
        private void OnCategorySelected(object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, есть ли выбранный элемент
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
