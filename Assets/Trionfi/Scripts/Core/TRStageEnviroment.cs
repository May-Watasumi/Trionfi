using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
#if !TR_PARSEONLY
 using UnityEngine;
#endif
using System.Linq;
using System.IO;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace Trionfi
{
    public enum TR_UITEXTID
    {
        TITLE_TEXT,
        TITLE_TAPSCREEN,
        MESSAGEWINDOW_LOG,
        MESSAGEWINDOW_AUTO,
        MESSAGEWINDOW_SKIP,
        MESSAGEWINDOW_LOAD,
        MESSAGEWINDOW_SAVE,
        MESSAGEWINDOW_CONFIG,
        MESSAGEWINDOW_CLOSE,
        CONFIG_MASTERVOLUME,
        CONFIG_BGMVOLUME,
        CONFIG_SEVOLUME,
        CONFIG_VOICEVOLUME,
        CONFIG_TEXTSPEED,
        CONFIG_AUTOWAIT,
        CONFIG_EFFECTSKIP,
        CONFIG_READSKIP,
        CONFIG_INITIALIZE,
    }

#if !TR_PARSEONLY
    [System.Serializable]
    public class TRMultiLanguageText : SerializableDictionary<int, TRTextData> { }
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
    public class TRMultiLanguageText : Dictionary<int, TRTextData> { }
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


    [Serializable]
    public class TRTextData
    {
        [Name("id")]
        public int id { get; set; }
        [Name("text")]
        public string textJP { get; set; }
        public string textEN { get; set; }

        //Localizer
        public string GetText(LocalizeID id)
        {
            switch (id)
            {
                case LocalizeID.JAPAN:
                default:
                    return textJP;
                case LocalizeID.ENGLISH:
                    return textEN;
            }
        }
    }


    [System.Serializable]
    public class TRActorInfo
    {
        public int imageID = -1;
        public int emotionID = -1;
        [Name("prefix")]
        public string prefix { get; set; }
        [Name("hasvoice")]
        public bool hasVoice { get; set; }
        [Name("name")]
        public string displayNameJP { get; set; }
        [Name("textcolor")]
        public string textColor { get; set; }

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
        [Name("suffix")]
        public string suffix { get; set; }
        [Name("name")]
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


    public class TREnviromentCSVLoader
    {
        static CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        static public TRMultiLanguageText LoadTextData(string text)
        {
            StringReader sr = new StringReader(text);
            var reader = new CsvReader(sr, config);
            {
                var infoes = reader.GetRecords<TRTextData>();
                TRMultiLanguageText textData = new TRMultiLanguageText();

                foreach (var info in infoes)
                {
                    textData[info.id] = info;

                }
                return textData;
            }
        }

        static public TRActorInfoes LoadActorInfo(LocalizeID id, string actorCSV)
        {
            StringReader sr = new StringReader(actorCSV);
              
            var reader = new CsvReader(sr, config);
            {
                var infoes = reader.GetRecords<TRActorInfo>();

                TRActorInfoes actorInfo = new TRActorInfoes();
                foreach (var info in infoes)
                {
                    actorInfo[info.GetActorName(id)] = info;
                }
                return actorInfo;
            }
        }

        static public TRActPatternAlias LoadEmotionList(LocalizeID id, string emotionCSV)
        {
            StringReader sr = new StringReader(emotionCSV);

            var reader = new CsvReader(sr, config);
            {
                TRActPatternAlias actPatternAlias = new TRActPatternAlias();

                var infoes = reader.GetRecords<TRActPatternInfo>();

                foreach (var info in infoes)
                {
                    actPatternAlias[info.GetPatternName(id)] = info.suffix;
                }

                return actPatternAlias;
            }

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
        public TextAsset UITextCSV;

        [SerializeField]
        public TextAsset CharacterNameListCSV;

        [SerializeField]
        public TextAsset CharacterEmotionPatternListCSV;

        [SerializeField]
        TRActorParamAsset actorInfoInstance = null;

        public TRMultiLanguageText uiText;

        public TRActPatternAlias actPatternAlias = new TRActPatternAlias();
        public TRActorInfoes actorInfoes = new TRActorInfoes();


        public string GetUIText(TR_UITEXTID id)
        {
            return uiText[(int)id].GetText(TRSystemConfig.instance.localizeID);       
        }

        public void Initialize()
        {
            if (actorInfoInstance != null)
                actorInfoes = actorInfoInstance.actorInfo;


            if (UITextCSV != null)
               uiText = TREnviromentCSVLoader.LoadTextData(UITextCSV.text);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
#endif
}
