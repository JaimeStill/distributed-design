using Microsoft.AspNetCore.SignalR.Client;

namespace Distributed.Core.Services;
public class EventClientStatus
{
    public string? ConnectionId { get; set; }
    public HubConnectionState State { get; set; }

    public EventClientStatus(string? connectionId, HubConnectionState state)
    {
        ConnectionId = connectionId;
        State = state;
    }
}