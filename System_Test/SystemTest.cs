using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SquareDMS.RestEndpoint.Authentication;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace System_Test
{
    public abstract class SystemTest
    {
        /// <summary>
        /// Creates intstance of the httpClient and the web app
        /// </summary>
        public SystemTest()
        {
            var appFactory = new WebApplicationFactory<SquareDMS.RestEndpoint.Startup>();
            TestClient = appFactory.CreateClient();
        }

        /// <summary>
        /// HttpClient used for testing the rest api
        /// </summary>
        protected HttpClient TestClient { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Response or null if JSON is invalid</returns>
        public async Task<Response> GetLoginResponseAsync(Request request)
        {
            var response = await PostAsJsonAsync("api/v1/users/login", request);

            var loginResponseSerialized = await response.Content.ReadAsStringAsync();

            try
            {
                return JsonConvert.DeserializeObject<Response>(loginResponseSerialized);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> PostAsJsonAsync(string url, object entity)
        {
            var serializedEntity = JsonConvert.SerializeObject(entity);
            var content = new StringContent(serializedEntity, Encoding.UTF8, "application/json");

            return await TestClient.PostAsync(url, content);
        }
    }
}
