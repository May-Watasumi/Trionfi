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

    public abstract class IResourceLoader<T>
    {
        public T instance;
        public abstract IEnumerator Load(string storage);
    }

    public class TRDefaultTextLoader : IResourceLoader<string>
    {
        public override IEnumerator Load(string storage)
        {
            instance = Resources.Load<TextAsset>(storage).text;
            yield return null;
        }
    }

    public class TRDefaultAudioLoader : IResourceLoader<AudioClip>
    {
        public override IEnumerator Load(string storage)
        {
            instance = Resources.Load<AudioClip>(storage);
            yield return null;
        }
    }

    public class TRDefaultTextureLoader : IResourceLoader<Texture2D>
    {
        public override IEnumerator Load(string storage)
        {
            instance = Resources.Load<Texture2D>(storage);
            yield return null;
        }
    }

    public class TRDefaultAssetBundleLoader : IResourceLoader<AssetBundle>
    {
        public override IEnumerator Load(string storage)
        {
            instance = Resources.Load<AssetBundle>(storage);
            yield return null;
        }
    }

    public class TRWebTextLoader : IResourceLoader<string>
    {
        public override IEnumerator Load(string storage)
        {
            instance = null;

            UnityWebRequest request = UnityWebRequest.Get(storage);

#if UNITY_2017_2_OR_NEWER
            yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
                instance = request.downloadHandler.text;
        }
    }

    public class TRWebAudioLoader : IResourceLoader<AudioClip>
    {
        readonly Dictionary<string, AudioType> audioType = new Dictionary<string, AudioType>()
        {
            { "wav", AudioType.WAV },
            { "mp3", AudioType.MPEG },
            { "ogg", AudioType.OGGVORBIS },
        };

        public override IEnumerator Load(string storage)
        {
            instance = null;

            AudioType _type = audioType[(System.IO.Path.GetExtension(storage)).ToLower()];

#if UNITY_2017_1_OR_NEWER
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(storage, _type);
 #if UNITY_2017_2_OR_NEWER
            yield return request.SendWebRequest();
 #else
            yield return request.Send();
 #endif
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
                instance = DownloadHandlerAudioClip.GetContent(request);
#else
            UnityWebRequest request = UnityWebRequest.GetAudioClip(storage, _type);
            yield return request.Send();
#endif
            yield return null;
        }
    }

    public class TRWebTextureLoader : IResourceLoader<Texture2D>
    {
        public override IEnumerator Load(string storage)
        {
            instance = null;

#if UNITY_2017_1_OR_NEWER
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(storage);
 #if UNITY_2017_2_OR_NEWER
            yield return request.SendWebRequest();
 #else
            yield return request.Send();
 #endif

            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
                instance = DownloadHandlerTexture.GetContent(request);
#else
            request = UnityWebRequestTexture.GetTexture(storage);
            yield return request.Send();
#endif
        }
    }

    public class TRWebAssetBundleLoader : IResourceLoader<AssetBundle>
    {
        public override IEnumerator Load(string storage)
        {
            instance = null;

#if UNITY_2018_1_OR_NEWER
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(storage);
#else
            UnityWebRequest request = UnityWebRequest.GetAssetBundle(storage);
 #if UNITY_2017_2_OR_NEWER
            yield return request.SendWebRequest();
 #else
            yield return request.Send();
 #endif

            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
                instance = DownloadHandlerAssetBundle.GetContent(request);
#endif
            yield return null;
        }
    }

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

        public IResourceLoader<AudioClip> defaultAudioLoader;
        public IResourceLoader<Texture2D> defaultTextureLoader;
        public IResourceLoader<string> defaultTextLoader;
        public IResourceLoader<AssetBundle> defaultAssetBundleLoader;

        TRDefaultTextLoader resourcesTextLoader = new TRDefaultTextLoader();
        TRDefaultAudioLoader resourcesAudioLoader = new TRDefaultAudioLoader();
        TRDefaultTextureLoader resourcesTextureLoader = new TRDefaultTextureLoader();
        TRDefaultAssetBundleLoader resourcesAssetBundleLoader = new TRDefaultAssetBundleLoader();

        public IEnumerator LoadText(string storage, IResourceLoader<string> loader = null)
        {
            if (loader == null)
                loader = defaultTextLoader;
            yield return loader.Load(storage);
        }

        public IEnumerator LoadAudio(string storage, IResourceLoader<AudioClip> loader = null)
        {
            if (loader == null)
                loader = defaultAudioLoader;
            yield return loader.Load(storage);
        }

        public IEnumerator LoadTexture(string storage, IResourceLoader<Texture2D> loader = null)
        {
            if (loader == null)
                loader = defaultTextureLoader;
            yield return loader.Load(storage);
        }

        public IEnumerator LoadAssetBundle(string storage, IResourceLoader<AssetBundle> loader = null)
        {
            if (loader == null)
                loader = defaultAssetBundleLoader;
            yield return loader.Load(storage);
        }

        private void Start()
        {
            defaultTextLoader = resourcesTextLoader;
            defaultAudioLoader = resourcesAudioLoader;
            defaultTextureLoader = resourcesTextureLoader;
            defaultAssetBundleLoader = resourcesAssetBundleLoader;
        }
    }
}
