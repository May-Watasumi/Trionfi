using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Text;

namespace Trionfi
{
    [Serializable]
    public class UserSaveDataInfo
    {
        public string currentFile = "";
        public int currentIndex = -1;

        public string name = "";
        public string title = "";
        public string description = "";
        public string date = "";
        public string currentMessage = "";

        //画面のキャプチャ情報
        public string cap_img_file = "";

        //ToDo:レイヤ、音等の状態保存
    }

    public enum TRSTACKTYPES
    {
        MACRO,
        FUNCTION,
        IF
    }

    //マクロ＆関数オブジェクト。
    [Serializable]
    public class InvocationObject
    {
//        public string name;
        public string file_name;
        public int index;


        public TRSTACKTYPES type;

        public InvocationObject(/*string _name,*/ string _file_name, int _index, TRSTACKTYPES _type)
        {
//            name = _name;
            file_name = _file_name;
            index = _index;
            type = _type;
        }
    }

    //コールスタック。関数とマクロ共用。（返値を保存する以外の実装に違いはない）。
    [Serializable]
    public class CallStackObject
    {
        public ParamDictionary tempParam = new ParamDictionary();//仮引数
        public string scenarioNname;
        public int index;

        public CallStackObject(string scenario_name, int _index, ParamDictionary _param)
        {
            scenarioNname = scenario_name;
            index = _index;
            tempParam = _param;
        }
    }

    public class TRCallStack : Stack<CallStackObject>
    {
        public new void Push(CallStackObject _object)
        {
            //variable["mp"] = dicVar;
            base.Push(_object);
        }

        public new CallStackObject Pop()
        {
            //variable["mp"] = c.dicVar;
            return base.Pop();
        }
    }

    //if文の入れ子などを管理するスタック
    [Serializable]
    public class IfStack
    {
        public bool isIfProcess = false;

        public IfStack() { }

        public IfStack(bool val)
        {
            isIfProcess = val;
        }
    }

    public class TRVitualMachine
    {
        //変数インスタンスは１つ
        public static UserSaveDataInfo saveDataInfo = new UserSaveDataInfo();
        public static Variable variableInstance = new Variable();

        public static string currentScriptName;
        public static int currenScriptPosition = -1;

        public static TRCallStack callStack = new TRCallStack();
        public static Stack<bool> ifStack = new Stack<bool>();

        public static Dictionary<string, InvocationObject> invovationInstance = new Dictionary<string, InvocationObject>();

        //スタックをすべて削除します
        public static void RemoveAllStacks()
        {
            callStack.Clear();
            ifStack.Clear();
        }

        //ToDo:
        public static bool Serialize(string name) { return true; }
        public static bool Deserialize(string name) { return false; }
    }

 #if false
    public class Serializer
    {
        public Serializer() { }

		//グローバルを保存します
		public static void SaveGlobalObject(string storage)
		{
            //ToDo?
//			string json = LitJson.JsonMapper.ToJson();

            //WebPlayer の場合保存方法に変化を入れる
#if false
            if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
			{
				PlayerPrefs.SetString(storage, json);
			}
			else
#endif
            {
				string path = StorageManager.Instance.savedataPath + storage;

				//ディレクトリ存在チェック
				if (!Directory.Exists(StorageManager.Instance.savedataPath))
					Directory.CreateDirectory(StorageManager.Instance.savedataPath);

				FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
				StreamWriter sw = new StreamWriter(fs);
                //ToDo?
//				sw.Write(json);
				sw.Flush();
				sw.Close();
				fs.Close();
			}
		}
		public static void LoadGlobalObject(string storage) {
            //WebPlayer の場合保存方法に変化を入れる
#if false
			if(Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer) {
				if(!PlayerPrefs.HasKey(storage))
					SaveGlobalObject(storage);

				string json = PlayerPrefs.GetString(storage);

				SaveGlobalObject obj = LitJson.JsonMapper.ToObject<SaveGlobalObject> (json);
				globalObject = obj;
			}
			else
#endif
            {
                string path = StorageManager.Instance.savedataPath + storage;

				//ファイル作成
				if(!File.Exists(path))
					SaveGlobalObject(storage);

				FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

				StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);
				string json = sr.ReadToEnd();

                //ToDo:
                //SaveGlobalObject obj = LitJson.JsonMapper.ToObject<SaveGlobalObject>(json);

				sr.Close();
				fs.Close();

//				Trionfi.Instance.UserDataManager.globalSetting = obj;
			}

            //ToDo:
			//グローバル変数を格納する
			//ScriptDecoder.Instance.variable.replaceAll("global", globalObject.globalVar);
			//ScriptDecoder.Instance.variable.trace("global");
		}

		public static UserSaveDataInfo GetSaveObject(string storage)
        {
			string fullpath = /*StorageManager.Instance.PATH_SAVEDATA +*/ storage;
            UserSaveDataInfo obj = (UserSaveDataInfo)LoadFromBinaryFile(fullpath);
			return obj;
		}

