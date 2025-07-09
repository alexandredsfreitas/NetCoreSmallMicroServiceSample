namespace ECommerce.Stock.Domain.Models;

public record StockQuantity
{
    public int Value { get; }
        
    public StockQuantity(int value)
    {
        if (value < 0)
            throw new ArgumentException("Quantidade em estoque não pode ser negativa");
        Value = value;
    }
        
    public static implicit operator int(StockQuantity quantity) => quantity.Value;
    public static implicit operator StockQuantity(int value) => new(value);
}