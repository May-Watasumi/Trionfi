using System;
using System.Collections;
using System.Collections.Generic;
using Jace;
using Jace.Execution;
using Jace.Tokenizer;
using Jace.Operations;
using TRVariable = Jace.Operations.VariableCalcurator;

namespace Trionfi
{
    public enum FunctionalObjectType
    { Script, Function, Macro };

    [Serializable]
    public class FunctionalObjectInstance
    {
        public FunctionalObjectInstance(FunctionalObjectType _type, string _scriptName, int _pos, int _endPos)
        {
            type = _type;
            scriptName = _scriptName;
            startPos = _pos;

            if (_type == FunctionalObjectType.Script)
                _endPos = tagInstance.arrayComponents.Count;

            endPos = _endPos;
        }

        public TRTagInstance tagInstance { get { return Trionfi.Instance.scriptInstance[scriptName].instance; } }

        public FunctionalObjectType type = FunctionalObjectType.Macro;

        public string scriptName;
        public int startPos;
        public int endPos;// { get { return tagInstance.arrayComponents.Count; } }

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

    public enum ResumeTaskType
    { 
        JUMP,
        RELOAD,
        LOAD
    }

    public class ResumeTask
    {
        public ResumeTaskType type;
        public FunctionalObjectInstance instance;
    }

    //ToDo:refactoring
    public class TRVirtualMachine : SingletonMonoBehaviour<TRVirtualMachine>
    {
        public enum State { Sleep, Run, Reboot, Load }

        public State state = State.Sleep;

        public TRTagInstance currentTagInstance { get { return Trionfi.instance.scriptInstance[callStack.Peek().scriptName].instance; } }
        public FunctionalObjectInstance currentCallStack { get { return callStack.Peek(); } }
        //
        public TRVariableDictionary globalVariableInstance = new TRVariableDictionary();
        public Queue<ResumeTask> nextTempFunc = new Queue<ResumeTask>();
        public Stack<FunctionalObjectInstance> callStack = new Stack<FunctionalObjectInstance>();
        public Stack<bool> ifStack = new Stack<bool>();
        public VariableStack vstack = new VariableStack();
 
        //マクロ、関数の情報（タグインスタンスの指定とタグ位置）。マクロと関数の実装的な区別はない。
        //スクリプトコンパイルの副産物なのでSerializeの必要はないはず。
        public Dictionary<string, FunctionalObjectInstance> functionalObjects = new Dictionary<string, FunctionalObjectInstance>();
        //タグのエイリアス（主にKAGとの互換性用途？）
        public  Dictionary<string, AbstractComponent> aliasTagInstance = new Dictionary<string, AbstractComponent>();

         public IEnumerator Run(string storage, Dictionary<string, VariableCalcurator> param = null)
        {
            if (Trionfi.instance.scriptInstance.ContainsKey(storage))
            {
            //                Trionfi.instance.AwakeTrionfi();
                FunctionalObjectInstance _func = new FunctionalObjectInstance(FunctionalObjectType.Script, storage, 0, 0);

BEGINLOOP:
                do
                {
                    var coroutine = Execute(_func, param);
                    yield return StartCoroutine(coroutine);
                   _func = (FunctionalObjectInstance)coroutine.Current;

                } while (callStack.Count > 0);

                ErrorLogger.Log("End of Script");

                if (nextTempFunc.Count != 0)
                {
                    ResumeTask resumeTask = (nextTempFunc.Dequeue());
                    switch (resumeTask.type)
                    {
                        case ResumeTaskType.JUMP:
                            _func = resumeTask.instance;
                            break;
                        case ResumeTaskType.RELOAD:
                            Trionfi.instance.scriptInstance.Remove(resumeTask.instance.scriptName);
                            yield return Trionfi.instance.LoadScript(resumeTask.instance.scriptName);
                            _func = new FunctionalObjectInstance(FunctionalObjectType.Script, resumeTask.instance.scriptName, 0, 0);
                            break;
//                        case ResumeTaskType.LOAD:                   
                    }

                    goto BEGINLOOP;
                }

                if (Trionfi.instance.enableEndCallback)
                    Trionfi.instance.SleepTrionfi();
            }
            else
                ErrorLogger.Log("not find script file:" + storage);
        }

        public IEnumerator Execute(FunctionalObjectInstance _func, Dictionary<string, VariableCalcurator>  _param)
        {
            //if(_func.type != FunctionalObjectType.Macro)
                callStack.Push(_func);

            if (_param != null)
                vstack.Push(_param);

            TRTagInstance _tag  = Trionfi.instance.scriptInstance[_func.scriptName].instance;

            _func.currentPos = _func.startPos;

            state = State.Run;

            do
            {
                AbstractComponent _tagComponent = _tag.arrayComponents[_func.currentPos];

                if (_func.type == FunctionalObjectType.Macro && _tagComponent is MacroendComponent)
                    goto Macro_End;

                yield return _tagComponent.Execute();

                _func.currentPos++;

            } while (_func.currentPos < _tag.arrayComponents.Count);

Macro_End:
            if (_param != null)
                vstack.Pop();

            //if (_func.type != FunctionalObjectType.Macro)
                yield return callStack.Pop();
            //else
            //    yield return null;
        }

        readonly TokenReader tokenReader = new TokenReader(System.Globalization.CultureInfo.InvariantCulture);
        public IFunctionRegistry functionRegistry = new FunctionRegistry(false);
        AstBuilder astBuilder;// = new Jace.AstBuilder(functionRegistry, false);
        readonly Interpreter interpreter = new Interpreter();

        public TRVariable Evaluation(string formula)
        {
            List<Token> tokens = tokenReader.Read(formula);

            Operation operation = astBuilder.Build(tokens);

            TRVariable result = interpreter.Execute(operation, functionRegistry, vstack);

            return result;
        }

        //ToDo:
        public bool Serialize(string name) { return true; }
        public bool Deserialize(string name) { return false; }

		private void Start()
		{
            astBuilder = new Jace.AstBuilder(functionRegistry, false);
            vstack.Push(globalVariableInstance);
        }

		public void Update()
		{
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F5) && state == State.Run)
            {
                currentCallStack.currentPos = TRVirtualMachine.Instance.currentCallStack.endPos + 1;
                ResumeTask task = new ResumeTask();
                task.instance = new FunctionalObjectInstance(FunctionalObjectType.Script, callStack.Peek().scriptName, 0, 0);
                task.type = ResumeTaskType.RELOAD;

                nextTempFunc.Enqueue(task);

                state = State.Reboot;
            }

        }

	}
}
