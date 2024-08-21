using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Net.Http.Json;

namespace SmartAssistant.Agent.Plugins;

public class LightsPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LightsPlugin> _logger;

    public LightsPlugin(HttpClient httpClient, ILogger<LightsPlugin> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    [KernelFunction("get_lights")]
    [Description("Gets a list of lights and their current state")]
    [return: Description("A collection of lights")]
    public async Task<IEnumerable<Light>> GetLightsAsync()
    {
        _logger.LogInformation("Getting all the lights...");

        var response = await _httpClient.GetAsync("api/lights");
        response.EnsureSuccessStatusCode();
        var lights = await response.Content.ReadFromJsonAsync<IEnumerable<Light>>()
                                           .ConfigureAwait(false);
        return lights ?? Array.Empty<Light>();
    }

    [KernelFunction("get_light_details")]
    [Description("gets the details of a specific light by id")]
    [return: Description("the details of a specific light")]
    public async Task<Light?> GetById([Description("The ID of the light")] int id)
    {
        _logger.LogInformation("Getting details for light {id}...", id);

        var response = await _httpClient.GetAsync($"api/lights/{id}");
        response.EnsureSuccessStatusCode();
        var light = await response.Content.ReadFromJsonAsync<Light>()
                                          .ConfigureAwait(false);
        return light;
    }

    [KernelFunction("set_light_state")]
    [Description("updates the state of a specific light by id")]
    [return: Description("the new details of a specific light")]
    public async Task<Light?> SetLightState(
        [Description("The ID of the light")] int id,
        [Description("the new state for the light")] bool newState)
    {
        _logger.LogInformation("Setting state for light {id} to {newState}...", id, newState);

        var response = await _httpClient.PostAsJsonAsync($"api/lights/{id}/state", newState);
        var light = await response.Content.ReadFromJsonAsync<Light>()
                                          .ConfigureAwait(false);
        return light;
    }
}
