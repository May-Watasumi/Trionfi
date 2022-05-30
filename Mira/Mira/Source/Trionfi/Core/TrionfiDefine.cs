using System;
using System.Collections;
using System.Collections.Generic;

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
    public class MediaInstanceKey<T> : SerializableDictionary<T, string>
    { };

    [Serializable]
    public class TRMediaInstance<T>
    {
        [SerializeField]
        public TRResourceType resourceType;
        [SerializeField]
        public string path;
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
        public string actor;
    }

    [Serializable]
    public class SerializeInfo
    {
        [SerializeField]
        public string storage;
        [SerializeField]
        public TRResourceType type;
        [SerializeField]
        public string reservedString;
        [SerializeField]
        public int reservedNum;
    }

    [Serializable]
    public class SerializeData
    {
        [SerializeField]
        public SerializeInfo[] layer;
        [SerializeField]
        public SerializeInfo[] audio;
        [SerializeField]
        public SerializeInfo[] script;
        [SerializeField]
        public FunctionalObjectInstance[] callStack;
        [SerializeField]
        TRVariableDictionary variable;

        public void Serialize()
        {
            callStack = TRVirtualMachine.Instance.callStack.ToArray();

            audio = new SerializeInfo[Trionfi.Instance.audioInstance.Count];
            layer = new SerializeInfo[Trionfi.Instance.layerInstance.Count];
            script = new SerializeInfo[Trionfi.Instance.scriptInstance.Count];

            int count = 0;

            foreach (KeyValuePair<TRLayerID, TRLayer> instance in Trionfi.Instance.layerInstance)
            {
                layer[count] = new SerializeInfo();
                layer[count].storage = instance.Value.path;
                layer[count].type = instance.Value.resourceType;
                layer[count].reservedNum = (int)instance.Key;
                count++;
            }

            count = 0;
            foreach (KeyValuePair<TRAudioID, TRAudio> instance in Trionfi.Instance.audioInstance)
            {
                audio[count] = new SerializeInfo();
                audio[count].storage = instance.Value.path;
                audio[count].type = instance.Value.resourceType;
                audio[count].reservedNum = (int)instance.Key;
                count++;
            }

            count = 0;
            foreach (KeyValuePair<string, TRScript> instance in Trionfi.Instance.scriptInstance)
            {
                script[count] = new SerializeInfo();
                script[count].storage = instance.Value.path;
                script[count].type = instance.Value.resourceType;
                script[count].reservedString = instance.Key;
                count++;
            }

            variable = TRVirtualMachine.Instance.globalVariableInstance;
        }

        public IEnumerator Deserialize()
        {
            TRVirtualMachine.Instance.callStack.Clear();

            for (int count = 0; count < callStack.Length; count++)
                TRVirtualMachine.Instance.callStack.Push(callStack[count]);

            for (int count = 0; count < layer.Length; count++)
            {
                yield return Trionfi.Instance.LoadImage(layer[count].reservedNum, layer[count].storage, layer[count].type);
            }

            for (int count = 0; count < audio.Length; count++)
            {
                yield return Trionfi.Instance.LoadAudio(audio[count].reservedNum, audio[count].storage, audio[count].type);
            }

            for (int count = 0; count < script.Length; count++)
            {
                yield return Trionfi.Instance.LoadScript(layer[count].storage, layer[count].type);
            }

            TRVirtualMachine.Instance.globalVariableInstance = variable;
        }
    }
#endif
}