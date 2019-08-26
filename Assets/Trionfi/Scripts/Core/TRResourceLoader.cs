using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
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
            yield return instance;
        }
    }

    public class TRDefaultAudioLoader : IAssetLoader<AudioClip>
    {
        public override IEnumerator Load(string storage)
        {
            instance = Resources.Load<AudioClip>(storage);
            yield return instance;
        }
    }

    public class TRDefaultTextureLoader : IAssetLoader<Texture2D>
    {
        public override IEnumerator Load(string storage)
        {
            instance = Resources.Load<Texture2D>(storage);
            yield return instance;
        }
    }

    public class TRDefaultAssetBundleLoader : IAssetLoader<AssetBundle>
    {
        public override IEnumerator Load(string storage)
        {
            instance = Resources.Load<AssetBundle>(storage);
            yield return instance;

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

            yield return instance;

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
            yield return instance;
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
            yield return instance;
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
            yield return instance;
        }
    }

    public class TRStreamTextLoader : IAssetLoader<string>
    {
        public override IEnumerator Load(string storage)
        {
            string fullPath = Application.streamingAssetsPath + storage;
            instance = File.ReadAllText(fullPath);
            yield return instance;
        }
    }

    public class TRStreamAssetBundleLoader : IAssetLoader<AssetBundle>
    {
        public override IEnumerator Load(string storage)
        {
            instance = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, storage));
            yield return instance;
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

        public const TRResourceType defaultResourceType = TRResourceType.LocalStatic;

        public Dictionary<string, AssetBundle> assetBundleList;

        public Dictionary<TRResourceType, IAssetLoader<AudioClip>> audioLoader = new Dictionary<TRResourceType, IAssetLoader<AudioClip>>();
        public Dictionary<TRResourceType, IAssetLoader<Texture2D>> textureLoader = new Dictionary<TRResourceType, IAssetLoader<Texture2D>>();
        public Dictionary<TRResourceType, IAssetLoader<string>> textLoader = new Dictionary<TRResourceType, IAssetLoader<string>>();
        public Dictionary<TRResourceType, IAssetLoader<AssetBundle>> assetBundleLoader = new Dictionary<TRResourceType, IAssetLoader<AssetBundle>>();

        public void Initialize()
        {
            audioLoader[TRResourceType.LocalStatic] = new TRDefaultAudioLoader();
            audioLoader[TRResourceType.WWW] = new TRWebAudioLoader();

            textureLoader[TRResourceType.LocalStatic] = new TRDefaultTextureLoader();
            textureLoader[TRResourceType.WWW] = new TRWebTextureLoader();

            textLoader[TRResourceType.LocalStatic] = new TRDefaultTextLoader();
            textLoader[TRResourceType.WWW] = new TRWebTextLoader();

            assetBundleLoader[TRResourceType.LocalStatic] = new TRDefaultAssetBundleLoader();
            assetBundleLoader[TRResourceType.WWW] = new TRWebAssetBundleLoader();

            audioLoader[TRResourceType.LocalStreaming] = new TRWebAudioLoader();
            textureLoader[TRResourceType.LocalStreaming] = new TRWebTextureLoader();
            textLoader[TRResourceType.LocalStreaming] = new TRStreamTextLoader();
            assetBundleLoader[TRResourceType.LocalStreaming] = new TRStreamAssetBundleLoader();
        }

        public IEnumerator LoadAudio(string storage, TRResourceType type = defaultResourceType)
        {
            var coroutine = audioLoader[type].Load(storage);
            yield return StartCoroutine(coroutine);
            yield return coroutine.Current;           
        }

        public IEnumerator LoadText(string storage, TRResourceType type = defaultResourceType)
        {
            var coroutine =  textLoader[type].Load(storage);
            yield return StartCoroutine(coroutine);
            yield return coroutine.Current;
        }

        public IEnumerator LoadTexture(string storage, TRResourceType type = defaultResourceType)
        {
            var coroutine = textureLoader[type].Load(storage); ;
            yield return StartCoroutine(coroutine);
            yield return coroutine.Current;
        }
        public IEnumerator LoadAssetBundle(string storage, TRResourceType type = defaultResourceType)
        {
            var coroutine = assetBundleLoader[type].Load(storage);
            yield return StartCoroutine(coroutine);
            yield return coroutine.Current;
        }

        public IEnumerator LoadAudioFromBundle(string storage, string bundle)
        {
            yield return assetBundleList[bundle].LoadAsset<AudioClip>(storage);
        }

        public IEnumerator LoadTextFromBundle(string storage, string bundle)
        {
            yield return assetBundleList[bundle].LoadAsset<TextAsset>(storage).text;
        }

        public IEnumerator LoadTextureFromBundle(string storage, string bundle)
        {
            yield return assetBundleList[bundle].LoadAsset<Texture2D>(storage);
        }

        private void Start()
        {
            Initialize();
        }
    }
}
