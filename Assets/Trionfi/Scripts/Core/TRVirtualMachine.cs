﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Jace;
using Jace.Execution;
using Jace.Tokenizer;
using Jace.Operations;

#if !TR_PARSEONLY
using Cysharp.Threading.Tasks;
using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskFunc = Cysharp.Threading.Tasks.UniTask<Trionfi.FunctionalObjectInstance>;

using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;

#else
using TRTask = System.Threading.Tasks.Task;
using TRTaskString = System.Threading.Tasks.Task<string>;
#endif

using TRVariable = Jace.Operations.VariableCalcurator;

namespace Trionfi
{
    public enum FunctionalObjectType
    { Script, Function, Macro };

    [Serializable]
    public class FunctionalObjectInstance
    {
        public FunctionalObjectInstance(){;}

        public FunctionalObjectInstance(FunctionalObjectType _type, string _scriptName, int _pos, int _endPos)
        {
            type = _type;
            scriptName = _scriptName;
            startPos = _pos;

            if (_type == FunctionalObjectType.Script)
                _endPos = tagInstance().arrayComponents.Count;

            endPos = _endPos;
        }

        public TRTagInstance tagInstance() { return Trionfi.Instance.scriptInstance[scriptName].instance; }

        public FunctionalObjectType type = FunctionalObjectType.Macro;

        public string scriptName;

        public int startPos;
        public int endPos;
        public int currentPos;

        public bool LocalJump(string label)
        {
            TRTagList arrayComponents = tagInstance().arrayComponents;

            if (!arrayComponents.labelPos.ContainsKey(label))
                return false;

            currentPos = arrayComponents.labelPos[label];
            return true;
        }

        public void SkipTo<T>()
        {
            TRTagList arrayComponents = tagInstance().arrayComponents;

            while (arrayComponents[currentPos].GetType() != typeof(T))
                currentPos++;
        }

        public void SkipTo<T, Y>()
        {
            TRTagList arrayComponents = tagInstance().arrayComponents;

            while (arrayComponents[currentPos].GetType() != typeof(T) && arrayComponents[currentPos].GetType() != typeof(Y))
                currentPos++;
        }

        public void SkipTo<X, Y, Z>()
        {
            TRTagList arrayComponents = tagInstance().arrayComponents;

            while (arrayComponents[currentPos].GetType() != typeof(X) && arrayComponents[currentPos].GetType() != typeof(Y) && arrayComponents[currentPos].GetType() != typeof(Z))
                currentPos++;
        }
    }

    public enum ResumeTaskType
    { 
        JUMP,
        RELOAD,
        LOAD_DATA0
    }

    public class ResumeTask
    {
        public ResumeTaskType type;
        public FunctionalObjectInstance instance;
    }

    //ToDo:refactoring
    public class TRVirtualMachine : SingletonMonoBehaviour<TRVirtualMachine>
    {
        public CancellationTokenSource tokenSource;

        public enum State { Sleep, Run, Reboot, Load }

        public State state = State.Sleep;

        public TRTagInstance currentTagInstance { get { return Trionfi.instance.scriptInstance[callStack.Peek().scriptName].instance; } }
        public FunctionalObjectInstance currentCallStack { get { return callStack.Peek(); } }

        public TRVariableDictionary globalVariableInstance = new TRVariableDictionary();
        public Queue<ResumeTask> nextTempFunc = new Queue<ResumeTask>();
        public Stack<FunctionalObjectInstance> callStack = new Stack<FunctionalObjectInstance>();
        public Stack<bool> ifStack = new Stack<bool>();
        public VariableStack vstack = new VariableStack();
 
        //マクロ、関数の情報（タグインスタンスの指定とタグ位置）。マクロと関数の実装的な区別はない。
        //スクリプトコンパイルの副産物なのでSerializeの必要はないはず。
        public Dictionary<string, FunctionalObjectInstance> functionalObjects = new Dictionary<string, FunctionalObjectInstance>();

        public Dictionary<string, AbstractComponent> aliasTagInstance = new Dictionary<string, AbstractComponent>();

