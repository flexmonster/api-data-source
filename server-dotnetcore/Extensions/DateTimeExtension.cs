using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreServer.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Convert date to Unix timestamp
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static double ToUnixTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.Kind == DateTimeKind.Local ? date.ToUniversalTime() - origin : date - origin;
            return Math.Floor(diff.TotalMilliseconds);
        }
    }
}
