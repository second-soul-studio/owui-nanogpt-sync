using System.Text;
using Flurl;
using Newtonsoft.Json;
using OWUINanoGPTSync.Configuration;

namespace OWUINanoGPTSync.APIAccess;

internal static class OWUIAccessor
{
    private static readonly HttpClient _httpClient = new();

    private static readonly Url _allBaseModelsUrl;

    private static readonly Url _allConfiguredBaseModelsUrl;

    private static readonly Url _createModelUrl;

    private static readonly Url _updateModelUrl;

    static OWUIAccessor()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {LocalConfigurationProvider.OWUIApiKey}");

        var baseUrl = LocalConfigurationProvider.OWUIBaseUrl;

        _allBaseModelsUrl =
            new Url(baseUrl).AppendPathSegment("api/v1/models");

        _allConfiguredBaseModelsUrl =
            new Url(baseUrl).AppendPathSegment("api/v1/models/base");

        _createModelUrl =
            new Url(baseUrl).AppendPathSegment("api/v1/models/create");

        _updateModelUrl =
            new Url(baseUrl).AppendPathSegment("api/v1/models/model/update");
    }

    public static IReadOnlyDictionary<string, dynamic> GetAllBaseModels()
    {
        var response = _httpClient.GetAsync(_allBaseModelsUrl).Result;
        var content = response.Content.ReadAsStringAsync().Result;

        var deserialized = JsonConvert.DeserializeObject<dynamic>(content);

        var data = deserialized["data"]?.ToObject<List<dynamic>>();

        if (data == null)
        {
            throw new Exception("failed to retrieve base models from OWUI");
        }

        var result = new Dictionary<string, dynamic>();

        foreach (var item in data)
        {
            result[(string)item.id] = item;
        }

        return result;
    }

    public static IReadOnlyDictionary<string, dynamic> GetAllConfiguredBaseModels()
    {
        var response = _httpClient.GetAsync(_allConfiguredBaseModelsUrl).Result;
        var content = response.Content.ReadAsStringAsync().Result;

        var deserialized = JsonConvert.DeserializeObject<dynamic>(content);

        if (deserialized == null)
        {
            throw new Exception("failed to retrieve configured base models from OWUI");
        }

        var result = new Dictionary<string, dynamic>();

        foreach (var item in deserialized)
        {
            result[(string)item.id] = item;
        }

        return result;
    }

    public static dynamic CreateModel(dynamic payload)
    {
        var serialized = JsonConvert.SerializeObject(payload);

        var response = _httpClient.PostAsync(_createModelUrl,
            new StringContent(serialized, Encoding.UTF8, "application/json")).Result;
        var content = response.Content.ReadAsStringAsync().Result;

        var deserialized = JsonConvert.DeserializeObject<dynamic>(content);

        if (deserialized == null)
        {
            throw new Exception("failed to retrieve configured base models from OWUI");
        }

        return deserialized;
    }

    public static dynamic UpdateModel(dynamic payload)
    {
        var serialized = JsonConvert.SerializeObject(payload);

        var response = _httpClient.PostAsync(_updateModelUrl,
            new StringContent(serialized, Encoding.UTF8, "application/json")).Result;

        var content = response.Content.ReadAsStringAsync().Result;

        var deserialized = JsonConvert.DeserializeObject<dynamic>(content);

        if (deserialized == null)
        {
            throw new Exception("failed to retrieve configured base models from OWUI");
        }

        return deserialized;
    }
}