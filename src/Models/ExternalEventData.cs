using System;

namespace XProtect.ExternalEvents.IconPlugin.Models
{
    /// <summary>
    /// Estado visual do ícone exibido no Smart Client.
    /// </summary>
    public enum IconState
    {
        /// <summary>Dispositivo operacional — anel oculto, dot verde.</summary>
        Normal,

        /// <summary>Alarme ativo — anel vermelho pulsante.</summary>
        Alarm,

        /// <summary>Falha de dispositivo — anel âmbar tracejado.</summary>
        Failure
    }

    /// <summary>
    /// Dados recebidos de um evento externo do XProtect.
    /// Mapeado a partir da mensagem do EnvironmentManager.
    /// </summary>
    public class ExternalEventData
    {
        /// <summary>ID único do dispositivo/câmera associado ao evento.</summary>
        public Guid DeviceId { get; set; }

        /// <summary>Tipo do evento: "Alarm", "Failure" ou "Normal".</summary>
        public string EventType { get; set; } = "Normal";

        /// <summary>Nome legível do dispositivo.</summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>Timestamp UTC do evento.</summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>Prioridade: High, Medium, Low.</summary>
        public string Priority { get; set; } = "Medium";

        /// <summary>Mensagem descritiva do evento.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Converte EventType string para IconState enum.</summary>
        public IconState ToIconState() => EventType switch
        {
            "Alarm"   => IconState.Alarm,
            "Failure" => IconState.Failure,
            _         => IconState.Normal
        };
    }
}
