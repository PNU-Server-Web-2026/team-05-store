namespace OnlineStore.Api.Features.Warehouses;

/// <summary>
/// DTO запиту на створення нового складу. Id не передається, оскільки його генерує сервер.
/// </summary>
/// <param name="Name">Назва складу (до 100 символів).</param>
/// <param name="City">Місто розташування складу (до 100 символів).</param>
/// <param name="Address">Адреса складу (до 300 символів).</param>
/// <param name="Priority">Пріоритет резервування 1–100 (з меншим пріоритетом списується першим).</param>
public sealed record CreateWarehouseRequest(
    string Name,
    string City,
    string Address,
    int Priority);

/// <summary>
/// DTO запиту на оновлення існуючого складу.
/// </summary>
/// <param name="Name">Назва складу (до 100 символів).</param>
/// <param name="City">Місто розташування складу (до 100 символів).</param>
/// <param name="Address">Адреса складу (до 300 символів).</param>
/// <param name="Priority">Пріоритет резервування 1–100.</param>
public sealed record UpdateWarehouseRequest(
    string Name,
    string City,
    string Address,
    int Priority);

/// <summary>
/// DTO відповіді сервера з даними складу.
/// </summary>
/// <param name="Id">Унікальний ідентифікатор складу.</param>
/// <param name="Name">Назва складу.</param>
/// <param name="City">Місто розташування складу.</param>
/// <param name="Address">Адреса складу.</param>
/// <param name="Priority">Пріоритет резервування.</param>
public sealed record WarehouseResponse(
    int Id,
    string Name,
    string City,
    string Address,
    int Priority);
