namespace SampleApp.MyTools;

/// <summary>
/// Weather tool çalıştırıldıktan sonra dönen örnek hava durumu sonucudur.
/// </summary>
public sealed record WeatherOutput(
    string City,
    int TemperatureCelsius,
    string Condition,
    string Summary);