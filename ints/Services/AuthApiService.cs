using ints.DtosJsonRead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ints.Services
{
    public class AuthApiService
    {
        private readonly ApiClient _api;

        public AuthApiService(ApiClient api)
        {
            _api = api;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var req = new LoginRequest { Username = username, Password = password };

            // ВАЖНО: у тебя endpoint в API называется "/login" (без api/auth)
            var resp = await _api.Http.PostAsJsonAsync("login", req);
            if (!resp.IsSuccessStatusCode)
                return null;

            var data = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            if (data == null || string.IsNullOrWhiteSpace(data.access_token))
                return null;

            _api.SetToken(data.access_token);
            return data.access_token;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            var req = new RegisterRequest { Username = username, Password = password };

            // ВАЖНО: endpoint "/register"
            var resp = await _api.Http.PostAsJsonAsync("register", req);
            return resp.IsSuccessStatusCode;
        }
    }
}

