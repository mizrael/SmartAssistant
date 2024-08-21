using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Net.Http.Json;

namespace SmartAssistant.Agent;

public class DoorSensorsPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DoorSensorsPlugin> _logger;

    public DoorSensorsPlugin(HttpClient httpClient, ILogger<DoorSensorsPlugin> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    [KernelFunction("get_door_sensors")]
    [Description("Gets a list of door sensors and their current state")]
    [return: Description("A collection of door sensors")]
    public async Task<IEnumerable<DoorSensor>> GetDoorSensors()
    {
        _logger.LogInformation("Getting all the door sensors...");

        var response = await _httpClient.GetAsync("api/doorsensors");
        response.EnsureSuccessStatusCode();
        var sensors = await response.Content.ReadFromJsonAsync<IEnumerable<DoorSensor>>()
                                           .ConfigureAwait(false);
        return sensors ?? Array.Empty<DoorSensor>();
    }

    [KernelFunction("get_door_sensor_details")]
    [Description("gets the details of a specific doors ensor by id")]
    [return: Description("the details of a specific door sensor")]
    public async Task<DoorSensor?> GetById([Description("The ID of the door sensor")] int id)
    {
        _logger.LogInformation("Getting details for door sensor {id}...", id);

        var response = await _httpClient.GetAsync($"api/doorsensors/{id}");
        response.EnsureSuccessStatusCode();
        var sensor = await response.Content.ReadFromJsonAsync<DoorSensor>()
                                          .ConfigureAwait(false);
        return sensor;
    }

    [KernelFunction("set_door_sensor_state")]
    [Description("updates the state of a specific door sensor by id")]
    [return: Description("the new details of a specific door sensor")]
    public async Task<DoorSensor?> SetLightState(
        [Description("The ID of the door sensor")] int id, 
        [Description("the new state for the door sensor")] bool newState)
    {
        _logger.LogInformation("Setting state for door sensor {id} to {newState}...", id, newState);

        var response = await _httpClient.PostAsJsonAsync($"api/doorsensors/{id}/state", newState);
        var sensor = await response.Content.ReadFromJsonAsync<DoorSensor>()
                                          .ConfigureAwait(false);
        return sensor;
    }
}
