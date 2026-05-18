using Runiq.Agents.Tools;

namespace SampleApp.MyTools;

/// <summary>
/// Örnek finans verisi sorgulamak için kullanılan demo tool'dur.
/// </summary>
[RuniqTool(
    name: "finance",
    description: "Gets a simple financial market summary for a symbol.")]
public sealed class FinanceTool : IRuniqTool<FinanceInput, FinanceOutput>
{
    /// <inheritdoc />
    public Task<FinanceOutput> ExecuteAsync(
        FinanceInput input,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        var symbol = string.IsNullOrWhiteSpace(input.Symbol)
            ? "XAUUSD"
            : input.Symbol.Trim().ToUpperInvariant();

        return Task.FromResult(new FinanceOutput(
            Symbol: symbol,
            LastPrice: 2345.75m,
            Currency: "USD",
            Summary: $"{symbol} için örnek piyasa fiyatı 2345.75 USD olarak döndü."));
    }
}

/// <summary>
/// Örnek haber başlığı sorgulamak için kullanılan demo tool'dur.
/// </summary>
[RuniqTool(
    name: "news",
    description: "Gets sample latest headlines for a topic.")]
public sealed class NewsTool : IRuniqTool<NewsInput, NewsOutput>
{
    /// <inheritdoc />
    public Task<NewsOutput> ExecuteAsync(
        NewsInput input,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        var topic = string.IsNullOrWhiteSpace(input.Topic)
            ? "technology"
            : input.Topic.Trim();

        return Task.FromResult(new NewsOutput(
            Topic: topic,
            Headlines:
            [
                $"{topic} konusunda örnek haber başlığı 1",
                $"{topic} konusunda örnek haber başlığı 2"
            ]));
    }
}

/// <summary>
/// Örnek takvim uygunluğu sorgulamak için kullanılan demo tool'dur.
/// </summary>
[RuniqTool(
    name: "calendar",
    description: "Checks sample calendar availability for a given day.")]
public sealed class CalendarTool : IRuniqTool<CalendarInput, CalendarOutput>
{
    /// <inheritdoc />
    public Task<CalendarOutput> ExecuteAsync(
        CalendarInput input,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        var day = string.IsNullOrWhiteSpace(input.Day)
            ? "today"
            : input.Day.Trim();

        return Task.FromResult(new CalendarOutput(
            Day: day,
            IsAvailable: true,
            Summary: $"{day} için örnek takvim uygunluğu müsait olarak döndü."));
    }
}

/// <summary>
/// Finance tool input modelidir.
/// </summary>
public sealed record FinanceInput(string Symbol);

/// <summary>
/// Finance tool output modelidir.
/// </summary>
public sealed record FinanceOutput(
    string Symbol,
    decimal LastPrice,
    string Currency,
    string Summary);

/// <summary>
/// News tool input modelidir.
/// </summary>
public sealed record NewsInput(string Topic);

/// <summary>
/// News tool output modelidir.
/// </summary>
public sealed record NewsOutput(
    string Topic,
    IReadOnlyList<string> Headlines);

/// <summary>
/// Calendar tool input modelidir.
/// </summary>
public sealed record CalendarInput(string Day);

/// <summary>
/// Calendar tool output modelidir.
/// </summary>
public sealed record CalendarOutput(
    string Day,
    bool IsAvailable,
    string Summary);