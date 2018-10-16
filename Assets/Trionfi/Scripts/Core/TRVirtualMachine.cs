using System;
using UnityEngine;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Text;
using Jace.Execution;

namespace Trionfi
{
    [Serializable]
    public class UserSaveDataInfo
    {
        public string currentFile = "";
        public int currentIndex = -1;

        public string name = "";
        public string title = "";
        public string description = "";
        public string date = "";
        public string currentMessage = "";

        //画面のキャプチャ情報
        public string screenCaptureFile = "";

        //ToDo:レイヤ、音等の状態保存
    }
    /*
        public enum TRSTACKTYPES
        {
            MACRO,
            FUNCTION,
            IF
        }

        //コールスタック。関数とマクロ共用。（返値を保存する以外の実装に違いはない）。
        [Serializable]
        public class CallStackObject
        {
            public TRVariable tempParam = new TRVariable();//仮引数
            public string scenarioNname;
            public int index;

            public CallStackObject(string scenario_name, int _index, TRVariable _param)
            {
                scenarioNname = scenario_name;
                index = _index;
                tempParam = _param;
            }
        }
    */
    //if文の入れ子などを管理するスタック
    [Serializable]
    public class IfStack
    {
        public bool isIfProcess = false;

        public IfStack() { }

        public IfStack(bool val)
        {
            isIfProcess = val;
        }
    }

    public class TRVirtualMachine : SingletonMonoBehaviour<TRVirtualMachine>
    {
        public enum FunctionalObjectType
        { Script, Function, Macro };

        [Serializable]
        public class FunctionalObjectInstance
        {
            public FunctionalObjectInstance(FunctionalObjectType _type, string _scriptName, int _pos)
            {
                type = _type;
                scriptName = _scriptName;
                startPos = _pos;
            }

            public TRTagInstance tagInstance { get { return tagInstances[scriptName]; } }

            public TRVariable tempParam = new TRVariable();//仮引数
            public FunctionalObjectType type = FunctionalObjectType.Macro;
            public string scriptName;
            public int startPos;
            public int currentPos;

            public bool LocalJump(string label)
            {
                TRTagList arrayComponents = tagInstance.arrayComponents;

                if (!arrayComponents.labelPos.ContainsKey(label))
                    return false;

                currentPos = arrayComponents.labelPos[label];
                return true;
            }

            public void SkipTo<T>()
            {
                TRTagList arrayComponents = tagInstance.arrayComponents;

                while (arrayComponents[currentPos].GetType() != typeof(T))
                    currentPos++;
            }

            public void SkipTo<T, Y>()
            {
                TRTagList arrayComponents = tagInstance.arrayComponents;

                while (arrayComponents[currentPos].GetType() != typeof(T) && arrayComponents[currentPos].GetType() != typeof(Y)) ;
                currentPos++;
            }

        }

        public class TRCallStack : Stack<FunctionalObjectInstance>
        {
            /*
                    public new void Push(CallStackObject _object)
                    {
                        //variable["mp"] = dicVar;
                        base.Push(_object);
                    }

                    public new CallStackObject Pop()
                    {
                        //variable["mp"] = c.dicVar;
                        return base.Pop();
                    }
                }
            */
        }

        public static TRTagInstance currentTagInstance { get { return tagInstances[callStack.Peek().scriptName]; } }
        public static FunctionalObjectInstance currentCallStack { get { return callStack.Peek(); } }

        public static UserSaveDataInfo saveDataInfo = new UserSaveDataInfo();
        public static TRVariable variableInstance = new TRVariable();
        public static TRCallStack callStack = new TRCallStack();
        public static Stack<bool> ifStack = new Stack<bool>();

        //スクリプトをコンパイルしたタグの集合体
        public static Dictionary<string, TRTagInstance> tagInstances = new Dictionary<string, TRTagInstance>();
        //マクロ、関数の情報（タグインスタンスの指定とタグ位置）。マクロと関数の実装的な区別はない。
        public static Dictionary<string, FunctionalObjectInstance> functionalObjects = new Dictionary<string, FunctionalObjectInstance>();
        //タグのエイリアス（主にKAGとの互換性用途？）
        public static Dictionary<string, AbstractComponent> aliasTagInstance = new Dictionary<string, AbstractComponent>();

        //スタックをすべて削除します
        public static void RemoveAllStacks()
        {
            callStack.Clear();
            ifStack.Clear();
        }

