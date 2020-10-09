using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SquareDMS.DataLibrary;
using SquareDMS.DataLibrary.Entities;
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
    public class TestHttpClient
    {
        /// <summary>
        /// Creates intstance of the httpClient and the web app
        /// </summary>
        public TestHttpClient()
        {
            var appFactory = new WebApplicationFactory<SquareDMS.RestEndpoint.Startup>();
            TestClient = appFactory.CreateClient();
        }

        /// <summary>
        /// HttpClient used for testing the rest api
        /// </summary>
        private HttpClient TestClient { get; }

        /// <summary>
        /// Does a POST Operation (ManipulationResult)
        /// </summary>
        /// <returns>Tuple of HttpStatusCode and ManipulationResult or (null, null) in case of an error</returns>
        public async Task<(HttpStatusCode?, ManipulationResult<T>)> PostAsync<T>(string url, T entity,
            string jwt = null) where T : IDataTransferObject
        {
            var serializedEntity = JsonConvert.SerializeObject(entity);
            var content = new StringContent(serializedEntity, Encoding.UTF8, "application/json");

            // if a jwt is supplied we set the header
            if (jwt != null)
            {
                TestClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", jwt);
            }

            var httpResponse = await TestClient.PostAsync(url, content);

            if (jwt != null)
            {
                // delete auth header after request was made
                TestClient.DefaultRequestHeaders.Authorization = null;
            }

            var serializedManipulationResult = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                return (httpResponse.StatusCode, DeserializeManipulationResult<T>(serializedManipulationResult));
            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }

        /// <summary>
        /// Does a GET Operation (RetrievalResult)
        /// </summary>
        public async Task<(HttpStatusCode?, RetrievalResult<T>)> GetAsync<T>(string url, string jwt) where T : IDataTransferObject
        {
            TestClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", jwt);

            var httpResponse = await TestClient.GetAsync(url);

            // delete auth header after request was made
            TestClient.DefaultRequestHeaders.Authorization = null;

            var serializedRetrievalResult = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                return (httpResponse.StatusCode, DeserializeRetrievalResult<T>(serializedRetrievalResult));
            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }

        /// <summary>
        /// Does a LOGIN Operation (Auth::Response)
        /// </summary>
        public async Task<(HttpStatusCode?, Response)> PostLoginAsync(Request entity)
        {
            var serializedEntity = JsonConvert.SerializeObject(entity);
            var content = new StringContent(serializedEntity, Encoding.UTF8, "application/json");

            var httpResponse = await TestClient.PostAsync("api/v1/users/login", content);

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
        /// 
        /// </summary>
        public async Task<(HttpStatusCode?, DocumentVersion)> PostDocumentVersion(string url, DocumentVersion docVersion, string jwt)
        {
            TestClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", jwt);

            var postContent = new MultipartFormDataContent();
            var payload = new ByteArrayContent(docVersion.RawFile);

            postContent.Add(payload, "UploadFile", "test.pdf");
            postContent.Add(new StringContent(docVersion.DocumentId.ToString()), "DocumentId");
            postContent.Add(new StringContent(docVersion.FileFormatId.ToString()), "FileFormatId");

            var httpResponse = await TestClient.PostAsync(url, postContent);

            // delete auth header after request was made
            TestClient.DefaultRequestHeaders.Authorization = null;

            var serializedResponse = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                return (httpResponse.StatusCode, JsonConvert.DeserializeObject<DocumentVersion>(serializedResponse));
            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }

        /// <summary>
        /// Deserializes a ManipulationResult of type T
        /// </summary>
        private ManipulationResult<T> DeserializeManipulationResult<T>(string serializedManipulationResult) where T : IDataTransferObject
        {
            var parsedInput = JObject.Parse(serializedManipulationResult);

            // extract operations from the serialized ManipulationResult
            var operationsSerialized = parsedInput["operations"].Children().ToList();

            var operations = new List<Operation>();
            operationsSerialized.ForEach(op => operations.Add(op.ToObject<Operation>()));

            var errorCodeSerialized = parsedInput["errorCode"];
            var errorCode = errorCodeSerialized.ToObject<int?>();

            var manipulatedEntitySerialized = parsedInput["manipulatedEntity"];
            var manipulatedEntity = manipulatedEntitySerialized.ToObject<T>();

            return new ManipulationResult<T>(errorCode.GetValueOrDefault(), manipulatedEntity, operations.ToArray());
        }

        /// <summary>
        /// Deserializes a RetrievalResult of type T
        /// </summary>
        /// <typeparam name="T">Typ der Werte im Result-Set</typeparam>
        private RetrievalResult<T> DeserializeRetrievalResult<T>(string serializedRetrievalResult) where T : IDataTransferObject
        {
            var parsedInput = JObject.Parse(serializedRetrievalResult);

            // extract resultset from the serialized RetrievalResult
            var resultSetSerialized = parsedInput["resultset"].Children().ToList();

            var resultSet = new List<T>();
            resultSetSerialized.ForEach(item => resultSet.Add(item.ToObject<T>()));

            var errorCodeSerialized = parsedInput["errorCode"];
            var errorCode = errorCodeSerialized.ToObject<int?>();

            return new RetrievalResult<T>(errorCode.GetValueOrDefault(), resultSet);
        }
    }
}
