using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class IntegerExtensions
    {
        public static int GoesTo(this int first, int second)
        {
            if (first != second)
                first += (first < second) ? 1 : -1;

            return first;
        }
    }
}
