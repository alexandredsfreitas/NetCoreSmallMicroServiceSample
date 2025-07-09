namespace ECommerce.Stock.Application.DTOs;

public record ProductStockDto(Guid Id, string Name, int Available, int Reserved);