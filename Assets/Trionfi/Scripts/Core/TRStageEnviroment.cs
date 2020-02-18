using System;
using System.Text;
using TinyCsvParser.Tokenizer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer.RFC4180;

namespace Trionfi
{
    public class TRStageEnviroment : SingletonMonoBehaviour<TRStageEnviroment>
    {
        [SerializeField]
        public string _FILE_HEADER_ = "portrait_";
        [SerializeField]
        public string _LAYER_PATH_ = "sprite/character/";
        [SerializeField]
        public string _BGM_PATH_ = "sound/bgm/";
        [SerializeField]
        public string _LOGICON_PREFIX_ = "logicon_";

        [System.Serializable]
        public class TREnviroment : SerializableDictionary<string, Color> { }
        [System.Serializable]
        public class TRKeyboardEvent : SerializableDictionary<KeyCode, TRKeyboardShortCut> { }
        [System.Serializable]
        public class TRActPatternAlias : SerializableDictionary<string, string> { }
        [System.Serializable]
        public class TRActorInfoes : SerializableDictionary<string, TRActorInfo> { }
        [System.Serializable]
        public class TRLayerAlias : SerializableDictionary<string, int> { }
        [System.Serializable]
        public class TRLayerID : SerializableDictionary<string, int> { }

        [System.Serializable]
        public class TRActorInfo
        {
            public int imageID = -1;
            public int emotionID = -1;
            public string displayName { get; set; }
            public string prefix { get; set; }
            public bool hasVoice { get; set; }
        }

        public class TRActPatternInfo
        {
            public string alias { get; set; }
            public string suffix { get; set; }
        }

        [SerializeField]
        TREnviroment bgEnviroment = new TREnviroment()
        {
            { "昼" , new Color(1.0f, 1.0f, 1.0f) },
            { "夕" , new Color(1.0f, 0.375f, 0.0625f, 0.75f) },
            { "夜" , new Color(0.0f, 0.0f, 0.25f, 0.75f) },
        };
        [SerializeField]
        TREnviroment charaEnviroment = new TREnviroment()
        {
            { "昼" , new Color(1.0f, 1.0f, 1.0f) },
            { "夕" , new Color(1.0f, 0.0f, 0.0f, 1.0f) },
            { "夜" , new Color(0.25f, 0.0f, 0.5f, 1.0f) },
        };

        public readonly TRLayerAlias layerAlias = new TRLayerAlias()
        {
            { "左", 1 },
            { "中央", 2 },
            { "右", 3 },
            { "左中", 4 },
            { "右中", 5 },
        };

        public class CsvActorMapping : CsvMapping<TRActorInfo>
        {
            public CsvActorMapping()
            {
                MapProperty(0, x => x.displayName);
                MapProperty(1, x => x.prefix);
                MapProperty(2, x => x.hasVoice);
            }
        }

        public class CsvActPatternMapping : CsvMapping<TRActPatternInfo>
        {
            public CsvActPatternMapping()
            {
                MapProperty(0, x => x.alias);
                MapProperty(1, x => x.suffix);
            }
        }

        [SerializeField]
        public TextAsset CharacterNameListCSV;

        [SerializeField]
        public TextAsset CharacterEmotionPatternListCSV;

        public TRActPatternAlias actPatternAlias = new TRActPatternAlias();
        public TRActorInfoes actorInfoes = new TRActorInfoes();

        // Use this for initialization
        void Start()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvActorMapping csvMapper = new CsvActorMapping();
            CsvActPatternMapping csvMapper2 = new CsvActPatternMapping();

            if (CharacterNameListCSV != null)
            {
                CsvParser<TRActorInfo> csvParser = new CsvParser<TRActorInfo>(csvParserOptions, csvMapper);

                var result = csvParser.ReadFromString(csvReaderOptions, CharacterNameListCSV.text).ToList();
                foreach (var _info in result)
                {
                    actorInfoes[_info.Result.displayName] = _info.Result;
                }
            }

            if (CharacterEmotionPatternListCSV != null)
            {
                CsvParser<TRActPatternInfo> csvParser = new CsvParser<TRActPatternInfo>(csvParserOptions, csvMapper2);

                var result = csvParser.ReadFromString(csvReaderOptions, CharacterEmotionPatternListCSV.text).ToList();
                foreach (var _info in result)
                {
                    actPatternAlias[_info.Result.alias] = _info.Result.suffix;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
