using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NovelEx
{
    [Serializable]
	public class ScriptDecoder
    {
        [NonSerialized]
        public static ScriptDecoder Instance = null;

        //マクロ情報はシステム共通
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

        //変数インスタンスは１つ
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
		public int currentComponentIndex = -1;		//-1は未初期化		//EX変更点：0からスタート

		public ScriptDecoder() { Instance = this; }

		public void StartScenario(string currentScenario, int index, List<AbstractComponent> arrayComponents = null)
		{
			if (arrayComponents != null)
			{
				this.arrayComponents = arrayComponents;
            }

            this.currentComponentIndex = index;
			StatusManager.Instance.currentScenario = currentScenario;
			StatusManager.Instance.MessageShow();
		}

		private Scenario GetInstance(string file_name)
        {
//			JOKEREX.Instance.showLog("JOKEREX:LoadScenario \"" + file_name + "\"" + " exist=\"" + dicScenario.ContainsKey (file_name) + "\"");

			//シナリオからロードしてきた時はnull になってるからね
			if(dicScenario == null)
				dicScenario = new Dictionary<string,Scenario>();

			if(dicScenario.ContainsKey(file_name))
				return dicScenario[file_name];
			else
				return null;
		}

		const string dummyScenarioKey = "DUMMY_SCENARIO_KEY";	

		public Scenario LoadScenariofromString(string text, string dicKey) {
			Debug.Log("JOKEREX:LoadScenario \"" + dicKey + "\"");	

			Scenario sce = String.IsNullOrEmpty(dicKey) ? null : GetInstance(dicKey);

			if(String.IsNullOrEmpty(dicKey))
				dicKey = dummyScenarioKey;	

			if(sce == null)
			{
				sce = new Scenario();

				ErrorLogger.Clear();

				//パーサーを動作させる
				sce.arrayComponent = TRScriptParser.Instance.parseScript(text);

				if(ErrorLogger.showAll())
				{
					ErrorLogger.stopError("<color=red>致命的なエラーがあります。ゲームを開始できません</color>");
				}
				AddScenario(dicKey, sce.arrayComponent);
			}

			arrayComponents = sce.arrayComponent;

			StartScenario(dicKey, 0, sce.arrayComponent);

			return sce;
		}

		public Scenario LoadScenario(string storage)
		{
//			Debug.Log("JOKEREX:LoadScenario \"" + storage + "\"");
			Scenario sce = GetInstance(storage);

			if(sce == null) 
			{
				string fullpath = /*useStoragePath ? StorageManager.Instance.PATH_SD_SCENARIO :*/ "";
				string script_text = StorageManager.Instance.LoadTextAsset(fullpath + storage);

				sce = LoadScenariofromString(script_text, storage);
			}
			else
			{
				StartScenario(storage, 0, sce.arrayComponent);
			}

			return sce;
		}

		//シナリオの追加 ラベルの位置計算もここでやる
		public void AddScenario(string scenario_name, List<AbstractComponent> list) {
			dicScenario [scenario_name] = new Scenario(scenario_name,list);
			int index = 0;

			foreach(AbstractComponent cmp in list)
            {
				if (cmp.tagName == "label")
					dicScenario[scenario_name].addLabel(cmp.originalParamDic["name"],index);

				index++;
			}	
		}

		public int GetIndex(string scenario_name, string label_name)
        {
			//シナリオがまだ読み込まれていない場合は読み込みを行う
			if (!dicScenario.ContainsKey (scenario_name))
				return -1;

			return dicScenario[scenario_name].getIndex (label_name);
		}

		public void AddStack(string scenario_name, int index, Dictionary<string,string> dicVar) {
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

			qStack.Add(new CallStack(scenario_name,index,mp));

			//スタックを追加した時点で使用できる引数変数を格納する
			variable.replaceAll("mp", dicVar); ;
		}

		public CallStack PopStack() {
			try{
				CallStack c = qStack[qStack.Count-1];

				//var mp = StatusManager.variable.getType ("mp");

				/*
				Debug.Log ("== pop stack ===============");
				foreach (KeyValuePair<string, string> kvp in c.dicVar){
					Debug.Log (kvp.Key);
					Debug.Log(kvp.Value);
				}
				*/
				variable.replaceAll("mp", c.dicVar);

				qStack.RemoveAt(qStack.Count-1);

				return c;

			}
			catch(System.Exception e) {
				ErrorLogger.stopError("スタックが不足しています。callとreturnの関係を確認して下さい");
				Debug.Log (e.ToString());
				return null;
			}
		}

		public int CountStack()
        {
			return qStack.Count;
		}

		/// <summary>
		/// //////if 周りのスタック管理
		/// </summary>

		public void AddIfStack(bool proccess) {
			ifStack.Add (new IfStack(proccess));
		}

		public IfStack PopIfStack() {
			try{ 
				IfStack c = ifStack[ifStack.Count-1];
				ifStack.RemoveAt(ifStack.Count-1);

				return c;
			}
			catch(System.Exception e)
            {
				ErrorLogger.stopError("スタックが不足しています。callとreturnの関係を確認して下さい");
				Debug.Log(e.ToString());
				return null;
			}
		}

		//現在のifスタックの状態を確認する
		public bool CurrentIfStack()
        {
			return ifStack[ifStack.Count-1].isIfProcess;	
		}

		//スタックの状態を変更する
		public void ChangeIfStack(bool proccess)
        {
			IfStack s = PopIfStack();
			s.isIfProcess = proccess;
			ifStack.Add(s);
		}

		public int CountIfStack()
        {
			return this.ifStack.Count;
		}

		//スタックをすべて削除します
		public void RemoveAllStacks() {
			//未実装
			qStack.Clear();
			ifStack.Clear();
			ifNum = 0;
		}

//// macro ///////
		public void AddMacro(string macro_name,string file_name, int index)
		{
			dicMacro[macro_name] = new Macro(macro_name,file_name,index);
		}

		/*
		public void addMacroStack(string macro_name,Dictionary<string,string> dicVar){
			this.macroNum++;
			Macro macro = dicMacro [macro_name];
			this.addStack (macro.file_name, macro.index, dicVar);

		}
		*/

		public Macro GetMacro(string macro_name)
        {
			if(!dicMacro.ContainsKey (macro_name))
				ErrorLogger.stopError("マクロ「" + macro_name + "」は見つかりませんでした");

			return dicMacro [macro_name]; 
		}

//		public bool hasComponent = false;
		
		public void RunScenario(string nextStorage = "")
		{            
//			hasComponent = true;

			//doComponentがtrueである限り1フレームで回る
//			while(doComponent())
			{
//				if (NovelSingletonEx.StatusManager.isMessageShowing == true)
//					continue;

				//nextOrder を指定されたなら、クリックは有効になる
//				NovelSingletonEx.StatusManager.enableClickOrder = true;
			}
//			hasComponent = false;
		}

//0=デフォルト1=componentのフラグが立ってない-1シナリオ最後に
		public IEnumerator Run()
		{
			if(currentComponentIndex < arrayComponents.Count)
			{
				AbstractComponent cmp = arrayComponents[currentComponentIndex];

				cmp.Before();

				//タグ
//				if (StatusManager.Instance.currentState == JokerState.SkipOrder)
				{
					Debug.Log("SkipOrderされました");
				}
//				else
				{
					cmp.CalcVariable();
					cmp.Validate();

					string p = "";
					foreach (KeyValuePair<string, string> kvp in cmp.paramDic)
					{
						p += kvp.Key + "=" + kvp.Value + " ";
					}

					if(SystemConfig.Instance.showTag)
					{
						Debug.Log("[" + cmp.tagName + " " + p + " ]");
					}

					yield return cmp.Start();
				}

				//EX変更：Afterも必ず実行される
				cmp.After();

				//ToDo:flag
				currentComponentIndex++;
//				Debug.Log("flag="+cmp.allowNextOrder());
//				cmp.allowNextOrder();

                yield return null;
			}

			//シナリオファイルの最後まで来た時。スタックが存在するならreturn する
			//スタックがあるならreturn する
			if(CountStack() > 0)
			{
                ReturnComponent _ret = new ReturnComponent();
                _ret.Start();
//                TRScriptParser.Instance.StartTag("[return]");
			}
			else
			{
				StatusManager.Instance.EndScenario();
			}
            yield return null;			
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
