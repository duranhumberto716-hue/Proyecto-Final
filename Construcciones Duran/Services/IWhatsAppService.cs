namespace Proyecto_Final.Services;

public interface IWhatsAppService
{
    Task<bool> EnviarMensajeAsync(string mensaje, string? numeroTelefono = null);
    Task<bool> AbrirChatDirectoAsync(string? numeroTelefono = null);
}

