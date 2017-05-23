using UnityEngine;
using UnityEditor;

public static class EditorHelper
{
    public static int DrawIndexSelector(int index, int max)
    {
        int selectedIndex = 0;

        if (max > 0)
        {
            if (index >= max)
                index = max - 1;

            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = index == 0 ? Color.gray : Color.green;
            if (GUILayout.Button("<<") && index > 0)
                index -= 1;
            GUI.backgroundColor = Color.white;

            index = EditorGUILayout.IntField(index + 1, GUILayout.Width(40.0f)) - 1;
            EditorGUILayout.LabelField("/" + max, GUILayout.Width(40));

            if (index < 0)
                index = 0;
            else if (index >= max)
                index = max - 1;

            GUI.backgroundColor = index >= (max - 1) ? Color.gray : Color.green;
            if (GUILayout.Button(">>") && index < (max - 1))
                index += 1;
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

            selectedIndex = index;
        }

        return selectedIndex;
    }

    public static bool AddElementButton(string label = "+")
    {
        bool shallAdd = false;
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button(label, GUILayout.Width(110.0f)))
        {
            shallAdd = true;
        }
        GUI.backgroundColor = Color.white;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        return shallAdd;
    }

    public static bool DeleteElementButton(string label = "Delete")
    {
        bool shallDelete = false;
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button(label, GUILayout.Width(50.0f)))
        {
            shallDelete = true;
        }
        GUI.backgroundColor = Color.white;
        return shallDelete;
    }


    public static bool FoldoutBar(bool foldout, string text)
    {

        EditorGUILayout.BeginVertical("Toolbar");

        EditorGUILayout.BeginHorizontal();

        foldout = EditorGUILayout.Foldout(foldout, text);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        return foldout;

    }

    public static bool BeginFoldoutToolbar(bool foldout, string text)
    {
        EditorGUILayout.BeginVertical("Toolbar");

        EditorGUILayout.BeginHorizontal();

        foldout = EditorGUILayout.Foldout(foldout, text);

        return foldout;
    }

    public static void BeginToolbar()
    {
        EditorGUILayout.BeginVertical("Toolbar");
        EditorGUILayout.BeginHorizontal();
    }

    public static void BeginToolbar(string title)
    {
        EditorGUILayout.BeginVertical("Toolbar");
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(title);

    }

    public static void EndToolbar()
    {
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    public static void Toolbar(string title)
    {

        BeginToolbar(title);
        EndToolbar();

    }

    public static Color barBackgroundColor = Color.white;

    public static bool ToolbarButton(string title, string selected)
    {
        if (title.Equals(selected))
        {
            return ToolbarButton(title, Color.green);
        }
        else
        {
            return ToolbarButton(title, Color.white);
        }
    }

    public static bool ToolbarButton(string title)
    {
        return ToolbarButton(title, Color.white);
    }

    public static bool ToolbarButton(string title, Color color)
    {
        GUI.backgroundColor = color;
        if (GUILayout.Button(title, EditorStyles.toolbarButton))
        {
            GUI.backgroundColor = Color.white;
            return true;
        }
        return false;
    }

    public static bool ArrayField(bool expand, SerializedProperty propArray, string text)
    {

        expand = EditorGUILayout.Foldout(expand, " " + text);

        if (expand)
        {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            propArray.arraySize = EditorGUILayout.IntField("Count", propArray.arraySize);

            for (int i = 0; i < propArray.arraySize; i++)
            {
                SerializedProperty property = propArray.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(property);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

        }
        return expand;
    }
}