		public static void SaveToBinaryFile(UserSaveDataInfo obj, string storage)
		{
            string json = JsonUtility.ToJson(obj);//  LitJson.JsonMapper.ToJson(obj);
#if false
            //WebPlayer の場合保存方法に変化を入れる
            if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
				PlayerPrefs.SetString(storage, json);
			else
#endif
			{
				if (!Directory.Exists(StorageManager.Instance.savedataPath))
					Directory.CreateDirectory(StorageManager.Instance.savedataPath);

				FileStream fs = new FileStream(StorageManager.Instance.savedataPath + storage, FileMode.Create, FileAccess.Write);

				StreamWriter sw = new StreamWriter(fs);
				sw.Write(json);
				sw.Flush();
				sw.Close();
				fs.Close();
			}
		}

		public static object LoadFromBinaryFile(string storage)
		{
#if false
			//WebPlayer の場合保存方法に変化を入れる
			if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
			{
				string json = PlayerPrefs.GetString(storage);
				SaveObject obj = LitJson.JsonMapper.ToObject<SaveObject>(json);
				return obj;
			}
			else
#endif
			{
				if (!File.Exists(storage))
					return null;

				FileStream fs = new FileStream(storage, FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(storage, System.Text.Encoding.Default);

				string json = sr.ReadToEnd();

				if (json == "")
					return null;

                UserSaveDataInfo obj = JsonUtility.FromJson<UserSaveDataInfo>(json);// LitJson.JsonMapper.ToObject<UserSaveData>(json);

				sr.Close();
				fs.Close();

				return obj;
			}
		}

		public static void applySaveVariable(string storage, Variable variable, string var_name = "save")
		{
            //最初のセーブデータを取得するか。
            UserSaveDataInfo sobj = GetSaveObject(storage);
			//this.gameManager.saveManager.getSaveData ("save_"+current_index);
			variable.Set(var_name + ".name", storage);
			//ToDo:
			if (sobj != null)
			{
				variable.Set(var_name + ".title", sobj.title);
				variable.Set(var_name + ".date", sobj.date);
				variable.Set(var_name + ".description", sobj.description);
				variable.Set(var_name + ".name", storage);
				//StatusManager.variable.set ("save.img", sobj.cap_img_file);
			}
			else
			{
				variable.Set(var_name + ".title", "データがありません");
				variable.Set(var_name + ".date", "");
				variable.Set(var_name + ".description", "");
				variable.Set(var_name + ".name", storage);
				//StatusManager.variable.set ("save.img", "");
			}
		}

		//一時退避しておいたスナップから保存を実行する
		public void SavefromSnap(string storage)
        {
			//一時領域からデータ取得
			string fullpath = StorageManager.Instance.savedataPath + storage;
			object obj = LoadFromBinaryFile(fullpath);

			if (obj == null)
            {
			
			}
			else
            {
                UserSaveDataInfo sobj = (UserSaveDataInfo)LoadFromBinaryFile(fullpath);
				string w_path = fullpath;
				SaveToBinaryFile(sobj, w_path);
			}
		}

		//plus が true の場合は、一つ進めたところをロードさせる。sleepgameの後とか戻ってきた時用
		//EX:たぶんplusとかいらなくなる
		public static void Serialize(string storage, bool plus = false)
        {
			Debug.Log("Trionfi:SaveData\"" + storage + "\"");

            //ToDo_Future:この辺は大幅にOriginalから変わってる
            UserSaveDataInfo sobj = new UserSaveDataInfo();
			sobj.name = storage;

			//タイトルとか、基本情報を格納
			sobj.title = StatusManager.Instance.messageForSaveTitle;
			sobj.date = DateTime.Now.ToString ("yyyy/MM/dd HH:mm:ss");
			sobj.currentMessage = StatusManager.Instance.messageForSaveTitle;

//            sobj.dicImage = TRLayerObjectManager.Instance.dicObject;
//			sobj.dicTag = TRLayerObjectManager.dicTag;
//			sobj.dicEvent = EventManager.dicEvent;
//			sobj.scriptManager = ScriptDecoder.Instance;
//			sobj.variable = ScriptDecoder.Instance.variable;
			sobj.currentFile = StatusManager.Instance.currentScenario;
			sobj.currentIndex = StatusManager.Instance.currentScenarioPosition;
//ToDo:
//			sobj.logManager = Trionfi.Instance.LogManager;
//ToDo:
			//ステータス
//			sobj.visibleMessageFrame = StatusManager.visibleMessageFrame;
//			sobj.enableNextOrder = StatusManager.enableNextOrder;
//			sobj.enableEventClick = StatusManager.enableEventClick;
//			sobj.enableClickOrder = StatusManager.enableClickOrder;
//ToDo:

			//画面のキャプチャを作成して保存する
			//保存先のパス

			if(plus == true)
				sobj.currentIndex++;

			//sobjをシリアライズ化して保存 
			string path = StorageManager.Instance.savedataPath + storage/*+".sav"*/;
			SaveToBinaryFile(sobj, path);
		}

