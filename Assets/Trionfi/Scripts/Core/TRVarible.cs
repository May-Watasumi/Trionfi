using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

namespace Trionfi
{
    public enum TRDataType
    {
        Null,
        Bool,
        Int,
        Float,
        Hex,
        Literal,
        Identifier,
        Label
    }
    
    public class TRVariable : SerializableDictionary<string, KeyValuePair<string, TRDataType>>
    {
        public static Color ToARGB(uint val)
        {
            var inv = 1f / 255f;
            var c = Color.black;
            c.a = inv * ((val >> 24) & 0xFF);
            c.r = inv * ((val >> 16) & 0xFF);
            c.g = inv * ((val >> 8) & 0xFF);
            c.b = inv * (val & 0xFF);
            c.a = 1f;
            return c;
        }

        public static Color ToRGB(uint val)
        {
            var inv = 1f / 255f;
            var c = Color.black;
            c.a = 1.0f;
            c.r = inv * ((val >> 16) & 0xFF);
            c.g = inv * ((val >> 8) & 0xFF);
            c.b = inv * (val & 0xFF);
            c.a = 1f;
            return c;
        }

        public bool IsValid(ref bool res, string key)
        {
            res = false;
            if (ContainsKey(key))
            {
                if (this[key].Value != TRDataType.Bool)
                    return false;

                string v = this[key].Key;

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
                if (this[key].Value != TRDataType.Float )
                    return false;

                string v = this[key].Key;

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
                if (this[key].Value != TRDataType.Int)
                    return false;

                string v = this[key].Key;

                if (this[key].Value == TRDataType.Int)
                {
                    if (System.Int32.TryParse(v, out res))
                        return true;

                    ErrorLogger.Log("InvalidParam: key=" + key + " Value=" + v);
                    return false;
                }
                else
                {
                    string _temp = this[key].Key;
                    _temp.Remove(0, 2);                 
                    return int.TryParse(_temp, System.Globalization.NumberStyles.AllowHexSpecifier, null, out res);
                }
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
                return false;
            }

        }

        public bool IsValid(ref uint res, string key)
        {
            res = 0;
            if (ContainsKey(key))
            {
                if (this[key].Value != TRDataType.Int || this[key].Value != TRDataType.Hex)
                    return false;

                string v = this[key].Key;

                if (this[key].Value == TRDataType.Int)
                {
                    if(System.UInt32.TryParse(v, out res))
                        return true;

                    ErrorLogger.Log("InvalidParam: key=" + key + " Value=" + v);
                    return false;
                }
                else
                {
                    string _temp = this[key].Key;
                    _temp.Remove(0, 2);
                    return uint.TryParse(_temp, System.Globalization.NumberStyles.AllowHexSpecifier, null, out res);
                }
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
                return false;
            }

        }

        public bool IsValid(ref string res, string key)
        {
            res = "";
            if (ContainsKey(key))
            {
                if (this[key].Value != TRDataType.Literal )
                    return false;

                string v = this[key].Key;
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
            float _value = defaultValue;
            return IsValid(ref _value, key) ? _value : defaultValue;
        }

        public int Int(string key, int defaultValue = 0)
        {
            int _value = defaultValue;
            return IsValid(ref _value, key) ? _value : defaultValue;
        }

        public uint Uint(string key, uint defaultValue = 0)
        {
            uint _value = defaultValue;
            return IsValid(ref _value, key) ? _value : defaultValue;
        }

        public string Literal(string key, string defaultValue = "")
        {
            string _value = defaultValue;
            return IsValid(ref _value, key) ? _value : defaultValue;
        }

        public bool Bool(string key, bool defaultValue = false)
        {
            bool _value = defaultValue;
            return IsValid(ref _value, key) ? _value : defaultValue;
        }

        public string Identifier(string key, string defaultValue = "")
        {
            if (!this.ContainsKey(key))
                return null;
            if (this[key].Value != TRDataType.Identifier)
                ErrorLogger.Log("Invalid Identifier");
            return this[key].Key;
        }

        public string Label(string key)
        {
            if (!this.ContainsKey(key))
                return null;
            if (this[key].Value != TRDataType.Label)
                ErrorLogger.Log("Invalid Label");
            return this[key].Key;
        }
 
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
