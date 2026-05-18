namespace Runiq.Core.Dashboard;

/// <summary>
/// Runiq Dashboard uç noktasının host uygulama içinde nasıl yayınlanacağını belirleyen ayarlardır.
/// </summary>
public sealed class RuniqDashboardOptions
{
    /// <summary>
    /// Dashboard'un yayınlanacağı temel path değeridir.
    /// Varsayılan değer "/runiq" olur.
    /// </summary>
    public string Path { get; set; } = "/runiq";

    /// <summary>
    /// Dashboard uygulamasında gösterilecek başlık bilgisidir.
    /// Varsayılan değer "Runiq Dashboard" olur.
    /// </summary>
    public string Title { get; set; } = "Runiq Dashboard";
}