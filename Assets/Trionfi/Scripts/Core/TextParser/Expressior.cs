using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace Trionfi
{
	[Serializable]
	public class ExpObject
    {
        ExpObject(ref Variable _v)
        { _var = _v; }

        //参照渡し
        public static Variable _var;

		public string type;
		public string name;
		public string exp;

		public ExpObject(string exp)
        {
			string str_left = "";
			string str_right = "";

            for (var i = 0; i < exp.Length; i++)
            {
				string c = exp [i].ToString();

                if (c == "=")
                {
					str_right = exp.Substring (i+1);
					break;
				}

                str_left += c;
			}

			string[] tmp2 = str_left.Split ('.');

			string variable = tmp2 [0].Trim();
			string variable_name = tmp2 [1].Trim();
			string exp_str = str_right.Trim();

			this.type = variable;
			this.name = variable_name;
			this.exp = exp_str;
		}

		//変数を実際の値に置き換えて返却する
		public static string replaceVariable(string str_right)
        {
			Dictionary<string,string > dicVar = new ParamDictionary();

			bool flag_var_now = false;
			string var_name = "";

			string new_str_right ="";

			Stack<string> var_stack = new Stack<string>(); //２重、３重カッコに対応

			for (var i = 0; i < str_right.Length; i++)
            {
				string c = str_right [i].ToString();

                if (flag_var_now == true && c == "}")
                {
					//変数を格納
					//Debug.Log ("var_name ============");
					//Debug.Log (var_name);
					string var_val = _var.Get(var_name);

					if (var_stack.Count == 0)
                    {
						dicVar [var_name] = var_val;
						flag_var_now = false;
						var_name = "";
						new_str_right += var_val;
					}
					else
                    {
						//var_nameに変数を差し込む
						string stack_var_name = var_stack.Pop();
						var_name = stack_var_name + var_val;
					}
					//タグが確定
					continue;
				}

				if(c != "{" && flag_var_now == true)
					var_name += c;

                if (c == "{" && flag_var_now == true)
                {
					var_stack.Push (var_name);
					var_name = "";
				
				}
				else if (c == "{" && flag_var_now == false)
                {
					flag_var_now = true;
					var_name = "";

				}
				else
                {
					//タグに文字を追加
					if (flag_var_now == false)
						new_str_right += c;
				}
			}

			/*
			foreach (KeyValuePair<string, string> kvp in dicVar) {
				string old_var = kvp.Key;
				string new_var = kvp.Value;

				str_right = str_right.Replace ("{" + old_var + "}", new_var);

			}
			*/
			return new_str_right;
		}

		public static string evaluateString(string exp)
        {
			//変数かどうかを判定する。今のところの定義は「最初の文字がアルファベット＆'.'がある」
			if(Regex.IsMatch(exp[0].ToString(), "^[a-zA-Z_]+$") && exp.IndexOf(".") != -1)
				return _var.Get(exp);

			return exp;
		}

		//式をを計算して結果を返す　評価はまた別
		public static string calc(string exp)
        {
			//ToDo:型チェック

			//比較計算とかの場合は、別途
			//文字列比較の場合
			if(exp.IndexOf ("==") != -1) {

				string[] delimiter = { "==" };

				string[] t = exp.Split (delimiter, StringSplitOptions.None);
				string left = t [0].Trim();
				string right = t [1].Trim();

				left = evaluateString(left);
				right = evaluateString(right);

				if (left == right)
					return "true";
				else
					return "false";
			}
			else if(exp.IndexOf ("!=") != -1) {
				string[] delimiter = { "!=" };

				string[] t = exp.Split (delimiter, StringSplitOptions.None);
				string left = t [0].Trim();
				string right = t [1].Trim();

				left = evaluateString(left);
				right = evaluateString(right);
				
				if (left != right)
					return "true";
				else
					return "false";
			}
			else if(exp.IndexOf (">=") != -1) {
				string[] delimiter = { ">=" };
				string[] t = exp.Split (delimiter, StringSplitOptions.None);
				string left = t [0].Trim();
				string right = t [1].Trim();

				left = evaluateString(left);
				right = evaluateString(right);			

				if (float.Parse (left) >= float.Parse(right))
					return "true";
				else
					return "false";
			}
			else if(exp.IndexOf ("<=") != -1) {
				string[] delimiter = { "<=" };
				string[] t = exp.Split (delimiter, StringSplitOptions.None);
				string left = t [0].Trim();
				string right = t [1].Trim();

				left = evaluateString(left);
				right = evaluateString(right);

				if (float.Parse(left) <= float.Parse(right))
					return "true";
				else
					return "false";
			}
			else if(exp.IndexOf (">") != -1) {
				string[] delimiter = { ">" };
				string[] t = exp.Split (delimiter, StringSplitOptions.None);
				string left = t [0].Trim();
				string right = t [1].Trim();

				left = evaluateString(left);
				right = evaluateString(right);

				if (float.Parse(left) > float.Parse(right))
					return "true";
				else
					return "false";
			}
			 else if (exp.IndexOf ("<") != -1) {
				string[] delimiter = { "<" };
				string[] t = exp.Split (delimiter, StringSplitOptions.None);
				string left = t [0].Trim();
				string right = t [1].Trim();

				left = evaluateString(left);
				right = evaluateString(right);

				if (float.Parse(left) < float.Parse(right))
					return "true";
				else
					return "false";
			}
			else if (exp.IndexOf ("*") != -1) {
				string[] delimiter = { "*" };
				string[] t = exp.Split (delimiter, StringSplitOptions.None);
				string left = t [0].Trim();
				string right = t [1].Trim();

				left = evaluateString(left);
				right = evaluateString(right);

				float k = float.Parse(left) * float.Parse(right);
				return "" + k;

			}
			else if (exp.IndexOf ("/") != -1) {
				string[] delimiter = { "/" };
				string[] t = exp.Split (delimiter, StringSplitOptions.None);
				string left = t [0].Trim();
				string right = t [1].Trim();

				left = evaluateString(left);
				right = evaluateString(right);

				float k = float.Parse(left) / float.Parse(right);
				return "" + k;

			}
			else if(exp.IndexOf ("+") != -1) {
				//数値の先頭が-で始まって、かつもう一つ-があれば、計算。それ以外は代入
				if (exp [0] == '+') {
					if (exp.Substring (1).IndexOf ("+") == -1)
						return exp;
					else
						exp = exp.Substring (1);
				}
				string[] delimiter = { "+" };
				string[] t = exp.Split (delimiter, StringSplitOptions.None);
				string left = t [0].Trim();
				string right = t [1].Trim();

				left = evaluateString(left);
				right = evaluateString(right);

				float k = float.Parse(left) + float.Parse(right);

				return "" + k;

			}
			else if(exp.IndexOf ("-") != -1) {
				//数値の先頭が-で始まって、かつもう一つ-があれば、計算。それ以外は代入
				bool flag_minus = false;
				if (exp [0] == '-') {
					if (exp.Substring (1).IndexOf ("-") == -1)
						return exp;
					else {
						flag_minus = true;
						exp = exp.Substring (1);
					}
				}

				string[] delimiter = { "-" };
				string[] t = exp.Split (delimiter, StringSplitOptions.None);
				string left = t [0].Trim();
				string right = t [1].Trim();

				left = evaluateString(left);
				right = evaluateString(right);

				if (flag_minus == true)
					left = "-" + left;

				float k = float.Parse (left) - float.Parse (right);

				return "" + k;
			}
			else {
				return exp;
			}
		}
	}
}
