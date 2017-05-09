using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Text;
using NovelEx;

namespace NovelEx {
	public class Serializer {
		public Serializer() { }

		private SaveGlobalObject globalObject = new SaveGlobalObject();
		public SaveGlobalObject globalSetting {
			get {
				return globalObject;
			}
		}

		//グローバルセッティングを保存します
		public void SaveGlobalObject(string storage)
		{
			string json = LitJson.JsonMapper.ToJson(this);

            //WebPlayer の場合保存方法に変化を入れる
#if false
            if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
			{
				PlayerPrefs.SetString(storage, json);
			}
			else
#endif
    {
				string path = JOKEREX.Instance.StorageManager.path_savedata.path + storage;

				//ディレクトリ存在チェック
				if (!Directory.Exists(JOKEREX.Instance.StorageManager.path_savedata.path))
					Directory.CreateDirectory(JOKEREX.Instance.StorageManager.path_savedata.path);

				FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
				StreamWriter sw = new StreamWriter(fs);
				sw.Write(json);
				sw.Flush();
				sw.Close();
				fs.Close();
			}
		}
		public void LoadGlobalObject(string storage) {
			//WebPlayer の場合保存方法に変化を入れる
			if(Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer) {
				if(!PlayerPrefs.HasKey(storage))
					SaveGlobalObject(storage);

				string json = PlayerPrefs.GetString(storage);

				SaveGlobalObject obj = LitJson.JsonMapper.ToObject<SaveGlobalObject> (json);
				globalObject = obj;
			}
			else {
				string path = JOKEREX.Instance.StorageManager.path_savedata.path + storage;

				//ファイル作成
				if(!File.Exists(path))
					SaveGlobalObject(storage);

				FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

				StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);
				string json = sr.ReadToEnd();

				SaveGlobalObject obj = LitJson.JsonMapper.ToObject<SaveGlobalObject> (json);

				sr.Close();
				fs.Close();

//				JOKEREX.Instance.UserDataManager.globalSetting = obj;
			}

			//グローバル変数を格納する
			JOKEREX.Instance.ScenarioManager.variable.replaceAll("global", globalObject.globalVar);
			JOKEREX.Instance.ScenarioManager.variable.trace("global");
		}

		public SaveObject GetSaveObject(string storage) {
			string fullpath = JOKEREX.Instance.StorageManager.PATH_SAVEDATA + storage;
			SaveObject obj = (SaveObject)LoadFromBinaryFile(fullpath);
			return obj;
		}

		public void SaveToBinaryFile(SaveObject obj, string storage)
		{
			string json = LitJson.JsonMapper.ToJson(obj);

			//WebPlayer の場合保存方法に変化を入れる
			if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
				PlayerPrefs.SetString(storage, json);
			else
			{
				if (!Directory.Exists(JOKEREX.Instance.StorageManager.PATH_SAVEDATA))
					Directory.CreateDirectory(JOKEREX.Instance.StorageManager.PATH_SAVEDATA);

				FileStream fs = new FileStream(JOKEREX.Instance.StorageManager.PATH_SAVEDATA + storage, FileMode.Create, FileAccess.Write);

				StreamWriter sw = new StreamWriter(fs);
				sw.Write(json);
				sw.Flush();
				sw.Close();
				fs.Close();
			}
		}

		public object LoadFromBinaryFile(string storage)
		{
			//WebPlayer の場合保存方法に変化を入れる
			if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
			{
				string json = PlayerPrefs.GetString(storage);
				SaveObject obj = LitJson.JsonMapper.ToObject<SaveObject>(json);
				return obj;
			}
			else
			{
				if (!File.Exists(storage))
					return null;

				FileStream fs = new FileStream(storage, FileMode.Open, FileAccess.Read);
				StreamReader sr = new StreamReader(storage, System.Text.Encoding.Default);

				string json = sr.ReadToEnd();

				if (json == "")
					return null;

				SaveObject obj = LitJson.JsonMapper.ToObject<SaveObject>(json);

				sr.Close();
				fs.Close();

				return obj;
			}
		}

		public void applySaveVariable(string storage, Variable variable, string var_name = "save")
		{
			//最初のセーブデータを取得するか。
			SaveObject sobj = GetSaveObject(storage);
			//this.gameManager.saveManager.getSaveData ("save_"+current_index);
			variable.set(var_name + ".name", storage);
			//ToDo:
			if (sobj != null)
			{
				variable.set(var_name + ".title", sobj.title);
				variable.set(var_name + ".date", sobj.date);
				variable.set(var_name + ".description", sobj.description);
				variable.set(var_name + ".name", storage);
				//StatusManager.variable.set ("save.img", sobj.cap_img_file);
			}
			else
			{
				variable.set(var_name + ".title", "データがありません");
				variable.set(var_name + ".date", "");
				variable.set(var_name + ".description", "");
				variable.set(var_name + ".name", storage);
				//StatusManager.variable.set ("save.img", "");
			}
		}


		//一時退避しておいたスナップから保存を実行する
		public void SavefromSnap(string storage) {
			//一時領域からデータ取得
			string fullpath = JOKEREX.Instance.StorageManager.PATH_SAVEDATA + storage;
			object obj = LoadFromBinaryFile(fullpath);

			if (obj == null) {
			
			}
			else {
				SaveObject sobj = (SaveObject)LoadFromBinaryFile(fullpath);
				string w_path = fullpath;
				SaveToBinaryFile(sobj, w_path);
			}
		}

