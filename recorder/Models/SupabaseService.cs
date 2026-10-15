using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public sealed class SupabaseService
{
    private readonly HttpClient _httpClient;

    public SupabaseService(
        HttpClient httpClient,
        string supabaseUrl,
        string apiKey)
    {
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri(
            $"{supabaseUrl.TrimEnd('/')}/rest/v1/");

        _httpClient.DefaultRequestHeaders.Add(
            "apikey", apiKey);

        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/json"));
    }

    public async Task<List<T>> GetAsync<T>(
        string table,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"{table}?select=*",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(
            cancellationToken);

        return JsonSerializer.Deserialize<List<T>>(json)
            ?? [];
    }

    public async Task PostAsync<T>(
        string table,
        T data,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(data);

        using var content = new StringContent(
            json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PostAsync(
            table, content, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task PatchAsync<T>(
        string table,
        string filter,
        T data,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(data);

        using var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"{table}?{filter}")
        {
            Content = new StringContent(
                json, Encoding.UTF8, "application/json")
        };

        using var response = await _httpClient.SendAsync(
            request, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}