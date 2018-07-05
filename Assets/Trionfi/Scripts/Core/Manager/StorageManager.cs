using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
    public enum TRDataType
    {
        None,
        TextAsset,
        JsonText,
        BinaryData,
        Character,
        BG,
        Event,
        UI,
        Live2D,
        FBX,
        Emote,
        BGM,
        SE,
        Voice,
        Terminate,
    }

    //	[System.Serializable]
    public enum TRStorageType
	{
		LocalFile,
		URL,
		AssetBundle,
        Terminate
	};
	
//	[System.Serializable]
	public class StorageInfo
	{
		public string path;
//		public StorageTypes type;
	}

//	[System.Serializable]
	public class StorageManager : SingletonMonoBehaviour<StorageManager>
	{
        bool onDataLoading;

        static public Dictionary<string, TRDataType> dataTypes =  new Dictionary<string, TRDataType>()
        {
 //            { "BG", TRDataType.None },
            { "TEXT", TRDataType.TextAsset },
            { "JSON", TRDataType.JsonText },
            { "BIN", TRDataType.BinaryData },
            { "CHARA", TRDataType.Character },
            { "BG", TRDataType.BG },
            { "EVENT", TRDataType. Event },
            { "UI", TRDataType. UI },
            { "LIVE2D", TRDataType.Live2D },
            { "FBX", TRDataType.FBX },
            { "EMOTE", TRDataType. Emote },
            { "BGM", TRDataType.BGM },
            { "SE", TRDataType.SE },
            { "VOICE", TRDataType. Voice },
            { "", TRDataType.Terminate },              
        };

        private const string savesnapfile = "_TEMPSAVE_";

        [SerializeField]
        public string savedataPath = "savedata/";

        [SerializeField]
		public string localReourcesPath = "TRdata/";

        [SerializeField]
        public Dictionary<TRDataType, string> dataPath = new Dictionary<TRDataType, string>();

        [SerializeField]
        public Dictionary<TRStorageType, string> storagePath = new Dictionary<TRStorageType, string>();


		public string IsExistLocal(string storage)
        {
			Sprite g = Resources.Load<Sprite>(storage);

//			Debug.Log(g);

			if (g == null)
				return "ファイル「" + storage + "」が存在しません\n";

			return "";
		}

        public string MakeStoragePath(string file, TRDataType dataType)
        {
            string _basePath = dataPath[dataType];

            // "/"補完
            if (!_basePath.EndsWith("/"))
               _basePath += "/";

            return _basePath + file;
        }

        public Object LoadObject(string storage, TRDataType dataType, TRStorageType storageType = TRStorageType.LocalFile)
        {
            Object resultObject;

            //ToDo:
            string fullPath = storagePath[storageType] + MakeStoragePath(storage, dataType);

            //            if(IsExistLocal(storage) != )
            switch(storageType) {
            case TRStorageType.LocalFile:
            default:
                    resultObject = Resources.Load(fullPath) as GameObject;
                    onDataLoading = false;
                    break;
            }

            if (resultObject == null)
                            ErrorLogger.stopError("Trionfi:\"" + storage + "\"("+resultObject.GetType().Name+"\")が見つかりませんでした。");

            return resultObject;
		}
	}
}
