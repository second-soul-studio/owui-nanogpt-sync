using Flurl;
using OWUINanoGPTSync.Configuration;
using OWUINanoGPTSync.SystemPrompt;

namespace OWUINanoGPTSync.Transformation;

internal static class ModelTransformer
{
    private static readonly string _baseUrl;

    static ModelTransformer()
    {
        _baseUrl = LocalConfigurationProvider.NanoGPTBaseUrl;
    }

    public static (bool, dynamic) Transform(dynamic baseModel, dynamic? configuredBaseModel, dynamic nanoGPTModel)
    {
        var create = configuredBaseModel == null;

        var modelId = (string)nanoGPTModel.id;

        var profileImageUrl =
            string.IsNullOrWhiteSpace((string)nanoGPTModel.icon_url)
                ? "/static/favicon.png"
                : new Url(_baseUrl).AppendPathSegment(nanoGPTModel.icon_url).ToString();

        var hasVision =
            nanoGPTModel.capabilities.vision != null && (bool)nanoGPTModel.capabilities.vision;

        var actualBase = configuredBaseModel ?? baseModel;

        var model = new Dictionary<string, dynamic>
        {
            ["id"] = modelId,
            ["object"] = (string)baseModel["object"],
            ["updated_at"] = (int?)actualBase["updated_at"],
            ["created_at"] = (int?)baseModel["created"],
            ["owned_by"] = (string)baseModel["owned_by"],
            ["connection_type"] = (string)baseModel["connection_type"],
            ["name"] = (string)actualBase["name"],
            ["openai"] = baseModel["openai"],
            ["access_control"] = baseModel["access_control"],
            ["actions"] = baseModel["actions"],
            ["filters"] = baseModel["filters"]
        };

        model["meta"] = new Dictionary<string, dynamic>();

        model["params"] = new Dictionary<string, dynamic>();

        model["meta"]["tags"] = baseModel["tags"];

        model["meta"]["capabilities"] = new Dictionary<string, bool>
        {
            ["file_upload"] = true,
            ["web_search"] = true,
            ["image_generation"] = true,
            ["code_interpreter"] = true,
            ["citations"] = true,
            ["status_updates"] = true,
            ["usage"] = true,
            ["vision"] = hasVision
        };

        model["id"] = modelId;

        model["name"] = nanoGPTModel.name;
        model["meta"]["description"] = nanoGPTModel.description;
        model["meta"]["profile_image_url"] = profileImageUrl;

        model["params"]["system"] = SystemPromptProvider.GetSystemPrompt(modelId);
        model["params"]["num_ctx"] = nanoGPTModel.context_length;

        return (create, model);
    }
}