using System;
using System.Text;
using TinyCsvParser.Tokenizer;
using System.Collections;
using System.Collections.Generic;
#if !TR_PARSEONLY
 using UnityEngine;
#endif
using System.Linq;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer.RFC4180;

namespace Trionfi
{
#if !TR_PARSEONLY
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
//    [System.Serializable]
//    public class TRLayerID : SerializableDictionary<string, int> { }
#else
    [System.Serializable]
    public class TREnviroment : Dictionary<string, int> { }
    [System.Serializable]
    public class TRKeyboardEvent : Dictionary<int, TRKeyboardShortCut> { }
    [System.Serializable]
    public class TRActPatternAlias : Dictionary<string, string> { }
    [System.Serializable]
    public class TRActorInfoes : Dictionary<string, TRActorInfo> { }
    [System.Serializable]
    public class TRLayerAlias : Dictionary<string, int> { }
#endif


    [System.Serializable]
    public class TRActorInfo
    {
        public int imageID = -1;
        public int emotionID = -1;

        public string prefix { get; set; }
        public bool hasVoice { get; set; }
        public string displayNameJP { get; set; }
        public string displayNameEN { get; set; }

        //Unity依存しない
//        public Sprite logIcon;

        //Localizer
        public string GetActorName(LocalizeID id)
        {
            switch (id)
            {
                case LocalizeID.JAPAN:
                default:
                    return displayNameJP;
                case LocalizeID.ENGLISH:
                    return displayNameEN;
            }
        }
    }

    [Serializable]
    public class TRActPatternInfo
    {
        public string suffix { get; set; }
        public string aliasJP { get; set; }
        public string aliasEN { get; set; }

        //Localizer
        public string GetPatternName(LocalizeID id)
        {
            switch (id)
            {
                case LocalizeID.JAPAN:
                default:
                    return aliasJP;
                case LocalizeID.ENGLISH:
                    return aliasEN;
            }
        }
    }

    public class CsvActorMapping : CsvMapping<TRActorInfo>
    {
        public CsvActorMapping()
        {
            MapProperty(0, x => x.prefix);
            MapProperty(1, x => x.hasVoice);
            MapProperty(2, x => x.displayNameJP);
            MapProperty(3, x => x.displayNameEN);
        }
    }

    public class CsvActPatternMapping : CsvMapping<TRActPatternInfo>
    {
        public CsvActPatternMapping()
        {
            MapProperty(0, x => x.suffix);
            MapProperty(1, x => x.aliasJP);
            MapProperty(1, x => x.aliasEN);
        }
    }

    public class TREnviromentCSVLoader
    {

        static public TRActorInfoes LoadActorInfo(LocalizeID id, string nameList)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvActorMapping csvMapper = new CsvActorMapping();

            TRActorInfoes actorInfoes = new TRActorInfoes();

            if (!string.IsNullOrEmpty(nameList))
            {
                CsvParser<TRActorInfo> csvParser = new CsvParser<TRActorInfo>(csvParserOptions, csvMapper);

                var result = csvParser.ReadFromString(csvReaderOptions, nameList).ToList();
                foreach (var _info in result)
                {
                    actorInfoes[_info.Result.GetActorName(id)] = _info.Result;
                }
                return actorInfoes;
            }
            return null;
        }

        static public TRActPatternAlias LoadEmotionList(LocalizeID id, string emotionList)
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvActPatternMapping csvMapper2 = new CsvActPatternMapping();

            TRActPatternAlias actPatternAlias = new TRActPatternAlias();
            
            if (string.IsNullOrEmpty(emotionList))
            {
                CsvParser<TRActPatternInfo> csvParser = new CsvParser<TRActPatternInfo>(csvParserOptions, csvMapper2);

                var result = csvParser.ReadFromString(csvReaderOptions, emotionList).ToList();
                foreach (var _info in result)
                {
                    actPatternAlias[_info.Result.GetPatternName(id)] = _info.Result.suffix;
                }
                return actPatternAlias;
            }
            return null;
        }
    }

#if !TR_PARSEONLY
    public class TRStageEnviroment : SingletonMonoBehaviour<TRStageEnviroment>
    {
        [SerializeField]
        public string _CHARACTER_PREFIX_ = "portrait_";
        [SerializeField]
        public string _CHARACTER_PATH_ = "sprite/character/";
        [SerializeField]
        public string _BGM_PATH_ = "sound/bgm/";
        [SerializeField]
        public string _VOICE_PATH_ = "sound/voice/";
        [SerializeField]
        public string _SE_PATH_ = "sound/se/";
        [SerializeField]
        public string _LOGICON_PREFIX_ = "logicon_";
      
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

        [SerializeField]
        public TextAsset CharacterNameListCSV;

        [SerializeField]
        public TextAsset CharacterEmotionPatternListCSV;

        [SerializeField]
        TRActorParamAsset actorInfoInstance;

        public TRActPatternAlias actPatternAlias = new TRActPatternAlias();
        public TRActorInfoes actorInfoes = new TRActorInfoes();

        // Use this for initialization
        void Start()
        {
            if (actorInfoInstance != null)
                actorInfoes = actorInfoInstance.actorInfo;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
#endif
}
