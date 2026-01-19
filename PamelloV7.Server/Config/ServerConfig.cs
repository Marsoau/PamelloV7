using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using PamelloV7.Core.Attributes;

namespace PamelloV7.Server.Config;

[StaticConfigPart("Server")]
public static class ServerConfig
{
    public static Root.Root Root { get; private set; }
}
