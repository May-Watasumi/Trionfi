using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Novel;
using System.IO;
using System;


public class Exporter
{
	[MenuItem("Assets/ExportWithSettings")]
	static void Export () {
		//AssetDatabase.ExportPackage ("Assets", "JOKER.unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies );
		AssetDatabase.ExportPackage ("Assets", "JOKER.unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets | ExportPackageOptions.IncludeDependencies);

	}

	[MenuItem("Assets/createDocs")]
	static void createDoc(){
		DocGenerator.start ();
	}

}

public class DocObject{

	public string desc = "";
	public string sample ="";
	public Dictionary<string,string> dicParam = new Dictionary<string,string> ();
	public Dictionary<string,string> dicParamVital = new Dictionary<string,string> ();
	public Dictionary<string,string> dicParamDefault = new Dictionary<string,string> ();

	public Dictionary<string,string> dicTag = new Dictionary<string,string> ();

}

public class DocManager{


	List<DocObject> arrDoc = new List<DocObject>();
	DocObject obj = new DocObject();
	string status ="";

	public DocManager(){

	}

	public void showInfo(){

		string html = "";

		Debug.Log ("========== show info ");
		foreach (DocObject d in this.arrDoc) {

			Debug.Log (d.dicTag ["title"]);
			Debug.Log (d.desc);

			html += d.dicTag["title"]+ "\n";

			foreach (KeyValuePair<string, string> pair in d.dicParam) {

				string def ="";
				string vital = "×";

				if (d.dicParamVital.ContainsKey (pair.Key)) {
					vital = "◯";
				}

				if (d.dicParamDefault.ContainsKey (pair.Key)) {
					def = d.dicParamDefault [pair.Key];
				}


				html += pair.Key +"="+ pair.Value+":default= "+def+"|vital="+vital +"\n"; 

			}


		}


		StreamWriter writer = new StreamWriter("./doc.txt");
		writer.WriteLine(html);
		writer.Close();

	}




	public void addInfo(string line){


		line = line.Trim ();
		//タグ情報の作製
		switch (line) {

		case "[doc]":
			this.status = "tag";
			break;

		case "[desc]":
			this.status = "desc";
			break;

		case "[sample]":
			this.status = "sample";
			break;

		case "[param]":
			this.status = "param";
			break;
		case "[_doc]":
			this.flush ();
			break;

		default:

			if (this.status == "tag") {
				string[] tmp = line.Split ('=');
				this.obj.dicTag [tmp [0]] = tmp [1];
			} else if (this.status == "param") {
				string[] tmp = line.Split ('=');
				this.obj.dicParam [tmp [0]] = tmp [1];
			} else if (this.status == "desc") {
				this.obj.desc += line + System.Environment.NewLine;
			} else if (this.status == "sample") {
				this.obj.sample += line + System.Environment.NewLine;
			}



			break;


		}

	}

	public void flush(){
		System.Console.WriteLine ("FLSH!!!!!!!!!");

		string tag = this.obj.dicTag ["tag"];

		System.Globalization.TextInfo tf = new System.Globalization.CultureInfo ("en-US", false).TextInfo;

		string className = "Novel." + tf.ToTitleCase (tag) + "Component";

		Debug.Log (className);

		//Novel.Bg_removeComponent

		AbstractComponent test = new Novel.Bg_removeComponent ();
		Debug.Log (test);
		Debug.Log (test.GetType ().FullName);

		className = test.GetType ().FullName;

		//リフレクションで動的型付け　
		Type masterType = Type.GetType (className);

		Debug.Log (masterType);

		AbstractComponent cmp;
		cmp = (AbstractComponent)Activator.CreateInstance (masterType);

		this.obj.dicParamDefault = cmp.originalParam;

		Dictionary<string,string> tmpDic = new Dictionary<string,string> ();
		List<string> l = cmp.arrayVitalParam;
		for (var i = 0; i < l.Count; i++) {
			string vital = l [i];
			tmpDic [vital] = "yes";
		}

		this.obj.dicParamVital	= tmpDic; 
		//必須パラメータとかデフォルト値を取得
	 
		this.arrDoc.Add (this.obj);
		this.obj = new DocObject ();
		this.status = "";
	}

}

public class DocGenerator{

	public static void start(){

		Debug.Log ("start document generator ");

		Debug.Log (System.Environment.CurrentDirectory);

		string path = System.Environment.CurrentDirectory +"/Assets/JOKER/Scripts/Novel/Components/";

		//指定フォルダ以下のディレクトリ全部読み込み
		string[] files = System.IO.Directory.GetFiles(
			path, "*", System.IO.SearchOption.AllDirectories);

		List<string > arrDoc = new List<string> ();

		for (var i = 0; i < files.Length; i++) {
			System.Console.WriteLine (files [i]);

			System.IO.StreamReader cReader = (
				new System.IO.StreamReader(files[i], System.Text.Encoding.Default)
			);


			bool flag_doc = false;
			// 読み込みできる文字がなくなるまで繰り返す
			while (cReader.Peek () >= 0) {
				// ファイルを 1 行ずつ読み込む
				string stBuffer = cReader.ReadLine ().Trim ();

				if (stBuffer == "") {
					continue;
				}

				if (stBuffer == "[doc]") {
					flag_doc = true;
				}

				if (flag_doc == true) {
					arrDoc.Add(stBuffer);
				}

				if (stBuffer == "[_doc]") {
					flag_doc = false;
				}
			}

			// cReader を閉じる (正しくは オブジェクトの破棄を保証する を参照)
			cReader.Close();


		}

		DocManager dm = new DocManager ();

		//パーサーを動作させる

		foreach (string doc in arrDoc) {
			System.Console.WriteLine (doc);
			dm.addInfo (doc);
		}

		dm.showInfo ();

	}

}
