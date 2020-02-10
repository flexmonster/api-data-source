using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NetCoreServer.Models.DataModels;
using NetCoreServer.Models;

namespace NetCoreServer.Comparators
{
    /// <summary>
    /// Compare months by its name
    /// </summary>
    public class MonthComparator : Comparer<Value>
    {
        public override int Compare([AllowNull] Value x, [AllowNull] Value y)
        {
            if (x == null || y == null) return -1;
            if (Enum.TryParse(x.StringValue, out Month xInEnum))
            {
                if (Enum.TryParse(y.StringValue, out Month yInEnum))
                {
                    return xInEnum.CompareTo(yInEnum);
                }
            }
            if (Enum.TryParse(x.StringValue, out ShortMonth xInShortEnum))
            {
                if (Enum.TryParse(y.StringValue, out ShortMonth yInShortEnum))
                {
                    return xInShortEnum.CompareTo(yInShortEnum);
                }
            }
            return -1;
        }
    }
}
