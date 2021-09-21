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
using Jace.Operations;
using TRVariable = Jace.Operations.VariableCalcurator;
using TRDataType = Jace.DataType;

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


    //ToDo:refactoring
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
            
            public Dictionary<string, VariableCalcurator> tempParam = new Dictionary<string, VariableCalcurator>();//仮引数
            public FunctionalObjectType type = FunctionalObjectType.Macro;

            public string scriptName;
            public int startPos;
            public int currentPos;
            public int endPos { get { return tagInstance.arrayComponents.Count; } }


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

                while (arrayComponents[currentPos].GetType() != typeof(T) && arrayComponents[currentPos].GetType() != typeof(Y))
                    currentPos++;
            }

            public void SkipTo<X, Y, Z>()
            {
                TRTagList arrayComponents = tagInstance.arrayComponents;

                while (arrayComponents[currentPos].GetType() != typeof(X) && arrayComponents[currentPos].GetType() != typeof(Y) && arrayComponents[currentPos].GetType() != typeof(Z))
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
        public static TRVariableDictionary variableInstance = new TRVariableDictionary();
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

        public static VariableCalcurator Calc(string formula, TRVariableDictionary _variable = null)
        {
            Jace.Tokenizer.TokenReader reader = new Jace.Tokenizer.TokenReader(System.Globalization.CultureInfo.InvariantCulture);
            List<Jace.Tokenizer.Token> tokens = reader.Read(formula);

            Jace.Execution.IFunctionRegistry functionRegistry = new Jace.Execution.FunctionRegistry(false);

            Jace.AstBuilder astBuilder = new Jace.AstBuilder(functionRegistry, false);
            Operation operation = astBuilder.Build(tokens);
            Jace.Execution.Interpreter executor = new Jace.Execution.Interpreter();

            VariableCalcurator result = executor.Execute(operation, null, variableInstance);

            return result;
        }

        public IEnumerator LoadScenarioAsset(string storage, TRResourceType type = TRResourceLoader.defaultResourceType, bool execute = false)
        {
            var _coroutine = TRResourceLoader.Instance.LoadText(storage);
            yield return StartCoroutine(_coroutine);

            if (!string.IsNullOrEmpty((string)_coroutine.Current))
            {
                TRTagInstance _instance = new TRTagInstance();
                _instance.CompileScriptString((string)_coroutine.Current);

                tagInstances[storage] = _instance;

                if (execute)
                    StartCoroutine(Run(storage));
            }
        }

        public IEnumerator Run(string storage, Dictionary<string, VariableCalcurator> param = null)
        {
            if (tagInstances.ContainsKey(storage))
            {
                TRTagInstance tag = tagInstances[storage];
                FunctionalObjectInstance _func = new FunctionalObjectInstance(FunctionalObjectType.Script, storage, 0);
                callStack.Push(_func);

                do
                {
                    yield return Execute(_func, param);

                    _func = callStack.Pop();

                } while (callStack.Count > 0);
            }
            else
                ErrorLogger.Log("not find script file:" + storage);
        }
  
 
        public IEnumerator Call(FunctionalObjectInstance func, Dictionary<string, VariableCalcurator> param)
        {
            callStack.Push(func);
            yield return Execute(func, param);// tag, index);
        }

        protected IEnumerator Execute(FunctionalObjectInstance _func, Dictionary<string, VariableCalcurator>  _param)
        {
            TRTagInstance _tag = null;

            _func.currentPos = _func.startPos;
            _func.tempParam = _param;

            _tag = tagInstances[_func.scriptName];

            do
            {
                AbstractComponent _tagComponent = _tag.arrayComponents[_func.currentPos];

                _tagComponent.Before();

#if UNITY_EDITOR || TR_DEBUG
                if (TRSystemConfig.Instance.showTag)
                {
                    string _params = "";

                    foreach (KeyValuePair<string, TRVariable> key in _tagComponent.tagParam)
                    {
                        _params += " " + key.Key + "= " + key.Value.paramString;
                    }
                    ErrorLogger.Log("[" + _tagComponent.tagName + _params + " ]");
                }
#endif
                _tagComponent.Execute();

                _tagComponent.After();

                yield return _tagComponent.TagSyncFunction();

                _func.currentPos++;

            } while (_func.currentPos < _tag.arrayComponents.Count);

    		yield return null;
        }
   
        //ToDo:
        public static bool Serialize(string name) { return true; }
        public static bool Deserialize(string name) { return false; }
    }
}
