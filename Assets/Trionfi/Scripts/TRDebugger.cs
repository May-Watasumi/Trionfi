﻿
#if UNITY_EDITOR
using UnityEditor;
using EditorCoroutines;
#endif

using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Trionfi
{
    public enum MessageType { Error, Warning, Info };

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

#if UNITY_EDITOR
    public class TRDebugger : EditorWindow
    {
        string consoleLog;
        Vector2 logScroll;

        [MenuItem("Trionfi/Debug")]
        private static void Open()
        {
            var window = GetWindow<TRDebugger>("TrinofiDebugger");
        }

        [MenuItem("Trionfi/OpenScriptFile")]
        private static void ExecuteScriptFile()
        {
            
            string path = EditorUtility.OpenFilePanel("シナリオファイル", Application.dataPath, "txt");
            if (path.Length != 0)
            {
                string fileContent = File.ReadAllText(path);
                Trionfi.Instance.currentTagInstance.CompileScriptString(fileContent);
            }
        }

        [MenuItem("Trionfi/Reset PlayerPrefs")]
        public static void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        string scriptText;

        private void OnGUI()
        {
            scriptText = EditorGUILayout.TextField("スクリプト", scriptText);
            if (GUILayout.Button("実行"))
            {
                TRTagParser tagParser = new TRTagParser(scriptText);
                AbstractComponent tagComponent = tagParser.Parse();
                if(tagComponent != null)
                {
                    consoleLog += ("Tag: " + tagComponent.tagName + "\n");

                    tagComponent.Execute();
                    this.StartCoroutine(tagComponent.TagAsyncWait());
                }
                else
                    consoleLog += ("Invalid Tag!");
            }

            GUILayout.Label("ログ最大サイズ");

            //        GUILayout.TextField("ログ最大サイズ");

            if (GUILayout.Button("ログ削除"))
            {
                consoleLog = "";
            }

            logScroll = EditorGUILayout.BeginScrollView(logScroll);
            consoleLog = EditorGUILayout.TextArea(consoleLog, GUILayout.Height(position.height - 30));
            EditorGUILayout.EndScrollView();
        }
    }
#endif
}