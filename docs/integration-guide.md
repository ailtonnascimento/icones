# Guia de Integração — XProtect Icon Plugin

## Configuração do ambiente de desenvolvimento

### 1. Instalar o SDK do Milestone

Faça o download do **XProtect Smart Client SDK** no portal do desenvolvedor Milestone:
https://developer.milestonesys.com

Instale e anote o caminho — o padrão é:
```
C:\Program Files\Milestone\XProtect Smart Client SDK\
```

### 2. Referenciar DLLs no projeto

Abra `src/Plugin/XProtectIconPlugin.csproj` e confirme os caminhos:

```xml
<HintPath>$(ProgramFiles)\Milestone\XProtect Smart Client SDK\VideoOS.Platform.dll</HintPath>
```

Se o SDK estiver em outro local, ajuste o `HintPath` ou adicione via NuGet:
```
VideoOS.Platform.SDK (disponível no feed NuGet do Milestone)
```

---

## Fluxo de mensagens

```
XProtect Server
     │
     │  Message (ID_SERVER_GENERALREQUEST)
     ▼
EnvironmentManager.Instance
     │
     │  OnMessageReceived()
     ▼
EventSubscriptionService
     │
     │  ParseMessage() → ExternalEventData
     ▼
IconManager.SetStateByDeviceId()
     │
     ├─ IconState.Alarm   → Anel vermelho pulsante
     ├─ IconState.Failure → Anel âmbar tracejado
     └─ IconState.Normal  → Dot verde, sem anel
```

---

## Adicionar um novo ícone via código

```csharp
// Em qualquer lugar que tenha acesso ao IconManager
var config = new IconConfiguration
{
    DisplayName  = "Sensor Porta B2",
    EventType    = "Controle de acesso",
    CameraId     = Guid.Parse("...guid-da-camera..."),
    AlarmPriority = "High",
    AlarmRingColor = "Red",
    AnimationType  = "Pulse",
    ShowNotification = true,
    AutoOpenCamera   = false
};

using var stream = File.OpenRead("sensor_door.png");
_iconManager.Register("sensor_porta_b2", stream, config);
```

---

## Simular um evento externo (para testes)

```csharp
// Injetar mensagem fake no EnvironmentManager para testes locais
var testMessage = new Message(Message.ID_SERVER_GENERALREQUEST)
{
    Data = new Dictionary<string, object>
    {
        { "DeviceId",   "seu-guid-aqui" },
        { "EventType",  "Alarm" },
        { "DeviceName", "Sensor Porta B2" },
        { "Priority",   "High" },
        { "Message",    "Intrusão detectada" }
    }
};

EnvironmentManager.Instance.PostMessage(testMessage);
```

---

## Deploy no Smart Client

1. Compile em modo `Release`
2. Copie o conteúdo de `bin/Release/` para:
   ```
   C:\Program Files\Milestone\XProtect Smart Client\MIPPlugins\XProtectIconPlugin\
   ```
3. Reinicie o XProtect Smart Client
4. Acesse **Configurações → Plugins → External Events Icon Manager**

---

## Formato esperado da mensagem do servidor

O plugin espera mensagens com `Data` sendo um `Dictionary<string, object>`:

| Chave       | Tipo   | Valores possíveis            |
|-------------|--------|------------------------------|
| DeviceId    | string | GUID da câmera/dispositivo   |
| EventType   | string | "Alarm" / "Failure" / "Normal" |
| DeviceName  | string | Nome legível do dispositivo  |
| Priority    | string | "High" / "Medium" / "Low"    |
| Message     | string | Descrição do evento          |

Adapte o método `ParseMessage()` em `EventSubscriptionService.cs` conforme o formato do seu sistema.
