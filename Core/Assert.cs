using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ModU3DToolkit.Core
{
	public class Asserts
	{
        public static void Assert(bool value)
        {
            if (UnityEngine.Debug.isDebugBuild && !value)
            {
                StackTrace st = new StackTrace(true);
                string filename = "empty";
                int line = -1;
                if (st.FrameCount > 1) {
                    filename = st.GetFrame(1).GetFileName();
                    line     = st.GetFrame(1).GetFileLineNumber();
                }
                UnityEngine.Debug.LogError("assertion failed @ " + filename + " (" + line.ToString() + ")");
                UnityEngine.Debug.Break();
            }
        }

        public static void Assert(bool value, string msg)
        {
            if (UnityEngine.Debug.isDebugBuild && !value)
            {
                StackTrace st = new StackTrace(true);
                string filename = "empty";
                int line = -1;
                if (st.FrameCount > 1)
                {
                    filename = st.GetFrame(1).GetFileName();
                    line = st.GetFrame(1).GetFileLineNumber();
                }
                UnityEngine.Debug.LogError("assertion failed @ " + filename + " (" + line.ToString() + ")\nmessage: " + msg);
                UnityEngine.Debug.Break();
            }
        }
    }
}
