using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NovelEx;

namespace NovelEx {
	//コンポーネントのパラメータとなる
	/// <summary>
	/// コンポーネント用のパラメータ
	/// </summary>
	public class Tag
	{
		private string name = "";
		private string original = "";
		private Dictionary<string,string> dicParam = new Dictionary<string,string>();
		//パラメータを保持する
		public Tag (string str)
		{

			this.original = str;
		
			str = str.Replace ("[", "").Replace ("]", "");

			//storage = 3 name=4 param=5 みたいな文字列が渡ってくる
			//name storage = 4
			//bool flag_start_tag = false;
			bool flag_finish_tag = false;

			string tag_name = "";

			int end_tag_index = 0;

			for (int i = 0; i < str.Length; i++) {
				string c = str [i].ToString();
				if (c == " ") {
					//flag_start_tag = false;
					this.name = tag_name;
					end_tag_index = i;

					break;

				} else {
					tag_name += c;
					continue;
				}

			}

			if (this.name == "") {
				this.name = tag_name;
				flag_finish_tag = true;
			}


			if (!flag_finish_tag) {

				//ここまでで、タグ解析完了
				string param_str = str.Substring (end_tag_index).Trim();

				bool flag_eq = false;
				bool flag_qt = false;
				bool flag_eq_ch = false; //イコール後に文字列が来たかどうか

				string key = "";
				string val = "";

				for (int i = 0; i < param_str.Length; i++) {

					string c = param_str [i].ToString();

					//イコール前の空白は無視
					if (c == " " && flag_eq == false) {
						continue;
					}

					if (c == "=" && flag_eq == false) {
						flag_eq = true;
						continue;
					}

					if (flag_eq == false) {
						key += c;
						continue;
					}

					//イコール以後の解析部分
					if (flag_eq == true) {

						if (flag_eq_ch == false && c == " ") {
							continue;
						}

						if (c == "\"" || c == "'") {

							if (flag_qt == false) {
								flag_qt = true;
								flag_eq_ch = true;
								continue;
							} else {

								//パラメータ設定の終わり
								flag_eq = false;
								flag_qt = false;
								flag_eq_ch = false;

								//値を登録
								this.dicParam [key] = val;
								key = "";
								val = "";


							}


						} else {

							if (c == " " && flag_qt == false) {

								flag_eq = false;
								flag_qt = false;
								flag_eq_ch = false;

								this.dicParam [key] = val;
								key = "";
								val = "";

							} else {
								val += c;
								flag_eq_ch = true;
							}

							if (i == param_str.Length - 1) {
								//最後の文字の場合
								this.dicParam [key] = val;
							}
						}			
					}
				}

				/*
			foreach(string t in this.dicParam.Keys)
			{
				Debug.Log(t +"="+ this.dicParam[t]);
			}
			*/
			}
		}

		public string Original {
			get{ return this.original; }
		}

		public string Name {
			get{ return this.name; }
		}

		public string getTagName()
		{
			return this.name;
		}

		public string getParam (string key)
		{

			if (this.dicParam.ContainsKey (key)) {
				return this.dicParam [key];
			} else {
				return null;
			}

		}

		public Dictionary<string,string> getParamByDictionary()
		{
			Dictionary<string,string> dic = new Dictionary<string,string>();

			foreach (KeyValuePair<string, string> pair in this.dicParam) {
				dic [pair.Key] = pair.Value;
			}

			return dic;

		}
	}
	//スクリプトファイルを読み込んで、適切な形にパースして返します
	/*

*start

[cm  ]test [l][r]
[back  storage="room.jpg"  time="5000"  ]

*/
	public class NovelParser : SingletonMonoBehaviour<NovelParser>{
		public string errorMessage = "";
		public string warningMessage = "";

		private System.Globalization.TextInfo tf = new System.Globalization.CultureInfo ("en-US", false).TextInfo;

		private string classPrerix = "";

		public NovelParser(string classPrefix){
			this.classPrerix = classPrefix;
		}