		//ゲームをロードします
		public static void Deserialize(string storage)
        {
			Debug.Log("Trionfi:LoadData\"" + storage + "\"");

            UserSaveDataInfo sobj = GetSaveObject(storage);

			Dictionary<string, TRLayerObjectBehaviour> dic = sobj.dicImage;

			//イメージオブジェクトを画面に復元する
			foreach (KeyValuePair<string, TRLayerObjectBehaviour>kvp in sobj.dicImage)
			{
                //画面を復元していきます
                //				ImageObject image = new Image(dic[kvp.Key].dicSave);
                //				image.dicFace = dic[kvp.Key].dicFace;
                //TRLayerObjectManager.Instance.Create(kvp.Key, TRDataType.BG);
			}

			//タグも復元
//			EventManager.dicEvent = sobj.dicEvent;
//ToDo:Save
//			ScenarioManager = sobj.scenarioManager;
//			ScriptDecoder.Instance.variable = sobj.variable;
			//ToDo:Logmanagerクリア

			//グローバルで置き換える
//ToDo:
//			Trionfi.Instance.Serializer.LoadGlobalObject();
			//StatusManager.variable.replaceAll("global", NovelSingleton.GameManager.globalSetting.globalVar);

//			ScriptDecoder.Instance.LoadScenario(StatusManager.Instance.currentScenario);
			//開始位置の確認
			//StatusManager.Instance.currentScenario = sobj.currentFile;
//			ScriptDecoder.Instance.currentComponentIndex = sobj.currentIndex - 1;
			//テキストを復元する
			//Trionfi.Instance.MainMessage.CurrentMessage = sobj.currentMessage;
			StatusManager.Instance.messageForSaveTitle = sobj.currentMessage;

			//ステータス復元
//			StatusManager.Instance.visibleMessageFrame = sobj.visibleMessageFrame;
///			StatusManager.Instance.enableNextOrder = sobj.enableNextOrder;
//			StatusManager.Instance.enableClickOrder = sobj.enableClickOrder;
			StatusManager.Instance.enableEventClick = sobj.enableEventClick;
			StatusManager.Instance.currentPlayBgm = sobj.currentPlayBgm;
			StatusManager.Instance.isEventStop = sobj.isEventStop;
			//Debug.Log ("wwww:" + sobj.isEventStop);

			//ToDo:
			/*		Trionfi.Instance.LogManager = sobj.logManager;

					//メッセージウィドウが表示状態なら、ここで表示する

					if (StatusManager.visibleMessageFrame == true)
					{
						NovelSingleton.GameView.showMessageWithoutNextOrder(0f);
					}
					else
					{
						NovelSingleton.GameView.hideMessageWithoutNextOrder(0f);
					}
			*/
            /*
			if(StatusManager.Instance.currentPlayBgm != "") {
				Trionfi.AbstractComponent cmp = TRScriptParser.Instance.makeTag("[playbgm wait=false next=false storage='" + StatusManager.Instance.currentPlayBgm + "']");
				yield return cmp.Start();
			}
            */
			//何故か、、ここにいれないと。メッセージがすごく遅くなる
			//EX:いらないけど一応
			//NovelSingleton.GameManager.scene.messageSpeed = 0.02f;

			//ToDo:画面遷移を考えるとLoadLevelをしたほうがいいのかも。
		}
	

		/*		

		public string Base64FromStringComp(string st)
		{
			// 文字列をバイト配列に変換します
			byte[] source = Encoding.UTF8.GetBytes(st);
			return this._Base64FromStringComp(source);

		}

		public string _Base64FromStringComp(byte[] source)
		{



			// 入出力用のストリームを生成します
			MemoryStream ms = new MemoryStream();
			DeflateStream CompressedStream = new DeflateStream(ms, CompressionMode.Compress);

			// ストリームに圧縮するデータを書き込みます
			CompressedStream.Write(source, 0, source.Length);
			CompressedStream.Close();

			// 圧縮されたデータを バイト配列で取得します
			byte[] destination = ms.ToArray();

			//Base64で文字列に変換
			string base64String;
			base64String = System.Convert.ToBase64String(destination, Base64FormattingOptions.InsertLineBreaks);

			return base64String;
		}

//------------------------------------------
//BASE64文字列を戻し解凍の上で文字列に変換して返す
//
//------------------------------------------
		public string StringFromBase64Comp(string st)
		{
			Debug.Log (st);
#region BASE64文字列を圧縮バイナリへ戻す
			byte [] bs = System.Convert.FromBase64String(st);
#endregion

#region 圧縮バイナリを文字列へ解凍する

			// 入出力用のストリームを生成します
			MemoryStream ms = new MemoryStream(bs);
			MemoryStream ms2 = new MemoryStream();

			DeflateStream CompressedStream = new DeflateStream(ms, CompressionMode.Decompress);

			//　MemoryStream に展開します
			while (true)
			{
				int rb = CompressedStream.ReadByte();
				// 読み終わったとき while 処理を抜けます
				if (rb == -1)
				{
					break;
				}
				// メモリに展開したデータを読み込みます
				ms2.WriteByte((byte)rb);
			}

			string result = Encoding.UTF8.GetString(ms2.ToArray());
#endregion

			return result;
		}
		*/

	}
#endif
}