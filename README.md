# XProtect External Events — Icon Plugin

Plugin para o **Milestone XProtect Smart Client** que permite adicionar ícones personalizados a eventos externos, com suporte a anéis animados de alarme e falha.

## Funcionalidades

- Upload de ícones customizados (PNG, SVG, ICO) via formulário
- Anel vermelho pulsante ao redor do ícone quando há **alarme ativo**
- Anel âmbar tracejado ao redor do ícone quando há **falha de dispositivo**
- Dot verde quando dispositivo está **operacional**
- Notificação no Smart Client e popup de câmera associada
- Gerenciamento de múltiplos dispositivos com log de eventos

## Pré-requisitos

- Visual Studio 2019 ou superior
- .NET Framework 4.7.2+
- Milestone XProtect Smart Client SDK (2022 R1 ou superior)
- NuGet: `VideoOS.Platform` e `VideoOS.SmartClient.Infrastructure`

## Estrutura do Projeto

```
xprotect-icon-plugin/
├── src/
│   ├── Plugin/
│   │   ├── PluginDefinition.cs       # Ponto de entrada do plugin
│   │   └── XProtectIconPlugin.csproj
│   ├── Controls/
│   │   ├── AlarmRingControl.xaml     # Controle WPF do anel animado
│   │   └── AlarmRingControl.xaml.cs
│   ├── Models/
│   │   ├── ExternalEventData.cs      # Modelo de evento externo
│   │   └── IconConfiguration.cs     # Configuração de ícone
│   ├── Services/
│   │   ├── IconManager.cs            # Gerenciador de ícones
│   │   └── EventSubscriptionService.cs
│   └── Resources/
│       └── icons/                    # Ícones padrão do plugin
├── docs/
│   └── integration-guide.md
├── tests/
│   └── IconManagerTests.cs
└── README.md
```

## Instalação

1. Clone o repositório:
   ```bash
   git clone https://github.com/SEU_USUARIO/xprotect-icon-plugin.git
   cd xprotect-icon-plugin
   ```

2. Abra `src/Plugin/XProtectIconPlugin.csproj` no Visual Studio

3. Restaure os pacotes NuGet:
   ```bash
   nuget restore
   ```

4. Compile em modo `Release`

5. Copie a pasta `/bin/Release/` para o diretório de plugins do Smart Client:
   ```
   C:\Program Files\Milestone\XProtect Smart Client\MIPPlugins\
   ```

6. Reinicie o Smart Client

## Uso rápido

Após instalar, acesse **Configurações → Plugins → External Events Icon Manager** no Smart Client.

## Licença

MIT License — veja [LICENSE](LICENSE)
# icone_claude
