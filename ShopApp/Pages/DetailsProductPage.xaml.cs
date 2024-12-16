using ShopApp.Models;
using Microsoft.Maui.Controls;

namespace ShopApp.Pages
{
    public partial class DetailsProductPage : ContentPage
    {
        private Product _product;
        private Client _client;
        private int _quantity = 1; // Переменная для отслеживания количества товара

        // Передаем продукт и клиента в конструктор
        public DetailsProductPage(Product product, Client client)
        {
            InitializeComponent();
            _product = product;
            _client = client; // Присваиваем клиента
            BindProductDetails(); // Заполняем элементы значениями продукта
            LabelClientName.Text = _client.FullName; // Отображаем имя пользователя

            // Инициализируем количество на старте
            QuantityLabel.Text = _quantity.ToString();
        }

        // Заполнение элементов страницы данными продукта
        private void BindProductDetails()
        {
            ProductImage.Source = _product.Photo;
            ProductName.Text = _product.Name;
            ProductPrice.Text = $"Цена: {_product.Price:C}";
            ProductDescription.Text = _product.Description;
        }

        // Обработчик кнопки "+"
        private void OnIncreaseButtonClicked(object sender, EventArgs e)
        {
            _quantity++; // Увеличиваем количество
            QuantityLabel.Text = _quantity.ToString(); // Обновляем отображаемое количество
        }

        // Обработчик кнопки "-"
        private void OnDecreaseButtonClicked(object sender, EventArgs e)
        {
            if (_quantity > 1) // Проверяем, что количество не меньше 1
            {
                _quantity--; // Уменьшаем количество
                QuantityLabel.Text = _quantity.ToString(); // Обновляем отображаемое количество
            }
        }

        // Обработчик кнопки "Купить"
        private async void OnBuyButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Купить", $"Вы добавили {_product.Name} в корзину. Количество: {_quantity}", "ОК");
            // Логика добавления товара в корзину
        }
    }
}