		//コンフィグファイルを読み込んで返す
		public Dictionary<string,string> parseConfig (string config_text)
		{
			Dictionary<string,string> dicConfig = new Dictionary<string,string>(); //コンフィグ

			string[] lines = config_text.Split ('\n');

			//lines の前に、一文字ずつ解析してすべての要素を分解する必要がある
			for (int i = 0; i < lines.Length; i++) {

				string line = lines [i].Trim();
				line = line.Replace ("\r", "").Replace ("\n", "").Replace ("\"", "").Replace ("'", "");

				if (line == "")
					continue;

				if (line [0].ToString() == ";") {
					continue;
				}

				string[] arrayVal = line.Split ('=');

				string key = arrayVal [0].Trim();
				string val = arrayVal [1].Trim();

				dicConfig [key] = val;
			}

			return dicConfig;
		}

		struct LineObject
        {
			public int line_num;
			public string line;
			public LineObject (int line_num, string line)
			{
				this.line_num = line_num;
				this.line = line;
			}
		}

//EX:予約語	
		private List<string> RevervedWords = new List<string> {
			"#SetignoreCR",
			"#ResetignoreCR",
			"#include",
			"#define",
		};	

		public bool parsePreproseccor(string lineText) {
			switch (lineText) {
			case "#SetignoreCR":
				JOKEREX.Instance.SystemConfig.ignoreCR = true;
				break;
			case "#ResetignoreCR":
				JOKEREX.Instance.SystemConfig.ignoreCR = false;
				break;
			default:
				return false;
			}
			return true;
		}

		public List<AbstractComponent> parseScript (string script_text) {
			//GameManager gameManager = NovelSingleton.GameManager;
			List<AbstractComponent> components = new List<AbstractComponent>();
			string[] lines = script_text.Split ('\n');

			List<LineObject> line_objects = new List<LineObject>();

			bool isCommentNow = false;

			//lines の前に、一文字ずつ解析してすべての要素を分解する必要がある
			for(int i = 0; i < lines.Length; i++) { 
				string line = lines[i].Trim();
//EX:
				if (line == "\r" || line == "\n" || line == "\r\n" || line == "") { 
					line_objects.Add(new LineObject(i + 1, "\r"));
					continue;
				}

				line = line.Replace ("\r", "").Replace ("\n", "");

				//Debug.Log (line);

				if(line == "")
					continue;

				string firstChar = line[0].ToString();

				//コメント開始
				if(line.IndexOf ("/*") != -1)
					isCommentNow = true;

				if (line.IndexOf("*/") != -1) {
					isCommentNow = false;
					continue;
				}

				if(isCommentNow == true)
					continue;

				// ;で始まってたらコメントなので無視する
				if (firstChar == ";")
					continue;

				line = line.Replace("|", "\r\n");

				//ラベルを表します
				if (line.IndexOf("*/") == -1 && firstChar == "*")
					line = "[label name='"+line.Replace("*","").Trim()+"' ]";

				//１行の命令なので、残りの文字列をまとめて、タグ作成に回す
				//１行のタグ命令にして渡す
				if (firstChar == "@")
					line = "["+ line.Replace("@","") +"]";

				if (firstChar == "#") {
					line_objects.Add(new LineObject(i + 1, line));
					continue;
				}

				bool flag_now_tag = false;
				StringBuilder tag_line = new StringBuilder();

				for (int k = 0; k < line.Length; k++) {
					string c = line [k].ToString();

					if (c == "[" && flag_now_tag == true) {
						line_objects.Add (new LineObject (i + 1, tag_line.ToString()));
						flag_now_tag = false;
						tag_line = new StringBuilder();
					}

					tag_line.Append (c);

					//最後の一文字の場合
					if (k == line.Length - 1) {
						line_objects.Add (new LineObject (i + 1, tag_line.ToString()));
						continue;
					}

					flag_now_tag = true;

					if(c == "]") {
						flag_now_tag = false;
						line_objects.Add (new LineObject (i + 1, tag_line.ToString()));
						tag_line = new StringBuilder();
					}
				}
			}
//EX:
			bool isText = false;

			foreach (LineObject lo in line_objects) {
				string line = lo.line;
				int line_num = lo.line_num;

				string firstChar = line[0].ToString();		

//EX:プリプロセッサ的なアレ。
//ToDo:名前タグを【】にする
				if(!string.IsNullOrEmpty(JOKEREX.Instance.SystemConfig.ActorMarker)) {
					if (firstChar == JOKEREX.Instance.SystemConfig.ActorMarker.Substring(0, 1)) {
						if(JOKEREX.Instance.SystemConfig.ActorMarker.Length <= 1) {
							line = "[" + JOKEREX.Instance.SystemConfig.ActorCallBack/*  talk_name */ + " val='" + line.Replace(firstChar, "") + "' ]";
							AbstractComponent cmp = this.makeTag(line, line_num);
							components.Add(cmp);
						}
						else if (line[line.Length-1] == JOKEREX.Instance.SystemConfig.ActorMarker[1]) {
								line = line.Replace(JOKEREX.Instance.SystemConfig.ActorMarker[1].ToString() , "");
								line = "[" + JOKEREX.Instance.SystemConfig.ActorCallBack/*  talk_name */ + " val='" + line.Replace(firstChar, "") + "' ]";
								AbstractComponent cmp = this.makeTag(line, line_num);
								components.Add(cmp);
						}
						else { /*error?*/}
						continue;
					}
				}				

				if (firstChar == "#") {
					if(RevervedWords.Contains(line)) {
						parsePreproseccor(line);
					}
					else {
						line = "[" + JOKEREX.Instance.SystemConfig.ActorCallBack/* talk_name */ + " val='" + line.Replace("#", "") + "' ]";
						AbstractComponent cmp = this.makeTag(line, line_num);
						components.Add(cmp);
					}
					continue;
				}

				if(line == "\r"){
//ToDo:直前のRを消す
					if(isText == true && JOKEREX.Instance.SystemConfig.ignoreCR)
						components.Add(new PComponent());
					
					isText = false;
					
					continue;
				}
	
				//テキストファイルの場合
				if (firstChar != "[" && firstChar != "@") {			
					line = "[story val=\"" + line + "\"]"; 
					firstChar = "[";
					isText = true;
				}
				else
					isText = false;

				if (firstChar == "[" || firstChar == "@") {
					//Debug.Log ("------------");
					//Debug.Log (line);

					AbstractComponent cmp = this.makeTag (line,line_num);

					//リストに追加
					components.Add (cmp);
				}

				if(isText == true && JOKEREX.Instance.SystemConfig.ignoreCR)
					components.Add(new RComponent());

			}

//			Debug.Log ("parse finish!");

			return components;
		}

