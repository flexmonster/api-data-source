using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NetCoreServer.Models;

namespace NetCoreServer.Comparators
{
    /// <summary>
    /// Compare months by its name
    /// </summary>
    public class MonthComparator<T> : Comparer<T>
    {
        public override int Compare([AllowNull] T x, [AllowNull] T y)
        {

            if (x == null || y == null) return -1;
            if (Enum.TryParse(x.ToString(), out Month xInEnum))
            {
                if (Enum.TryParse(y.ToString(), out Month yInEnum))
                {
                    return xInEnum.CompareTo(yInEnum);
                }
            }
            if (Enum.TryParse(x.ToString(), out ShortMonth xInShortEnum))
            {
                if (Enum.TryParse(y.ToString(), out ShortMonth yInShortEnum))
                {
                    return xInShortEnum.CompareTo(yInShortEnum);
                }
            }
            return -1;
        }
    }
}
