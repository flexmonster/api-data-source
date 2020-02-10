using System.Linq;
using NetCoreServer.Models;
using NetCoreServer.Models.DataModels;
using NetCoreServer.Parsers;


namespace NetCoreServer.DataLoaders
{
    public class DataLoader : IDataLoader
    {
        private readonly IParser _parser;
        /// <summary>
        /// Parse data with given Parser
        /// </summary>
        /// <param name="parser">Parser</param>
        public DataLoader(IParser parser)
        {
            _parser = parser;
        }
        /// <summary>
        /// Load data from data source
        /// </summary>
        /// <returns></returns>
        public IDataStructure Load()
        {
            var data = new ColumnListDataStructure();
            foreach (var dataBlock in _parser.Parse())
            {
                if (data.GetColumnNames().Count == 0)
                {
                    data.AddColumns(dataBlock.Keys.ToList());
                }
                data.AddBlock(dataBlock);
            }
            return data;
        }
    }
}
