using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HuaHuaCaoCao.Client
{
    public class PlantClient
    {
        HttpClient client;
        HuaHuaCaoCaoOptions settings;
        ILogger<PlantClient> logger;
        JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public PlantClient(HttpClient httpClient, IOptions<HuaHuaCaoCaoOptions> settingsOption, ILogger<PlantClient> logger)
        {
            this.client = httpClient;
            this.logger = logger;
            this.settings = settingsOption.Value;
            this.client.BaseAddress = settings.ApiEndpoint;
            client.DefaultRequestHeaders.Add("X-Hhcc-Region", settings.Region);
            client.DefaultRequestHeaders.Add("X-Hhcc-Token", "");
            client.DefaultRequestHeaders.Add("X-Real-Ip", "192.168.0.1");
        }

        public class AuthenticationRequestData
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        public class GetDetailsRequestData
        {
            public string? PID { get; set; }
            public string? Lang { get; set; }
        }

        public class SearchRequestData
        {
            public string? Alias { get; set; }
            public int Count { get; set; }
            public string? Lang { get; set; }
            public int Limit { get; set; }
        }

        public class Response<TData> where TData : class
        {
            public TData Data { get; set; } = null!;
            public string? Description { get; set; }
            public int? Status { get; set; }
        }

        public class AuthenticationResponseData
        {
            public bool IsCreate { get; set; }
            public bool IsModifyNick { get; set; }
            public string? Migrate { get; set; }
            public string? ThirdNick { get; set; }
            public string? Token { get; set; }
        }

        public class RequestExtra
        {
            public string? AppChannel { get; set; }
            public string? Country { get; set; }
            public string? Lang { get; set; }
            public string? Model { get; set; }
            public string? Phone { get; set; }
            public List<object?> Position { get; set; } = null!;
            public string? Version { get; set; }
            public int? Zone { get; set; }
        }

        public class PlantItem
        {
            public string PID { get; set; } = null!;
            public string Alias { get; set; } = null!;
            public string? DisplayPID { get; set; }
            public string Image { get; set; } = null!;
        }

        public class Basic
        {
            public string? Blooming { get; set; }
            public string? Category { get; set; }
            public string? Color { get; set; }
            public string? FloralLanguage { get; set; }
            public string? Origin { get; set; }
            public string? Production { get; set; }
        }

        public class Maintenance
        {
            public string? Fertilization { get; set; }
            public string? Pruning { get; set; }
            public string? Size { get; set; }
            public string? Soil { get; set; }
            public string? Sunlight { get; set; }
            public string? Watering { get; set; }
        }

        public class Parameter
        {
            public int MinEnvHumid { get; set; }
            public int MaxEnvHumid { get; set; }

            public int MinLightLux { get; set; }
            public int MaxLightLux { get; set; }

            public int MinLightMmol { get; set; }
            public int MaxLightMmol { get; set; }

            public int MinSoilEc { get; set; }
            public int MaxSoilEc { get; set; }

            public int MinSoilMoist { get; set; }
            public int MaxSoilMoist { get; set; }

            public int MinTemp { get; set; }
            public int MaxTemp { get; set; }
        }

        public class GetDetailsReponse
        {
            public Basic? Basic { get; set; }
            public string? DisplayPID { get; set; }
            public string? Image { get; set; }
            public Maintenance? Maintenance { get; set; }
            public Parameter? Parameter { get; set; }
            public string? PID { get; set; }
        }

        public class Request<TData> where TData : class
        {
            public TData? Data { get; set; }
            public RequestExtra? Extra { get; set; }
            public string? Method { get; set; }
            public string? Path { get; set; }
            public string? Service { get; set; }
        }

        public async Task AuthenticateAsync()
        {
            var response = await PostAsync<AuthenticationRequestData, AuthenticationResponseData>(new AuthenticationRequestData()
            {
                Email = settings.Email,
                Password = settings.Password
            }, "auth", "/token/email");
            if (response.Status != 100)
            {
                throw new Exception(response.Description);
            }
            client.DefaultRequestHeaders.Remove("X-Hhcc-Token");
            client.DefaultRequestHeaders.Add("X-Hhcc-Token", response.Data.Token);
        }

        public async Task<IEnumerable<PlantItem>> SearchAsync(string alias)
        {
            logger.LogTrace("SearchAsync");
            List<PlantItem> plants = new List<PlantItem>();
            var done = false;
            do
            {
                var result = await PostAsync<SearchRequestData, List<PlantItem>>(new SearchRequestData() { Alias = alias, Lang = "en", Count = plants.Count, Limit = 20 }, "pkb", "/plant/alias");
                done = result.Status != 100;
                if (!done)
                {
                    plants.AddRange(result.Data);
                    logger.LogTrace("Got {0} plants", result.Data.Count);
                    await Task.Delay(500);
                }
            }
            while (!done);
            return plants;
        }

        public async Task<GetDetailsReponse> GetDetailsAsync(string pid)
        {
            var result = await PostAsync<GetDetailsRequestData, GetDetailsReponse>(new GetDetailsRequestData() { PID = pid, Lang = "en" }, "pkb", "/plant/detail");
            if (result.Status != 100)
            {
                throw new Exception("Failed to get details");
            }
            return result.Data;
        }



        private async Task<Response<TResponse>> PostAsync<TRequestData, TResponse>(TRequestData requestData, string service, string path) where TRequestData : class where TResponse : class
        {
            var request = new Request<TRequestData>()
            {
                Data = requestData,
                Extra = new RequestExtra()
                {
                    AppChannel = "google",
                    Country = "CN",
                    Lang = "en",
                    Model = "",
                    Phone = "samsung_SM-G955F_26",
                    Position = new List<object?>() { null, null },
                    Version = "AS_3044_5.4.6",
                    Zone = 1
                },
                Method = "GET",
                Path = path,
                Service = service
            };
            var response = await client.PostAsync("", new StringContent(JsonConvert.SerializeObject(request, serializerSettings), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<TResponse>>(content, serializerSettings);
            if (result == null)
            {
                throw new InvalidOperationException("HuaHuaCaoCao endpoint returned null, this is not expected!");
            }
            if (result.Data == null)
            {
                throw new InvalidOperationException("HuaHuaCaoCao endpoint returned null, this is not expected!");
            }
            return result;
        }
    }
}
