using System;
using System.Collections.Generic;
using XProtect.ExternalEvents.IconPlugin.Models;

#if !NO_MILESTONE_SDK
using System.Windows;
using VideoOS.Platform;
#endif

namespace XProtect.ExternalEvents.IconPlugin.Services
{
    public class EventSubscriptionService
    {
        private readonly IconManager _iconManager;

        public EventSubscriptionService(IconManager iconManager)
        {
            _iconManager = iconManager;
        }

#if !NO_MILESTONE_SDK
        public object OnMessageReceived(Message message, FQID sender, FQID dest)
        {
            try
            {
                var eventData = ParseMessage(message);
                if (eventData == null) return null;

                var newState = eventData.ToIconState();

                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    _iconManager.SetStateByDeviceId(eventData.DeviceId, newState);
                    HandleSideEffects(eventData, newState);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EventService] Erro: {ex.Message}");
            }
            return null;
        }

        private ExternalEventData? ParseMessage(Message message)
        {
            if (message?.Data == null) return null;

            if (message.Data is Dictionary<string, object> dict)
            {
                return new ExternalEventData
                {
                    DeviceId   = dict.TryGetValue("DeviceId",   out var id) ? Guid.Parse(id.ToString()!) : Guid.Empty,
                    EventType  = dict.TryGetValue("EventType",  out var et) ? et.ToString()! : "Normal",
                    DeviceName = dict.TryGetValue("DeviceName", out var dn) ? dn.ToString()! : "Desconhecido",
                    Priority   = dict.TryGetValue("Priority",   out var pr) ? pr.ToString()! : "Medium",
                    Message    = dict.TryGetValue("Message",    out var ms) ? ms.ToString()! : string.Empty,
                    Timestamp  = DateTime.UtcNow
                };
            }
            return message.Data as ExternalEventData;
        }

        private void HandleSideEffects(ExternalEventData eventData, IconState state)
        {
            if (state == IconState.Alarm || state == IconState.Failure)
            {
                string msg = state == IconState.Alarm
                    ? $"ALARME: {eventData.DeviceName} — {eventData.Message}"
                    : $"FALHA: {eventData.DeviceName} sem comunicação";

                EnvironmentManager.Instance.PostMessage(
                    new Message(Message.ID_SHOW_POPUP_MESSAGE) { Data = msg }
                );
            }
        }
#else
        // Stub para compilação sem SDK — substituído pela implementação real com SDK
        public void SimulateEvent(Guid deviceId, string eventType)
        {
            var data = new ExternalEventData
            {
                DeviceId  = deviceId,
                EventType = eventType,
                Timestamp = DateTime.UtcNow
            };
            _iconManager.SetStateByDeviceId(data.DeviceId, data.ToIconState());
            Console.WriteLine($"[EventService] Simulado: {deviceId} → {eventType}");
        }
#endif
    }
}
