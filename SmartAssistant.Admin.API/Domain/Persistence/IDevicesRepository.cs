namespace SmartAssistant.Admin.API.Domain.Persistence;

public interface IDevicesRepository<TD> where TD : Device
{
    void Add(TD device);
    TD? Get(int id);
    IEnumerable<TD> GetAll();
    void Remove(int id);
}