using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace PaymentGateway.WebApi.Tests;

public sealed class BankSimulatorFixture : IAsyncLifetime
{
    private const int BankSimulatorLoadTimeInSeconds = 60;

    public IContainer BankSimulatorContainer { get; }

    public BankSimulatorFixture()
    {
        BankSimulatorContainer = new ContainerBuilder()
            .WithName("bank_simulator")
            .WithImage("bbyars/mountebank:2.8.1")
            .WithPortBinding(2525, 2525)
            .WithPortBinding(8080, 8080)
            .WithCommand("--configfile", "/imposters/bank_simulator.ejs")
            .WithReuse(true)
            .WithBindMount(
                Path.GetFullPath("imposters"),
                "/imposters",
                AccessMode.ReadWrite)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request
                    .ForPort(2525)
                    .ForPath("/")
                    .UsingTls(false)
                    .ForResponseMessageMatching(r => Task.FromResult(r.IsSuccessStatusCode))
                ))
            .Build();
    }

    public async Task InitializeAsync()
    {
        if (!await IsBankSimulatorServiceAlreadyUpAsync())
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(BankSimulatorLoadTimeInSeconds));
            await BankSimulatorContainer.StartAsync(cts.Token);
        }
    }

    public async Task DisposeAsync()
    {
        await BankSimulatorContainer.StopAsync();
    }

    private async Task<bool> IsBankSimulatorServiceAlreadyUpAsync()
    {
        try
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://localhost:2525");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

[CollectionDefinition("BankSimulator collection")]
public sealed class BankSimulatorCollection : ICollectionFixture<BankSimulatorFixture> {}