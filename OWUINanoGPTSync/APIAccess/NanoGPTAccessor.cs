using Flurl;
using Newtonsoft.Json;
using OWUINanoGPTSync.Configuration;

namespace OWUINanoGPTSync.APIAccess;

internal static class NanoGPTAccessor
{
    private static readonly HttpClient _httpClient = new();

    private static readonly Url _retrievalUrl;

    static NanoGPTAccessor()
    {
        _retrievalUrl =
            new Url(LocalConfigurationProvider.NanoGPTBaseUrl)
                .AppendPathSegment("api/v1/models")
                .AppendQueryParam("detailed", "true");
    }

    public static IReadOnlyDictionary<string, dynamic> GetAllModels()
    {
        var response = _httpClient.GetAsync(_retrievalUrl).Result;
        var content = response.Content.ReadAsStringAsync().Result;

        var data = JsonConvert.DeserializeObject<dynamic>(content)?["data"];

        if (data == null)
        {
            throw new Exception("failed to retrieve models from NanoGPT");
        }

        var result = new Dictionary<string, dynamic>();

        foreach (var model in data)
        {
            var id = (string)model["id"];

            result[id] = model;
        }

        return result;
    }
}