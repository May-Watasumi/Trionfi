using UnityEngine;
using System.Collections;
using System;

namespace NovelEx {
	[System.Serializable]
	public enum StorageTypes
	{
		LocalFile,
		URL,
		AssetBundle
	};
	
	[System.Serializable]
	public class StorageInfo
	{
		public string path;
		public StorageTypes type;
	}

//	[System.Serializable]
	public class StorageManager : SingletonMonoBehaviour<StorageManager>
	{
		[SerializeField]		
		public string basepath = "novelex/data/";

		public StorageInfo path_prefab = new StorageInfo() { path = "prefab/", type = StorageTypes.LocalFile };
		public StorageInfo path_imageroot = new StorageInfo() { path = "images/", type = StorageTypes.LocalFile };
		public StorageInfo path_charaimage = new StorageInfo() { path = "images/character/", type = StorageTypes.LocalFile };
		public StorageInfo path_bgimage = new StorageInfo() { path = "images/background/", type = StorageTypes.LocalFile };
		public StorageInfo path_standimage = new StorageInfo() { path = "images/sd/", type = StorageTypes.LocalFile };
		public StorageInfo path_systemimage = new StorageInfo() { path = "images/system/", type = StorageTypes.LocalFile };
		public StorageInfo path_otherimage = new StorageInfo() { path = "images/image/", type = StorageTypes.LocalFile };
		public StorageInfo path_scenario = new StorageInfo() { path = "scenario/", type = StorageTypes.LocalFile };
		public StorageInfo path_bgm = new StorageInfo() { path = "bgm/", type = StorageTypes.LocalFile };
		public StorageInfo path_se = new StorageInfo() { path = "sound/", type = StorageTypes.LocalFile };
		public StorageInfo path_voice = new StorageInfo() { path = "voice/", type = StorageTypes.LocalFile };
		public StorageInfo path_animation = new StorageInfo() { path = "anim", type = StorageTypes.LocalFile };
		public StorageInfo path_live2d = new StorageInfo() { path = "L2DModel/", type = StorageTypes.LocalFile };

		public StorageInfo path_savedata = new StorageInfo() { path = /*Application.persistentDataPath+*/"/novel/", type = StorageTypes.LocalFile };
		private const string globaldatafile = "setting.dat";
		private const string savesnapfile = "_TEMPSAVE_.dat";

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
		public string PATH_IMAGE_ROOT
		{
			get
			{
				return basepath + path_imageroot.path;
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
		public string PATH_SYSTEM_IMAGE
		{
			get
			{
				return basepath + path_systemimage.path;
			}
		}
		public string PATH_IMAGE
		{
			get
			{
				return basepath + path_otherimage.path;
			}
		}

		public string PATH_SD_SCENARIO
		{
			get
			{
				return basepath + path_scenario.path;
			}
		}
		public string PATH_SD_OBJECT
		{
			get
			{
				return basepath + path_standimage.path;
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
		public string PATH_ANIM_FILE
		{
			get
			{
				return basepath + path_animation.path;
			}
		}
		public string PATH_LIVE2D_FILE
		{
			get
			{
				return /*basepath + */ path_live2d.path;
			}
		}

		public string PATH_SAVEDATA
		{
			get
			{
				return path_savedata.path;
			}
		}

		public string checkStorage(string storage) {
			Sprite g = Resources.Load<Sprite>(storage);

			Debug.Log(g);

			if (g == null)
				return "ファイル「" + storage + "」が存在しません\n";

			return "";
		}

//ToDo:AssetBundle
		public GameObject loadObject(string storage) {
			GameObject obj = Resources.Load(storage) as GameObject;

			if (obj == null)
				JOKEREX.Instance.errorManager.stopError("JOKEREX:\"" + storage + "\"(object)が見つかりませんでした。");

			return obj;
		}

		public string loadTextAsset(string storage) {
			// TextAssetとして、Resourcesフォルダからテキストデータをロードする
			TextAsset stageTextAsset = Resources.Load(storage) as TextAsset;

			if (stageTextAsset == null)
				JOKEREX.Instance.errorManager.stopError("JOKEREX:\"" + storage + "\"(text)が見つかりませんでした。");			

			// 文字列を代入
			string stageData = stageTextAsset.text;

			// 空白を置換で削除
			return stageData;
		}

		public AudioClip loadAudioAsset(string storage) {
			AudioClip clip = new AudioClip() ;     
            clip = (AudioClip)(Resources.Load(storage)) as AudioClip;

			if(clip == null)
				JOKEREX.Instance.errorManager.stopError("JOKEREX:\"" + storage + "\"(audio)が見つかりませんでした。");

			return clip	;
		}

		public Sprite loadSprite(string storage) {
			Sprite sp = Resources.Load<Sprite>(storage);

			if (sp == null)
				JOKEREX.Instance.errorManager.stopError("JOKEREX:\"" + storage + "\"(sprite)が見つかりませんでした。");			

			return sp;
		}

		public Sprite loadImage(string storage) {
			Sprite sp = Resources.Load<Sprite>(PATH_IMAGE_ROOT + storage);

			if (sp == null)
				JOKEREX.Instance.errorManager.stopError("JOKEREX:\"" + storage + "\"(Image)が見つかりませんでした。");

			return sp;
		}

		public AudioClip loadVoice(string storage) {
			return loadAudioAsset(PATH_AUDIO_VOICE + storage);
		}


		public GameObject loadLive2D(string storage) {
			return loadObject(PATH_LIVE2D_FILE + storage);
		}

		public GameObject loadPrefab(string storage) {
			return loadObject(PATH_PREFAB + storage);
		}

		public AnimationClip loadAnimation(string storage) {
			return Resources.Load(PATH_ANIM_FILE + storage) as AnimationClip;
		}

		public AbstractObject GetCustomObject(string type, GameObject g) {
/*
			switch (type) {
			case "Message_Magic":
				return g.AddComponent<Message_magicObject>();
			default:
*/

//EX:
//カスタムオブジェクトを定義する場合はここ。
			return  g.AddComponent<ImageObject>();
		}

		public void Start() {
			path_savedata.path = Application.persistentDataPath+"/novel/";
		 }

	}
}
