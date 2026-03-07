using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NextDnsMcp;

public class NextDnsClient
{
    private readonly HttpClient _http;
    private readonly string _profileId;

    public NextDnsClient(HttpClient http, string profileId)
    {
        _http = http;
        _profileId = profileId;
    }

    public string ProfileId => _profileId;

    // --- Profile ---

    public async Task<JsonElement> GetProfileAsync(CancellationToken ct = default)
    {
        var resp = await _http.GetFromJsonAsync<ApiResponse>($"profiles/{_profileId}", ct);
        return resp!.Data;
    }

    // --- Analytics ---

    public async Task<JsonElement> GetAnalyticsStatusAsync(string? from = null, CancellationToken ct = default)
    {
        var url = BuildAnalyticsUrl("status", from);
        var resp = await _http.GetFromJsonAsync<ApiResponse>(url, ct);
        return resp!.Data;
    }

    public async Task<JsonElement> GetTopDomainsAsync(string? status = null, int limit = 10, string? from = null, CancellationToken ct = default)
    {
        var url = BuildAnalyticsUrl("domains", from);
        if (!string.IsNullOrEmpty(status)) url += $"&status={status}";
        url += $"&limit={limit}";
        var resp = await _http.GetFromJsonAsync<ApiResponse>(url, ct);
        return resp!.Data;
    }

    public async Task<JsonElement> GetDevicesAsync(string? from = null, CancellationToken ct = default)
    {
        var url = BuildAnalyticsUrl("devices", from);
        var resp = await _http.GetFromJsonAsync<ApiResponse>(url, ct);
        return resp!.Data;
    }

    // --- Parental Controls ---

    public async Task<List<ParentalControlService>> GetParentalControlServicesAsync(CancellationToken ct = default)
    {
        var resp = await _http.GetFromJsonAsync<ApiResponse<List<ParentalControlService>>>(
            $"profiles/{_profileId}/parentalControl/services", ct);
        return resp!.Data;
    }

    public async Task SetParentalControlServiceAsync(string serviceId, bool active, CancellationToken ct = default)
    {
        var url = $"profiles/{_profileId}/parentalControl/services/{serviceId}";
        var content = JsonContent.Create(new { active });
        var resp = await _http.PatchAsync(url, content, ct);
        resp.EnsureSuccessStatusCode();
    }

    // --- Helpers ---

    private string BuildAnalyticsUrl(string endpoint, string? from)
    {
        var url = $"profiles/{_profileId}/analytics/{endpoint}?";
        if (!string.IsNullOrEmpty(from)) url += $"from={Uri.EscapeDataString(from)}&";
        return url.TrimEnd('&', '?');
    }
}

public class ApiResponse
{
    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}

public class ApiResponse<T>
{
    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;
}

public class ParentalControlService
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("active")]
    public bool Active { get; set; }
}
