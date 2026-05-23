using Runiq.Agents.Tools;

namespace SampleApp.MyTools;

/// <summary>
/// Hava durumu bağlamına göre kısa şehir gezi planı üreten örnek tool'dur.
/// Gerçek servis entegrasyonu yerine deterministic örnek veri döner.
/// </summary>
[RuniqTool(
    name: "trip_plan",
    description: "Creates a short city trip plan using city, duration, weather condition, and temperature context.")]
public sealed class TripPlanTool : IRuniqTool<TripPlanInput, TripPlanOutput>
{
    /// <inheritdoc />
    public Task<TripPlanOutput> ExecuteAsync(
        TripPlanInput input,
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
        var durationHours = input.DurationHours <= 0 ? 3 : input.DurationHours;
        var condition = input.WeatherCondition?.Trim() ?? string.Empty;
        var temperature = input.TemperatureCelsius;

        var isGoodForOutdoor =
            temperature is >= 16 and <= 28 &&
            (
                condition.Contains("sun", StringComparison.OrdinalIgnoreCase) ||
                condition.Contains("clear", StringComparison.OrdinalIgnoreCase) ||
                condition.Contains("güneş", StringComparison.OrdinalIgnoreCase)
            );

        var planType = isGoodForOutdoor
            ? "OutdoorFocused"
            : "IndoorFriendly";

        var stops = CreateStops(city, durationHours, isGoodForOutdoor);

        var output = new TripPlanOutput(
            City: city,
            PlanType: planType,
            DurationHours: durationHours,
            Stops: stops,
            Reason: CreateReason(city, condition, temperature, isGoodForOutdoor),
            Summary: CreateSummary(city, durationHours, stops));

        return Task.FromResult(output);
    }

    private static IReadOnlyList<TripPlanStop> CreateStops(
        string city,
        int durationHours,
        bool isGoodForOutdoor)
    {
        if (city.Equals("Istanbul", StringComparison.OrdinalIgnoreCase))
        {
            if (isGoodForOutdoor)
            {
                return
                    [
                        new TripPlanStop(
                            Order: 1,
                            Name: "Eminönü ve Mısır Çarşısı çevresi",
                            Type: "outdoor",
                            EstimatedMinutes: 60,
                            Note: "Kısa sürede tarihi yarımada dokusunu görmek için iyi başlangıç."),

                        new TripPlanStop(
                            Order: 2,
                            Name: "Galata Köprüsü ve Karaköy yürüyüşü",
                            Type: "outdoor",
                            EstimatedMinutes: 50,
                            Note: "Güneşli havada manzara ve kısa yürüyüş için uygun."),

                        new TripPlanStop(
                            Order: 3,
                            Name: "Galata Kulesi çevresi ve kahve molası",
                            Type: "mixed",
                            EstimatedMinutes: 45,
                            Note: "Rotayı yormadan bitirmek için iyi kapanış noktası.")
                    ];
            }

            return
            [
                new TripPlanStop(
                    Order: 1,
                    Name: "İstanbul Arkeoloji Müzeleri",
                    Type: "indoor",
                    EstimatedMinutes: 70,
                    Note: "Hava uygun değilse kapalı ve merkezi bir başlangıç."),

                new TripPlanStop(
                    Order: 2,
                    Name: "Kapalıçarşı çevresi",
                    Type: "indoor",
                    EstimatedMinutes: 60,
                    Note: "Kısa sürede gezilebilir, hava etkisini azaltır."),

                new TripPlanStop(
                    Order: 3,
                    Name: "Karaköy kapalı mekan molası",
                    Type: "indoor",
                    EstimatedMinutes: 40,
                    Note: "Planı düşük tempoyla tamamlamak için uygun.")
            ];
        }

        return
        [
            new TripPlanStop(
                Order: 1,
                Name: $"{city} merkezi",
                Type: isGoodForOutdoor ? "outdoor" : "indoor",
                EstimatedMinutes: 60,
                Note: "Kısa şehir keşfi için başlangıç noktası."),

            new TripPlanStop(
                Order: 2,
                Name: $"{city} yerel kafe molası",
                Type: "mixed",
                EstimatedMinutes: 40,
                Note: "Rotayı dinlendiren kısa mola."),

            new TripPlanStop(
                Order: 3,
                Name: $"{city} kısa yürüyüş veya kapalı mekan alternatifi",
                Type: isGoodForOutdoor ? "outdoor" : "indoor",
                EstimatedMinutes: 50,
                Note: "Hava durumuna göre esnek son durak.")
        ];
    }

    private static string CreateReason(
        string city,
        string weatherCondition,
        int temperatureCelsius,
        bool isGoodForOutdoor)
    {
        var weatherText = string.IsNullOrWhiteSpace(weatherCondition)
            ? "hava durumu bilgisi sınırlı olduğu için"
            : $"{weatherCondition}, {temperatureCelsius}°C olduğu için";

        var planText = isGoodForOutdoor
            ? "dış mekan ağırlıklı ve yürünebilir bir rota seçildi"
            : "kapalı mekan ağırlıklı, hava koşullarından daha az etkilenen bir rota seçildi";

        return $"{city} için {weatherText} {planText}.";
    }

    private static string CreateSummary(
        string city,
        int durationHours,
        IReadOnlyList<TripPlanStop> stops)
    {
        var stopNames = string.Join(" → ", stops.Select(stop => stop.Name));

        return $"{city} için yaklaşık {durationHours} saatlik rota: {stopNames}.";
    }
}

/// <summary>
/// Trip plan tool için şehir, süre ve hava durumu bağlamını taşıyan input modelidir.
/// </summary>
public sealed record TripPlanInput(
    string City,
    int DurationHours,
    string? WeatherCondition,
    int TemperatureCelsius);

/// <summary>
/// Trip plan tool sonucunda üretilen rota bilgisidir.
/// </summary>
public sealed record TripPlanOutput(
    string City,
    string PlanType,
    int DurationHours,
    IReadOnlyList<TripPlanStop> Stops,
    string Reason,
    string Summary);

/// <summary>
/// Gezi planındaki tek bir durağı temsil eder.
/// </summary>
public sealed record TripPlanStop(
    int Order,
    string Name,
    string Type,
    int EstimatedMinutes,
    string Note);