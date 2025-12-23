using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using cursach.Models;

namespace cursach.Services;

public interface IDataService
{
    List<Order> GetOrders();
    void AddOrder(Order order);
    void UpdateOrder(Order order);
    List<Product> GetProducts();
    Product? GetProduct(int id);
    void AddProduct(Product product);
    void UpdateProduct(Product product);
    void SaveToXml();
    void LoadFromXml();
}

public class DataService : IDataService
{
    private List<Order> _orders = new();
    private List<Product> _products = new();
    private int _nextOrderId = 1;
    private int _nextProductId = 1;

    public DataService()
    {
        _products.Add(new Product { Id = _nextProductId++, Name = "Ноутбук", Price = 50000, Stock = 10 });
        _products.Add(new Product { Id = _nextProductId++, Name = "Мышь", Price = 1500, Stock = 50 });
        _products.Add(new Product { Id = _nextProductId++, Name = "Клавиатура", Price = 3000, Stock = 30 });
    }

    public List<Order> GetOrders() => _orders;

    public void AddOrder(Order order)
    {
        order.Id = _nextOrderId++;
        _orders.Add(order);
    }

    public void UpdateOrder(Order order)
    {
        var index = _orders.FindIndex(o => o.Id == order.Id);
        if (index >= 0) _orders[index] = order;
    }

    public List<Product> GetProducts() => _products;

    public Product? GetProduct(int id) => _products.FirstOrDefault(p => p.Id == id);

    public void AddProduct(Product product)
    {
        product.Id = _nextProductId++;
        _products.Add(product);
    }

    public void UpdateProduct(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index >= 0) _products[index] = product;
    }

    public void SaveToXml()
    {
        var file = "data.xml";
        var data = new DataWrapper { Orders = _orders, Products = _products };
        using var writer = new StreamWriter(file);
        new XmlSerializer(typeof(DataWrapper)).Serialize(writer, data);
    }

    public void LoadFromXml()
    {
        var file = "data.xml";
        if (!File.Exists(file)) return;

        using var reader = new StreamReader(file);
        var data = (DataWrapper?)new XmlSerializer(typeof(DataWrapper)).Deserialize(reader);
        if (data != null)
        {
            _orders = data.Orders;
            _products = data.Products;

            _nextOrderId = _orders.Count > 0 ? _orders.Max(o => o.Id) + 1 : 1;
            _nextProductId = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 1;
        }
    }

    [XmlRoot("Data")]
    public class DataWrapper
    {
        public List<Order> Orders { get; set; } = new();
        public List<Product> Products { get; set; } = new();
    }
}