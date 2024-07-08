using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;

namespace Trionfi
{

    [Serializable]
    public class TRSerializeData
    {
        [SerializeField]
        SerializableDictionary<int, TRVariableDictionary> layerParam = new SerializableDictionary<int, TRVariableDictionary>();
        [SerializeField]
        SerializableDictionary<int, TRVariableDictionary> audioParam = new SerializableDictionary<int, TRVariableDictionary>();
        [SerializeField]
        SerializableDictionary<string, TRVariableDictionary> scriptParam = new SerializableDictionary<string, TRVariableDictionary>();

        [SerializeField]
        public string layerJson;
        [SerializeField]
        public string audioJson;
        [SerializeField]
        public string scriptJson;

        [SerializeField]
        public FunctionalObjectInstance[] callStack;
        [SerializeField]
        public TRVariableDictionary variable;

        public string Serialize()
        {
            foreach (KeyValuePair<TRLayerID, TRLayer> instance in Trionfi.Instance.layerInstance)
            {
                layerParam[instance.Key] = instance.Value.tagParam;
            }

            foreach (KeyValuePair<TRAudioID, TRAudio> instance in Trionfi.Instance.audioInstance)
            {
                audioParam[instance.Key] = instance.Value.tagParam;
            }

            foreach (KeyValuePair<string, TRScript> instance in Trionfi.Instance.scriptInstance)
            {
                scriptParam[instance.Key] = instance.Value.tagParam;
            }

            callStack = TRVirtualMachine.Instance.callStack.ToArray();
            variable = TRVirtualMachine.Instance.globalVariableInstance;

            layerJson = JsonConvert.SerializeObject(layerParam);
            audioJson = JsonConvert.SerializeObject(audioParam);
            scriptJson = JsonConvert.SerializeObject(scriptParam);

            //            return JsonUtility.ToJson(this);
            return JsonConvert.SerializeObject(this);
        }

        public async TRTask Deserialize()
        {
            layerParam = JsonConvert.DeserializeObject<SerializableDictionary<int, TRVariableDictionary>>(layerJson);
            audioParam = JsonConvert.DeserializeObject<SerializableDictionary<int, TRVariableDictionary>>(audioJson);
            scriptParam = JsonConvert.DeserializeObject<SerializableDictionary<string, TRVariableDictionary>>(scriptJson);

            TRVirtualMachine.Instance.callStack.Clear();

            //トップスタックは本体側で入る
            for (int count = 0; count < callStack.Length - 1; count++)
                TRVirtualMachine.Instance.callStack.Push(callStack[count]);

            foreach (KeyValuePair<int, TRVariableDictionary> instance in layerParam)
            {
                if (instance.Value.Count != 0)
                {
                    ImageComponent executer = new ImageComponent();
                    executer.tagParam = instance.Value;
                    await executer.Execute();
                }
            }

            foreach (KeyValuePair<int, TRVariableDictionary> instance in audioParam)
            {
                if (instance.Value.Count != 0)
                {
                    AudioComponent executer = new AudioComponent();
                    executer.tagParam = instance.Value;
                    await executer.Execute();
                }
            }

            foreach (KeyValuePair<string, TRVariableDictionary> instance in scriptParam)
            {
                if (instance.Value.Count != 0)
                    await Trionfi.Instance.LoadScript(instance.Key);
            }

            if (variable != null)
                TRVirtualMachine.Instance.globalVariableInstance = variable;
        }
    }

    [Serializable]
    public class TRSaveDataInfo
    {
        public string date;
        public string subject;
    }
}
