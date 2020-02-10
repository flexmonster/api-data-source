using NetCoreServer.Models;
using System.Collections.Generic;

namespace NetCoreServer.Parsers
{
    public interface IParser
    {
        /// <summary>
        /// Parse block of data
        /// </summary>
        IEnumerable<Dictionary<string, List<Value>>> Parse();
    }
}
