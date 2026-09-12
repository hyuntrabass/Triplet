using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Scripts.Core
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list != null && list.Count() == 0;
        }
    }
}
