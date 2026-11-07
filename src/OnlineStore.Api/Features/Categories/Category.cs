namespace OnlineStore.Api.Features.Categories;

/// <summary>
/// Сутність категорії товарів у PostgreSQL.
/// Дерево (ParentCategoryId) і унікальний індекс Slug додаються в окремих задачах.
/// </summary>
public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }
}
