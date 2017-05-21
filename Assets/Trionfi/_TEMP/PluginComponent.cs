using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if false
namespace NovelEx{
	public class JoinComponent:AbstractComponent
	{
		public JoinComponent ()
		{
			//必須項目
			arrayVitalParam = new List<string> {
				"var","arg1","arg2"
			};

			originalParamDic = new ParamDictionary () {
				{"var",""},
				{"arg1",""},
				{"arg2",""},
			};
		}
		public override void start ()
		{
			string var_name = paramDic ["var"];
			string arg1 = paramDic ["arg1"];
			string arg2 = paramDic ["arg2"];

			string arg_result = arg1 + arg2;

			//変数に結果を格納
			StatusManager.variable.set (var_name, arg_result);

			//次のシナリオに進む処理
			this.gameManager.nextOrder ();
		}
	}
}
#endif
