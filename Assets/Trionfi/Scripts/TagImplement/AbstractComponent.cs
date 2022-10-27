﻿using System;
using System.Diagnostics;
using System.Collections.Generic;


#if !TR_PARSEONLY
using UnityEngine;
using Cysharp.Threading.Tasks;

using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;

#else
using TRTask = System.Threading.Tasks.Task;
using TRTaskString = System.Threading.Tasks.Task<string>;
#endif

using TRVariable = Jace.Operations.VariableCalcurator;
using TRDataType = Jace.DataType;

namespace Trionfi
{
	[Serializable]
    public abstract class AbstractComponent
    {
        const string storageArgumentString = "storage";
        const string layerArgumentString = "layer";
        const string audioArgumentString = "audio";
        const string aoundArgumentString = "sound";
        const string resourceTypeArgumentString = "type";
        const string bufferArgumentString = "buf";

        public AbstractComponent()
        {
           tagName = GetType().Name.Replace("Component", "");
        }

#if !TR_PARSEONLY
        [SerializeField]
#endif
        public TRVariableDictionary tagParam;

#if !TR_PARSEONLY
        [SerializeField]
#endif
        public int lineCount;

#if !TR_PARSEONLY
        [SerializeField]
#endif
        protected bool hasSync = false;

        //タグ名を取得（デバッグ用？）
#if !TR_PARSEONLY
        [SerializeField]
#endif
        public string tagName;


        string sourceName = "";

        public List<string> essentialParams = new List<string>();
        public List<string> essentialMoreOneParams = new List<string>();

#if UNITY_EDITOR || TR_DEBUG
        [Conditional("UNITY_EDITOR"), Conditional("TR_DEBUG"), Conditional("DEVELOPMENT_BUILD")]
#endif
        public void Log()
        {
#if !TR_PARSEONLY
            if (TRSystemConfig.Instance.showTag)
#endif
            {
                string _params = "";

                foreach (KeyValuePair<string, TRVariable> key in tagParam)
                {
                    _params += " " + key.Key + "=" + key.Value.paramString;
                }
                ErrorLogger.Log("[" + tagName + _params + " ]");
            }
        }

        [Conditional("UNITY_EDITOR"), Conditional("TR_DEBUG"), Conditional("DEVELOPMENT_BUILD")]
        public void Validate(bool stopOnError = false)
        {
            string message = string.Empty;

            //タグから必須項目が漏れていないか、デフォルト値が入ってない場合はエラーとして警告を返す
            foreach (string param in essentialParams)
            {
                if (!tagParam.ContainsKey(param))
                {
                    //エラーを追加
                    message = "必須パラメータ「" + param + "」が不足しています";
                    ErrorLogger.AddLog(message, sourceName, lineCount, false);
                }
            }

            string tagParams = string.Empty;

            foreach (string param in essentialMoreOneParams)
            {
                tagParams += param + ", ";
                
                if (tagParam.ContainsKey(param))
                {
                    return;
                }
            }

            if (!string.IsNullOrEmpty(tagParams))
            {
                //エラーを追加
                message = "パラメータ「" + tagParams + "」のいずれかが必要です";
                ErrorLogger.AddLog(message, sourceName, lineCount, false);
            }
        }

#if !TR_PARSEONLY
        protected TRResourceType GetResourceType()
        {
            if (!tagParam.ContainsKey(resourceTypeArgumentString))
                return TRResourceLoader.defaultResourceType;
            else if(tagParam[resourceTypeArgumentString].DataType == TRDataType.Integer || tagParam[resourceTypeArgumentString].DataType == TRDataType.UnsighnedInteger)
                return (TRResourceType)(tagParam[resourceTypeArgumentString].Int());
            else if (tagParam[resourceTypeArgumentString].DataType == TRDataType.Literal)
            {
                switch (tagParam[resourceTypeArgumentString].Literal())
                {
                    default:
                    case "local":
                        return TRResourceType.LocalStatic;
                    case "stream":
                        return TRResourceType.LocalStreaming;
                    case "www":
                        return TRResourceType.WWW;
                    case "bundle":
                        return TRResourceType.AssetBundle;
                }
            }

            //例外飛ばしたほうがいいかも？
            else
                return TRResourceLoader.defaultResourceType;
        }
#endif
        protected virtual async TRTaskString TagFunction() { return string.Empty; }

        /*
        //タグ実行本体
        abstract protected void TagFunction();
        public virtual IEnumerator TagSyncFunction() { yield return null; }
        public virtual void TagSyncFinished() {  }
        public virtual void Before() { }
        public virtual void After() { }
        */

        public async TRTask Execute()
        {
            Validate();

            Log();

            string mes = await TagFunction();

            if (!string.IsNullOrEmpty(mes))
                ErrorLogger.Log(mes);
        }
    }

    //無名タグ。コンパイル時に生成される。
    [Serializable]
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

#if !TR_PARSEONLY
        protected override async TRTaskString TagFunction()
        {
            if (tagParam.ContainsKey("name"))
            {
                string macroName = tagParam["name"].Literal();

                if (TRVirtualMachine.Instance.functionalObjects.ContainsKey(macroName))
                {
                    FunctionalObjectInstance func = TRVirtualMachine.Instance.functionalObjects[macroName];

                    if (func.type != FunctionalObjectType.Macro)
                        return("\"" + macroName + "\"はマクロではありません");
                    else
                        await TRVirtualMachine.Instance.Execute(func, tagParam);
                }
                else
                    return( "マクロ\"" + macroName + "\"は存在しません");
            }
 
            return ("必須パラメータnameが存在しません");
        }
#endif
    }

    //アクタータグ。
    [Serializable]
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            string _paramString;
            _paramString = tagParam["param"].Literal();

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
                {
                    prefix = TRStageEnviroment.Instance.actorInfoes[_param].prefix;
                    tagParam["actor"] = new TRVariable(_param);
                }

                if (TRStageEnviroment.Instance.actPatternAlias.ContainsKey(_param))
                    suffix = TRStageEnviroment.Instance.actPatternAlias[_param];
                if(TRStageEnviroment.Instance.layerAlias.ContainsKey(_param))
                    id = TRStageEnviroment.Instance.layerAlias[_param];

                string storage = TRStageEnviroment.Instance._CHARACTER_PATH_ + TRStageEnviroment.Instance._CHARACTER_PREFIX_ + prefix + "_" + suffix;

                tagParam["layer"] = new TRVariable(id);
                tagParam["storage"] = new TRVariable(storage);
            }

            ImageComponent _tag = new ImageComponent();
            _tag.tagParam = tagParam;
            await _tag.Execute();

            return string.Empty;
#endif
    }

}
}

