namespace SmartAssistant.Admin.API.Domain;

public record Light : Device
{
    public bool IsOn { get; set; }
}

