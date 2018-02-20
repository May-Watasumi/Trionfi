using UnityEditor;
using UnityEngine;
using EditorCoroutines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NovelEx;

public class TRDebugger : EditorWindow
{
    string consoleLog;
    Vector2 logScroll;

    [MenuItem("Trionfi/Debug")]
    private static void Open()
    {
        var window = GetWindow<TRDebugger>("TrinofiDebugger");
    }

    string scriptText;

    private void OnGUI()
    {
        scriptText = EditorGUILayout.TextField("スクリプト", scriptText);
        if (GUILayout.Button("実行"))
        {
            AbstractComponent abs = TRScriptParser.Instance.makeTag(scriptText);
            if (abs != null)
            {
                Debug.Log("Tag: " + abs.tagName);

                abs.Execute();
                this.StartCoroutine(abs.TagAsyncWait());
            }
            else
                Debug.Log("Invalid Tag!");
        }

        GUILayout.TextField("ログ最大サイズ");

        if (GUILayout.Button("ログ削除"))
        {
            consoleLog = "";
        }

        logScroll = EditorGUILayout.BeginScrollView(logScroll);
        consoleLog = EditorGUILayout.TextArea(consoleLog, GUILayout.Height(position.height - 30));
        EditorGUILayout.EndScrollView();
    }
}
