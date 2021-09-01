#if !TR_PARSEONLY
 using UnityEngine;
 using UnityEngine.UI;
 using UnityEngine.EventSystems;
#endif
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public class ErrorLogger
    {
        private static List<string> errorMessage = new List<string>();
        private static List<string> warningMessage = new List<string>();

        public static void Clear()
        {
            errorMessage.Clear();
            warningMessage.Clear();
        }

        public static void AddLog(string message, string file, int line, bool stop)
        {
            if (stop)
            {
                string str = "<color=green>Novel</color>[" + file + "]:<color=red>Error:" + line + "行目 " + message + "</color>\n";
                errorMessage.Add(str);
            }
            else
            {
                string str = "<color=green>Novel</color>[" + file + "]:<color=yellow>Warning:" + line + "行目 " + message + "</color>\n";
                warningMessage.Add(str);
            }
        }

        public static bool ShowAll()
        {
            foreach (string message in errorMessage)
            {
#if !TR_PARSEONLY
                UnityEngine.Debug.LogError(message);
#else
                Debug.WriteLine(message);
#endif
            }
            foreach (string message in warningMessage)
            {
#if !TR_PARSEONLY
                UnityEngine.Debug.LogWarning(message);
#else
                Debug.WriteLine(message);
#endif
            }
            return errorMessage.Count > 0 ? true : false;
        }

        public static void StopError(string message)
        {
#if !TR_PARSEONLY
            UnityEngine.Debug.LogError(message);
            throw new UnityException(message);
#else
            Debug.WriteLine(message);
            throw new Exception(message);
#endif
        }

        [Conditional("TR_DEBUG")]
        public static void Log(string message, bool autoReturn = true)
        {
#if !TR_PARSEONLY
            UnityEngine.Debug.Log(message + (autoReturn ? "\n" : ""));
#else
            Debug.WriteLine(message);
#endif
        }
    };
}
