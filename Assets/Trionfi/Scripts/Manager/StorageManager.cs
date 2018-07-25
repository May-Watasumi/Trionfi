using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public enum TRAssetType
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
        LocalResources,
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
	public class TRResourceLoader : SingletonMonoBehaviour<TRResourceLoader>
	{
        bool onDataLoading = false;

        static public Dictionary<string, TRAssetType> dataTypes =  new Dictionary<string, TRAssetType>()
        {
 //            { "BG", TRAssetType.None },
            { "TEXT", TRAssetType.TextAsset },
            { "JSON", TRAssetType.JsonText },
            { "BIN", TRAssetType.BinaryData },
            { "CHARA", TRAssetType.Character },
            { "BG", TRAssetType.BG },
            { "EVENT", TRAssetType. Event },
            { "UI", TRAssetType. UI },
            { "LIVE2D", TRAssetType.Live2D },
            { "FBX", TRAssetType.FBX },
            { "EMOTE", TRAssetType. Emote },
            { "BGM", TRAssetType.BGM },
            { "SE", TRAssetType.SE },
            { "VOICE", TRAssetType. Voice },
            { "", TRAssetType.Terminate },              
        };

        [SerializeField]
        public string savedataPath = "savedata/";

        [SerializeField]
		public string localReourcesPath = "TRdata/";

        [SerializeField]
        public Dictionary<TRAssetType, string> dataPath = new Dictionary<TRAssetType, string>();

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

        public string MakeStoragePath(string file, TRAssetType dataType)
        {
            string _basePath = dataPath[dataType];

            // "/"補完
            if (!_basePath.EndsWith("/"))
               _basePath += "/";

            return _basePath + file;
        }

        public Object LoadObject(string storage, TRAssetType dataType, TRStorageType storageType = TRStorageType.LocalFile)
        {
            Object resultObject;

            //ToDo:
            string fullPath = storagePath[storageType] + MakeStoragePath(storage, dataType);

            //            if(IsExistLocal(storage) != )
            switch(storageType) {
                case TRStorageType.LocalResources:
                default:
                    resultObject = Resources.Load(fullPath) as GameObject;
                    onDataLoading = false;
                    break;
//                case TRStorageType.LocalFile:
//                    break;
            }

            if(resultObject == null)
                ErrorLogger.StopError("Trionfi:\"" + storage + "\"("+resultObject.GetType().Name+"\")が見つかりませんでした。");

            return resultObject;
		}

        int errorCode = 0;
        bool onLoading = false;
        AbstractComponent calledComponent;

        private IEnumerator Load(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            //3.isNetworkErrorとisHttpErrorでエラー判定
            if(request.isHttpError || request.isNetworkError)
            {
                //4.エラー確認
                Debug.Log(request.error);
            }
            else
            {
                //4.結果確認
                Debug.Log(request.downloadHandler.text);
            }

        }
    }
}
