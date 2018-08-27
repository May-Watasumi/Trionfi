using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
                UnityEngine.Debug.LogError(message);
            }
            foreach (string message in warningMessage)
            {
                UnityEngine.Debug.LogWarning(message);
            }
            return errorMessage.Count > 0 ? true : false;
        }

        public static void StopError(string message)
        {
            UnityEngine.Debug.LogError(message);
            throw new UnityException(message);
        }

        [Conditional("TR_DEBUG")]
        public static void Log(string message, bool autoReturn = true)
        {
            UnityEngine.Debug.Log(message + (autoReturn ? "\n" : ""));
        }
    };
}
