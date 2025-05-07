using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
namespace Trionfi
{
    [CustomEditor(typeof(TRActorParamAsset))]
    public class TRActorParamEditor : Editor
    {
        TRActorParamAsset asset;
//        static bool showTileEditor = false;

        public void OnEnable()
        {
            asset = (TRActorParamAsset)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("ReadCSV"))
            {
                string path = EditorUtility.OpenFilePanel("ReadCSV", "", "csv");
                if (path.Length != 0)
                {
                    string fileContent = File.ReadAllText(path);

                    var list = TREnviromentCSVLoader.LoadActorInfo(LocalizeID.JAPAN, fileContent);

                    foreach (var info in list)
                    {
                        asset.actorInfo[info.Key] = info.Value;
                    }
                }
            }
        }
    }
}
