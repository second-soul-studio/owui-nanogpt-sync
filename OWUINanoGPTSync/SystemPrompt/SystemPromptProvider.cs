using OWUINanoGPTSync.Configuration;

namespace OWUINanoGPTSync.SystemPrompt;

internal sealed class SystemPromptProvider
{
    private static readonly string _systemPrompt;

    static SystemPromptProvider()
    {
        var systemPromptFile = LocalConfigurationProvider.SystemPromptFile;

        if (!string.IsNullOrWhiteSpace(systemPromptFile))
        {
            _systemPrompt = File.ReadAllText(systemPromptFile);
        }
        else
        {
            _systemPrompt = File.ReadAllText("default-systemprompt.txt");
        }
    }

    public static string GetSystemPrompt(string modelId)
    {
        // todo: add functionality for model specific (jailbreaking, etc...) system prompts
        return _systemPrompt;
    }
}