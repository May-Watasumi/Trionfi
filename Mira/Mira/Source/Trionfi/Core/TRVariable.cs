using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#if !TR_PARSEONLY
 using UnityEngine;
#endif
using Jace.Operations;

namespace Trionfi
{
    [Serializable]
    public class TRVariableDictionary : Dictionary<string, VariableCalcurator>
    {
        public TRVariableDictionary() { }

        protected TRVariableDictionary(SerializationInfo info, StreamingContext context)
        {

        }


        readonly string[] reservedWords = new string[]
        {
            "storage",
            "layer",
            "audio",
            "sound",
            "type",
            "buf"
        };

#if !TR_PARSEONLY
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
#endif

        const float defaultFloat = 0.0f;
        const int defaultInt = 0;
        string defaultString = string.Empty;
        const uint defaultUint = 0;
        const bool defaultBool = false;

        public float this[string t, float u]
        {
            get
            {
                if (ContainsKey(t))
                    return this[t].Float();
                else return u;
            }
        }

        public int this[string t, int u]
        {
            get
            {
                if (ContainsKey(t))
                    return this[t].Int();
                else return u;
            }
        }

        public uint this[string t, uint u]
        {
            get
            {
                if (ContainsKey(t))
                    return this[t].Uint();
                else return u;
            }
        }

        public bool this[string t, bool u]
        {
            get
            {
                if (ContainsKey(t))
                    return this[t].Bool();
                else return u;
            }
        }

        public string this[string t, string u]
        {
            get
            {
                if (ContainsKey(t))
                    return this[t].Literal();
                else return u;
            }
        }


#if false
    public enum TRDataType
    {
        Null,
        Bool,
        Int,
        Float,
        Hex,
        String,
//        Literal,
//        Identifier,
//        Label
    }

    public class TRVariable
    {
        public TRDataType dataType;
        public string paramString;
        bool lastResult;

        public TRVariable(string param, TRDataType type)
        {
            paramString = param;
            dataType = type;
        }

        public TRVariable(bool param) { Set(param); }
        public TRVariable(int param) { Set(param); }
        public TRVariable(float param) { Set(param); }
        public TRVariable(string param) { Set(param); }

        /*
        TRDataType Set(string param)
        {
            return TRDataType.Null;
        }
        */
        public void Set(int param)
        {
            paramString = param.ToString();
            dataType = TRDataType.Int;
        }

        public void Set(float param)
        {
            paramString = param.ToString();
            dataType = TRDataType.Float;
        }

        public void Set(string param)
        {
            paramString = param;
            dataType = TRDataType.String;//.Literal;
        }

        public void Set(bool param)
        {
            paramString = param.ToString();
            dataType = TRDataType.Bool;
        }


        public bool Bool(bool defaultValue = false)
        {
            bool result = false;
            return (lastResult = bool.TryParse(paramString, out result)) ? result : defaultValue;
        }
        public float Float(float defaultValue = 0.0f)
        {
            float result = 0.0f;
            return (lastResult = float.TryParse(paramString, out result)) ? result : defaultValue;
        }

        public int Int(int defaultValue = 0)
        {
            int result = 0;
            return (lastResult = int.TryParse(paramString, out result)) ? result : defaultValue;
        }

        public uint Hex(uint defaultValue = 0)
        {
            uint result = 0;

            return ( lastResult = 
                    paramString[0] == '0' && paramString.Length > 2 && (paramString[1] == 'x' || paramString[1] == 'X') &&
                     uint.TryParse(paramString, System.Globalization.NumberStyles.AllowHexSpecifier, null, out result)
                   ) ? result : defaultValue;
        }

        public string Literal(string defaultValue = null)
        {
            return paramString;
        }

        public static TRVariable operator +(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
                case TRDataType.Bool:
                    return new TRVariable(true);
                case TRDataType.Int:
                    return new TRVariable(src.Int() + dest.Int());
                case TRDataType.Float:
                    return new TRVariable(src.Float() + dest.Float());
                case TRDataType.Hex:
                    return new TRVariable(src.Hex() + dest.Hex());
                case TRDataType.String:
                    return new TRVariable(src.Literal() + dest.Literal());
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
        }

        public static TRVariable operator -(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
 //               case TRDataType.Bool:
 //                   return new TRVariable(true);
                case TRDataType.Int:
                    return new TRVariable(src.Int() - dest.Int());
                case TRDataType.Float:
                    return new TRVariable(src.Float() - dest.Float());
                case TRDataType.Hex:
                    return new TRVariable(src.Hex() - dest.Hex());
                case TRDataType.String:
                    return new TRVariable(string.Empty);
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
        }

        public static TRVariable operator *(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
//                case TRDataType.Bool:
//                    return new TRVariable(true);
                case TRDataType.Int:
                    return new TRVariable(src.Int() * dest.Int());
                case TRDataType.Float:
                    return new TRVariable(src.Float() * dest.Float());
                case TRDataType.Hex:
                    return new TRVariable(src.Hex() * dest.Hex());
//                case TRDataType.Literal:
//                    return new TRVariable(string.Empty);
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
        }

        public static TRVariable operator /(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
//               case TRDataType.Bool:
//                    return new TRVariable(true);
                case TRDataType.Int:
                    return new TRVariable(src.Int() / dest.Int());
                case TRDataType.Float:
                    return new TRVariable(src.Float() / dest.Float());
                case TRDataType.Hex:
                    return new TRVariable(src.Hex() / dest.Hex());
//                case TRDataType.Literal:
//                    return new TRVariable(src.Literal() + dest.Literal());
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
        }

