using Milen.UnityUIThings;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using WeatherApp;

/// <summary>
/// Provides a message containing the current location and temperature.
/// </summary>
public class WeatherAtLocation : MonoBehaviour,
	IMessageProvider
{
	public string Message { get; private set; } = string.Empty;

	private Coroutine m_locationCoroutine;
	private Coroutine m_weatherReportCoroutine;

	private LocationInfo? m_location;

	private readonly IWeatherProvider m_weatherProvider = new OpenMeteoWeatherProvider();

	private const float WeatherRequestInterval = 60f;
	private const float LocationUpdateInterval = 30f;
	private const float RetryInterval = 4f;


	void Start()
    {
		Message = "Fetching weather info";

		var callbacks = new PermissionCallbacks();

		callbacks.PermissionDenied += (s) =>
		{
			Debug.LogFormat("User denied permission: {0}", s);
		};

		callbacks.PermissionGranted += (s) =>
		{
			Debug.LogFormat("User granted permission: {0}", s);

			m_locationCoroutine = StartCoroutine(GetLocationRepeatedly());

			m_weatherReportCoroutine = StartCoroutine(GetWeatherRepeatedly());
		};

		Permission.RequestUserPermission(Permission.FineLocation, callbacks);
	}

	void OnDestroy()
	{
		if (m_weatherReportCoroutine != null)
		{
			StopCoroutine(m_weatherReportCoroutine);
		}

		if (m_locationCoroutine != null)
		{
			StopCoroutine(m_locationCoroutine);
		}
	}

	private IEnumerator UpdateLocation()
	{
		if (!Input.location.isEnabledByUser)
		{
			m_location = null;
			Message = "Location not enabled on device or app has no permission to access location";

			Debug.Log(Message);

			yield break;
		}

		// Start the location service
		Input.location.Start();

		// Wait for the location service to initialize
		int maxWait = 20;

		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
		}

		if (Input.location.status == LocationServiceStatus.Running)
		{
			m_location = Input.location.lastData;
		}
		else
		{
			m_location = null;
			Message = "Location not available";
		}

		Input.location.Stop();
	}

	private IEnumerator GetLocationRepeatedly()
	{
		while (true)
		{
			yield return UpdateLocation();

			float timeout = m_location.HasValue ? LocationUpdateInterval : RetryInterval;

			yield return new WaitForSeconds(timeout);
		}
	}

	private IEnumerator GetWeatherRepeatedly()
	{
		while (true)
		{
			if (m_location.HasValue)
			{
				yield return m_weatherProvider.FetchWeather(m_location.Value.latitude, m_location.Value.longitude);

				float waitDuration;

				if (m_weatherProvider.Weather == null)
				{
					Message = m_weatherProvider.ErrorMessage;
					waitDuration = RetryInterval;
				}
				else
				{
					Message = m_weatherProvider.Weather.ToString();
					waitDuration = WeatherRequestInterval;
				}

				yield return new WaitForSeconds(waitDuration);
			}
			else
			{
				Debug.Log("Location has no value, retrying in 5 sec.");

				// Location is not available, retry soon
				yield return new WaitForSeconds(RetryInterval);
			}
		}
	}
}
