using System;
using System.Windows;
using VideoOS.Platform;
using XProtect.ExternalEvents.IconPlugin.Models;

namespace XProtect.ExternalEvents.IconPlugin.Services
{
    /// <summary>
    /// Subscreve e processa mensagens de eventos externos recebidas do XProtect.
    /// Traduz mensagens do SDK para mudanças de estado nos ícones.
    /// </summary>
    public class EventSubscriptionService
    {
        private readonly IconManager _iconManager;

        public EventSubscriptionService(IconManager iconManager)
        {
            _iconManager = iconManager;
        }

        /// <summary>
        /// Callback registrado no EnvironmentManager para receber mensagens do servidor.
        /// Executado em thread de background — despacha para UI thread via Dispatcher.
        /// </summary>
        public object OnMessageReceived(Message message, FQID sender, FQID dest)
        {
            try
            {
                var eventData = ParseMessage(message);
                if (eventData == null) return null;

                var newState = eventData.ToIconState();

                // Despachar para UI thread (WPF requer atualização no thread principal)
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    _iconManager.SetStateByDeviceId(eventData.DeviceId, newState);
                    HandleSideEffects(eventData, newState);
                });
            }
            catch (Exception ex)
            {
                Log.Error($"[EventSubscriptionService] Erro ao processar mensagem: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Faz o parse da mensagem genérica do XProtect para ExternalEventData.
        /// Adapte conforme o formato de mensagem do seu sistema integrado.
        /// </summary>
        private ExternalEventData? ParseMessage(Message message)
        {
            if (message?.Data == null) return null;

            // Exemplo: mensagem com dicionário de propriedades
            if (message.Data is System.Collections.Generic.Dictionary<string, object> dict)
            {
                return new ExternalEventData
                {
                    DeviceId  = dict.TryGetValue("DeviceId", out var id)
                                    ? Guid.Parse(id.ToString()!) : Guid.Empty,
                    EventType = dict.TryGetValue("EventType", out var et)
                                    ? et.ToString()! : "Normal",
                    DeviceName = dict.TryGetValue("DeviceName", out var dn)
                                    ? dn.ToString()! : "Desconhecido",
                    Priority  = dict.TryGetValue("Priority", out var pr)
                                    ? pr.ToString()! : "Medium",
                    Message   = dict.TryGetValue("Message", out var msg)
                                    ? msg.ToString()! : string.Empty,
                    Timestamp = DateTime.UtcNow
                };
            }

            // Fallback: se o dado for diretamente ExternalEventData
            return message.Data as ExternalEventData;
        }

        /// <summary>
        /// Efeitos colaterais baseados no estado e configuração do ícone:
        /// notificações, popup de câmera, envio de e-mail.
        /// </summary>
        private void HandleSideEffects(ExternalEventData eventData, IconState state)
        {
            var config = _iconManager.GetAll() is var all
                ? null
                : null; // Aqui você buscaria a config pelo DeviceId

            // Notificação no Smart Client
            if (state == IconState.Alarm || state == IconState.Failure)
            {
                string msg = state == IconState.Alarm
                    ? $"ALARME: {eventData.DeviceName} — {eventData.Message}"
                    : $"FALHA: {eventData.DeviceName} sem comunicação";

                EnvironmentManager.Instance.PostMessage(
                    new Message(Message.ID_SHOW_POPUP_MESSAGE) { Data = msg }
                );
            }

            Log.Info($"[EventService] Estado atualizado: {eventData.DeviceName} → {state} ({eventData.Timestamp:HH:mm:ss})");
        }
    }
}
