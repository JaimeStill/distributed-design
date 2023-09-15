namespace Distributed.Core.Messages;
public class ValidationMessage
{
    List<string> Messages { get; set; }
    public string Message => string.Join('\n', Messages);
    public bool IsValid => Messages.Count < 1;

    public ValidationMessage()
    {
        Messages = new ();
    }

    public ValidationMessage(string message)
    {
        Messages = new() { message };
    }

    public ValidationMessage(ValidationMessage parent)
    {
        Messages = parent.Messages;
    }

    public ValidationMessage(ValidationMessage a, ValidationMessage b)
    {
        Messages = a.Messages
            .Union(b.Messages)
            .ToList();
    }

    public void AddMessage(string message) =>
        Messages.Add(message);
}