using System;
using System.Linq;
using cursach.Models;

namespace cursach.Services;

public class WarehouseService
{
    private readonly DataService _dataService;

    public WarehouseService(DataService dataService)
    {
        _dataService = dataService;
    }

    public (bool success, string message) AddProductToOrder(Order order, Product product, int quantity = 1)
    {
        if (product.Stock <= 0)
        {
            return (false, $"Товара '{product.Name}' нет на складе");
        }

        if (product.Stock < quantity)
        {
            return (false, $"Недостаточно товара '{product.Name}' на складе. Доступно: {product.Stock} шт.");
        }

        var existingItem = order.Items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existingItem != null)
        {
            if (product.Stock < existingItem.Quantity + quantity)
            {
                return (false, $"Недостаточно товара '{product.Name}' на складе. Доступно: {product.Stock} шт., в заказе уже: {existingItem.Quantity} шт.");
            }
            existingItem.Quantity += quantity;
        }
        else
        {
            var newItem = new OrderItem 
            { 
                ProductId = product.Id, 
                Name = product.Name, 
                Price = product.Price, 
                Quantity = quantity 
            };
            order.Items.Add(newItem);
        }
        
        return (true, "Товар успешно добавлен в заказ");
    }

    public (bool success, string message) PayOrder(Order order)
    {
        if (order.Status == OrderStatus.Completed)
        {
            return (false, "Заказ уже завершен");
        }

        if (order.Status == OrderStatus.Paid)
        {
            return (false, "Заказ уже оплачен");
        }

        if (order.Items.Count == 0)
        {
            return (false, "Нельзя оплатить пустой заказ. Добавьте товары в заказ.");
        }

        if (order.Total <= 0)
        {
            return (false, "Сумма заказа должна быть больше 0");
        }
        
        order.PayFullAmount();
        return (true, $"Заказ оплачен. Сумма: {order.Total} руб.");
    }

    public (bool success, string message) ShipOrder(Order order)
    {
        if (!order.IsPaid)
        {
            return (false, "Заказ не оплачен");
        }

        if (order.Status == OrderStatus.Completed)
        {
            return (false, "Заказ уже отгружен");
        }

        foreach (var item in order.Items)
        {
            var product = _dataService.GetProduct(item.ProductId);
            if (product == null)
            {
                return (false, $"Товар '{item.Name}' не найден на складе");
            }
            if (product.Stock < item.Quantity)
            {
                return (false, $"Недостаточно товара '{item.Name}' на складе. Доступно: {product.Stock} шт., требуется: {item.Quantity} шт.");
            }
        }

        foreach (var item in order.Items)
        {
            var product = _dataService.GetProduct(item.ProductId);
            product!.Stock -= item.Quantity;
        }

        order.Status = OrderStatus.Completed;
        return (true, "Заказ успешно отгружен");
    }
}