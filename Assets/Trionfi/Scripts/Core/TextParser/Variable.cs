using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Text;
using System.Globalization;

namespace Trionfi
{
    public class ParamDictionary : Dictionary<string, string>
    {
        public bool IsValid(ref bool res, string key)
        {
            res = false;
            if (ContainsKey(key))
            {
                string v = this[key];

                if (v == "true")
                    res = true;
                else if (v == "false")
                    res = false;
                else
                {
                    ErrorLogger.Log("InvalidParam: key=" + key + " Value=" + v);
                    return false;
                }
            }
            return true;
        }

        public bool IsValid(ref float res, string key)
        {
            res = 0.0f;
            if (ContainsKey(key))
            {
                string v = this[key];

                if (!float.TryParse(v, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out res))
                {
                    ErrorLogger.Log("InvalidParam: key=" + key + " Value=" + v);
                    return false;
                }
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
                return false;
            }
            return true;
        }

        public bool IsValid(ref int res, string key)
        {
            res = 0;
            if (ContainsKey(key))
            {
                string v = this[key];

                if (!System.Int32.TryParse(v, out res))
                {
                    ErrorLogger.Log("InvalidParam: key=" + key + " Value=" + v);
                    return false;
                }
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
                return false;
            }
            return true;
        }

        public bool IsValid(ref string res, string key)
        {
            res = "";
            if (ContainsKey(key))
            {
                res = this[key];
                return true;
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
                return false;
            }
        }

        public float Float(string key, float defaultValue = 0.0f)
        {
            float res = defaultValue;
            if (ContainsKey(key))
            {
                string v = this[key];

                if (float.TryParse(v, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out res))
                {
                    return res;
                }
                else
                {
                    int _resInt = 0;
                    if (System.Int32.TryParse(v, out _resInt))
                        return (float)_resInt;
                    else
                    {
                        ErrorLogger.Log("InvalidParam: key=" + key + " Value=" + v);
                        return defaultValue;
                    }
                }
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
            }
            return defaultValue;
        }

        public int Int(string key, int defaultValue = 0)
        {
            int res = defaultValue;
            if (ContainsKey(key))
            {
                string v = this[key];

                if (!System.Int32.TryParse(v, out res))
                {
                    ErrorLogger.Log("InvalidParam: key=" + key + " Value=" + v);
                    return defaultValue;
                }
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
                return defaultValue;
            }
            return res;
        }

        public string String(string key, string defaultValue = "")
        {
            string res = defaultValue;
            if (ContainsKey(key))
            {
                res = this[key];
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
            }
            return res;
        }

        public bool Bool(string key, bool defaultValue = false)
        {
            bool res = defaultValue;
            if(ContainsKey(key) && this[key] == "true" || this[key] == "TRUE")
            {
                res = true;
            }
            else
            {
                res = false;

                if(!ContainsKey(key) || (this[key] != "false" && this[key] != "FALSE"))
                    ErrorLogger.Log("InvalidParam: key=" + key);
            }
            return res;
        }

        public TRDataType Type(string key = "type", TRDataType defaultValue = TRDataType.None)
        {
            if(ContainsKey(key))
            {
                string _type = this[key];
                return StorageManager.dataTypes.ContainsKey(_type) ? defaultValue : StorageManager.dataTypes[_type];
            }
            return defaultValue;
        }

        /*
        public T ValidParam<T>(string key)
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)ValidInt(key);
            }
            else if (typeof(T) == typeof(float))
            {
                return (T)(object)ValidFloat(key);
            }
            else
            {
                return (T)(object)ValidString(key);
            }
        }
        */
    }

    //変数などバリアブルを保持する
    public class Variable : Dictionary<string, ParamDictionary>
    {
        public void Set(string exp, string val)
		{
			exp = exp.Replace("{", "").Replace("}", "");

			string[] tmp = exp.Split('.');

			string type = tmp[0].Trim();
			string variable_name = tmp[1].Trim();

			if(!ContainsKey(type))
			{
                //this.dicVar = new Dictionary<string,ParamDictionary>();
                this[type] = new ParamDictionary();
			}

			this[type][variable_name] = val;

			//グローバルなら即効反映
			if (type == "global")
			{
//ToDo
/*
                if (Trionfi.Instance.Serializer.globalSetting.globalVar == null)
					Trionfi.Instance.Serializer.globalSetting.globalVar = new Dictionary<string, string>();

				Trionfi.Instance.Serializer.globalSetting.globalVar[variable_name] = val;
                */
    //ToDo:
				//				Trionfi.Instance.Serializer.SaveGlobalObject(Trionfi.Instance..globalSetting);

			}        
		}

		public string Get(string exp)
		{
			exp = exp.Replace("{", "").Replace("}", "");

			string default_val = "null"; //default_val は nullという文字列を入れる
			if (exp.IndexOf("|") != -1)
			{
				string[] tmp_default = exp.Split('|');
				exp = tmp_default[0];
				default_val = tmp_default[1];

			}

			string[] tmp = exp.Split('.');

			string type = tmp[0].Trim();
			string variable_name = tmp[1].Trim();

			if(ContainsKey(type) && this[type].ContainsKey(variable_name))
				return this[type][variable_name];
			else
				return default_val;
		}

		public bool HasKey(string key)
		{
			string val = this.Get(key);

			return val == "null" ? false : true;
		}

		public ParamDictionary GetDictionary(string type)
		{
			if (!ContainsKey(type))
			{
                //this.dicVar = new Dictionary<string,ParamDictionary>();
                return new ParamDictionary();
			}
			else
				return this[type];
		}       
        
		public void Trace(string type)
		{
			string str = "[trace]" + type + "\n";

			foreach (KeyValuePair<string, string> kvp in this[type])
				str += kvp.Key + "=" + this[type][kvp.Key] + "\n";

			str += "-----------------";

			Debug.Log("<color=green>" + str + "</color>");
		}
	}
}
