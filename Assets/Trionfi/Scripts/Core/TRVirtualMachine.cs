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

    public class TRCallStack : Stack<CallStackObject>
    {
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
        public class FunctionalObject
        {
            public string scriptName;
            public int pos;
        }

        public static UserSaveDataInfo saveDataInfo = new UserSaveDataInfo();

        public static TRVariable variableInstance = new TRVariable();
        public static TRCallStack callStack = new TRCallStack();
        public static Stack<bool> ifStack = new Stack<bool>();

        public static string currentScriptName = "";
        public static TRTagInstance currentTagInstance { get { return tagInstance[currentScriptName]; } }
        public static Dictionary<string, TRTagInstance> tagInstance = new Dictionary<string, TRTagInstance>();
        public static Dictionary<string, FunctionalObject> functionalObject = new Dictionary<string, FunctionalObject>();

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
                tagInstance[storage] = _instance;
            }
        }

        public IEnumerator Run(string storage, int index = 0)
        {
            if (tagInstance.ContainsKey(storage))
            {
                TRTagInstance tag = tagInstance[storage];
                tag.currentComponentIndex = index;
                currentScriptName = storage;
                yield return Run(tag, index);
            }
            else
                ErrorLogger.Log("not find script file:" + storage);
        }

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

        //ToDo:
        public static bool Serialize(string name) { return true; }
        public static bool Deserialize(string name) { return false; }
    }
}
