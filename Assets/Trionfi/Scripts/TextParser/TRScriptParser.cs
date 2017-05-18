﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NovelEx;

namespace NovelEx
{
    //スクリプトファイルを読み込んで、適切な形にパースして返します

    public class TRScriptParser : SingletonMonoBehaviour<TRScriptParser>
    {
		public string errorMessage = "";
		public string warningMessage = "";

        public bool onRegistMacro = false;

        private System.Globalization.TextInfo tf = new System.Globalization.CultureInfo ("en-US", false).TextInfo;

        [SerializeField]
        public bool ignoreCR = true;

        [SerializeField]
        public string actorMarker = "【】";

        [SerializeField]
        public string actorTag = "talk_name";

        //ToDo:PlayerPrefsへ
        /*
                //コンフィグファイルを読み込んで返す
                public Dictionary<string,string> parseConfig (string config_text)
                {
                    Dictionary<string,string> dicConfig = new Dictionary<string,string>(); //コンフィグ

                    string[] lines = config_text.Split ('\n');

                    //lines の前に、一文字ずつ解析してすべての要素を分解する必要がある
                    for (int i = 0; i < lines.Length; i++)
                    {

                        string line = lines [i].Trim();
                        line = line.Replace ("\r", "").Replace ("\n", "").Replace ("\"", "").Replace ("'", "");

                        if (line == "")
                            continue;

                        if (line [0].ToString() == ";")
                            continue;

                        string[] arrayVal = line.Split ('=');

                        string key = arrayVal [0].Trim();
                        string val = arrayVal [1].Trim();

                        dicConfig [key] = val;
                    }

                    return dicConfig;
                }
        */
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
		private List<string> RevervedWords = new List<string>
        {
			"#SetignoreCR",
			"#ResetignoreCR",
			"#include",
			"#define",
		};	

		public bool parsePreproseccor(string lineText)
        {
			switch (lineText) {
			case "#SetignoreCR":
				ignoreCR = true;
				break;
			case "#ResetignoreCR":
				ignoreCR = false;
				break;
			default:
				return false;
			}
			return true;
		}

        //パースしたタグクラスのListを返す
		public List<AbstractComponent> parseScript (string script_text)
        {
			List<AbstractComponent> components = new List<AbstractComponent>();
			string[] lines = script_text.Split ('\n');

			List<LineObject> line_objects = new List<LineObject>();

			bool isCommentNow = false;

			//lines の前に、一文字ずつ解析してすべての要素を分解する必要がある
			for(int i = 0; i < lines.Length; i++)
            { 
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
				if(!string.IsNullOrEmpty(actorMarker)) {
					if (firstChar == actorMarker.Substring(0, 1)) {
						if(actorMarker.Length <= 1) {
							line = "[" + actorTag/*  talk_name */ + " val='" + line.Replace(firstChar, "") + "' ]";
							AbstractComponent cmp = this.makeTag(line, line_num);
							components.Add(cmp);
						}
						else if (line[line.Length-1] == actorMarker[1]) {
								line = line.Replace(actorMarker[1].ToString() , "");
								line = "[" + actorTag/*  talk_name */ + " val='" + line.Replace(firstChar, "") + "' ]";
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
						line = "[" + actorTag/* talk_name */ + " val='" + line.Replace("#", "") + "' ]";
						AbstractComponent cmp = this.makeTag(line, line_num);
						components.Add(cmp);
					}
					continue;
				}

				if(line == "\r"){
//ToDo:直前のRを消す
					if(isText == true && ignoreCR)
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

				if(isText == true && ignoreCR)
					components.Add(new RComponent());

			}

//			Debug.Log ("parse finish!");

			return components;
		}


		//１行のstringからタグを作成
		public AbstractComponent makeTag(string line)
        {
			AbstractComponent cmp = this.makeTag (line, 0);
			cmp.CalcVariable();
			return cmp;
		}

		//タグ名と引数の辞書からタグを生成
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
			cmp.CalcVariable();

			return cmp;
		}

		public AbstractComponent makeTag(string line, int line_num) {
			Tag tag = new Tag(line);

			//tagの種類によって、実装する命令が変わってくる
			AbstractComponent cmp = null;

			string className = tf.ToTitleCase(tag.Name) + "Component";

			//リフレクションで動的型付け
			Type masterType = Type.GetType(className);

			try
            {
				cmp = (AbstractComponent)Activator.CreateInstance(masterType);
			}
			catch(Exception e)
            {
#if UNITY_EDITOR
                Debug.Log(e.ToString());
                Debug.LogError("Tag:" + tag.Name);
#else
				//マクロとして登録
                ErrorLogger.Log("MacroStart:"+tag.Name);
				cmp = new _MacrostartComponent();
#endif
            }

            if(cmp != null)
            {
				cmp.Init(tag, line_num);
				//エラーメッセージの蓄積
				cmp.CheckParam();
				cmp.MergeDefaultParam();
			}

            return cmp;
		}
	}
}