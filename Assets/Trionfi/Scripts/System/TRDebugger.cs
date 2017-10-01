using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NovelEx;

public class TRDebugger : EditorWindow
{
#if false
    [MenuItem("Trionfi/Debug/ScriptTest")]
    private static void Open()
    {
        var window = GetWindow<TRDebugger>("スクリプト実行");
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
                IEnumerator t = abs.Start();
                while(t.MoveNext())
                {
                    Debug.Log("Current: " + t.Current);
                }
            }
            else
                Debug.Log("Invalid Tag!");

/*
            while (t.MoveNext())
            {
                Debug.Log("Current: " + t.Current);
            }
*/
        }
    }
#endif
}
