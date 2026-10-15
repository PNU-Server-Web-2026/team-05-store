using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Api.Features.Warehouses;

/// <summary>
/// Контролер керування складами магазину.
/// </summary>
[ApiController]
[Route("api/warehouses")]
public class WarehousesController(IWarehouseService warehouseService) : ControllerBase
{
    /// <summary>
    /// Отримує список усіх складів.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WarehouseResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var warehouses = await warehouseService.GetAllAsync(cancellationToken);
        return Ok(warehouses);
    }

    /// <summary>
    /// Отримує склад за його унікальним ідентифікатором.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<WarehouseResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var warehouse = await warehouseService.GetByIdAsync(id, cancellationToken);
        if (warehouse is null)
        {
            return NotFound();
        }

        return Ok(warehouse);
    }

    /// <summary>
    /// Створює новий склад.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WarehouseResponse>> Create([FromBody] CreateWarehouseRequest request, CancellationToken cancellationToken)
    {
        var created = await warehouseService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Оновлює дані існуючого складу.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWarehouseRequest request, CancellationToken cancellationToken)
    {
        bool updated = await warehouseService.UpdateAsync(id, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Видаляє склад за ідентифікатором.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        bool deleted = await warehouseService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
