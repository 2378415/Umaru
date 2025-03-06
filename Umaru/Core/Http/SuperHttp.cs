using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Umaru.Core.Services;

namespace Umaru.Core.Http
{
    public static class SuperHttp
    {
        public static string Post(string url, string json = "", Action<HttpRequestHeaders>? action = null)
        {
            var factory = ServiceLocator.Get<IHttpClientFactory>();
            if (factory == null) return string.Empty;
            var client = factory.CreateClient("IgnoreSSL");
            var postContent = string.IsNullOrEmpty(json) ? new StringContent("{}", Encoding.UTF8, "application/json") : new StringContent(json, Encoding.UTF8, "application/json");


            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = postContent };
            if (action != null) action(request.Headers);

            try
            {
                HttpResponseMessage httpResponseMessage = client.SendAsync(request).Result;
                var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string Get(string url, Action<HttpRequestHeaders>? action = null)
        {
            var factory = ServiceLocator.Get<IHttpClientFactory>();
            if (factory == null) return string.Empty;
            var client = factory.CreateClient("IgnoreSSL");

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (action != null) action(request.Headers);

            try
            {
                HttpResponseMessage httpResponseMessage = client.SendAsync(request).Result;
                var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
