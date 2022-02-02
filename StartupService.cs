namespace Redis.OM.Skeleton;

public class StartupService : IHostedService
{
    private readonly RedisConnectionProvider _provider;
    public StartupService(RedisConnectionProvider provider)
    {
        _provider = provider;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var info = (await _provider.Connection.ExecuteAsync("FT._LIST")).ToArray().Select(x=>x.ToString());
        if (info.All(x => x != "person-idx"))
        {
            await _provider.Connection.CreateIndexAsync(typeof(Person));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}