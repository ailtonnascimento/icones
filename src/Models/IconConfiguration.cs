using System;
using System.Drawing;

namespace XProtect.ExternalEvents.IconPlugin.Models
{
    /// <summary>
    /// Configuração de um ícone registrado no plugin.
    /// Armazenado em JSON no diretório de dados do plugin.
    /// </summary>
    public class IconConfiguration
    {
        /// <summary>Chave única do ícone (ex: "sensor_porta_b2").</summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>Nome exibido no Smart Client.</summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>Tipo de evento externo associado.</summary>
        public string EventType { get; set; } = string.Empty;

        /// <summary>GUID da câmera associada no XProtect.</summary>
        public Guid CameraId { get; set; }

        /// <summary>Caminho absoluto do arquivo de ícone.</summary>
        public string IconFilePath { get; set; } = string.Empty;

        /// <summary>Cor do anel de alarme: Red, Amber, Blue.</summary>
        public string AlarmRingColor { get; set; } = "Red";

        /// <summary>Tipo de animação: Pulse, FastBlink, SlowBlink, Static.</summary>
        public string AnimationType { get; set; } = "Pulse";

        /// <summary>Prioridade do alarme para este ícone.</summary>
        public string AlarmPriority { get; set; } = "High";

        /// <summary>Exibir notificação popup no Smart Client.</summary>
        public bool ShowNotification { get; set; } = true;

        /// <summary>Abrir câmera associada automaticamente ao alarme.</summary>
        public bool AutoOpenCamera { get; set; } = false;

        /// <summary>Enviar e-mail ao supervisor em alarmes de alta prioridade.</summary>
        public bool SendEmail { get; set; } = false;

        /// <summary>Data/hora de registro da configuração.</summary>
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Runtime — não serializado
        [System.Text.Json.Serialization.JsonIgnore]
        public Image? LoadedImage { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public IconState CurrentState { get; set; } = IconState.Normal;
    }
}
