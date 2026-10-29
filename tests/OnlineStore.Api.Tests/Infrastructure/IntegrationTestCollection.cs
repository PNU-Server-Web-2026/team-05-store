using OnlineStore.Api.Tests.Infrastructure;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace OnlineStore.Api.Tests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "Integration";
}
