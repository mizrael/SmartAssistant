using SmartAssistant.Admin.API.Domain.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace SmartAssistant.Admin.API.Triggers;

public class DoorSensorsTriggers
{
    private readonly IDoorSensorsRepository _repo;
    private readonly ILogger<DoorSensorsTriggers> _logger;

    public DoorSensorsTriggers(IDoorSensorsRepository repo, ILogger<DoorSensorsTriggers> logger)
    {
        _logger = logger;
        _repo = repo;
    }

    [Function(nameof(GetDoorSensorById))]
    public IActionResult GetDoorSensorById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "doorsensors/{sensorId:int}")] HttpRequestData req, int sensorId)
    {
        var sensor = _repo.Get(sensorId);
        if (sensor is null)
            return new NotFoundResult();
        return new OkObjectResult(sensor);
    }

    [Function(nameof(ChangeDoorSensorState))]
    public async Task<IActionResult> ChangeDoorSensorState(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "doorsensors/{sensorId:int}/state")] HttpRequestData req, 
        int sensorId)
    {
        var newState = await req.ReadFromJsonAsync<bool>().ConfigureAwait(false);

        var sensor = _repo.Get(sensorId);
        if (sensor is null)
            return new NotFoundResult();
        sensor.IsOpen = newState;
        return new OkObjectResult(sensor);
    }

    [Function(nameof(GetAllDoorSensors))]
    public IActionResult GetAllDoorSensors([HttpTrigger(AuthorizationLevel.Function, "get", Route = "doorsensors")] HttpRequest req)
    {
        var sensors = _repo.GetAll();
        return new OkObjectResult(sensors);
    }
}
