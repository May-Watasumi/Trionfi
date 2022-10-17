using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

#if !TR_PARSEONLY
using UnityEngine;
using UnityEngine.UI;
#endif


namespace Trionfi
{
    [Serializable]
    public enum MessageType { Error, Warning, Info };

    [Serializable]
    public enum LocalizeID
    {
        JAPAN = 0,
        ENGLISH = 1,
        CHINESE = 2,
        KOREAN = 3,
    }

    [Serializable]
    public enum TRStandPosition
    {
        CENTER = 0,
        LEFT = 1,
        RIGHT = 2,
        LEFT_CENTER = 3,
        RIGHT_CENTER
    }

    [Serializable]
    public enum TRAudioID
    {
        BGM = 0,
        SE1 = 1,
        SE2 = 2,
        VOICE1 = 11,
        UI = 99
    }

    [Serializable]
    public enum TRLayerID
    {
        BG = 0,
        STAND1 = 1,
        STAND2 = 2,
        STAND3 = 3,
        STAND4 = 4,
        STAND5 = 5,
        STAND6 = 6,
        STAND7 = 7,
        STAND8 = 8,
        STAND9 = 9,
        EVENT = 10,
        MESICON = 98,
        MOVIE = 99,
        UI = 100
    }

    [Serializable]
    public enum TRKeyboardShortCut
    {
        AutoMode,
        SkipMode,
        HideWindow,
        LogWindow,
        SystemWindow,
        AutoSave,
    }

    [Serializable]
    public enum TRResourceType
    {
        LocalStatic = 0,
        LocalStreaming = 1,
        WWW = 2,
        AssetBundle = 3,
        Unknown = 99
    }

    [Serializable]
    public enum TRAssetType
    {
        Texture,
        Audio,
        Text,
        Movie,
        AssetBundle,
        Unknown
    };

#if !TR_PARSEONLY
    [Serializable]
    public class TRMediaInstance<T>
    {
        [SerializeField]
        public TRResourceType resourceType;

        [SerializeField]
        public TRVariableDictionary tagParam;

        [SerializeField]
        public T instance;
    }

#if TR_USE_CRI
    [System.Serializable]
    public class TRAdx : TRMediaInstance<CriAtomExPlayer>
    {
        public static string basePath;
        public static void  LoadAcf(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            CriAtomEx.UnregisterAcf();
            CriAtomEx.RegisterAcf(null, path);
        }

        public CriAtomExAcb acb;
    }
#endif

    [Serializable]
    public class TRScript : TRMediaInstance<TRTagInstance> { }

    [Serializable]
    public class TRAudio : TRMediaInstance<AudioSource>
    {
        public float mainVolume = 1.0f;
        public float faderVolume = 1.0f;

        public void UpdateVolume()
        {
            instance.volume = faderVolume * mainVolume * TRGameConfig.configData.mastervolume;
        }
    }

    [Serializable]
    public class TRLayer : TRMediaInstance<RawImage>
    {
        [SerializeField]
        public string actor;
    }

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
                //                layer[(int)count] = new SerializeInfo();
                layerParam[(int)instance.Key] = instance.Value.tagParam;
//                layer[count].type = instance.Value.resourceType;
//                layer[count].reservedNum = (int)instance.Key;
            }

            foreach (KeyValuePair<TRAudioID, TRAudio> instance in Trionfi.Instance.audioInstance)
            {
                audioParam[instance.Key] = instance.Value.tagParam;
/*
                audio[count] = new SerializeInfo();
                audio[count].storage = instance.Value.path;
                audio[count].type = instance.Value.resourceType;
                audio[count].reservedNum = (int)instance.Key;
*/
            }

           foreach (KeyValuePair<string, TRScript> instance in Trionfi.Instance.scriptInstance)
            {
                scriptParam[instance.Key] = instance.Value.tagParam;
                /*
                script[count] = new SerializeInfo();
                script[count].storage = instance.Value.path;
                script[count].type = instance.Value.resourceType;
                script[count].reservedString = instance.Key;
                */
            }

            callStack = TRVirtualMachine.Instance.callStack.ToArray();
            variable = TRVirtualMachine.Instance.globalVariableInstance;
            
            layerJson = JsonConvert.SerializeObject(layerParam);
            audioJson = JsonConvert.SerializeObject(audioParam);
            scriptJson = JsonConvert.SerializeObject(scriptParam);

            Debug.Log(layerJson);
            Debug.Log(audioJson);
            Debug.Log(scriptJson);

            //            return JsonUtility.ToJson(this);
            return JsonConvert.SerializeObject(this);
        }

        public IEnumerator Deserialize()
        {
            layerParam = JsonConvert.DeserializeObject<SerializableDictionary<int, TRVariableDictionary>>(layerJson);
            audioParam = JsonConvert.DeserializeObject<SerializableDictionary<int, TRVariableDictionary>>(audioJson);
            scriptParam = JsonConvert.DeserializeObject<SerializableDictionary<string, TRVariableDictionary>>(scriptJson);

            TRVirtualMachine.Instance.callStack.Clear();

            //トップスタックは本体側で入る
            for (int count = 0; count < callStack.Length-1; count++)
                TRVirtualMachine.Instance.callStack.Push(callStack[count]);

            foreach (KeyValuePair <int, TRVariableDictionary> instance in layerParam)
            {
                if (instance.Value.Count != 0)
                {
                    ImageComponent executer = new ImageComponent();
                    executer.tagParam = instance.Value;
                    yield return executer.TagSyncFunction();//     Trionfi.Instance.LoadImage(instance.Value, TRResourceType.LocalStatic);//].reservedNum, layer[count].storage, layer[count].type);
                }

            }

            foreach (KeyValuePair<int, TRVariableDictionary> instance in audioParam)
            {
                if (instance.Value.Count != 0)
                {
                    AudioComponent executer = new AudioComponent();
                    executer.tagParam = instance.Value;
                    yield return executer.TagSyncFunction();
                }
            }
 
            foreach (KeyValuePair<string, TRVariableDictionary> instance in scriptParam)
            {
                if (instance.Value.Count != 0)
                    yield return Trionfi.Instance.LoadScript(instance.Key);
            }
            
            if(variable != null)
               TRVirtualMachine.Instance.globalVariableInstance = variable;
        }
    }
#endif
}
