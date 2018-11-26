using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    [SerializeField]
    public enum TRDataProtocol
    {
        Default,
        File,
        LocalResource,
        Network,
        Null,
    }

    public enum TRResourceType
    {
        Texture,
        Audio,
        Text,
        Movie,
        AssetBundle,
        Terminate
    };
    
    public class TRResourceLoader : SingletonMonoBehaviour<TRResourceLoader>
	{
        /*
                public string MakeStoragePath(string file, TRAssetType dataType)
                {
                    string _basePath = dataPath[dataType];

                    // "/"補完
                    if (!_basePath.EndsWith("/"))
                       _basePath += "/";

                    return _basePath + file;
                }       
        */

        readonly Dictionary<string, AudioType> audioType = new Dictionary<string, AudioType>()
        {
            { "wav", AudioType.WAV },
            { "mp3", AudioType.MPEG },
            { "ogg", AudioType.OGGVORBIS },
        };

        protected class LoadedResource
        {
            public bool result;
            public Texture2D texture;
            public AudioClip audio;
            public string text;
#if UNITY_STANDALONE
            public MovieTexture movie;
#endif
            public AssetBundle assetBundole;
        }

        [SerializeField]
        public string localReourcesPath = "";
        [SerializeField]
        public string localFilePath = "Trionfi/Example/Resources/";
        [SerializeField]
        public string saveDataPath = "savedata/";
        [SerializeField]
        public Image loadingIcon = null;
        [SerializeField]
        public TRDataProtocol defaultDataType = TRDataProtocol.LocalResource;

        TRDataProtocol lastDataType = TRDataProtocol.Null;

        public bool isLoading = false;

        LoadedResource resourceInstance = new LoadedResource();

        UnityWebRequest request;

        public bool isSuceeded
        {
            get
            {
                if (lastDataType == TRDataProtocol.Null)
                    return false;
                else if (lastDataType == TRDataProtocol.File || lastDataType == TRDataProtocol.Network)
                    return !TRResourceLoader.Instance.request.isHttpError && !TRResourceLoader.Instance.request.isNetworkError;
                else
                    return resourceInstance.result;
            }
        }

        public Texture2D texture
        {
            get
            {
                return lastDataType == TRDataProtocol.LocalResource ? resourceInstance.texture : DownloadHandlerTexture.GetContent(request);
            }
        }
        public new AudioClip audio
        {
            get
            {
                return lastDataType == TRDataProtocol.LocalResource ? resourceInstance.audio : DownloadHandlerAudioClip.GetContent(request);
            }
        }

        public string text
        {
            get
            {
                return lastDataType == TRDataProtocol.LocalResource ? resourceInstance.text : request.downloadHandler.text;
            }
        }
#if UNITY_STANDALONE
        public MovieTexture movie
        {
            get
            {
                return lastDataType == TRDataProtocol.LocalResource ? resourceInstance.movie : DownloadHandlerMovieTexture.GetContent(request);
            }
        }

#endif
        public AssetBundle assetBundole
        {
            get
            {
                return lastDataType == TRDataProtocol.LocalResource ? resourceInstance.assetBundole : DownloadHandlerAssetBundle.GetContent(request);
            }
        }

        public void Load(string url, TRResourceType type, TRDataProtocol protocol = TRDataProtocol.Default)
        {
            isLoading = true;

            if (protocol == TRDataProtocol.Default)
                protocol = defaultDataType;

            StartCoroutine(LoadCoroutine(url, type, protocol));
        }

        private IEnumerator LoadCoroutine(string url, TRResourceType type, TRDataProtocol protocol)
        {
            lastDataType = protocol;

            string fullpath;

            if (protocol == TRDataProtocol.LocalResource)
            {
                fullpath = localReourcesPath + url;
                switch (type)
                {
                    case TRResourceType.Texture:
                        resourceInstance.result = (resourceInstance.texture = Resources.Load<Texture2D>(fullpath)) != null;
                        break;
                    case TRResourceType.AssetBundle:
                        //たぶんそんなものはない。
                        resourceInstance.result = (resourceInstance.assetBundole = Resources.Load<AssetBundle>(fullpath)) != null;
                        break;
                    case TRResourceType.Audio:
                        resourceInstance.result = (resourceInstance.audio = Resources.Load<AudioClip>(fullpath)) != null;
                        break;
#if UNITY_STANDALONE
                        case TRResourceType.Movie:
                        //たぶんそんなものはない。
                        resourceInstance.result = (resourceInstance.movie = Resources.Load<MovieTexture>(fullpath)) != null;
                        break;
#endif
                        default:
                        resourceInstance.result = (resourceInstance.text = Resources.Load<TextAsset>(fullpath).text) != null;
                        break;
                }
            }
            else
            {
                fullpath = protocol == TRDataProtocol.Network ? url : "file:///" + Application.persistentDataPath + localFilePath + url;

                switch (type)
                {
                    case TRResourceType.Texture:
#if UNITY_2017_1_OR_NEWER
                        request = UnityWebRequestTexture.GetTexture(fullpath);
#else
                        request = UnityWebRequest.GetTexture(fullpath);
#endif
                        break;
                    case TRResourceType.AssetBundle:
#if UNITY_2018_1_OR_NEWER
                        request = UnityWebRequestAssetBundle.GetAssetBundle(fullpath);
#else
                        request = UnityWebRequest.GetAssetBundle(fullpath);
#endif
                        break;
                    case TRResourceType.Audio:
                        AudioType _type = audioType[(System.IO.Path.GetExtension(url)).ToLower()];
#if UNITY_2017_1_OR_NEWER
                        request = UnityWebRequestMultimedia.GetAudioClip(fullpath, _type);
#else
                        request = UnityWebRequest.GetAudioClip(fullpath, _type);
#endif
                        break;
#if UNITY_STANDALONE
                    case TRResourceType.Movie:
//ムービーテクスチャはモバイル非対応らしい。
#if UNITY_2017_1_OR_NEWER
                        request = UnityWebRequestMultimedia.GetMovieTexture(fullpath);
#else
                        request = UnityWebRequest.GetTexture(fullpath);
#endif
                        break;
#endif
                    default:
                        request = UnityWebRequest.Get(fullpath);
                        break;
                }

#if UNITY_2017_2_OR_NEWER               
                yield return request.SendWebRequest();
#else
                yield return request.Send();
#endif
            }


            if (!isSuceeded)// request.isHttpError || request.isNetworkError)
            {
                if (lastDataType != TRDataProtocol.LocalResource)
                    ErrorLogger.Log(request.error);
                else
                    ErrorLogger.Log("Resources not Loaded : " + url);
            }

            isLoading = false;
        }
    }
}
