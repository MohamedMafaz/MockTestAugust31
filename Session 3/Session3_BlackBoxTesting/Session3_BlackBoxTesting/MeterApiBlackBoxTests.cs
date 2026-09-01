using Microsoft.Testing.Platform.Extensions.Messages;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Session3_BlackBoxTesting
{
    public class Tests
    {
        public string token = "";

        public async Task LoadTokwen()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7281/api/auth/login");
            request.Headers.Add("accept", "*/*");
            var content = new StringContent("{\r\n  \"username\": \"mafaz\",\r\n  \"password\": \"1234\"\r\n}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            token = (await response.Content.ReadAsStringAsync());
        }


        public class MeterDTO
        {
            public int meterId { get; set; }
            public string meterSerialNumber { get; set; }
            public int facilityId { get; set; }
            public string locationZone { get; set; }
            public float maxVoltageCapacity { get; set; }
            public float baseTariffRate { get; set; }
            public string installationDate { get; set; }
            public bool isActive { get; set; }
            public bool isIndustrial { get; set; }
            public string description { get; set; }
        }


        [TestCase(1, 10, 1, true, 200)]
        [TestCase(100, 100, 1, true, 200)]
        [TestCase(1, 10, 2, false, 200)]
        [TestCase(2, 1, 1, true, 200)]
        [TestCase(2, 1, 1, true, 401)]

        public async Task GetAllTesting(int page, int pagesize, int facilityid, bool isactive, HttpStatusCode expectedcode)
        {
            await LoadTokwen();

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7281/api/meters?page={page}&pageSize={pagesize}&facilityId={facilityid}&isActive={isactive}");
            request.Headers.Add("accept", "*/*");
            if(expectedcode != HttpStatusCode.Unauthorized)
            {
                request.Headers.Add("Authorization", $"Bearer {token}");

            }
            var response = await client.SendAsync(request);


            if (response.IsSuccessStatusCode)
            {
                var list = JsonSerializer.Deserialize<List<MeterDTO>>(await response.Content.ReadAsStringAsync());
                if (page == 100)
                {
                    Assert.That(list.Count() == 0);
                }

            }

            Assert.That(response.StatusCode == expectedcode);
        }


        [TestCase(1, 404)]
        [TestCase(1001, 401)]
        [TestCase(1001,200)]
        [TestCase(10070,404)]

        public async Task GetSingleTesting(int id, HttpStatusCode expectedCode)
        {
            await LoadTokwen();

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7281/api/meters/{id}");
            request.Headers.Add("accept", "*/*");
            if (expectedCode != HttpStatusCode.Unauthorized)
            {
                request.Headers.Add("Authorization", $"Bearer {token}");

            }
            var response = await client.SendAsync(request);

            Assert.That(response.StatusCode == expectedCode);
        }




        [TestCase(10,201)]
        [TestCase(10, 401)]
        [TestCase(10,403)]
        [TestCase(-10,400)]
        [TestCase(10,409)]
        [TestCase(10,201)]
       
        public async Task PostTesting(int voltage, HttpStatusCode expectedCode)
        {
            await LoadTokwen();
            var meter = new MeterDTO
            {
                meterSerialNumber = Guid.NewGuid().ToString(),
                facilityId = 1,
                locationZone = "",
                maxVoltageCapacity = voltage,
                baseTariffRate = 0,
                installationDate = DateTime.Now.ToString("yyyy-MM-dd"),
                isActive = true,
                isIndustrial = true,
                description = "jhdsfb"
            };

            var code = await PostRequest(meter, expectedCode);

            if(expectedCode == HttpStatusCode.Conflict)
            {
                var newcode = await PostRequest(meter, expectedCode);

                Assert.That(newcode == expectedCode);
            }
            else
            {

                if (expectedCode == HttpStatusCode.Forbidden)
                {
                    await LoadUserToken();

                    var codes = await PostRequest(meter, expectedCode);
                    Assert.That(codes == expectedCode);
                }
                else
                {
                    Assert.That(code == expectedCode);

                }
            }

        }

        private async Task<HttpStatusCode> PostRequest(MeterDTO meter, HttpStatusCode expectedCode)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7281/api/meters");
            request.Headers.Add("accept", "*/*");

            if(expectedCode != HttpStatusCode.Unauthorized)
            {
                request.Headers.Add("Authorization", $"Bearer {token}");

            }
            var content = new StringContent(JsonSerializer.Serialize(meter), null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            return response.StatusCode;
        }

        private async Task LoadUserToken()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7281/api/auth/login");
            request.Headers.Add("accept", "*/*");
            var content = new StringContent("{\r\n  \"username\": \"another\",\r\n  \"password\": \"1234\"\r\n}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            token = (await response.Content.ReadAsStringAsync());
        }


        [TestCase(1001,200)]
        [TestCase(1001,400)]
        [TestCase(100000,404)]
        [TestCase(1002, 200)]
        [TestCase(100,401)]
        [TestCase(100,403)]
        public async Task PutEndpointTesting(int id, HttpStatusCode expectedCode)
        {
            await LoadTokwen();

            if(expectedCode == HttpStatusCode.Forbidden)
            {
                await LoadUserToken();
            }

            var meter = new MeterDTO
            {
                meterId = id,
                meterSerialNumber = Guid.NewGuid().ToString(),
                facilityId = 1,
                locationZone = "",
                maxVoltageCapacity = 10,
                baseTariffRate = 0,
                installationDate = DateTime.Now.ToString("yyyy-MM-dd"),
                isActive = true,
                isIndustrial = true,
                description = "jhdsfb"
            };

            if(expectedCode == HttpStatusCode.BadRequest)
            {
                meter.meterId = id + 1;
            }


            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:7281/api/meters/{id}");
            request.Headers.Add("accept", "*/*");

            if (expectedCode != HttpStatusCode.Unauthorized)
            {
                request.Headers.Add("Authorization", $"Bearer {token}");

            }
            var content = new StringContent(JsonSerializer.Serialize(meter), null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);

            Assert.That(response.StatusCode == expectedCode);
        }

        [TestCase(1015,200)]
        [TestCase(101500,404)]
        [TestCase(1001,409)]
        [TestCase(1015,401)]
        [TestCase(1015,403)]
        [TestCase(1015,200)]
        public async Task DeleteEndpointTestings(int id, HttpStatusCode expectedCode)
        {
            await LoadTokwen();

            if (expectedCode == HttpStatusCode.Forbidden)
            {
                await LoadUserToken();
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7281/api/meters/{id}");
            request.Headers.Add("accept", "*/*");

            if (expectedCode != HttpStatusCode.Unauthorized)
            {
                request.Headers.Add("Authorization", $"Bearer {token}");

            }
            var content = new StringContent("", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);

            Assert.That(response.StatusCode == expectedCode);
        }
    }
}
