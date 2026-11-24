namespace OnlineStore.Api.Features.Products;

/// <summary>
/// Імена політики та тега output cache для каталогу товарів.
/// Каталог не залежить від користувача — кешувати можна;
/// персоналізовані відповіді (Authorization / user claims) сюди не кладемо.
/// </summary>
public static class ProductsOutputCache
{
    public const string PolicyName = "Products";

    public const string Tag = "products";
}
