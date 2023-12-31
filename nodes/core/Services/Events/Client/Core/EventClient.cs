using System.Diagnostics.CodeAnalysis;
using Distributed.Core.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Distributed.Core.Services;
public abstract class EventClient<T> : IEventClient<T>
{
    protected readonly HubConnection connection;
    protected readonly string endpoint;

    public EventClientStatus Status => new(connection.ConnectionId, connection.State);

    public EventAction OnPing { get; protected set; }
    public EventAction OnSync { get; protected set; }

    public EventClient(string endpoint)
    {
        this.endpoint = endpoint;

        Console.WriteLine($"Building Sync connection at {endpoint}");
        connection = BuildHubConnection(endpoint);

        Initialize();
    }

    [MemberNotNull(
        nameof(OnPing),
        nameof(OnSync)
    )]
    void Initialize()
    {
        connection.Closed += async (error) =>
        {
            await Task.Delay(5000);
            await Connect();
        };

        OnPing = new("ping", connection);
        OnSync = new("sync", connection);
        OnPing.Set(() => Console.WriteLine("Pong"));
    }

    public async Task Ping()
    {
        if (Status.State == HubConnectionState.Connected)
            await connection.InvokeAsync("Ping");
    }

    public async Task Connect(CancellationToken token = new())
    {
        if (connection.State != HubConnectionState.Connected)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine($"Connecting to {endpoint}");
                    await connection.StartAsync(token);
                    Console.WriteLine($"Now listening on {endpoint}");
                    Console.WriteLine($"{Status.State} - {Status.ConnectionId}");
                    return;
                }
                catch when (token.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to {endpoint}");
                    Console.WriteLine(ex.Message);
                    await Task.Delay(5000, token);
                }
            }
        }
    }

    protected async Task ExecuteServiceAction<Service>(IServiceProvider provider, Func<Service, Task> action)
    where Service : notnull
    {
        using IServiceScope scope = provider.CreateScope();
        Service svc = scope.ServiceProvider.GetRequiredService<Service>();
        await action(svc);
    }

    protected virtual HubConnection BuildHubConnection(string endpoint) =>
        new HubConnectionBuilder()
            .ConfigureJsonFormat()
            .WithUrl(endpoint)
            .ConfigureLogging(logging =>
            {
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .WithAutomaticReconnect()
            .Build();

    public async ValueTask DisposeAsync()
    {
        DisposeEvents();
        
        await DisposeConnection()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected virtual void DisposeEvents()
    {
        OnPing.Dispose();
        OnSync.Dispose();
    }

    protected virtual async ValueTask DisposeConnection()
    {
        if (connection is not null)
        {
            await connection
                .DisposeAsync()
                .ConfigureAwait(false);
        }
    }
}