		//ラインからタグを作成

		//コンポーネントから即実行されるタグを生成
		public AbstractComponent makeTag(string line) {
			AbstractComponent cmp = this.makeTag (line, 0);
			cmp.calcVariable();
			return cmp;
		}

		//コンポーネントから即実行されるタグを生成
		public AbstractComponent makeTag(string tag_name,Dictionary<string,string> param)
        {
			string line = "["+tag_name+" ";
			string param_str = "";
			foreach (KeyValuePair<string, string> pair in param)
            {
				param_str += pair.Key +"="+ pair.Value+" "; 
			}

			line = line + param_str +"]";

			Debug.Log (line);

			AbstractComponent cmp = this.makeTag (line, 0);
			cmp.calcVariable();

			return cmp;
		}

		public AbstractComponent makeTag(string line, int line_num) {
			Tag tag = new Tag (line);

			//tagの種類によって、実装する命令が変わってくる
			AbstractComponent cmp = null;

			string className = this.classPrerix+"."+tf.ToTitleCase (tag.Name) + "Component";

			//リフレクションで動的型付け
			Type masterType = Type.GetType(className);

			try {
				cmp = (AbstractComponent)Activator.CreateInstance (masterType);
			}
			catch(Exception e) 	{
				Debug.Log (e.ToString());
				//マクロとして登録
				cmp = new _MacrostartComponent();
			}

			if (cmp != null) {
				cmp.init (tag, line_num);

				//エラーメッセージの蓄積
				cmp.checkVital();
				cmp.mergeDefaultParam();
			}
			return cmp;
		}
	}
}
