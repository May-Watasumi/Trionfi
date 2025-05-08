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
        [Index(0)]
        public int id { get; set; }
		[Index(1)]
		public string textJP { get; set; }
		[Index(2)]
		public string textEN { get; set; }
		[Index(3)]
		public string textCH { get; set; }
		[Index(4)]
		public string textKR { get; set; }

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
		[Name("name")]
		public string name { get; set; }
		[Name("prefix")]
        public string prefix { get; set; }
        [Name("hasvoice")]
        public bool hasVoice { get; set; }
        [Name("textcolor")]
        public string textColor { get; set; }

        public string en { get; set; }

        //Unity依存しない
//        public Sprite logIcon;

        //Localizer
        public string GetActorName(LocalizeID id)
        {
            switch (id)
            {
                case LocalizeID.JAPAN:
                default:
                    return name;
                case LocalizeID.ENGLISH:
                    return en;
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
        static CsvConfiguration config = new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
		{
			PrepareHeaderForMatch = args => args.Header.ToLower(),
			HasHeaderRecord = true,
        };

        static public TRMultiLanguageText LoadTextData(string text)
        {

            StringReader sr = new StringReader(text);
            var reader = new CsvReader(sr, config);
            {
				TRMultiLanguageText textData = new TRMultiLanguageText();

				reader.Read();
				//ヘッダを読み込みます
				reader.ReadHeader();
				//行毎に読み込みと処理を行います
				while (reader.Read())
				{
					var record = reader.GetRecord<TRTextData>();
					textData[record.id] = record;

					Debug.Log(record.textJP);
				}

//				var infoes = reader.GetRecords<TRTextData>();

                return textData;
            }
        }

        static public TRActorInfoes LoadActorInfo(LocalizeID id, string actorCSV)
        {
            StringReader sr = new StringReader(actorCSV);
              
            var reader = new CsvReader(sr, config);
            {
				TRActorInfoes actorInfo = new TRActorInfoes();

				reader.Read();
				//ヘッダを読み込みます
				reader.ReadHeader();
				//行毎に読み込みと処理を行います
				while (reader.Read())
				{
					var record = reader.GetRecord<TRActorInfo>();
					actorInfo[record.GetActorName(id)] = record;
				}

//				var infoes = reader.GetRecords<TRActorInfo>();

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

        public TRMultiLanguageText uiText;

        public TRActPatternAlias actPatternAlias = new TRActPatternAlias();
        public TRActorInfoes actorInfoes = new TRActorInfoes();


        public string GetUIText(TR_UITEXTID id)
        {
            return uiText[(int)id].GetText(TRSystemConfig.instance.localizeID);       
        }

        public void Initialize()
        {
            if (CharacterNameListCSV != null)
                actorInfoes = TREnviromentCSVLoader.LoadActorInfo(TRSystemConfig.Instance.localizeID, CharacterNameListCSV.text);


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
