using NetCoreServer.Models.DataModels;
using System.Collections.Generic;

namespace NetCoreServer.Parsers
{
    public interface IParser
    {
        /// <summary>
        /// Parse block of data
        /// </summary>
        IEnumerable<Dictionary<string, dynamic>> Parse();

        Dictionary<string, ColumnType> DataTypes { get; }
    }
}