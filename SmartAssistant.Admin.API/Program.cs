using SmartAssistant.Admin.API.Domain;
using SmartAssistant.Admin.API.Domain.Persistence;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton<ILightsRepository, LightsRepository>()
                .AddSingleton<IDoorSensorsRepository, DoorSensorsRepository>();
    })
    .Build();

// feed some fake data
var lightsRepo = host.Services.GetRequiredService<ILightsRepository>();
lightsRepo.Add(new Light { Id = 1, Name = "Bedroom", IsOn = false });
lightsRepo.Add(new Light { Id = 2, Name = "Living Room", IsOn = true });
lightsRepo.Add(new Light { Id = 3, Name = "Bathroom", IsOn = false });
lightsRepo.Add(new Light { Id = 4, Name = "Garage", IsOn = true });

var sensorsRepo = host.Services.GetRequiredService<IDoorSensorsRepository>();
sensorsRepo.Add(new DoorSensor { Id = 1, Name = "Front door" });
sensorsRepo.Add(new DoorSensor { Id = 2, Name = "Back door" });
sensorsRepo.Add(new DoorSensor { Id = 3, Name = "Garage door" });

host.Run();
