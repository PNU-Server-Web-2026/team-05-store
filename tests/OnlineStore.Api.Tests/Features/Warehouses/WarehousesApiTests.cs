using System.Net;
using System.Net.Http.Json;
using OnlineStore.Api.Features.Warehouses;

namespace OnlineStore.Api.Tests.Features.Warehouses;

public class WarehousesApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ReturnsOkAndList()
    {
        var response = await _client.GetAsync("/api/warehouses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var warehouses = await response.Content.ReadFromJsonAsync<IReadOnlyList<WarehouseResponse>>();
        Assert.NotNull(warehouses);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenWarehouseDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/warehouses/{nonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocationHeaderAndBody()
    {
        var request = new CreateWarehouseRequest(
            Name: "Центральний склад №1",
            City: "Івано-Франківськ",
            Address: "вул. Промислова, 10",
            Priority: 10);

        var response = await _client.PostAsJsonAsync("/api/warehouses", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<WarehouseResponse>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(request.Name, created.Name);
        Assert.Equal(request.City, created.City);
        Assert.Equal(request.Address, created.Address);
        Assert.Equal(request.Priority, created.Priority);

        // Перевіряємо, що Location заголовок містить правильний шлях до нового ресурсу
        Assert.EndsWith($"/api/warehouses/{created.Id}", response.Headers.Location.ToString());
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenWarehouseExists()
    {
        var createRequest = new CreateWarehouseRequest(
            Name: "Склад Західний",
            City: "Львів",
            Address: "вул. Городоцька, 200",
            Priority: 20);

        var createResponse = await _client.PostAsJsonAsync("/api/warehouses", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<WarehouseResponse>();
        Assert.NotNull(created);

        var response = await _client.GetAsync($"/api/warehouses/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var warehouse = await response.Content.ReadFromJsonAsync<WarehouseResponse>();
        Assert.NotNull(warehouse);
        Assert.Equal(created.Id, warehouse.Id);
        Assert.Equal(createRequest.Name, warehouse.Name);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenWarehouseExists()
    {
        var createRequest = new CreateWarehouseRequest(
            Name: "Склад Східний",
            City: "Харків",
            Address: "вул. Польова, 5",
            Priority: 30);

        var createResponse = await _client.PostAsJsonAsync("/api/warehouses", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<WarehouseResponse>();
        Assert.NotNull(created);

        var updateRequest = new UpdateWarehouseRequest(
            Name: "Склад Східний Оновлений",
            City: "Харків",
            Address: "вул. Польова, 5-А",
            Priority: 15);

        var updateResponse = await _client.PutAsJsonAsync($"/api/warehouses/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        // Перевіряємо, що дані справді оновилися
        var getResponse = await _client.GetAsync($"/api/warehouses/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<WarehouseResponse>();
        Assert.NotNull(updated);
        Assert.Equal(updateRequest.Name, updated.Name);
        Assert.Equal(updateRequest.Priority, updated.Priority);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenWarehouseDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new UpdateWarehouseRequest(
            Name: "Фантомний склад",
            City: "Київ",
            Address: "вул. Хрещатик, 1",
            Priority: 1);

        var response = await _client.PutAsJsonAsync($"/api/warehouses/{nonExistentId}", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenWarehouseExists()
    {
        var createRequest = new CreateWarehouseRequest(
            Name: "Тимчасовий склад",
            City: "Одеса",
            Address: "вул. Приморська, 12",
            Priority: 50);

        var createResponse = await _client.PostAsJsonAsync("/api/warehouses", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<WarehouseResponse>();
        Assert.NotNull(created);

        var deleteResponse = await _client.DeleteAsync($"/api/warehouses/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Перевіряємо, що після видалення склад більше не знаходиться (404)
        var getResponse = await _client.GetAsync($"/api/warehouses/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenWarehouseDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var response = await _client.DeleteAsync($"/api/warehouses/{nonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
