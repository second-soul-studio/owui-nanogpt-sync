using Microsoft.Extensions.Configuration;

namespace OWUINanoGPTSync.Configuration;

internal static class LocalConfigurationProvider
{
    private static readonly IConfigurationRoot _configuration;

    static LocalConfigurationProvider()
    {
        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>();

        _configuration = builder.Build();
    }

    public static string OWUIBaseUrl => GetSetting("OWUIBASEURL", "http://localhost:8080");

    public static string OWUIApiKey => GetRequiredSetting("OWUIAPIKEY");

    public static string NanoGPTBaseUrl => GetSetting("NANOGPTBASEURL", "https://nano-gpt.com");

    public static string SystemPromptFile => GetSetting("SYSTEMPROMPTFILE", "");

    public static TimeSpan Interval => TimeSpan.Parse(GetSetting("INTERVAL", "01:00:00"));

    private static string GetSetting(string key, string fallback)
    {
        return _configuration[key] ?? fallback;
    }

    private static string GetRequiredSetting(string key)
    {
        return _configuration[key] ?? throw new Exception($"missing required environment variable: {key}");
    }
}