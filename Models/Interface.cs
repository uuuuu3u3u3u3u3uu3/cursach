namespace cursach.Models;

public interface IEntity
{
    int Id { get; set; }
}

public interface IWarehouseOperation
{
    (bool success, string message) ValidateStock(Product product, int requiredQuantity);
    void UpdateStock(Product product, int quantity);
}