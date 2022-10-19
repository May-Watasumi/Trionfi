using System;
using System.IO;
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
#endif
}
