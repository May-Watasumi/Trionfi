using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Trionfi
{
    [Serializable]
	public class TRTagInstance
    {
        public List<AbstractComponent> tagComponents;
        public Dictionary<string, int> labelInfo = new Dictionary<string, int>();

        //マクロ情報はシステム共通
        [Serializable]
		public class Macro
		{
			public string name;
			public string file_name;
			public int index;

			public Macro() { }

			public Macro(string _name, string _file_name, int _index)
			{
				name = _name;
				file_name = _file_name;
				index = _index;
			}
		}

		//マクロ呼出しを保持するクラス
		[Serializable]
		public class CallStack
		{
			public ParamDictionary dicVar = new ParamDictionary();
			public string scenarioNname;
			public int index;

			public CallStack() { }

			public CallStack(string scenario_name, int _index, ParamDictionary _dicVar)
			{
				scenarioNname = scenario_name;
				index = _index;
				dicVar = _dicVar;
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

        public List<AbstractComponent> arrayComponents = new List<AbstractComponent>();
        public int currentComponentIndex = -1;      //-1は未初期化、0からスタート

        //stackを配列に置き換える
                                                    //public Stack<CallStack> qStack = new Stack<CallStack>();
                                                    //public Stack<IfStack> ifStack = new Stack<IfStack>();

        public List<CallStack> qStack = new List<CallStack>();
		public List<IfStack> ifStack = new List<IfStack>();

        public int ifNum = 0;
		public int macroNum = 0;

        public Dictionary<string, Macro> dicMacro = new Dictionary<string, Macro>();

		public bool CompileScriptString(string text) {
            ErrorLogger.Clear();

            //パーサーを動作させる
            tagComponents = TRScriptParser.Instance.Parse(text);

            int _index = 0;

            foreach (AbstractComponent _component in arrayComponents)
            {
                if (_component.tagName == "label")
                    AddLabel(_component.expressionedParams["name"], _index);

                _index++;
            }

            return ErrorLogger.showAll();
        }

		public bool CompileScriptFile(string storage)
		{
//				string fullpath = /*useStoragePath ? StorageManager.Instance.PATH_SD_SCENARIO :*/ "";
				TextAsset script_text = StorageManager.Instance.LoadObject(storage, TRDataType.TextAsset) as TextAsset;

				return CompileScriptString(script_text.text);
		}

        public void AddLabel(string label_name, int index)
        {
            labelInfo[label_name] = index;
        }

        public int GetLabelPosition(string label_name)
        {
            if (string.IsNullOrEmpty(label_name) || !labelInfo.ContainsKey(label_name))
            {
                ErrorLogger.stopError("にラベル「" + label_name + "」が見つかりません。");
                return -1;
            }

            return labelInfo[label_name];
        }

        public void AddStack(string scenario_name, int index, ParamDictionary dicVar) {
			//stack追加時にdicVarに呼び出し元情報を入れる
			//呼び出し元の情報はcaller_indexに入る。
			dicVar ["caller_index"] = "" + index;
			dicVar ["caller_file"] = scenario_name;
            //ToDo:
//			var mp = variable.getType("mp");

			////
			/*
			Debug.Log ("== add stack ===============");
			foreach (KeyValuePair<string, string> kvp in mp){
				Debug.Log (kvp.Key);
				Debug.Log(kvp.Value);
			}
			*/

			qStack.Add(new CallStack(scenario_name, index, dicVar));
//			qStack.Add(new CallStack(scenario_name, index, mp));
			//スタックを追加した時点で使用できる引数変数を格納する
///			variable["mp"] = dicVar;
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

                //ToDo:コールスタック復帰
				//variable["mp"] = c.dicVar;

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
		public void addMacroStack(string macro_name,ParamDictionary dicVar){
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
        public IEnumerator Run(int index = 0)
		{
                currentComponentIndex = index;
                //			StatusManager.Instance.currentScenario = this;
                //			StatusManager.Instance.MessageShow();

            if (currentComponentIndex < arrayComponents.Count)
			{
				AbstractComponent _tagComponent = arrayComponents[currentComponentIndex];

                _tagComponent.Before();

				//タグ
//				if (StatusManager.Instance.currentState == JokerState.SkipOrder)
				{
					Debug.Log("SkipOrderされました");
				}
//				else
				{
//                    _tagComponent.CalcVariable();
//                    _tagComponent.Validate();

					string p = "";
					foreach (KeyValuePair<string, string> kvp in _tagComponent.expressionedParams)
					{
						p += kvp.Key + "=" + kvp.Value + " ";
					}

					if(TRSystemConfig.Instance.showTag)
					{
						Debug.Log("[" + _tagComponent.tagName + " " + p + " ]");
					}

					_tagComponent.Execute();
				}

                _tagComponent.After();

                yield return _tagComponent.TagAsyncWait();

                //ToDo:flag
                currentComponentIndex++;
			}

			//シナリオファイルの最後まで来た時。スタックが存在するならreturn する
			//スタックがあるならreturn する
			if(CountStack() > 0)
			{
                ReturnComponent _ret = new ReturnComponent();
                _ret.Execute();
//                TRScriptParser.Instance.StartTag("[return]");
			}
			else
			{
				StatusManager.Instance.EndScenario();
			}

            yield return null;			
		}
	}
};
