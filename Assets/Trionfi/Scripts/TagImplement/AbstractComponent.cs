using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public abstract class AbstractComponent
    {
        public TRVariable tagParam;
        protected bool hasSync = false;
        
//        protected const string syncwait = "syncWait";

#if UNITY_EDITOR || TR_DEBUG
        string sourceName = "";
        int lineCount = 0;

        public List<string> essentialParams = new List<string>();

        [Conditional("UNITY_EDITOR"), Conditional("TR_DEBUG"), Conditional("DEVELOPMENT_BUILD")]
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
        public virtual IEnumerator TagSyncFunction() { yield return null; }
        public virtual void TagSyncFinished() {  }

        //タグの実行
        public void Execute()
        {
            TagFunction();

//            if (hasSync)
//                TRVirtualMachine.Instance.tagSyncFunction += TagSyncFunction;
        }

        public virtual void Before() { }
        public virtual void After() { }
    }

    //無名タグ。コンパイル時に生成される。
    public class UnknownComponent : AbstractComponent
    {          
        public UnknownComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "name"
            };
#endif
        }
        protected override void TagFunction()
        {
        }

        public override IEnumerator TagSyncFunction()
        {
            yield return TRVirtualMachine.Instance.Call(tagParam.Identifier("name"), tagParam);
        }
    }

    //アクタータグ。
    public class ActorComponent : AbstractComponent
    {
        public ActorComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "param"
            };
#endif
        }
        protected override void TagFunction()
        {
            string _paramString;
            _paramString = tagParam.Literal("param");

            string[] _params = _paramString.Split(new char[] { ' ', '　' } );

            List<string> paramList = new List<string>();

            paramList.AddRange(_params);

//            string dress = ""
            string suffix = "01";
            string prefix = "";
            int id = -1;

            foreach (string _param in paramList)
            {
                if (TRStageEnviroment.Instance.actorInfoes.ContainsKey(_param))
                    prefix = TRStageEnviroment.Instance.actorInfoes[_param].prefix;
                if (TRStageEnviroment.Instance.actPatterAlias.ContainsKey(_param))
                    suffix = TRStageEnviroment.Instance.actPatterAlias[_param];
                if(TRStageEnviroment.Instance.layerAlias.ContainsKey(_param))
                    id = TRStageEnviroment.Instance.layerAlias[_param];

                string storage = TRStageEnviroment.Instance._LAYER_PATH_ + TRStageEnviroment.Instance._FILE_HEADER_ + prefix + "_" + suffix;

                tagParam["layer"] = new KeyValuePair<string, TRDataType>(id.ToString(), TRDataType.Int);
                tagParam["storage"] = new KeyValuePair<string, TRDataType>(storage, TRDataType.Identifier);
            }
        }

        public override IEnumerator TagSyncFunction()
        {
            ImageComponent _tag = new ImageComponent();
            _tag.tagParam = tagParam;
            yield return _tag.TagSyncFunction();
        }
    }
}

