using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ints.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;

        public ApiClient()
        {
            _http = new HttpClient();

            
            _http.BaseAddress = new Uri("https://localhost:7204/");
        }

        public HttpClient Http => _http;

        public void SetToken(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _http.DefaultRequestHeaders.Authorization = null;
                return;
            }

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
