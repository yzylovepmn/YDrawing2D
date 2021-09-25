using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace YGeometry
{
    public class Debugs
    {
        // [low, high)
        public static void CheckRange(int value, int low, int high)
        {
#if DEBUG
            Debug.Assert(value >= low && value < high);
#endif
        }
    }
}