using System.Net.Http;
using System.Net.Http.Json;
using MiProyecto.Web.Models;
using Microsoft.JSInterop;
using MiProyecto.Application.Interfaces;

namespace MiProyecto.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public AuthService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new { username, password });
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result == null || string.IsNullOrEmpty(result.Token))
                return null; // Login fallido

            await _js.InvokeVoidAsync("localStorage.setItem", "token", result.Token);

            return result.Token;
        }
    }
}