		//plus が true の場合は、一つ進めたところをロードさせる。sleepgameの後とか戻ってきた時用
		//EX:たぶんplusとかいらなくなる
		public void Serialize(string storage, bool plus = false) {
			Debug.Log("JOKEREX:SaveData\"" + storage + "\"");

//ToDo_Future:この辺は大幅にOriginalから変わってる
			SaveObject sobj = new SaveObject();
			sobj.name = storage;

			//タイトルとか、基本情報を格納
			sobj.title = JOKEREX.Instance.StatusManager.messageForSaveTitle;
			sobj.date = DateTime.Now.ToString ("yyyy/MM/dd HH:mm:ss");
			sobj.currentMessage = JOKEREX.Instance.StatusManager.messageForSaveTitle;

			sobj.dicImage = JOKEREX.Instance.ImageManager.dicImage;
			sobj.dicTag = JOKEREX.Instance.ImageManager.dicTag;
			sobj.dicEvent = JOKEREX.Instance.EventManager.dicEvent;
			sobj.scenarioManager = JOKEREX.Instance.ScenarioManager;
			sobj.variable = JOKEREX.Instance.ScenarioManager.variable;
			sobj.currentFile = JOKEREX.Instance.StatusManager.currentScenario;
			sobj.currentIndex = JOKEREX.Instance.StatusManager.currentScenarioPosition;
//ToDo:
//			sobj.logManager = JOKEREX.Instance.LogManager;
//ToDo:
			//ステータス
//			sobj.visibleMessageFrame = StatusManager.visibleMessageFrame;
//			sobj.enableNextOrder = StatusManager.enableNextOrder;
//			sobj.enableEventClick = StatusManager.enableEventClick;
//			sobj.enableClickOrder = StatusManager.enableClickOrder;
//ToDo:
			sobj.currentPlayBgm = JOKEREX.Instance.StatusManager.currentPlayBgm;
			sobj.isEventStop = JOKEREX.Instance.StatusManager.isEventStop;

			//画面のキャプチャを作成して保存する
			//保存先のパス

			if(plus == true)
				sobj.currentIndex++;

			//sobjをシリアライズ化して保存 
			string path = JOKEREX.Instance.StorageManager.path_savedata.path + storage/*+".sav"*/;
			SaveToBinaryFile(sobj, path);
		}

		//ゲームをロードします
		public void Deserialize(string storage) {
			Debug.Log("JOKEREX:LoadData\"" + storage + "\"");

			SaveObject sobj = GetSaveObject(storage);

			Dictionary<string, Image> dic = sobj.dicImage;

			//イメージオブジェクトを画面に復元する
			foreach (KeyValuePair<string, Image> kvp in sobj.dicImage)
			{
				//画面を復元していきます
				Image image = new Image(dic[kvp.Key].dicSave);
				image.dicFace = dic[kvp.Key].dicFace;
				JOKEREX.Instance.ImageManager.addImage(image);
			}

			//タグも復元
			JOKEREX.Instance.ImageManager.dicTag = sobj.dicTag;
			JOKEREX.Instance.EventManager.dicEvent = sobj.dicEvent;
//ToDo:Save
//			JOKEREX.Instance.ScenarioManager = sobj.scenarioManager;
			JOKEREX.Instance.ScenarioManager.variable = sobj.variable;
			//ToDo:Logmanagerクリア

			//グローバルで置き換える
//ToDo:
//			JOKEREX.Instance.Serializer.LoadGlobalObject();
			//StatusManager.variable.replaceAll("global", NovelSingleton.GameManager.globalSetting.globalVar);

			JOKEREX.Instance.ScenarioManager.loadScenario(JOKEREX.Instance.StatusManager.currentScenario);
			//開始位置の確認
			//JOKEREX.Instance.StatusManager.currentScenario = sobj.currentFile;
			JOKEREX.Instance.ScenarioManager.currentComponentIndex = sobj.currentIndex - 1;
			//テキストを復元する
			//JOKEREX.Instance.MainMessage.CurrentMessage = sobj.currentMessage;
			JOKEREX.Instance.StatusManager.messageForSaveTitle = sobj.currentMessage;

			//ステータス復元
//			JOKEREX.Instance.StatusManager.visibleMessageFrame = sobj.visibleMessageFrame;
///			JOKEREX.Instance.StatusManager.enableNextOrder = sobj.enableNextOrder;
//			JOKEREX.Instance.StatusManager.enableClickOrder = sobj.enableClickOrder;
			JOKEREX.Instance.StatusManager.enableEventClick = sobj.enableEventClick;
			JOKEREX.Instance.StatusManager.currentPlayBgm = sobj.currentPlayBgm;
			JOKEREX.Instance.StatusManager.isEventStop = sobj.isEventStop;
			//Debug.Log ("wwww:" + sobj.isEventStop);

			//ToDo:
			/*		JOKEREX.Instance.LogManager = sobj.logManager;

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
			if(JOKEREX.Instance.StatusManager.currentPlayBgm != "") {
				NovelEx.AbstractComponent cmp = JOKEREX.Instance.ScenarioManager.NovelParser.makeTag("[playbgm wait=false next=false storage='" + JOKEREX.Instance.StatusManager.currentPlayBgm + "']");
				cmp.start();
			}
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

}