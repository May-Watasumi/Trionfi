using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx{

	public class JoinComponent:AbstractComponent
	{
		public JoinComponent ()
		{

			//必須項目
			this.arrayVitalParam = new List<string> {
				"var","arg1","arg2"
			};

			this.originalParam = new Dictionary<string,string> () {
				{"var",""},
				{"arg1",""},
				{"arg2",""},
			};

		}


		public override void start ()
		{

			string var_name = this.param ["var"];
			string arg1 = this.param ["arg1"];
			string arg2 = this.param ["arg2"];

			string arg_result = arg1 + arg2;

			//変数に結果を格納
			StatusManager.variable.set (var_name, arg_result);

			//次のシナリオに進む処理
			this.gameManager.nextOrder ();

		}

	}

}