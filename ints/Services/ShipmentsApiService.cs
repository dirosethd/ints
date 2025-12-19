using ints.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ints.Services
{
    public class ShipmentsApiService
    {
        private readonly ApiClient _api;

        public ShipmentsApiService(ApiClient api)
        {
            _api = api;
        }

        public async Task<List<Shipment>> GetAllAsync()
        {

            var list = await _api.Http.GetFromJsonAsync<List<Shipment>>("api/shipments");
            return list ?? new List<Shipment>();
        }
    }
}
