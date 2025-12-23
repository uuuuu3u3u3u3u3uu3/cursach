using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using cursach.Models;
using cursach.Services;

namespace cursach.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly DataService _dataService;
    private readonly WarehouseService _warehouseService;
    
    private Order? _selectedOrder;
    private Product? _selectedProduct;
    private string _newProductName = "";
    private decimal _newProductPrice;
    private int _newProductStock;
    private string _statusMessage = "Готов к работе";

    public ObservableCollection<Order> Orders { get; } = new();
    public ObservableCollection<Product> Products { get; } = new();

    public Order? SelectedOrder
    {
        get => _selectedOrder;
        set { _selectedOrder = value; OnPropertyChanged(); }
    }

    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set { _selectedProduct = value; OnPropertyChanged(); }
    }

    public string NewProductName
    {
        get => _newProductName;
        set { _newProductName = value; OnPropertyChanged(); }
    }

    public decimal NewProductPrice
    {
        get => _newProductPrice;
        set { _newProductPrice = value; OnPropertyChanged(); }
    }

    public int NewProductStock
    {
        get => _newProductStock;
        set { _newProductStock = value; OnPropertyChanged(); }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public MainViewModel()
    {
        _dataService = new DataService();
        _warehouseService = new WarehouseService(_dataService);
        LoadData();
    }

    private void LoadData()
    {
        Products.Clear();
        foreach (var product in _dataService.GetProducts())
            Products.Add(product);

        Orders.Clear();
        foreach (var order in _dataService.GetOrders())
            Orders.Add(order);
    }

    public void CreateOrder()
    {
        var order = new Order { Customer = "Новый клиент", Status = OrderStatus.New };
        _dataService.AddOrder(order);
        Orders.Add(order);
        SelectedOrder = order;
        StatusMessage = "Создан новый заказ";
    }

    public void AddProductToOrder()
    {
        if (SelectedProduct == null || SelectedOrder == null)
        {
            StatusMessage = "Выберите товар и заказ";
            return;
        }
        
        var result = _warehouseService.AddProductToOrder(SelectedOrder, SelectedProduct);
        
        if (result.success)
        {
            _dataService.UpdateOrder(SelectedOrder);
            OnPropertyChanged(nameof(SelectedOrder));
            StatusMessage = result.message;
        }
        else
        {
            StatusMessage = result.message;
        }
    }

    public void PayOrder()
    {
        if (SelectedOrder == null)
        {
            StatusMessage = "Выберите заказ для оплаты";
            return;
        }

        var result = _warehouseService.PayOrder(SelectedOrder);
        
        if (result.success)
        {
            _dataService.UpdateOrder(SelectedOrder);
            OnPropertyChanged(nameof(SelectedOrder));
            StatusMessage = result.message;
        }
        else
        {
            StatusMessage = result.message;
        }
    }

    public void ShipOrder()
    {
        if (SelectedOrder == null)
        {
            StatusMessage = "Выберите заказ для отгрузки";
            return;
        }

        var result = _warehouseService.ShipOrder(SelectedOrder);
        
        if (result.success)
        {
            _dataService.UpdateOrder(SelectedOrder);
            OnPropertyChanged(nameof(SelectedOrder));
            StatusMessage = result.message;
        }
        else
        {
            StatusMessage = result.message;
        }
    }

    public void AddNewProduct()
    {
        if (string.IsNullOrWhiteSpace(NewProductName))
        {
            StatusMessage = "Введите название товара";
            return;
        }

        if (NewProductPrice <= 0)
        {
            StatusMessage = "Цена должна быть больше 0";
            return;
        }

        if (NewProductStock < 0)
        {
            StatusMessage = "Количество не может быть отрицательным";
            return;
        }

        var product = new Product 
        { 
            Name = NewProductName, 
            Price = NewProductPrice, 
            Stock = NewProductStock 
        };
        _dataService.AddProduct(product);
        Products.Add(product);
        
        NewProductName = "";
        NewProductPrice = 0;
        NewProductStock = 0;
        
        StatusMessage = $"Товар '{product.Name}' добавлен на склад";
    }

    public void UpdateSelectedProduct()
    {
        if (SelectedProduct != null)
        {
            _dataService.UpdateProduct(SelectedProduct);
            StatusMessage = $"Товар '{SelectedProduct.Name}' обновлен";
        }
        else
        {
            StatusMessage = "Выберите товар для редактирования";
        }
    }

    public void SaveData()
    {
        _dataService.SaveToXml();
        StatusMessage = "Данные сохранены в data.xml";
    }

    public void LoadDataFromFile()
    {
        _dataService.LoadFromXml();
        LoadData();
        StatusMessage = "Данные загружены из data.xml";
    }
    public void DeleteOrder()
    {
        if (SelectedOrder == null)
        {
            StatusMessage = "Выберите заказ для удаления";
            return;
        }

        Orders.Remove(SelectedOrder);
        
        var allOrders = _dataService.GetOrders();
        var orderToRemove = allOrders.FirstOrDefault(o => o.Id == SelectedOrder.Id);
        if (orderToRemove != null)
        {
            allOrders.Remove(orderToRemove);
        }
        
        SelectedOrder = null;
        StatusMessage = "Заказ успешно удален";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string? propertyName = null) => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}