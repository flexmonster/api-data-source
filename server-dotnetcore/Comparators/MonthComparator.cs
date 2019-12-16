using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using NetCoreServer.Models;

namespace NetCoreServer.Comparators
{
    public class MonthComparator : Comparer<Value>
    {
        public override int Compare([AllowNull] Value x, [AllowNull] Value y)
        {
            if (x == null || y == null) return -1;
            var xInEnum = (Month)Enum.Parse(typeof(Month), x.StringValue, true);
            var yInEnum = (Month)Enum.Parse(typeof(Month), y.StringValue, true);
            return xInEnum.CompareTo(yInEnum);
        }
    }
}
