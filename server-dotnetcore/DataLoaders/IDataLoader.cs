using NetCoreServer.Models.DataModels;

namespace NetCoreServer.DataLoaders
{
    public interface IDataLoader
    {
        IDataStructure Load();
    }
}