        public async TRTask Run(string storage, Dictionary<string, VariableCalcurator> param = null)
        {
            if (Trionfi.instance.scriptInstance.ContainsKey(storage))
            {
                FunctionalObjectInstance _func = new FunctionalObjectInstance(FunctionalObjectType.Script, storage, 0, 0);
                _func.currentPos = _func.startPos;

BEGINLOOP:
                do
                {
                    _func = await Execute(_func, param);
                } while (callStack.Count > 0 && !tokenSource.IsCancellationRequested);

                ErrorLogger.Log("End of Script");

                if (tokenSource.IsCancellationRequested)
                    return;
                else if (nextTempFunc.Count != 0)
                {
                    ResumeTask resumeTask = (nextTempFunc.Dequeue());

                    switch (resumeTask.type)
                    {
                        case ResumeTaskType.JUMP:
                            _func = resumeTask.instance;
                            break;
                        case ResumeTaskType.RELOAD:
                            PrepareReboot();
                            Trionfi.instance.scriptInstance.Remove(resumeTask.instance.scriptName);
                            await Trionfi.instance.LoadScript(resumeTask.instance.scriptName);
                            _func = new FunctionalObjectInstance(FunctionalObjectType.Script, resumeTask.instance.scriptName, 0, 0);
                            break;
                    }

                    if (resumeTask.type >= ResumeTaskType.LOAD_DATA0)
                    {
                        PrepareReboot();
                        TRSerializeData info =  TRSerializeManager.instance.DeserializeFromFile(resumeTask.type - ResumeTaskType.LOAD_DATA0);
                        await info.Deserialize();
                        _func = info.callStack[0];//   callStack.Peek();   
                    }

                    goto BEGINLOOP;
                }

                if (Trionfi.instance.enableEndCallback)
                    Trionfi.instance.SleepTrionfi();
            }
            else
                ErrorLogger.Log("not find script file:" + storage);
        }

        public async TRTaskFunc Execute(FunctionalObjectInstance _func, Dictionary<string, VariableCalcurator>  _param)
        {
            callStack.Push(_func);

            if (_param != null)
                vstack.Push(_param);

            TRTagInstance _tag  = Trionfi.instance.scriptInstance[_func.scriptName].instance;

            state = State.Run;

            do
            {
                AbstractComponent _tagComponent = _tag.arrayComponents[_func.currentPos];

                if (_func.type == FunctionalObjectType.Macro && _tagComponent is MacroendComponent)
                    break;

                if (tokenSource.IsCancellationRequested)
                    return callStack.Pop();

                await _tagComponent.Execute();

                _func.currentPos++;

            } while (_func.currentPos < _tag.arrayComponents.Count);

            if (_param != null)
                vstack.Pop();

            return callStack.Pop();
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

        public void PrepareReboot()
        {
            Trionfi.instance.messageWindow.ClearMessage();
            Trionfi.instance.messageWindow.CloseWindow();
            Trionfi.instance.ActivateAllCanvas(false);
            Trionfi.instance.layerInstance.Reset();
            Trionfi.instance.audioInstance.Reset();
//            Trionfi.instance.ActivateAllCanvas(true);
        }

        public void BeginReboot()
        {
            currentCallStack.currentPos = currentCallStack.endPos + 1;

            ResumeTask task = new ResumeTask();
            task.instance = new FunctionalObjectInstance(FunctionalObjectType.Script, callStack.Peek().scriptName, 0, 0);
            task.type = ResumeTaskType.RELOAD;
            nextTempFunc.Enqueue(task);

            state = State.Reboot;
        }

        public void BeginLoad(int num)
        {
            currentCallStack.currentPos = currentCallStack.endPos + 1;

            ResumeTask task = new ResumeTask();
            //task.instance = new FunctionalObjectInstance(FunctionalObjectType.Script, callStack.Peek().scriptName, 0, 0);
            task.type = ResumeTaskType.LOAD_DATA0 + num;
            nextTempFunc.Enqueue(task);

            state = State.Load;
        }

        private void Start()
        {
            tokenSource = new CancellationTokenSource();
            astBuilder = new Jace.AstBuilder(functionRegistry, false);
            vstack.Push(globalVariableInstance);
        }

        public void Update()
		{
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F5) && state == State.Run)
            {
               BeginReboot();
            }
        }

		public void OnDestroy()
		{
            tokenSource.Cancel();
		}
	}
}
