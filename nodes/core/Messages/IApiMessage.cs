namespace Distributed.Core.Messages;
public interface IApiMessage
{
    string Message { get; set; }
    bool Error { get; set; }
    bool HasData { get; }
}