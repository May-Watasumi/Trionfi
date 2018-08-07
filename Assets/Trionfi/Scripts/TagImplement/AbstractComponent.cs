using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public delegate IEnumerator TRTagSyncFunction();

    public abstract class AbstractComponent
    {
        public TRVariable tagParam;
        protected bool hasSync = false;
        
//        protected const string syncwait = "syncWait";

#if UNITY_EDITOR || DEVELOPMENT_BUILD || TRIONFI_DEBUG
        string sourceName = "";
        int lineCount = 0;

        public List<string> essentialParams = new List<string>();

        [Conditional("UNITY_EDITOR"), Conditional("TRIONFI_DEBUG"), Conditional("DEVELOPMENT_BUILD")]
        public void Validate(bool stopOnError = false)
        {
            //タグから必須項目が漏れていないか、デフォルト値が入ってない場合はエラーとして警告を返す
            foreach (string param in essentialParams)
            {
                if (!tagParam.ContainsKey(param))
                {
                    //エラーを追加
                    string message = "必須パラメータ「" + param + "」が不足しています";
                    ErrorLogger.AddLog(message, sourceName, lineCount, false);
                }
            }
        }

        //タグ名を取得（デバッグ用？）
        public string tagName
        {
            get
            {
                string _tag = GetType().Name.Replace("Component", "");
                return _tag;
            }
        }
#endif

        //タグ実行本体
        abstract protected void TagFunction();
        protected virtual IEnumerator TagSyncFunction() { yield return null; }
        protected virtual void TagSyncFinished() { }

        //タグの実行
        public void Execute()
        {
            TagFunction();

            if (hasSync)
                TRVirtualMachine.Instance.tagSyncFunction += TagSyncFunction;
        }

        public virtual void Before() { }
        public virtual void After() { }
    }

    //無名タグ。コンパイル時に生成される。
    public class UnknownComponent : AbstractComponent
    {
        public UnknownComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "name"
            };
        }

        protected override void TagFunction()
        {
            //ToDo:関数呼び出し
        }
    }
}