        public static TRVariable operator &(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
                case TRDataType.Bool:
                    return new TRVariable(src.Bool() && dest.Bool());
                case TRDataType.Int:
                    return new TRVariable(src.Int() !=0 && dest.Int()!=1);
                case TRDataType.Float:
                    return new TRVariable(src.Float()!=0.0f && dest.Float()!=0.0f);
                case TRDataType.Hex:
                    return new TRVariable(src.Hex()!=0 && dest.Hex()!=0);
                //                case TRDataType.Literal:
                //                    return new TRVariable(src.Literal() + dest.Literal());
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
        }

        public static TRVariable operator |(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
                case TRDataType.Bool:
                    return new TRVariable(src.Bool() || dest.Bool());
                case TRDataType.Int:
                    return new TRVariable((src.Int() != 0) || (dest.Int() != 1));
                case TRDataType.Float:
                    return new TRVariable((src.Float() != 0.0f) || (dest.Float() != 0.0f));
                case TRDataType.Hex:
                    return new TRVariable((src.Hex() != 0) ||( dest.Hex() != 0));
                //                case TRDataType.Literal:
                //                    return new TRVariable(src.Literal() + dest.Literal());
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
        }

        public static TRVariable operator ==(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
                case TRDataType.Bool:
                    return new TRVariable(src.Bool() == dest.Bool());
                case TRDataType.Int:
                    return new TRVariable((src.Int() != 0) == (dest.Int() != 1));
                case TRDataType.Float:
                    return new TRVariable((src.Float() != 0.0f) == (dest.Float() != 0.0f));
                case TRDataType.Hex:
                    return new TRVariable((src.Hex() != 0) == (dest.Hex() != 0));
                case TRDataType.String:
                    return new TRVariable(src.Literal() == dest.Literal());
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
        }

        public static TRVariable operator !=(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
                case TRDataType.Bool:
                    return new TRVariable(src.Bool() != dest.Bool());
                case TRDataType.Int:
                    return new TRVariable((src.Int() != 0) != (dest.Int() != 1));
                case TRDataType.Float:
                    return new TRVariable((src.Float() != 0.0f) != (dest.Float() != 0.0f));
                case TRDataType.Hex:
                    return new TRVariable((src.Hex() != 0) != (dest.Hex() != 0));
                case TRDataType.String:
                    return new TRVariable(src.Literal() != dest.Literal());
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
        }

        public static TRVariable operator <=(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
                //               case TRDataType.Bool:
                //                    return new TRVariable(true);
                case TRDataType.Int:
                    return new TRVariable(src.Int() <= dest.Int());
                case TRDataType.Float:
                    return new TRVariable(src.Float() <= dest.Float());
                case TRDataType.Hex:
                    return new TRVariable(src.Hex() <= dest.Hex());
                //                case TRDataType.Literal:
                //                    return new TRVariable(src.Literal() + dest.Literal());
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
        }

        public static TRVariable operator >=(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
                //               case TRDataType.Bool:
                //                    return new TRVariable(true);
                case TRDataType.Int:
                    return new TRVariable(src.Int() >= dest.Int());
                case TRDataType.Float:
                    return new TRVariable(src.Float() >= dest.Float());
                case TRDataType.Hex:
                    return new TRVariable(src.Hex() >= dest.Hex());
                //                case TRDataType.Literal:
                //                    return new TRVariable(src.Literal() + dest.Literal());
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
        }

        public static TRVariable operator <(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
                //               case TRDataType.Bool:
                //                    return new TRVariable(true);
                case TRDataType.Int:
                    return new TRVariable(src.Int() < dest.Int());
                case TRDataType.Float:
                    return new TRVariable(src.Float() < dest.Float());
                case TRDataType.Hex:
                    return new TRVariable(src.Hex() < dest.Hex());
                //                case TRDataType.Literal:
                //                    return new TRVariable(src.Literal() + dest.Literal());
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
        }

        public static TRVariable operator >(TRVariable src, TRVariable dest)
        {
            switch (src.dataType)
            {
                //               case TRDataType.Bool:
                //                    return new TRVariable(true);
                case TRDataType.Int:
                    return new TRVariable(src.Int() > dest.Int());
                case TRDataType.Float:
                    return new TRVariable(src.Float() > dest.Float());
                case TRDataType.Hex:
                    return new TRVariable(src.Hex() > dest.Hex());
                //                case TRDataType.Literal:
                //                    return new TRVariable(src.Literal() + dest.Literal());
                default:
                    throw new TRParserExecption(TRParserError.UnmatchType);
            }
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
#endif

#if false
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
            return false;
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

                return true;
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
                return false;
            }
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
                if (this[key].Value != TRDataType.Literal && this[key].Value != TRDataType.Identifier)
                    return false;

                res = this[key].Key;
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

        public void Set(string key, int value)
        {

            this[key] = new KeyValuePair<string, TRDataType>(value.ToString(), TRDataType.Int);
        }

        public void Set(string key, bool value)
        {

            this[key] = new KeyValuePair<string, TRDataType>(value.ToString(), TRDataType.Bool);
        }

        public void Set(string key, float value)
        {

            this[key] = new KeyValuePair<string, TRDataType>(value.ToString(), TRDataType.Float);
        }
#endif
    }
}
