using System.Text.Json.Serialization;

namespace SimpleRandomTeams.Configuration
{
    public struct ConfigurationSettings
    {
        [JsonPropertyName("token")] public string Token { get; set; }

        [JsonPropertyName("prefix")] public string CommandPrefix { get; set; }
    }
}