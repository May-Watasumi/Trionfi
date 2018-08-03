using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using CsvReadWrite;

namespace Trionfi
{
    public class TRActorManagerObject : ScriptableObject
    {
        public static readonly string assetName = "Actor.asset";

        Dictionary<string, TRActorObject> actors = new Dictionary<string, TRActorObject>();

        [MenuItem("Trionfi/Actor/Newly")]
        static void CreateEnviroment()
        {
            TRActorObject _instance = CreateInstance<TRActorObject>();
            _instance.name = "new_actor";
            AssetDatabase.AddObjectToAsset(_instance, Trionfi.assetPath + assetName);
            AssetDatabase.ImportAsset(Trionfi.assetPath + assetName);
        }

        [MenuItem("Trionfi/Actor/List up from CSV")]
        private static void CreateActorList()
        {
            string path = EditorUtility.OpenFilePanel("CSVファイル", Application.dataPath, "csv");
            if (path.Length != 0)
            {
                List<string> row = null;

                using (var csv = new CsvReader(path))
                {
                    while ((row = csv.ReadRow()) != null)
                    {
                        TRActorObject _instance = CreateInstance<TRActorObject>();
                        _instance.name = row[0];
                        _instance.actorName = row[0];
                        _instance.voicePrefix = row[1];
                        _instance.hasVoice = int.Parse(row[2]) != 0 ? true : false;
                        AssetDatabase.AddObjectToAsset(_instance, Trionfi.assetPath + assetName);
                        AssetDatabase.ImportAsset(Trionfi.assetPath + assetName);
                    }
                }
            }
        }

        [MenuItem("Trionfi/Actor/Set Emotion from CSV")]
        private static void CreateActorFromCSV()
        {
            string path = EditorUtility.OpenFilePanel("CSVファイル", Application.dataPath, "csv");
            if (path.Length != 0)
            {
                TRActorObject _instance = CreateInstance<TRActorObject>();
                _instance.emotion.Clear();

                _instance.name = Path.GetFileNameWithoutExtension(path);
                AssetDatabase.AddObjectToAsset(_instance, Trionfi.assetPath + assetName);
                AssetDatabase.ImportAsset(Trionfi.assetPath + assetName);

                List<string> row = null;

                using (var csv = new CsvReader(path))
                {
                    while ((row = csv.ReadRow()) != null)
                    {
                        _instance.emotion[row[0]] = row[1];
                    }
                }
            }
        }
    }
}
