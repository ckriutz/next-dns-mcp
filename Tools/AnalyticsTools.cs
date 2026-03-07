using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace NextDnsMcp.Tools;

[McpServerToolType]
public static class AnalyticsTools
{
    [McpServerTool(Name = "get_analytics_status")]
    [Description("Get query count analytics broken down by status (default, blocked, allowed). Optionally specify a time range like '-24h', '-7d', or '-30d'.")]
    public static async Task<string> GetAnalyticsStatus(
        NextDnsClient client,
        [Description("Time range to query from, e.g. '-24h', '-7d', '-30d'. Defaults to last 24 hours.")] string? from = "-24h",
        CancellationToken ct = default)
    {
        var data = await client.GetAnalyticsStatusAsync(from, ct);
        return FormatJson(data);
    }

    [McpServerTool(Name = "get_top_domains")]
    [Description("Get the top queried domains. Optionally filter by status and limit the number of results.")]
    public static async Task<string> GetTopDomains(
        NextDnsClient client,
        [Description("Filter by status: 'default', 'blocked', or 'allowed'. Leave empty for all.")] string? status = null,
        [Description("Maximum number of domains to return. Defaults to 10.")] int limit = 10,
        [Description("Time range to query from, e.g. '-24h', '-7d'. Defaults to last 24 hours.")] string? from = "-24h",
        CancellationToken ct = default)
    {
        var data = await client.GetTopDomainsAsync(status, limit, from, ct);
        return FormatJson(data);
    }

    [McpServerTool(Name = "get_devices")]
    [Description("Get the list of devices that have made DNS queries, along with their query counts.")]
    public static async Task<string> GetDevices(
        NextDnsClient client,
        [Description("Time range to query from, e.g. '-24h', '-7d'. Defaults to last 24 hours.")] string? from = "-24h",
        CancellationToken ct = default)
    {
        var data = await client.GetDevicesAsync(from, ct);
        return FormatJson(data);
    }

    private static string FormatJson(JsonElement data)
    {
        return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
    }
}
