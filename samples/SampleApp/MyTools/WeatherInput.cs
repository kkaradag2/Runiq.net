namespace SampleApp.MyTools;

/// <summary>
/// Weather tool için şehir bazlı hava durumu input modelidir.
/// </summary>
public sealed record WeatherInput(
    string City);