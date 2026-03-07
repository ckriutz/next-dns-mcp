using System.ComponentModel;
using ModelContextProtocol.Server;

namespace NextDnsMcp.Tools;

[McpServerToolType]
public static class ParentalControlTools
{
    [McpServerTool(Name = "get_youtube_status")]
    [Description("Check whether YouTube is currently blocked or unblocked in the parental controls for this NextDNS profile.")]
    public static async Task<string> GetYouTubeStatus(NextDnsClient client, CancellationToken ct)
    {
        var services = await client.GetParentalControlServicesAsync(ct);
        var youtube = services.Find(s => s.Id.Equals("youtube", StringComparison.OrdinalIgnoreCase));

        if (youtube is null)
            return "YouTube is not configured in the parental control services. It is effectively **unblocked**.";

        return youtube.Active
            ? "🚫 YouTube is currently **blocked**."
            : "✅ YouTube is currently **unblocked**.";
    }

    [McpServerTool(Name = "set_youtube_blocked")]
    [Description("Block or unblock YouTube in the parental controls. Set blocked=true to block YouTube, or blocked=false to unblock it.")]
    public static async Task<string> SetYouTubeBlocked(
        NextDnsClient client,
        [Description("Set to true to block YouTube, or false to unblock it.")] bool blocked,
        CancellationToken ct)
    {
        await client.SetParentalControlServiceAsync("youtube", blocked, ct);

        return blocked
            ? "🚫 YouTube has been **blocked**."
            : "✅ YouTube has been **unblocked**.";
    }
}
