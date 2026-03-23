using System.Net.Http;
using System.Net.Http.Json;
using MiProyecto.Web.Models;
using Microsoft.JSInterop;

namespace MiProyecto.Web.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly AuthHttpService _authHttp;
        private readonly IJSRuntime _js;

        public AuthService(HttpClient http, AuthHttpService authHttp, IJSRuntime js)
        {
            _http = http;
            _authHttp = authHttp;
            _js = js;
        }

        public async Task Login(string username, string password)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new
            {
                username,
                password
            });

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                throw new Exception("Error al obtener el token del login");
            }

            // Guardar token
            await _js.InvokeVoidAsync("localStorage.setItem", "token", result.Token);
        }
    }
}