using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Api.Common;

/// <summary>
/// Бізнес-параметри інтернет-магазину. Числа з ТЗ не хардкодяться в сервісах,
/// а читаються з конфігурації через Options pattern.
/// </summary>
public sealed class StoreOptions
{
    public const string SectionName = "Store";

    /// <summary>Через скільки годин неоплачене замовлення (New) автоматично скасовується.</summary>
    [Required]
    [Range(1, 168)]
    public int UnpaidOrderTimeoutHours { get; set; } = 24;

    /// <summary>Максимальна кількість позицій в одному замовленні.</summary>
    [Required]
    [Range(1, 100)]
    public int MaxItemsPerOrder { get; set; } = 50;
}
