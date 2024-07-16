using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsLines.objects
{
    public static class Utils
    {
        public static (int, int) GetOrderedTuple(int v1, int v2) => v1 < v2 ? (v1, v2) : (v2, v1);
    }
}
