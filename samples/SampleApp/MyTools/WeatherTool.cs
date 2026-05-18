using Runiq.Agents.Tools;

namespace SampleApp.MyTools;

/// <summary>
/// Örnek hava durumu tool'udur.
/// Gerçek provider entegrasyonu yerine deterministic örnek veri döner.
/// </summary>
[RuniqTool(
    name: "weather",
    description: "Gets current weather information for a city.")]
public sealed class WeatherTool : IRuniqTool<WeatherInput, WeatherOutput>
{
    /// <inheritdoc />
    public Task<WeatherOutput> ExecuteAsync(
        WeatherInput input,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (string.IsNullOrWhiteSpace(input.City))
        {
            throw new ArgumentException(
                "City is required.",
                nameof(input));
        }

        var city = input.City.Trim();

        var output = new WeatherOutput(
            City: city,
            TemperatureCelsius: 23,
            Condition: "Sunny",
            Summary: $"{city} için hava güneşli ve yaklaşık 23 derece.");

        return Task.FromResult(output);
    }
}