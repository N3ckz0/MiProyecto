public class SaludoApiService
{
    private readonly HttpClient _http;

    public SaludoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> ObtenerSaludoAsync()
    {
        return await _http.GetStringAsync("api/saludo");
    }
}
