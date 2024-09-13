using Bumptech.Glide.Load.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Umaru.Core.Services;

namespace Umaru.Core.Http
{
	public static class SuperHttp
	{
		public static string Post(string url, string json = "")
		{
			var factory = ServiceLocator.Get<IHttpClientFactory>();
			var client = factory.CreateClient("IgnoreSSL");
			var postContent = string.IsNullOrEmpty(json) ? new StringContent("{}", Encoding.UTF8, "application/json") : new StringContent(json, Encoding.UTF8, "application/json");


			var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = postContent };

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

		public static string Get(string url)
		{
			var factory = ServiceLocator.Get<IHttpClientFactory>();
			var client = factory.CreateClient("IgnoreSSL");

			var request = new HttpRequestMessage(HttpMethod.Get, url);

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
