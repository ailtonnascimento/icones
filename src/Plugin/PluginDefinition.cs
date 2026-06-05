using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace XProtect.ExternalEvents.IconPlugin
{
    /// <summary>
    /// Ponto de entrada principal do plugin no Milestone Smart Client SDK.
    /// Registra o plugin, subscreve eventos externos e inicializa o gerenciador de ícones.
    /// </summary>
    public class PluginDefinition : PluginDefinitionBase
    {
        // GUID único do plugin — gere um novo via Tools > Create GUID no Visual Studio
        public static readonly Guid PluginId = new Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");

        private IconManager _iconManager;
        private EventSubscriptionService _eventService;

        #region PluginDefinitionBase overrides

        public override Guid Id => PluginId;

        public override string Name => "External Events Icon Manager";

        public override string Manufacturer => "Seu Nome / Empresa";

        public override string VersionString => "2.4.0";

        public override Image Icon =>
            LoadEmbeddedImage("XProtect.ExternalEvents.IconPlugin.Resources.plugin_icon.png");

        /// <summary>
        /// ViewItemPlugin opcional — expõe painel de configuração no Smart Client.
        /// </summary>
        public override List<ViewItemPlugin> ViewItemPlugins =>
            new List<ViewItemPlugin> { new IconManagerViewItemPlugin() };

        #endregion

        /// <summary>
        /// Chamado pelo Smart Client ao carregar o plugin.
        /// Inicializa serviços e subscreve eventos externos.
        /// </summary>
        public override void Init()
        {
            try
            {
                _iconManager = new IconManager();
                _eventService = new EventSubscriptionService(_iconManager);

                // Carregar ícones padrão da pasta Resources/icons
                LoadDefaultIcons();

                // Subscrever mensagens de eventos externos do servidor
                EnvironmentManager.Instance.RegisterReceiver(
                    _eventService.OnMessageReceived,
                    new MessageIdFilter(Message.ID_SERVER_GENERALREQUEST)
                );

                Log.Info($"[IconPlugin] Plugin inicializado — {_iconManager.Count} ícones carregados.");
            }
            catch (Exception ex)
            {
                Log.Error($"[IconPlugin] Falha na inicialização: {ex.Message}");
            }
        }

        /// <summary>
        /// Chamado ao descarregar o plugin — limpa recursos e cancelamentos de subscrição.
        /// </summary>
        public override void Close()
        {
            EnvironmentManager.Instance.UnRegisterReceiver(_eventService.OnMessageReceived);
            _iconManager?.Dispose();
            Log.Info("[IconPlugin] Plugin finalizado.");
        }

        #region Helpers

        private void LoadDefaultIcons()
        {
            string iconsPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "Resources", "icons"
            );

            if (!Directory.Exists(iconsPath)) return;

            foreach (string file in Directory.GetFiles(iconsPath, "*.png"))
            {
                string iconKey = Path.GetFileNameWithoutExtension(file);
                _iconManager.RegisterFromFile(iconKey, file);
            }
        }

        private Image LoadEmbeddedImage(string resourceName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream(resourceName);
                return stream != null ? Image.FromStream(stream) : SystemIcons.Shield.ToBitmap();
            }
            catch
            {
                return SystemIcons.Shield.ToBitmap();
            }
        }

        #endregion
    }
}
