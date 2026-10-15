namespace OnlineStore.Api.Features.Warehouses;

/// <summary>
/// Інтерфейс сервісу керування складами.
/// </summary>
public interface IWarehouseService
{
    /// <summary>
    /// Отримує список усіх складів.
    /// </summary>
    Task<IReadOnlyList<WarehouseResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримує склад за його ідентифікатором.
    /// </summary>
    Task<WarehouseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Створює новий склад.
    /// </summary>
    Task<WarehouseResponse> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Оновлює дані існуючого складу.
    /// </summary>
    /// <returns>true, якщо склад знайдено й оновлено; false, якщо склад не знайдено.</returns>
    Task<bool> UpdateAsync(int id, UpdateWarehouseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Видаляє склад за ідентифікатором.
    /// </summary>
    /// <returns>true, якщо склад знайдено й видалено; false, якщо склад не знайдено.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
