using System.Net.Http.Json;
using System.Text.Json;
using Distributed.Core.Extensions;

namespace Distributed.Core.Gateway;
public abstract class GatewayClient
{
    protected JsonSerializerOptions JsonFormat;
    protected HttpClient http = new();
    protected Endpoint endpoint;
    protected Guid? endpointId;

    protected bool Available => endpointId.HasValue;

    public GatewayClient(
        GatewayService gateway,
        string name,
        JsonSerializerOptions? format = null
    )
    {
        endpoint = gateway.GetEndpoint(name);
        JsonFormat = format
            ?? ServiceExtensions.ConfigureJsonOptions(new JsonSerializerOptions());
    }

    public async Task<Guid?> Initialize()
    {
        endpointId = await http.GetFromJsonAsync<Guid>(endpoint.Url);
        return endpointId;
    }

    protected async Task<Return?> Get<Return>(string method)
    {
        HttpResponseMessage response = await http.GetAsync(Path.Join(endpoint.Url, method));

        return response.IsSuccessStatusCode
            ? await ReadResult<Return?>(response)
            : default;
    }

    protected async Task<Return?> Post<Return, Data>(Data data, string method)
    {
        Return? result = await ReadResult<Return?>(
            await http.PostAsJsonAsync(
                Path.Join(endpoint.Url, method),
                data,
                JsonFormat
            )
        );

        return result;
    }

    protected async Task<Return?> Delete<Return, Data>(Data data, string method) =>
        await ReadResult<Return?>(
            await http.SendAsync(
                new()
                {
                    Content = JsonContent.Create(data, options: JsonFormat),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(
                        Path.Join(endpoint.Url, method)
                    )
                }
            )
        );

    protected async Task<Return?> ReadResult<Return>(HttpResponseMessage response)
    {
        try
        {
            Return? result = await response
                .Content
                .ReadFromJsonAsync<Return>(JsonFormat);

            return result ?? default;
        }
        catch
        {
            return default;
        }
    }
}