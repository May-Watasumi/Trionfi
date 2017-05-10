using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NovelEx {
	[Serializable]
	public class ScenarioManager
    {
		//マクロ情報
		[Serializable]
		public class Macro
		{
			public string name;
			public string file_name;
			public int index;

			public Macro() { }

			public Macro(string name, string file_name, int index)
			{
				this.name = name;
				this.file_name = file_name;
				this.index = index;
			}
		}

		//マクロ呼出しを保持するクラス
		[Serializable]
		public class CallStack
		{
			public Dictionary<string, string> dicVar = new Dictionary<string, string>();
			public string scenarioNname;
			public int index;

			public CallStack() { }

			public CallStack(string scenario_name, int index, Dictionary<string, string> dicVar)
			{
				this.scenarioNname = scenario_name;
				this.index = index;
				this.dicVar = dicVar;
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
				this.isIfProcess = val;
			}
		}

		[NonSerialized]
		private Dictionary<string, Scenario> dicScenario = new Dictionary<string,Scenario>();

		public Dictionary<string, Macro> dicMacro = new Dictionary<string,Macro>();

		[NonSerialized]
		NovelParser Parser = new NovelParser("NovelEx");
		public  NovelParser NovelParser{
			get{
				return Parser;
			}
		}

		//シナリオ変数
		[NonSerialized]
		public Variable variable = new Variable();

		//stackを配列に置き換える
		//public Stack<CallStack> qStack = new Stack<CallStack>();
		//public Stack<IfStack> ifStack = new Stack<IfStack>();

		public List<CallStack> qStack = new List<CallStack>();
		public List<IfStack> ifStack = new List<IfStack>();

		public int ifNum = 0;
		public int macroNum = 0;

		public List<AbstractComponent> arrayComponents = new List<AbstractComponent>();
		public int currentComponentIndex = -1;		//-1は未初期化		//EX変更点：０からスタート

		public ScenarioManager() { }

		public void StartScenario(string currentScenario, int index, List<AbstractComponent> arrayComponents = null)
		{
			if (arrayComponents != null)
			{
				this.arrayComponents = arrayComponents;
			}
			this.currentComponentIndex = index;
			JOKEREX.Instance.StatusManager.currentScenario = currentScenario;
			JOKEREX.Instance.StatusManager.MessageShow();
		}


		private Scenario getScenario(string file_name) {
//			JOKEREX.Instance.showLog("JOKEREX:LoadScenario \"" + file_name + "\"" + " exist=\"" + dicScenario.ContainsKey (file_name) + "\"");

			//シナリオからロードしてきた時はnull になってるからね
			if (this.dicScenario == null)
				this.dicScenario = new Dictionary<string,Scenario>();

			if(this.dicScenario.ContainsKey(file_name))
				return this.dicScenario[file_name];
			else
				return null;
		}

		const string dummyScenarioKey = "DUMMY_SCENARIO_KEY";	

		public Scenario loadScenariofromString(string text, string dicKey) {
			Debug.Log("JOKEREX:LoadScenario \"" + dicKey + "\"");	

			Scenario sce = String.IsNullOrEmpty(dicKey) ? null : getScenario(dicKey);

			if (String.IsNullOrEmpty(dicKey))
				dicKey = dummyScenarioKey;	

			if (sce == null)
			{
				sce = new Scenario();

				JOKEREX.Instance.errorManager.Clear();

				//パーサーを動作させる
				sce.arrayComponent = Parser.parseScript(text);

				if (JOKEREX.Instance.errorManager.showAll())
				{
					JOKEREX.Instance.errorManager.stopError("<color=red>致命的なエラーがあります。ゲームを開始できません</color>");
				}
				addScenario(dicKey, sce.arrayComponent);
			}

			arrayComponents = sce.arrayComponent;

			StartScenario(dicKey, 0, sce.arrayComponent);

			return sce;
		}

		public Scenario loadScenario(string storage)
		{
//			Debug.Log("JOKEREX:LoadScenario \"" + storage + "\"");
			Scenario sce = getScenario(storage);

			if(sce == null) 
			{
//				string fullpath = useStoragePath ? JOKEREX.Instance.StorageManager.PATH_SD_SCENARIO : "";
//				string script_text = JOKEREX.Instance.StorageManager.loadTextAsset(fullpath + storage);

				sce = loadScenariofromString(script_text, storage);
			}
			else
			{
				StartScenario(storage, 0, sce.arrayComponent);
			}

			return sce;
		}

		//シナリオの追加 ラベルの位置計算もここでやる
		public void addScenario(string scenario_name, List<AbstractComponent> list) {
			this.dicScenario [scenario_name] = new Scenario(scenario_name,list);
			int index = 0;

			foreach(AbstractComponent cmp in list){
				if (cmp.tagName == "label")
					this.dicScenario [scenario_name].addLabel(cmp.originalParam["name"],index);

				index++;
			}	
		}

		public int getIndex(string scenario_name,string label_name) {
			//シナリオがまだ読み込まれていない場合は読み込みを行う
			if (!this.dicScenario.ContainsKey (scenario_name))
				return -1;

			return this.dicScenario[scenario_name].getIndex (label_name);
		}

		public void addStack(string scenario_name,int index,Dictionary<string,string> dicVar) {
			//stack追加時にdicVarに呼び出し元情報を入れる
			//呼び出し元の情報はcaller_indexに入る。
			dicVar ["caller_index"] = ""+index;
			dicVar ["caller_file"] = scenario_name;

			var mp =     variable.getType("mp");

			////
			/*
			Debug.Log ("== add stack ===============");
			foreach (KeyValuePair<string, string> kvp in mp){
				Debug.Log (kvp.Key);
				Debug.Log(kvp.Value);
			}
			*/

			this.qStack.Add (new CallStack(scenario_name,index,mp));

			//スタックを追加した時点で使用できる引数変数を格納する
			variable.replaceAll("mp", dicVar); ;
		}

		public CallStack popStack() {
			try{
				CallStack c = this.qStack[this.qStack.Count-1];

				//var mp = StatusManager.variable.getType ("mp");

				/*
				Debug.Log ("== pop stack ===============");
				foreach (KeyValuePair<string, string> kvp in c.dicVar){
					Debug.Log (kvp.Key);
					Debug.Log(kvp.Value);
				}
				*/
				variable.replaceAll("mp", c.dicVar);

				this.qStack.RemoveAt(this.qStack.Count-1);

				return c;

			}
			catch(System.Exception e) {
				JOKEREX.Instance.errorManager.stopError("スタックが不足しています。callとreturnの関係を確認して下さい");
				Debug.Log (e.ToString());
				return null;
			}
		}

		public int countStack(){
			return this.qStack.Count;
		}

		/// <summary>
		/// //////if 周りのスタック管理
		/// </summary>

		public void addIfStack(bool proccess) {
			this.ifStack.Add (new IfStack(proccess));
		}

		public IfStack popIfStack() {
			try{ 
				IfStack c = this.ifStack[this.ifStack.Count-1];
				this.ifStack.RemoveAt(this.ifStack.Count-1);

				return c;

			}
			catch(System.Exception e) {
				JOKEREX.Instance.errorManager.stopError("スタックが不足しています。callとreturnの関係を確認して下さい");
				Debug.Log(e.ToString());
				return null;
			}
		}

		//現在のifスタックの状態を確認する
		public bool currentIfStack() {
			return this.ifStack[this.ifStack.Count-1].isIfProcess;	
		}

		//スタックの状態を変更する
		public void changeIfStack(bool proccess) {
			IfStack s = this.popIfStack();
			s.isIfProcess = proccess;
			this.ifStack.Add (s);
		}

		public int countIfStack() {
			return this.ifStack.Count;
		}

		//スタックをすべて削除します
		public void removeAllStacks() {
			//未実装
			this.qStack.Clear();
			this.ifStack.Clear();
			this.ifNum = 0;
		}

//// macro ///////
		public void addMacro(string macro_name,string file_name, int index)
		{
			this.dicMacro[macro_name] = new Macro(macro_name,file_name,index);
		}

		/*
		public void addMacroStack(string macro_name,Dictionary<string,string> dicVar){
			this.macroNum++;
			Macro macro = dicMacro [macro_name];
			this.addStack (macro.file_name, macro.index, dicVar);

		}
		*/

		public Macro getMacro(string macro_name) {
			if(!this.dicMacro.ContainsKey (macro_name))
				JOKEREX.Instance.errorManager.stopError("マクロ「" + macro_name + "」は見つかりませんでした");

			return this.dicMacro [macro_name]; 
		}

		public bool hasComponent = false;
		
		public void decodeScenario(string nextStorage = "")
		{
			hasComponent = true;

			//doComponentがtrueである限り1フレームで回る
			while(doComponent())
			{
//				if (NovelSingletonEx.StatusManager.isMessageShowing == true)
//					continue;

				//nextOrder を指定されたなら、クリックは有効になる
//				NovelSingletonEx.StatusManager.enableClickOrder = true;
			}
			hasComponent = false;
		}

//0=デフォルト1=componentのフラグが立ってない-1シナリオ最後に
		public bool doComponent()
		{
			if(currentComponentIndex < arrayComponents.Count)
			{
				AbstractComponent cmp = arrayComponents[currentComponentIndex];

				cmp.before();

				//タグ
				if (JOKEREX.Instance.StatusManager.currentState == JokerState.SkipOrder)
				{
					Debug.Log("SkipOrderされました");
				}
				else
				{
					cmp.calcVariable();
					cmp.validate();

					string p = "";
					foreach (KeyValuePair<string, string> kvp in cmp.param)
					{
						p += kvp.Key + "=" + kvp.Value + " ";
					}

					if(JOKEREX.Instance.SystemConfig.showTag)
					{
						Debug.Log("[" + cmp.tagName + " " + p + " ]");
					}

					cmp.start();
				}

				//EX変更：Afterも必ず実行される
				cmp.after();

				//ToDo:flag
				currentComponentIndex++;
//				Debug.Log("flag="+cmp.allowNextOrder());
				return cmp.allowNextOrder();
			}
			//シナリオファイルの最後まで来た時。スタックが存在するならreturn する
			//スタックがあるならreturn する
			if (JOKEREX.Instance.ScenarioManager.countStack() > 0)
			{
				JOKEREX.startTag("[return]");
			}
			else
			{
				JOKEREX.Instance.StatusManager.EndScenario();
			}
			return false;			
		}

		//次の命令へ
/*
		public void nextOrder() {
//			if (NovelSingletonEx.StatusManager.isMessageShowing == true)
//			{
//				return;
//			}

			//nextOrder を指定されたなら、クリックは有効になる
//			NovelSingletonEx.StatusManager.enableClickOrder = true;

			currentComponentIndex++;

			//シナリオファイルの最後まで来た時。スタックが存在するならreturn する
			if (currentComponentIndex >= arrayComponents.Count)
			{
				if (countStack() > 0)
				{
					//スタックがあるならreturn する
					JOKEREX.Instance.startTag("[return]");
				}
				else
				{			
					JOKEREX.Instance.terminateScenario();
				}
				return;
			}

			AbstractComponent cmp = arrayComponents[currentComponentIndex];

			cmp.before();

			if (NovelSingletonEx.StatusManager.skipOrder == false)
			{
				cmp.calcVariable();
				cmp.validate();

				string p = "";
				foreach (KeyValuePair<string, string> kvp in cmp.param)
				{
					p += kvp.Key + "=" + kvp.Value + " ";
				}
				//ToDo:
				//this.showLog("[" + cmp.tagName + " " + p + " ]");

				cmp.start();
				cmp.after();

			}
			else
			{
				this.nextOrder();
			}
		}
*/
	}
};
