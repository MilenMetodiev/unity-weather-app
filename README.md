# unity-weather-app

A Unity app for Android that shows the weather at the user's current location.

To show the current location and temperature, the user has to tap on the orange square in the middle of the screen.

The app uses the UIThings package to show a toast message with the current location and temperature. The device's location must be enabled, otherwise the app shows an error message.

## Design Notes

The WeatherAtLocation MonoBehaviour repeatedly fetches location and weather information at certain time intervals and maintains this information as a formatted string message.

The app scene contains:
* An invisible GameObject that has the WeatherAtLocation component.
* An orange square GameObject that has the ShowMessageOnClick component attached.

WeatherAtLocation implements the IMessageProvider interface and serves as a message provider for the ShowMessageOnClick component of the square.
When the square is tapped it retrieves the current state of WeatherAtLocation's message and displays it as an Android toast.
