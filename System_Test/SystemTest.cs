using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SquareDMS.DataLibrary;
using SquareDMS.DataLibrary.ProcedureResults;
using SquareDMS.RestEndpoint.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SquareDMS.System_Test
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
        /// Does a POST CRUD Operation (ManipulationResult)
        /// </summary>
        /// <returns>Tuple of HttpStatusCode and ManipulationResult or (null, null) in case of an error</returns>
        protected async Task<(HttpStatusCode?, ManipulationResult)> PostCRUDAsync(string url, object entity, string jwt)
        {
            var serializedEntity = JsonConvert.SerializeObject(entity);
            var content = new StringContent(serializedEntity, Encoding.UTF8, "application/json");

            TestClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", jwt);

            var httpResponse = await TestClient.PostAsync(url, content);

            // delete auth header after request was made
            TestClient.DefaultRequestHeaders.Authorization = null;

            var serializedManipulationResult = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                return (httpResponse.StatusCode, DeserializeManipulationResult(serializedManipulationResult));
            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }

        /// <summary>
        /// Does a LOGIN Operation (Auth::Response)
        /// </summary>
        protected async Task<(HttpStatusCode?, Response)> PostLoginAsync(string url, object entity)
        {
            var serializedEntity = JsonConvert.SerializeObject(entity);
            var content = new StringContent(serializedEntity, Encoding.UTF8, "application/json");

            var httpResponse = await TestClient.PostAsync(url, content);

            var serializedResponse = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                return (httpResponse.StatusCode, JsonConvert.DeserializeObject<Response>(serializedResponse));
            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }

        /// <summary>
        /// Deserializes a ManipulationResult
        /// </summary>
        private ManipulationResult DeserializeManipulationResult(string serializedManipulationResult)
        {
            var parsedInput = JObject.Parse(serializedManipulationResult);

            // extract operations from the serialized ManipulationResult
            var operationsSerialized = parsedInput["operations"].Children().ToList();

            var operations = new List<Operation>();
            operationsSerialized.ForEach(op => operations.Add(op.ToObject<Operation>()));

            var errorCodeSerialized = parsedInput["errorCode"];
            var errorCode = errorCodeSerialized.ToObject<int?>();

            return new ManipulationResult(errorCode.GetValueOrDefault(), operations.ToArray());
        }
    }
}
