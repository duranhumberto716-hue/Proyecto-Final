using System.Text.Json;

namespace Proyecto_Final.Services;

/// <summary>
/// Consume la API REST pública de tipo de cambio (https://open.er-api.com) para
/// convertir el valor del inventario de bolivianos (Bs) a dólares (USD).
/// Si no hay internet, usa el tipo de cambio oficial de Bolivia (6.96).
/// </summary>
public class TipoCambioService
{
    public const double TipoCambioOficial = 6.96;
    private const string UrlApi = "https://open.er-api.com/v6/latest/USD";

    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(8) };

    /// <returns>Bolivianos por 1 USD y si el dato vino de la API en línea.</returns>
    public async Task<(double BsPorDolar, bool EnLinea)> ObtenerBsPorDolarAsync()
    {
        try
        {
            await using var stream = await Http.GetStreamAsync(UrlApi);
            using var json = await JsonDocument.ParseAsync(stream);
            var bob = json.RootElement.GetProperty("rates").GetProperty("BOB").GetDouble();
            return bob > 0 ? (bob, true) : (TipoCambioOficial, false);
        }
        catch
        {
            return (TipoCambioOficial, false);
        }
    }
}

