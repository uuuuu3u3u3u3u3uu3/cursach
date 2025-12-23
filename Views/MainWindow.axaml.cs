using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using cursach.ViewModels;

namespace cursach.Views;

public partial class MainWindow : Window
{
    private MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
        
        this.Loaded += OnWindowLoaded;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnWindowLoaded(object? sender, RoutedEventArgs e)
    {
        InitializeControls();
    }

    private void InitializeControls()
    {
        var ordersList = this.FindControl<ListBox>("OrdersList");
        var productsList = this.FindControl<ListBox>("ProductsList");
        var statusMessageText = this.FindControl<TextBlock>("StatusMessageText");
        
        if (ordersList != null) ordersList.ItemsSource = _viewModel.Orders;
        if (productsList != null) productsList.ItemsSource = _viewModel.Products;
        if (statusMessageText != null) statusMessageText.DataContext = _viewModel;
        
        UpdateOrderDetails();
        UpdateProductEditFields();
    }

    private void CreateOrderClick(object sender, RoutedEventArgs e)
    {
        _viewModel.CreateOrder();
        var ordersList = this.FindControl<ListBox>("OrdersList");
        if (ordersList != null)
        {
            ordersList.ItemsSource = null;
            ordersList.ItemsSource = _viewModel.Orders;
        }
        UpdateOrderDetails();
    }

    private void AddProductClick(object sender, RoutedEventArgs e)
    {
        _viewModel.AddProductToOrder();
        UpdateOrderDetails();
    }

    private void PayOrderClick(object sender, RoutedEventArgs e)
    {
        _viewModel.PayOrder();
        UpdateOrderDetails();
    }

    private void ShipOrderClick(object sender, RoutedEventArgs e)
    {
        _viewModel.ShipOrder();
        UpdateOrderDetails();
    }

    private void AddNewProductClick(object sender, RoutedEventArgs e)
    {
        _viewModel.AddNewProduct();
        var productsList = this.FindControl<ListBox>("ProductsList");
        if (productsList != null)
        {
            productsList.ItemsSource = null;
            productsList.ItemsSource = _viewModel.Products;
        }
    }

    private void UpdateProductClick(object sender, RoutedEventArgs e)
    {
        _viewModel.UpdateSelectedProduct();
        var productsList = this.FindControl<ListBox>("ProductsList");
        if (productsList != null)
        {
            productsList.ItemsSource = null;
            productsList.ItemsSource = _viewModel.Products;
        }
    }

    private void SaveDataClick(object sender, RoutedEventArgs e) => _viewModel.SaveData();
    
    private void LoadDataClick(object sender, RoutedEventArgs e)
    {
        _viewModel.LoadDataFromFile();
        var ordersList = this.FindControl<ListBox>("OrdersList");
        var productsList = this.FindControl<ListBox>("ProductsList");
        
        if (ordersList != null)
        {
            ordersList.ItemsSource = null;
            ordersList.ItemsSource = _viewModel.Orders;
        }
        if (productsList != null)
        {
            productsList.ItemsSource = null;
            productsList.ItemsSource = _viewModel.Products;
        }
        
        UpdateOrderDetails();
        UpdateProductEditFields();
    }

