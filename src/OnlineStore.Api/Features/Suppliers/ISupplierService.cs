namespace OnlineStore.Api.Features.Suppliers;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<SupplierResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<SupplierResponse> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Guid id, UpdateSupplierRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
