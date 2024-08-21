using SmartAssistant.Admin.API.Domain.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace SmartAssistant.Admin.API.Triggers;

public class LightsTriggers
{
    private readonly ILightsRepository _repo;
    private readonly ILogger<LightsTriggers> _logger;

    public LightsTriggers(ILightsRepository repo, ILogger<LightsTriggers> logger)
    {
        _logger = logger;
        _repo = repo;
    }

    [Function(nameof(GetLightById))]
    public IActionResult GetLightById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "lights/{lightId:int}")] HttpRequestData req, int lightId)
    {
        var light = _repo.Get(lightId);
        if (light is null)
            return new NotFoundResult();
        return new OkObjectResult(light);
    }

    [Function(nameof(ChangeLightState))]
    public async Task<IActionResult> ChangeLightState(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "lights/{lightId:int}/state")] HttpRequestData req, 
        int lightId)
    {
        var newState = await req.ReadFromJsonAsync<bool>().ConfigureAwait(false);

        var light = _repo.Get(lightId);
        if (light is null)
            return new NotFoundResult();
        light.IsOn = newState;
        return new OkObjectResult(light);
    }

    [Function(nameof(GetAllLights))]
    public IActionResult GetAllLights([HttpTrigger(AuthorizationLevel.Function, "get", Route = "lights")] HttpRequest req)
    {
        var lights = _repo.GetAll();
        return new OkObjectResult(lights);
    }
}
