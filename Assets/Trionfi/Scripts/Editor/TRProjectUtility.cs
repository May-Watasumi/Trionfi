using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// テキスト入力Window
public class TRProjectUtility: EditorWindow
{
    string _input = "";
    bool _isInit = false;


    [MenuItem("Tools/Trionfi/New Project")]
    private static void Open()
    {
        GetWindow<TRProjectUtility>("Input ProjectName");
    }

    private void SetupProject(string name)
    {
        AssetDatabase.CopyAsset("Assets/Trionfi/Template", "Assets/Trionfi/"+name);
    }

    void OnGUI()
    {
        GUILayout.Label("新しいプロジェクト名");
        GUILayout.Space(10f);

        GUI.SetNextControlName("ForcusField");
        _input = GUILayout.TextField(_input);
        GUILayout.Space(10f);

        // 何かしら入力しないとOKボタンを押せないようにするDisableGroup
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_input));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK", GUILayout.Height(30f)))
        {
            SetupProject(_input);
            Close();
        }

        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button("CANCEL", GUILayout.Height(30f)))
        {
            Close();
        }

        GUILayout.EndHorizontal();

        if　(_isInit == false)
        {
            EditorGUI.FocusTextInControl("ForcusField");
        }
        _isInit = true;
    }
}