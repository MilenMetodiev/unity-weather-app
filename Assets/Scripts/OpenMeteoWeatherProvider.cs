using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;


namespace WeatherApp
{
	public class OpenMeteoWeatherProvider : IWeatherProvider
	{
		public WeatherInfo Weather { get; private set; }
		public string ErrorMessage { get; private set; } = string.Empty;

		public IEnumerator FetchWeather(float latitude, float longitude)
		{
			string apiUrl = BuildRequestUrl(latitude, longitude);

			using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
			{
				yield return request.SendWebRequest();

				Weather = null;

				if (request.result == UnityWebRequest.Result.ConnectionError ||
					request.result == UnityWebRequest.Result.ProtocolError)
				{
					ErrorMessage = $"Error fetching weather data: {request.error}";
				}
				else
				{
					ParseResponse(request.downloadHandler.text);
				}
			}
		}

		private void ParseResponse(string jsonResponse)
		{
			try
			{
				OpenMeteoResponse weatherResponse = JsonUtility.FromJson<OpenMeteoResponse>(jsonResponse);

				Weather = weatherResponse.ToWeatherInfo();

				ErrorMessage = (Weather == null) ? "Could not parse Open Meteo response" : string.Empty;
			}
			catch (Exception e)
			{
				ErrorMessage = "Error parsing Open Meteo response: " + e.Message;
			}
		}

		private static string BuildRequestUrl(float latitude, float longitude)
		{
			NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
			query["latitude"] = latitude.ToString("0.######", CultureInfo.InvariantCulture);
			query["longitude"] = longitude.ToString("0.######", CultureInfo.InvariantCulture);
			query["current_weather"] = "true";

			UriBuilder uriBuilder = new UriBuilder("https://api.open-meteo.com/v1/forecast");
			uriBuilder.Query = query.ToString();

			return uriBuilder.ToString();
		}
	}
}