using UnityEngine;
using System.Collections;
using System;

namespace NovelEx
{
    public enum TRObjectType
    {
        None,
        Character,
        BG,
        Event,
        UI,
        Live2D,
        FBX,
        Emote,
        BGM,
        SE,
        Voice
    }

    //	[System.Serializable]
    public enum StorageTypes
	{
		LocalFile,
		URL,
		AssetBundle
	};
	
//	[System.Serializable]
	public class StorageInfo
	{
		public string path;
		public StorageTypes type;
	}

//	[System.Serializable]
	public class StorageManager : SingletonMonoBehaviour<StorageManager>
	{
        [SerializeField]
        public GameObject imageBasePrefab;

        [SerializeField]		
		public string basepath = "TRdata/";

		public StorageInfo path_prefab = new StorageInfo() { path = "prefab/", type = StorageTypes.LocalFile };
		public StorageInfo path_spriteroot = new StorageInfo() { path = "sprite/", type = StorageTypes.LocalFile };
		public StorageInfo path_charaimage = new StorageInfo() { path = "sprite/character/", type = StorageTypes.LocalFile };
		public StorageInfo path_bgimage = new StorageInfo() { path = "sprite/background/", type = StorageTypes.LocalFile };
		public StorageInfo path_uiimage = new StorageInfo() { path = "sprite/ui/", type = StorageTypes.LocalFile };
		public StorageInfo path_scenario = new StorageInfo() { path = "scenario/", type = StorageTypes.LocalFile };
		public StorageInfo path_bgm = new StorageInfo() { path = "sound/bgm/", type = StorageTypes.LocalFile };
		public StorageInfo path_se = new StorageInfo() { path = "sound/se/", type = StorageTypes.LocalFile };
		public StorageInfo path_voice = new StorageInfo() { path = "sound/voice/", type = StorageTypes.LocalFile };
//		public StorageInfo path_live2d = new StorageInfo() { path = "L2DModel/", type = StorageTypes.LocalFile };

		public StorageInfo path_savedata = new StorageInfo() { path = /*Application.persistentDataPath+*/"/novel/", type = StorageTypes.LocalFile };
		private const string savesnapfile = "_TEMPSAVE_";

		public string PATH_SERVER {
			get {
				return basepath;
			 }
		}

		public string PATH_PREFAB
		{
			get
			{
				return basepath + path_prefab.path;
			}
		}
		public string PATH_SPRITE_ROOT
		{
			get
			{
				return basepath + path_spriteroot.path;
			}
		}
		public string PATH_CHARA_IMAGE
		{
			get
			{
				return basepath + path_charaimage.path;
			}
		}
		public string PATH_BG_IMAGE
		{
			get
			{
				return basepath + path_bgimage.path;
			}
		}
		public string PATH_UI_IMAGE
		{
			get
			{
				return basepath + path_uiimage.path;
			}
		}

		public string PATH_SD_SCENARIO
		{
			get
			{
				return basepath + path_scenario.path;
			}
		}

        public string PATH_AUDIO_BGM
		{
			get
			{
				return basepath + path_bgm.path;
			}
		}
		public string PATH_AUDIO_SE 
		{
			get
			{
				return basepath + path_se.path;
			}
		}
		public string PATH_AUDIO_VOICE
		{
			get
			{
				return basepath + path_voice.path;
			}
		}
#if false
        public string PATH_LIVE2D_FILE
		{
			get
			{
				return /*basepath + */ path_live2d.path;
			}
		}
#endif
		public string PATH_SAVEDATA
		{
			get
			{
				return path_savedata.path;
			}
		}

		public string IsExist(string storage) {
			Sprite g = Resources.Load<Sprite>(storage);

			Debug.Log(g);

			if (g == null)
				return "ファイル「" + storage + "」が存在しません\n";

			return "";
		}

		public GameObject LoadObject(string storage) {
			GameObject obj = Resources.Load(storage) as GameObject;

			if (obj == null)
                ErrorLogger.stopError("JOKEREX:\"" + storage + "\"(object)が見つかりませんでした。");

			return obj;
		}

		public string LoadTextAsset(string storage) {
			// TextAssetとして、Resourcesフォルダからテキストデータをロードする
			TextAsset stageTextAsset = Resources.Load(storage) as TextAsset;

			if (stageTextAsset == null)
                ErrorLogger.stopError("JOKEREX:\"" + storage + "\"(text)が見つかりませんでした。");			

			// 文字列を代入
			string stageData = stageTextAsset.text;

			// 空白を置換で削除
			return stageData;
		}

		public AudioClip LoadAudioAsset(string storage) {
			AudioClip clip = new AudioClip() ;     
            clip = (AudioClip)(Resources.Load(storage)) as AudioClip;

			if(clip == null)
                ErrorLogger.stopError("JOKEREX:\"" + storage + "\"(audio)が見つかりませんでした。");

			return clip	;
		}

		public Sprite LoadSprite(string storage) {
			Sprite sp = Resources.Load<Sprite>(storage);

			if (sp == null)
                ErrorLogger.stopError("JOKEREX:\"" + storage + "\"(sprite)が見つかりませんでした。");			

			return sp;
		}

		public Sprite LoadImage(string storage) {
			Sprite sp = Resources.Load<Sprite>(PATH_SPRITE_ROOT + storage);

			if (sp == null)
				ErrorLogger.stopError("JOKEREX:\"" + storage + "\"(Image)が見つかりませんでした。");

			return sp;
		}

		public AudioClip LoadVoice(string storage) {
			return LoadAudioAsset(PATH_AUDIO_VOICE + storage);
		}
/*
		public GameObject LoadLive2D(string storage) {
			return LoadObject(PATH_LIVE2D_FILE + storage);
		}
*/
		public GameObject loadPrefab(string storage) {
			return LoadObject(PATH_PREFAB + storage);
		}
/*
		public AnimationClip loadAnimation(string storage) {
			return Resources.Load(PATH_ANIM_FILE + storage) as AnimationClip;
		}

		public AbstractObject GetCustomObject(string type, GameObject g) {

			switch (type) {
			case "Message_Magic":
				return g.AddComponent<Message_magicObject>();
			default:
*/

//EX:
//カスタムオブジェクトを定義する場合はここ。
//			return  new AbstractObject();
//		}

		public void Start()
        {
			path_savedata.path = Application.persistentDataPath+"/novel/";
		}

	}
}
