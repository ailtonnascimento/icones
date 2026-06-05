using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using XProtect.ExternalEvents.IconPlugin.Models;

namespace XProtect.ExternalEvents.IconPlugin.Services
{
    /// <summary>
    /// Gerencia o registro, persistência e estado de todos os ícones do plugin.
    /// Thread-safe para uso com o dispatcher do Smart Client.
    /// </summary>
    public class IconManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, IconConfiguration> _icons = new();
        private readonly string _configPath;

        public int Count => _icons.Count;

        public IconManager()
        {
            _configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Milestone", "XProtect", "IconPlugin", "icons.json"
            );
            Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);
            LoadPersistedConfig();
        }

        /// <summary>
        /// Registra um ícone a partir de um stream (upload via formulário).
        /// </summary>
        public void Register(string key, Stream iconStream, IconConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key obrigatória.", nameof(key));

            // Salvar arquivo de ícone em disco
            string iconsDir = Path.GetDirectoryName(_configPath)!;
            string iconFile = Path.Combine(iconsDir, $"{key}.png");

            using (var fs = File.Create(iconFile))
                iconStream.CopyTo(fs);

            config.Key = key;
            config.IconFilePath = iconFile;
            config.LoadedImage = Image.FromFile(iconFile);

            _icons[key] = config;
            PersistConfig();
        }

        /// <summary>
        /// Registra um ícone a partir de um arquivo local (ícones padrão do plugin).
        /// </summary>
        public void RegisterFromFile(string key, string filePath)
        {
            if (!File.Exists(filePath)) return;

            var config = new IconConfiguration
            {
                Key = key,
                DisplayName = key,
                IconFilePath = filePath,
                LoadedImage = Image.FromFile(filePath)
            };

            _icons[key] = config;
        }

        /// <summary>
        /// Atualiza o estado do ícone e dispara o evento de mudança de estado.
        /// </summary>
        public void SetState(string key, IconState state)
        {
            if (!_icons.TryGetValue(key, out var config)) return;
            config.CurrentState = state;
            StateChanged?.Invoke(this, new IconStateChangedEventArgs(key, state, config));
        }

        /// <summary>
        /// Atualiza o estado por DeviceId (GUID da câmera associada).
        /// </summary>
        public void SetStateByDeviceId(Guid deviceId, IconState state)
        {
            foreach (var config in _icons.Values)
            {
                if (config.CameraId == deviceId)
                    SetState(config.Key, state);
            }
        }

        public IconConfiguration? Get(string key) =>
            _icons.TryGetValue(key, out var c) ? c : null;

        public IEnumerable<IconConfiguration> GetAll() => _icons.Values;

        public bool Remove(string key)
        {
            if (!_icons.TryRemove(key, out var config)) return false;
            config.LoadedImage?.Dispose();
            PersistConfig();
            return true;
        }

        /// <summary>
        /// Disparado sempre que o estado de um ícone muda.
        /// Conecte ao dispatcher WPF para atualizar a UI.
        /// </summary>
        public event EventHandler<IconStateChangedEventArgs>? StateChanged;

        #region Persistência JSON

        private void PersistConfig()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_icons.Values, options);
                File.WriteAllText(_configPath, json);
            }
            catch (Exception ex)
            {
                Log.Error($"[IconManager] Falha ao persistir configuração: {ex.Message}");
            }
        }

        private void LoadPersistedConfig()
        {
            try
            {
                if (!File.Exists(_configPath)) return;
                string json = File.ReadAllText(_configPath);
                var configs = JsonSerializer.Deserialize<List<IconConfiguration>>(json);
                if (configs == null) return;

                foreach (var config in configs)
                {
                    if (File.Exists(config.IconFilePath))
                        config.LoadedImage = Image.FromFile(config.IconFilePath);
                    _icons[config.Key] = config;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[IconManager] Falha ao carregar configuração: {ex.Message}");
            }
        }

        #endregion

        public void Dispose()
        {
            foreach (var config in _icons.Values)
                config.LoadedImage?.Dispose();
            _icons.Clear();
        }
    }

    public class IconStateChangedEventArgs : EventArgs
    {
        public string Key { get; }
        public IconState NewState { get; }
        public IconConfiguration Config { get; }

        public IconStateChangedEventArgs(string key, IconState state, IconConfiguration config)
        {
            Key = key;
            NewState = state;
            Config = config;
        }
    }
}
