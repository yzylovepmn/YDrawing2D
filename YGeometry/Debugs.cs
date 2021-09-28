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

        public static void Assert(bool value)
        {
#if DEBUG
            Debug.Assert(value);
#endif
        }

        public static void Log(string msg)
        {
#if DEBUG
            Debug.WriteLine(msg);
#endif
        }
    }
}