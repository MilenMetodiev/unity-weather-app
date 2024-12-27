using System;

namespace WeatherApp
{
	[Serializable]
	public class OpenMeteoResponse
	{
		public float latitude;
		public float longitude;
		public CurrentWeather current_weather;
		public CurrentWeatherUnits current_weather_units;

		public WeatherInfo ToWeatherInfo()
		{
			return new WeatherInfo(
				latitude,
				longitude,
				current_weather.temperature,
				current_weather_units.temperature);
		}
	}

	[Serializable]
	public class CurrentWeather
	{
		public float temperature;
		public float windspeed;
	}

	[Serializable]
	public class CurrentWeatherUnits
	{
		public string temperature;
		public string windspeed;
	}
}