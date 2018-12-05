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

        [MenuItem("Tools/Trionfi/OpenScriptFile")]
        private static void ExecuteScriptFile()
        {
            string path = EditorUtility.OpenFilePanel("シナリオファイル", Application.dataPath, "txt");
            if (path.Length != 0)
            {
                string _name = Regex.Replace(path, ".*Assets", "Assets");//  Path.GetFileNameWithoutExtension(path);
                string storage = Path.GetFileNameWithoutExtension(path);

                TextAsset _text = AssetDatabase.LoadAssetAtPath<TextAsset>(_name);

                TRTagInstance _instance = new TRTagInstance();
                _instance.CompileScriptString(_text.text);

                TRVirtualMachine.tagInstances[storage] = _instance;
                Trionfi.Instance.StartCoroutine(TRVirtualMachine.Instance.Run(storage));
            }
        }

        [MenuItem("Tools/Trionfi/Reset PlayerPrefs")]
        public static void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        private void OnGUI()
        {
            GUILayout.Label("1行スクリプト（[]は不要）");

            statementText = EditorGUILayout.TextField("", statementText);
            if (GUILayout.Button("実行"))
            {
                TRTagParser tagParser = new TRTagParser(statementText);
                AbstractComponent tagComponent = tagParser.Parse();
                if (tagComponent != null)
                {
                    consoleLog += ("Tag: " + tagComponent.tagName + "\n");

                    tagComponent.Execute();
                    Trionfi.Instance.StartCoroutine(tagComponent.TagSyncFunction());
                }
                else
                    consoleLog += ("Invalid Tag!\n");
            }

            GUILayout.Space(20);
            GUILayout.Label("expression Test");

            expressionText = EditorGUILayout.TextField("", expressionText);
            if (GUILayout.Button("実行"))
            {
                VariableCalcurator _var = TRVirtualMachine.Calc(expressionText, TRVirtualMachine.variableInstance);
                UnityEngine.Debug.Log(_var.paramString);
            }

            GUILayout.Space(20);
            GUILayout.Label("【デバッグログ】");

            GUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label("最大サイズ");
            //        GUILayout.TextField("ログ最大サイズ");
            if (GUILayout.Button("クリア"))
            {
                consoleLog = "";
            }

            GUILayout.EndHorizontal();

            logScroll = EditorGUILayout.BeginScrollView(logScroll);
            consoleLog = EditorGUILayout.TextArea(consoleLog, GUILayout.Height(200));
            EditorGUILayout.EndScrollView();

            GUILayout.Space(20);
            GUILayout.Label("【スクリプトテスト】");

            GUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label("内容");
            if (GUILayout.Button("実行"))
            {
                TRTagInstance _tagInstance = new TRTagInstance();

                if (_tagInstance.CompileScriptString(scriptText))
                {
                    TRVirtualMachine.tagInstances[_TEMPSCRIPTNAME_] = _tagInstance;
                    Trionfi.Instance.StartCoroutine(TRVirtualMachine.Instance.Run(_TEMPSCRIPTNAME_));
                }
                else
                    consoleLog += ("failed compile\n");
            }

            GUILayout.EndHorizontal();

            scriptScroll = EditorGUILayout.BeginScrollView(scriptScroll);
            scriptText = EditorGUILayout.TextArea(scriptText, GUILayout.Height(200));
            EditorGUILayout.EndScrollView();
        }
    }
}
