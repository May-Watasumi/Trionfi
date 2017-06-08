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

namespace NovelEx
{
    public class TagParam : ParamDictionary
    {
        public string tagName = "";
        public  string statement = "";
        //ForDebug
        public int lineCount;
        public string sourceFile = "";
        
        //タグのValidはここの前で保証しよう
        public TagParam(string str, int line, string source = null)
        {
            statement = str;
            lineCount = line;
            sourceFile = source;

            str = str.Replace("[", "").Replace("]", "");

            //storage = 3 name=4 param=5 みたいな文字列が渡ってくる
            bool flag_finish_tag = false;

            string _tag = "";

            int end_tag_index = 0;

            for (int i = 0; i < str.Length; i++)
            {
                string c = str[i].ToString();
                if(c == " ")
                {
                    //flag_start_tag = false;
                    tagName = _tag;
                    end_tag_index = i;

                    break;
                }
                else
                {
                    _tag += c;
                    continue;
                }
            }

            if(tagName == "")
            {
                tagName = _tag;
                flag_finish_tag = true;
            }

            if(!flag_finish_tag)
            {
                //ここまでで、タグ解析完了
                string param_str = str.Substring(end_tag_index).Trim();

                bool flag_eq = false;
                bool flag_qt = false;
                bool flag_eq_ch = false; //イコール後に文字列が来たかどうか

                string key = "";
                string val = "";

                for (int i = 0; i < param_str.Length; i++)
                {
                    string c = param_str[i].ToString();

                    //イコール前の空白は無視
                    if (c == " " && flag_eq == false)
                        continue;

                    if (c == "=" && flag_eq == false)
                    {
                        flag_eq = true;
                        continue;
                    }

                    if (flag_eq == false)
                    {
                        key += c;
                        continue;
                    }

                    //イコール以後の解析部分
                    if (flag_eq == true)
                    {
                        if (flag_eq_ch == false && c == " ")
                            continue;

                        if (c == "\"" || c == "'")
                        {
                            if (flag_qt == false)
                            {
                                flag_qt = true;
                                flag_eq_ch = true;
                                continue;
                            }
                            else
                            {
                                //パラメータ設定の終わり
                                flag_eq = false;
                                flag_qt = false;
                                flag_eq_ch = false;

                                //値を登録
                                this[key] = val;
                                key = "";
                                val = "";
                            }
                        }
                        else
                        {
                            if (c == " " && flag_qt == false)
                            {
                                flag_eq = false;
                                flag_qt = false;
                                flag_eq_ch = false;

                                this[key] = val;
                                key = "";
                                val = "";

                            }
                            else
                            {
                                val += c;
                                flag_eq_ch = true;
                            }

                            if (i == param_str.Length - 1)
                            {
                                //最後の文字の場合
                                this[key] = val;
                            }
                        }
                    }
                }
            }
        }

        //実行前にパラメータを解析して変数を格納する
        public ParamDictionary Expression()
        {
            ParamDictionary tempParam = new ParamDictionary();

            //タグに入れる
            foreach (KeyValuePair<string, string> pair in this)
            {
                tempParam[pair.Key] = ExpObject.replaceVariable(pair.Value);
            }

            //タグにデフォルト値を設定中かつ、tag が指定されていない場合
            if (StatusManager.Instance.TagDefaultVal != "")
            {
                if (tempParam.ContainsKey("tag") && tempParam["tag"] == "")
                    tempParam["tag"] = StatusManager.Instance.TagDefaultVal;
            }

            return tempParam;
        }

        public void DebugOutputParams()
        {
            foreach(string t in this.Keys)
            {
                ErrorLogger.Log(t +"="+ this[t]);
            }
        }
    }
}