        public static double Calc(TRVariable _variable, string calcString)
        {
            Dictionary<string, double> calcValue = new Dictionary<string, double>();
            Jace.CalculationEngine engine = new Jace.CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            double result = engine.Calculate(calcString, calcValue);

            /*
                                    foreach(KeyValuePair<string, KeyValuePair<string, TRDataType>> _pair in _variable)
                                    {
                                        float _value = 0.0f;

                                        if (_variable.IsValid(ref _value, _pair.Key))
                                            calcValue[_pair.Key] = _value;
                                        else
                                            calcValue[_pair.Key] = float.PositiveInfinity;
                                    }

                                    double result = engine.Calculate(calcString, calcValue);
                                    return result;
                        */
            return result;
        }

        //ToDo:boolの評価。Jaceの拡張
        public void CompileScriptFile(string storage)
        {
            StartCoroutine(LoadScenarioAsset(storage));
        }

        public IEnumerator LoadScenarioAsset(string storage)
        {
            TRResourceLoader.Instance.Load(storage, TRResourceType.Text);

            while (TRResourceLoader.Instance.isLoading)
                yield return new WaitForSeconds(1.0f);

            if (TRResourceLoader.Instance.isSuceeded)
            {
                TRTagInstance _instance = new TRTagInstance();
                _instance.CompileScriptString(TRResourceLoader.Instance.text);
                tagInstances[storage] = _instance;
            }
        }

        public IEnumerator Run(string storage, int index = 0, TRVariable param = null)
        {
            if (tagInstances.ContainsKey(storage))
            {
                TRTagInstance tag = tagInstances[storage];
                FunctionalObjectInstance _func = new FunctionalObjectInstance(FunctionalObjectType.Script, storage, index);
//                callStack.Push(_func);

                yield return Execute(_func, param);// tag, index);
            }
            else
                ErrorLogger.Log("not find script file:" + storage);
        }

        public IEnumerator Call(string name, TRVariable param)
        {
            if (functionalObjects.ContainsKey(name))
            {
                FunctionalObjectInstance _func = functionalObjects[name];
//                callStack.Push(_func);

                yield return Execute(_func, param);// tag, index);
            }
            else
                ErrorLogger.Log("not find Function:" + name);
        }

        public IEnumerator Execute(FunctionalObjectInstance _func, TRVariable _param)
        {
            TRTagInstance _tag = null;
            _func.currentPos = _func.startPos;
            _func.tempParam = _param;

            callStack.Push(_func);

            do
            {
//                _func = callStack.Peek();
                _tag = tagInstances[_func.scriptName];

                AbstractComponent _tagComponent = _tag.arrayComponents[_func.currentPos];

                _tagComponent.Before();

#if UNITY_EDITOR || TR_DEBUG
                if (TRSystemConfig.Instance.showTag)
                {
                    string _params = "";

                    foreach (KeyValuePair<string, KeyValuePair<string, TRDataType>> key in _tagComponent.tagParam)
                    {
                        _params += " " + key.Key + "= " + key.Value.Key;
                    }
                    ErrorLogger.Log("[" + _tagComponent.tagName + _params + " ]");
                }
#endif
                _tagComponent.Execute();

                _tagComponent.After();

                yield return _tagComponent.TagSyncFunction();

                _func.currentPos++;

            } while (_func.currentPos < _tag.arrayComponents.Count);

            callStack.Pop();

            yield return null;
        }
    /*
        public IEnumerator Run(TRTagInstance tag, int index = 0)
        {
            tag.currentComponentIndex = index;

            while (tag.currentComponentIndex < tag.arrayComponents.Count)
            {
                AbstractComponent _tagComponent = tag.arrayComponents[tag.currentComponentIndex];

                _tagComponent.Before();

#if UNITY_EDITOR || TR_DEBUG
                if (TRSystemConfig.Instance.showTag)
                {
                    string _params = "";

                    foreach (KeyValuePair<string, KeyValuePair<string, TRDataType>> key in _tagComponent.tagParam)
                    {
                        _params += " " + key.Key + "= " + key.Value.Key;
                    }
                    ErrorLogger.Log("[" + _tagComponent.tagName + _params + " ]");
                }
#endif
                _tagComponent.Execute();

                _tagComponent.After();

                yield return _tagComponent.TagSyncFunction();

                //ToDo:flag
                tag.currentComponentIndex++;
            }

            yield return null;
        }
*/
        //ToDo:
        public static bool Serialize(string name) { return true; }
        public static bool Deserialize(string name) { return false; }
    }
}
