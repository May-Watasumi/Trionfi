
#if !UNITY_WEBPLAYER
using UnityEngine;
using System.Collections;
//using UnityEditor;

using System.Collections.Generic;
using Novel;
using System.IO;
using System;

namespace Novel{

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

			//html += "<div class='howtoArea'> \n";


			var dicNav = new Dictionary<string,List<DocObject>> ();

			html += @"<div class='howtoMain'>
					  	<div><h2>基本</h2>
							<p>  [ ] で囲まれた部分がタグになります。 <br>                        
							@で始まる行も、タグとして認識しますが、１行で記述しなければなりません<br>
                    	    ;(セミコロン)で始まる行はコメントとして扱われます。<br>
                    	    複数行にわたってコメントにする場合は、/* からはじめて */ で 閉じることでコメントになります。　<br>
                    	    </p>
				    	 </div>";

			// タグの名前
			string array_script_str = "";

			foreach (DocObject d in this.arrDoc) {

				array_script_str += "'"+d.dicTag["tag"]+"',";

				//グループの作製
				if (!dicNav.ContainsKey (d.dicTag ["group"])) {

					dicNav [d.dicTag ["group"]] = new List<DocObject> ();

				}

				dicNav [d.dicTag ["group"]].Add (d);

				//グループ作製ここまで

				string html_main = "";


				//html += d.dicTag["title"]+ "\n";
				//howtoMain作製
				html_main +=@"
							<hr><br>            
								<a name='"+d.dicTag["tag"]+@"'></a>            
				             	<h3>["+d.dicTag["tag"]+"]"+d.dicTag["title"] +@"</h3>            
								<p>
									"+d.desc.Replace("\n","<br />")+@"
								</p>            

							";

				string html_param = "";
				html_param +=@"
					              <table>
					              <tbody><tr>
					              <th>パラメータ名</th><th>必須 </th><th>ﾃﾞﾌｫﾙﾄ</th><th>説明 </th></tr>
					              ";

				//パラメータを作っていく
				foreach (KeyValuePair<string, string> pair in d.dicParam) {

					string def ="";
					string vital = "×";

					if (d.dicParamVital.ContainsKey (pair.Key)) {
						vital = "◯";
					}

					if (d.dicParamDefault.ContainsKey (pair.Key)) {
						def = d.dicParamDefault [pair.Key];
					}

					//ここを繰り返して、パラメータを作る
					html_param +=@"
								  <tr>
								  <td>"+pair.Key+@"</td>
					              <td>"+vital+@"</td>
					              <td>"+def+@"</td>
					              <td>"+pair.Value+@"</td>
								  </tr>              
					            ";

//html += pair.Key +"="+ pair.Value+":default= "+def+"|vital="+vital +"\n"; 

				}

				if (d.dicParam.Count == 0) {

					html_param += "<tr><td colspan='4'>指定できるパラメータはありません</td></tr>";

				}


				html_param +=@"
					              </tbody>
					              </table>
					              ";


				string html_foot = "";

				html_foot +=@"
								  <br>                      
					              <p>
					              <span style='font-style:italic'>サンプルコード</span><br>
					              <code>"+d.sample.Replace("\n","<br/>")+@"</code>
					              </p>

					              <p style='float:right;font-size:10px;font-style:italic'>"+d.dicTag["group"]+@"</p>
					              <br style='clear:both'>
					              
								";

				html += html_main + html_param + html_foot + "";

			}


			html += "</div> <!--how_to_main -->";


			string html_nav = "<div class='howtoNav'> \n"; 


			foreach (KeyValuePair<string, List<DocObject>> pair in dicNav) {

				html_nav += "<p style='font-weight:bold'>■"+pair.Key+"</p>";
				html_nav += "<ul>";
				foreach (DocObject obj in dicNav[pair.Key]) {
					html_nav += "<div style='padding:2px'><a href='#"+obj.dicTag["tag"]+"'>["+obj.dicTag["tag"]+"]　<span style='font-style:italic;color:gray'>("+obj.dicTag["title"]+")</span></a></div>";
				}
				html_nav += "</ul>";

			}

			html_nav += "</div> <!-- howtoNav -->";
			html += html_nav;


			//quick_search
			string search_script = "";
			search_script += "<script type='text/javascript'>";
			search_script += "var availableTags = [";
				  
			search_script += array_script_str;

			search_script+="	];";

			search_script += "</script>";

			html += search_script;

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
					if (line == "")
						break;
					string[] tmp = line.Split ('=');
					this.obj.dicTag [tmp [0]] = tmp [1];
				} else if (this.status == "param") {
					if (line == "")
						break;

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

			//Novel.Bg_removeComponent

			//リフレクションで動的型付け　
			Debug.Log (className);
			Type masterType = Type.GetType (className);

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
						//continue;
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

			Debug.Log ("finish export doc ");

		}

	}

	

}

#endif



