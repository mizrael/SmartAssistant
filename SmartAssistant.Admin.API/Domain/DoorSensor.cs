namespace SmartAssistant.Admin.API.Domain;

public record DoorSensor : Device
{
    public bool IsOpen { get; set; }
}