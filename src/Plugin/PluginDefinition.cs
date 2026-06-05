using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using XProtect.ExternalEvents.IconPlugin.Models;
using XProtect.ExternalEvents.IconPlugin.Services;

#if !NO_MILESTONE_SDK
using VideoOS.Platform;
using VideoOS.Platform.Client;
#endif

namespace XProtect.ExternalEvents.IconPlugin
{
    /// <summary>
    /// Ponto de entrada principal do plugin no Milestone Smart Client SDK.
    /// Compile com o símbolo NO_MILESTONE_SDK removido quando o SDK estiver instalado.
    /// </summary>
#if NO_MILESTONE_SDK
    public class PluginDefinition
#else
    public class PluginDefinition : PluginDefinitionBase
#endif
    {
        public static readonly Guid PluginId = new Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");

        private IconManager? _iconManager;
        private EventSubscriptionService? _eventService;

#if !NO_MILESTONE_SDK
        public override Guid Id => PluginId;
        public override string Name => "External Events Icon Manager";
        public override string Manufacturer => "Seu Nome / Empresa";
        public override string VersionString => "2.4.0";
        public override Image Icon => LoadEmbeddedImage("XProtect.ExternalEvents.IconPlugin.Resources.plugin_icon.png");
#endif

        public void Init()
        {
            try
            {
                _iconManager = new IconManager();
                _eventService = new EventSubscriptionService(_iconManager);

                LoadDefaultIcons();

#if !NO_MILESTONE_SDK
                EnvironmentManager.Instance.RegisterReceiver(
                    _eventService.OnMessageReceived,
                    new MessageIdFilter(Message.ID_SERVER_GENERALREQUEST)
                );
#endif
                Console.WriteLine($"[IconPlugin] Inicializado — {_iconManager.Count} ícones carregados.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IconPlugin] Erro na inicialização: {ex.Message}");
            }
        }

        public void Close()
        {
#if !NO_MILESTONE_SDK
            if (_eventService != null)
                EnvironmentManager.Instance.UnRegisterReceiver(_eventService.OnMessageReceived);
#endif
            _iconManager?.Dispose();
        }

        private void LoadDefaultIcons()
        {
            string iconsPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "Resources", "icons"
            );
            if (!Directory.Exists(iconsPath)) return;

            foreach (string file in Directory.GetFiles(iconsPath, "*.png"))
            {
                string key = Path.GetFileNameWithoutExtension(file);
                _iconManager!.RegisterFromFile(key, file);
            }
        }

        private Image LoadEmbeddedImage(string resourceName)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream(resourceName);
                return stream != null ? Image.FromStream(stream) : SystemIcons.Shield.ToBitmap();
            }
            catch
            {
                return SystemIcons.Shield.ToBitmap();
            }
        }
    }
}
