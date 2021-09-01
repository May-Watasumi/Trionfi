using System;
using System.IO;
using TinyCsvParser.Tokenizer;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer.RFC4180;

namespace Trionfi
{
    [CustomEditor(typeof(TRActorParamAsset))]
    public class TRActorParamEditor : Editor
    {
        TRActorParamAsset asset;
        static bool showTileEditor = false;

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

                    CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
                    CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
                    CsvActorMapping csvMapper = new CsvActorMapping();

                    if (!string.IsNullOrEmpty(fileContent))
                    {
                        CsvParser<TRActorInfo> csvParser = new CsvParser<TRActorInfo>(csvParserOptions, csvMapper);

                        asset.actorInfo.Clear();

                        var result = csvParser.ReadFromString(csvReaderOptions, fileContent).ToList();
                        foreach (var _info in result)
                        {
                            asset.actorInfo[_info.Result.displayNameJP] = _info.Result;
                        }
                    }
                }
            }
        }
    }
}
