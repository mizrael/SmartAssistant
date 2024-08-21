namespace SmartAssistant.Admin.API.Domain;

public abstract record Device
{
    public int Id { get; init; }
    public string Name { get; init; }
}
