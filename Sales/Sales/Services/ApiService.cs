
namespace Sales.Services
{
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Newtonsoft.Json;
	using Plugin.Connectivity;
	using Common.Models;
	using Helpers;
	public class ApiService
	{
		public async Task<Response> CheckConnection()
		{
			if (!CrossConnectivity.Current.IsConnected)
			{
				return new Response
				{
					IsSuccess = false,
					Message =Languages.TurnOnInternet,
				};
			}

			var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
			if (!isReachable)
			{
				return new Response
				{
					IsSuccess = false,
					Message = Languages.NoInternet,
				};
			}

			return new Response
			{
				IsSuccess = true,
			};
		}


		/// <summary>
		/// Método Genérico que consume cualquier servicios webApi y cualquier lista.
		/// </summary>
		/// <typeparam name="T">Generic</typeparam>
		/// <param name="urlbase">Url del Servicio API</param>
		/// <param name="prefix">Salse.API</param>
		/// <param name="controller">Controlador</param>
		/// <returns></returns>
		public async Task<Response> GetList<T>(string urlbase, string prefix, string controller)
		{
			try
			{
				var client = new HttpClient();
				client.BaseAddress = new Uri(urlbase);
				var url = $"{prefix}{controller}";
				var response = await client.GetAsync(url);
				var answer = await response.Content.ReadAsStringAsync();
				if (!response.IsSuccessStatusCode)
				{
					return new Response
					{
						IsSuccess = false,
						Message = answer
					};
				}

				var list = JsonConvert.DeserializeObject<List<T>>(answer);
				return new Response
				{
					IsSuccess = true,
					Result = list
				};
			}
			catch (Exception Ex)
			{
				return new Response
				{
					IsSuccess = false,
					Message = Ex.Message
				};
			}
		}
	}
}
