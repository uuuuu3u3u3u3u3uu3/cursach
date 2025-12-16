using System;
using System.Collections.Generic;
using System.Linq;

namespace cursach.Models;

public enum OrderStatus { New, Paid, Completed }

public class Order
{
    public int Id { get; set; }
    public string Customer { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.Now;
    public OrderStatus Status { get; set; }
    public decimal Total => Items.Sum(i => i.Price * i.Quantity);
    public decimal Paid { get; set; }
    public bool IsPaid => Paid >= Total;
    public List<OrderItem> Items { get; set; } = new();

    public void PayFullAmount()
    {
        Paid = Total;
        Status = OrderStatus.Paid;
    }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}