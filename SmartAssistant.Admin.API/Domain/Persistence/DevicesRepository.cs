using System.Collections.Concurrent;

namespace SmartAssistant.Admin.API.Domain.Persistence;

internal abstract class DevicesRepository<TD> : IDevicesRepository<TD> where TD : Device
{
    private readonly ConcurrentDictionary<int, TD> _devices = new();

    public TD? Get(int id)
        => _devices.TryGetValue(id, out var device) ? device : default;

    public void Add(TD device)
    {
        ArgumentNullException.ThrowIfNull(device, nameof(device));
        if (_devices.ContainsKey(device.Id))
            throw new InvalidOperationException($"Device with id {device.Id} already exists.");
        _devices.TryAdd(device.Id, device);
    }

    public void Remove(int id)
        => _devices.TryRemove(id, out _);

    public IEnumerable<TD> GetAll()
        => _devices.Values;
}
