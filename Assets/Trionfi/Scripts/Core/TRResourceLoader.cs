using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public abstract class IAssetLoader<T>
    {
        public T instance;
        public abstract IEnumerator Load(string storage);
    }

    public class TRDefaultTextLoader : IAssetLoader<string>
    {
        public override IEnumerator Load(string storage)
        {
            instance = Resources.Load<TextAsset>(storage).text;
            yield return null;
        }
    }

    public class TRDefaultAudioLoader : IAssetLoader<AudioClip>
    {
        public override IEnumerator Load(string storage)
        {
            instance = Resources.Load<AudioClip>(storage);
            yield return null;
        }
    }

    public class TRDefaultTextureLoader : IAssetLoader<Texture2D>
    {
        public override IEnumerator Load(string storage)
        {
            instance = Resources.Load<Texture2D>(storage);
            yield return null;
        }
    }

    public class TRDefaultAssetBundleLoader : IAssetLoader<AssetBundle>
    {
        public override IEnumerator Load(string storage)
        {
            instance = Resources.Load<AssetBundle>(storage);
            yield return null;
        }
    }

    public class TRWebTextLoader : IAssetLoader<string>
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

    public class TRWebAudioLoader : IAssetLoader<AudioClip>
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

    public class TRWebTextureLoader : IAssetLoader<Texture2D>
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

    public class TRWebAssetBundleLoader : IAssetLoader<AssetBundle>
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

        public IAssetLoader<AudioClip> defaultAudioLoader;
        public IAssetLoader<Texture2D> defaultTextureLoader;
        public IAssetLoader<string> defaultTextLoader;
        public IAssetLoader<AssetBundle> defaultAssetBundleLoader;

        TRDefaultTextLoader resourcesTextLoader = new TRDefaultTextLoader();
        TRDefaultAudioLoader resourcesAudioLoader = new TRDefaultAudioLoader();
        TRDefaultTextureLoader resourcesTextureLoader = new TRDefaultTextureLoader();
        TRDefaultAssetBundleLoader resourcesAssetBundleLoader = new TRDefaultAssetBundleLoader();

        public IEnumerator LoadText(string storage, IAssetLoader<string> loader = null)
        {
            if (loader == null)
                loader = defaultTextLoader;
            yield return loader.Load(storage);
        }

        public IEnumerator LoadAudio(string storage, IAssetLoader<AudioClip> loader = null)
        {
            if (loader == null)
                loader = defaultAudioLoader;
            yield return loader.Load(storage);
        }

        public IEnumerator LoadTexture(string storage, IAssetLoader<Texture2D> loader = null)
        {
            if (loader == null)
                loader = defaultTextureLoader;
            yield return loader.Load(storage);
        }

        public IEnumerator LoadAssetBundle(string storage, IAssetLoader<AssetBundle> loader = null)
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
