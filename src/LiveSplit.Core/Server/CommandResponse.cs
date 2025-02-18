using System.Collections.Generic;
using System.Text;

namespace LiveSplit.Server;

public class CommandResponse
{
    public bool Success { get; set; } = true;
    public string State { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public string RequestId { get; set; } = "";
    public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

    public string GetResponse()
    {
        var builder = new StringBuilder();

        builder.AppendLine(">>");
        builder.AppendLine($"RequestId: {RequestId}");
        builder.AppendLine($"Success: {Success}");
        builder.AppendLine($"State: {State}");
        builder.AppendLine($"Command: {Command}");

        foreach (KeyValuePair<string, string> pair in Data)
        {
            string val = pair.Value.Replace("\n", "\\n").Replace("\r", "\\r");
            builder.AppendLine($"{pair.Key}: {val}");
        }

        builder.AppendLine("<<");
        return builder.ToString();
    }

    public static CommandResponse CreateFailed(string cmd, string requestId, string message, string state = "Failed")
    {
        return new CommandResponse()
        {
            Command = cmd,
            RequestId = requestId,
            State = state,
            Success = false,
            Data = { { "message", message } }
        };
    }

    public static CommandResponse CreateSuccess(string cmd, string requestId)
    {
        return new CommandResponse()
        {
            Command = cmd,
            RequestId = requestId,
            State = "Success",
            Success = true,
        };
    }
}
