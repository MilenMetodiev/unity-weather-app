using System.Collections;
using System.Globalization;

namespace WeatherApp
{
	public class WeatherInfo
	{
		public float Latitude { get; }
		public float Longitude { get; }
		public float Temperature { get; }
		public string TemperatureUnit { get; }

		public WeatherInfo(float latitude, float longitude, float temperature, string temperatureUnit)
		{
			Latitude = latitude;
			Longitude = longitude;
			Temperature = temperature;
			TemperatureUnit = temperatureUnit ?? string.Empty;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "({0:0.####}, {1:0.####}) {2}{3}",
				Latitude,
				Longitude,
				Temperature,
				TemperatureUnit);
		}
	}

	public interface IWeatherProvider
	{
		public IEnumerator FetchWeather(float latitude, float longitude);

		public WeatherInfo Weather { get; }

		public string ErrorMessage { get; }
	}
}