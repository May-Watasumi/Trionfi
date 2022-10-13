using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Jace.Operations;

namespace Trionfi
{
    [CustomPropertyDrawer(typeof(Trionfi.TRAudioInstance))]
    [CustomPropertyDrawer(typeof(Trionfi.TRImageInstance))]
    public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

    public class TRDebugger : EditorWindow
    {
        const string _TEMPSCRIPTNAME_ = "__temp__";

        string consoleLog;
        Vector2 logScroll;

        string scriptText;
        Vector2 scriptScroll;

        string statementText;
        string expressionText;

        [MenuItem("Tools/Trionfi/DebugWindow")]
        private static void Open()
        {
            GetWindow<TRDebugger>("Trinofi DebugWindow");
        }

        [MenuItem("Tools/Trionfi/Reset PlayerPrefs")]
        public static void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        private void OnGUI()
        {
            GUILayout.Label("1-line script(without [])");

            statementText = EditorGUILayout.TextField("", statementText);
            if (GUILayout.Button("Execute"))
            {
                AbstractComponent tagComponent = Trionfi.Instance.GetTagComponent(statementText);

                if (tagComponent != null)
                {
                    consoleLog += ("Tag: " + tagComponent.tagName + "\n");
                    Trionfi.Instance.StartCoroutine(tagComponent.Execute());
                }
                else
                    consoleLog += ("Invalid Tag!\n");
            }

            GUILayout.Space(20);
            GUILayout.Label("expression Test");

            expressionText = EditorGUILayout.TextField("", expressionText);
            if (GUILayout.Button("Execute"))
            {
                VariableCalcurator _var = TRVirtualMachine.Instance.Evaluation(expressionText);
                UnityEngine.Debug.Log(_var.paramString);
            }

            GUILayout.Space(20);
            GUILayout.Label("<Debug Log>");

            GUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label("Max size");
            //        GUILayout.TextField("ログ最大サイズ");
            if (GUILayout.Button("Clear"))
            {
                consoleLog = "";
            }

            GUILayout.EndHorizontal();

            logScroll = EditorGUILayout.BeginScrollView(logScroll);
            consoleLog = EditorGUILayout.TextArea(consoleLog, GUILayout.Height(200));
            EditorGUILayout.EndScrollView();


            if (GUILayout.Button("HotLoad"))
            {
                if (TRVirtualMachine.Instance.state == TRVirtualMachine.State.Run)
                    TRVirtualMachine.Instance.BeginReboot();
            }

        }
    }
}
