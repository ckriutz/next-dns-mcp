using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace NextDnsMcp.Tools;

[McpServerToolType]
public static class ConnectionTools
{
    [McpServerTool(Name = "check_connection")]
    [Description("Verify the connection to NextDNS by retrieving the profile information. Returns the profile name and ID to confirm the API key and profile are valid.")]
    public static async Task<string> CheckConnection(NextDnsClient client, CancellationToken ct)
    {
        var profile = await client.GetProfileAsync(ct);

        var name = profile.TryGetProperty("name", out var n) ? n.GetString() : "Unknown";
        return $"✅ Connected to NextDNS profile \"{name}\" (ID: {client.ProfileId})";
    }
}
