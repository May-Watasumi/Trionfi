using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

namespace Trionfi
{
    public class TRVariable : Dictionary<string, string>
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

        /*
        public static string evaluateString(string exp)
        {
            //変数かどうかを判定する。今のところの定義は「最初の文字がアルファベット＆'.'がある」
            if (Regex.IsMatch(exp[0].ToString(), "^[a-zA-Z_]+$") && exp.IndexOf(".") != -1)
                return _var.Get(exp);

            return exp;
        }
        */
    }
}