    private void OrdersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var ordersList = sender as ListBox;
        if (ordersList?.SelectedItem is Models.Order order)
        {
            _viewModel.SelectedOrder = order;
            UpdateOrderDetails();
        }
    }

    private void ProductsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var productsList = sender as ListBox;
        if (productsList?.SelectedItem is Models.Product product)
        {
            _viewModel.SelectedProduct = product;
            UpdateProductEditFields();
        }
    }

    private void CustomerTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_viewModel.SelectedOrder != null)
        {
            _viewModel.SelectedOrder.Customer = (sender as TextBox)?.Text ?? "";
        }
    }

    // Обработчики для нового товара
    private void NewProductNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _viewModel.NewProductName = (sender as TextBox)?.Text ?? "";
    }

    private void NewProductPriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (decimal.TryParse((sender as TextBox)?.Text, out decimal price))
        {
            _viewModel.NewProductPrice = price;
        }
    }

    private void NewProductStockTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse((sender as TextBox)?.Text, out int stock))
        {
            _viewModel.NewProductStock = stock;
        }
    }

    private void EditProductNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_viewModel.SelectedProduct != null)
        {
            _viewModel.SelectedProduct.Name = (sender as TextBox)?.Text ?? "";
        }
    }

    private void EditProductPriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_viewModel.SelectedProduct != null && decimal.TryParse((sender as TextBox)?.Text, out decimal price))
        {
            _viewModel.SelectedProduct.Price = price;
        }
    }

    private void EditProductStockTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_viewModel.SelectedProduct != null && int.TryParse((sender as TextBox)?.Text, out int stock))
        {
            _viewModel.SelectedProduct.Stock = stock;
        }
    }
    private void DeleteOrderClick(object sender, RoutedEventArgs e)
{
    _viewModel.DeleteOrder();
    var ordersList = this.FindControl<ListBox>("OrdersList");
    if (ordersList != null)
    {
        ordersList.ItemsSource = null;
        ordersList.ItemsSource = _viewModel.Orders;
    }
    UpdateOrderDetails();
}

    private void UpdateOrderDetails()
    {
        var customerTextBox = this.FindControl<TextBox>("CustomerTextBox");
        var totalText = this.FindControl<TextBlock>("TotalText");
        var paidText = this.FindControl<TextBlock>("PaidText");
        var statusText = this.FindControl<TextBlock>("StatusText");
        var itemsCountText = this.FindControl<TextBlock>("ItemsCountText");
        var orderItemsList = this.FindControl<ListBox>("OrderItemsList");

        if (_viewModel.SelectedOrder != null)
        {
            if (customerTextBox != null) customerTextBox.Text = _viewModel.SelectedOrder.Customer;
            if (totalText != null) totalText.Text = $"Общая сумма: {_viewModel.SelectedOrder.Total} руб.";
            if (paidText != null) paidText.Text = $"Оплачено: {_viewModel.SelectedOrder.Paid} руб.";
            if (statusText != null) statusText.Text = $"Статус: {_viewModel.SelectedOrder.Status}";
            if (itemsCountText != null) itemsCountText.Text = $"Товаров в заказе: {_viewModel.SelectedOrder.Items.Count}";
            if (orderItemsList != null)
            {
                orderItemsList.ItemsSource = null;
                orderItemsList.ItemsSource = _viewModel.SelectedOrder.Items;
            }
        }
        else
        {
            if (customerTextBox != null) customerTextBox.Text = "";
            if (totalText != null) totalText.Text = "Общая сумма: 0 руб.";
            if (paidText != null) paidText.Text = "Оплачено: 0 руб.";
            if (statusText != null) statusText.Text = "Статус: -";
            if (itemsCountText != null) itemsCountText.Text = "Товаров в заказе: 0";
            if (orderItemsList != null) orderItemsList.ItemsSource = null;
        }
    }

    private void UpdateProductEditFields()
    {
        var editProductNameTextBox = this.FindControl<TextBox>("EditProductNameTextBox");
        var editProductPriceTextBox = this.FindControl<TextBox>("EditProductPriceTextBox");
        var editProductStockTextBox = this.FindControl<TextBox>("EditProductStockTextBox");

        if (_viewModel.SelectedProduct != null)
        {
            if (editProductNameTextBox != null) editProductNameTextBox.Text = _viewModel.SelectedProduct.Name;
            if (editProductPriceTextBox != null) editProductPriceTextBox.Text = _viewModel.SelectedProduct.Price.ToString();
            if (editProductStockTextBox != null) editProductStockTextBox.Text = _viewModel.SelectedProduct.Stock.ToString();
        }
        else
        {
            if (editProductNameTextBox != null) editProductNameTextBox.Text = "";
            if (editProductPriceTextBox != null) editProductPriceTextBox.Text = "";
            if (editProductStockTextBox != null) editProductStockTextBox.Text = "";
        }
    }
}