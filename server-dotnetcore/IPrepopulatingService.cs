using System.Threading.Tasks;

namespace NetCoreServer
{
    public interface IPrepopulatingService
    {
        public Task Prepopulate();
    }
}