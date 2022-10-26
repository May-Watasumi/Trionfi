using System.IO;
using System.Collections;
using System.Collections.Generic;

#if !TR_PARSEONLY
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

using TRTask = Cysharp.Threading.Tasks.UniTask;

//using TRTaskTextAsset = Cysharp.Threading.Tasks.UniTask<UnityEngine.TextAsset>;
using TRTaskAudio = Cysharp.Threading.Tasks.UniTask<UnityEngine.AudioClip>;
using TRTaskTexture = Cysharp.Threading.Tasks.UniTask<UnityEngine.Texture2D>;
using TRTaskSprite = Cysharp.Threading.Tasks.UniTask<UnityEngine.Sprite>;
using TRTaskAssetBundle = Cysharp.Threading.Tasks.UniTask<UnityEngine.AssetBundle>;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;
#else
using System.Threading.Tasks;

using TRTask = System.Threading.Tasks.Task;
using TRTaskTextAsset = System.Threading.Tasks.Task<UnityEngine.TextAsset>;
using TRTaskAudio = System.Threading.Tasks.Task<UnityEngine.AudioClip>;
using TRTaskTexture = System.Threading.Tasks.Task<UnityEngine.Texture2D>;
using TRTaskSprite = System.Threading.Tasks.Task<UnityEngine.Sprite>;
using TRTaskAssetBundle = System.Threading.Tasks.Task<UnityEngine.AssetBundle>;
using TRTaskString = System.Threading.Tasks.Task<string>;
#endif

namespace Trionfi
{
    public abstract class IAssetLoader<T>
    {
        public T instance;
        public virtual async Cysharp.Threading.Tasks.UniTask<T> Load(string storage) { return instance; }
    }

    public class TRDefaultTextLoader : IAssetLoader<string>
    {
        public override async TRTaskString Load(string storage)
        {
            TextAsset text = await Resources.LoadAsync<TextAsset>(storage) as TextAsset ;

            instance = text.text;

            return instance;
            /*
            if (text != null)
                return text.text;
            else
                return string.Empty;
            */
        }
    }

    public class TRDefaultAudioLoader : IAssetLoader<AudioClip>
    {
        public override async TRTaskAudio Load(string storage)
        {
            instance = await Resources.LoadAsync<AudioClip>(storage) as AudioClip;
            return instance;
        }
    }

    public class TRDefaultTextureLoader : IAssetLoader<Texture2D>
    {
        public override async TRTaskTexture Load(string storage)
        {
            instance = await Resources.LoadAsync<Texture2D>(storage) as Texture2D;
            return instance;
        }
    }

    public class TRDefaultSpriteLoader : IAssetLoader<Sprite>
    {
        public override async TRTaskSprite Load(string storage)
        {
            instance = await Resources.LoadAsync<Sprite>(storage) as Sprite;
            return instance;
        }
    }

    public class TRDefaultAssetBundleLoader : IAssetLoader<AssetBundle>
    {
        public override async TRTaskAssetBundle Load(string storage)
        {
            instance = await Resources.LoadAsync<AssetBundle>(storage) as AssetBundle;
            return instance;
        }
    }

    public class TRWebTextLoader : IAssetLoader<string>
    {
        public override async TRTaskString Load(string storage)
        {
            instance = null;

            UnityWebRequest request = UnityWebRequest.Get(storage);

            await request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (!request.isNetworkError)
#endif
                Debug.Log(request.error);
            else
                instance = request.downloadHandler.text;

            return instance;

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

        public override async TRTaskAudio Load(string storage)
        {
           instance = null;

            AudioType _type = audioType[(System.IO.Path.GetExtension(storage)).ToLower()];

            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(storage, _type);

            await request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (!request.isNetworkError)
#endif
                Debug.Log(request.error);
            else
                instance = DownloadHandlerAudioClip.GetContent(request);
            
            return instance;
        }
    }

    public class TRWebTextureLoader : IAssetLoader<Texture2D>
    {
        public override async TRTaskTexture Load(string storage)
        {
            instance = null;

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(storage);
            await request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (!request.isNetworkError)
#endif
                Debug.Log(request.error);
            else
                instance = DownloadHandlerTexture.GetContent(request);

            return instance;
        }
    }

    public class TRWebAssetBundleLoader : IAssetLoader<AssetBundle>
    {
        public override async TRTaskAssetBundle Load(string storage)
        {
            instance = null;

            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(storage);

            await request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (!request.isNetworkError)
#endif
                Debug.Log(request.error);
            else
                instance = DownloadHandlerAssetBundle.GetContent(request);

            return instance;
        }
    }

    public class TRStreamTextLoader : IAssetLoader<string>
    {
        public override async TRTaskString Load(string storage)
        {
            string fullPath = Application.streamingAssetsPath + storage;
            instance = File.ReadAllText(fullPath);
            return instance;
        }
    }

    public class TRStreamAssetBundleLoader : IAssetLoader<AssetBundle>
    {
        public override async TRTaskAssetBundle Load(string storage)
        {
            instance = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, storage));
            return instance;
        }
    }

    public class TRResourceLoader  : SingletonMonoBehaviour<TRResourceLoader>
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
        public Dictionary<TRResourceType, IAssetLoader<Sprite>> spriteLoader = new Dictionary<TRResourceType, IAssetLoader<Sprite>>();
        public Dictionary<TRResourceType, IAssetLoader<string>> textLoader = new Dictionary<TRResourceType, IAssetLoader<string>>();
        public Dictionary<TRResourceType, IAssetLoader<AssetBundle>> assetBundleLoader = new Dictionary<TRResourceType, IAssetLoader<AssetBundle>>();

        public void Initialize()
        {
            audioLoader[TRResourceType.LocalStatic] = new TRDefaultAudioLoader();
            audioLoader[TRResourceType.WWW] = new TRWebAudioLoader();

            textureLoader[TRResourceType.LocalStatic] = new TRDefaultTextureLoader();
            textureLoader[TRResourceType.WWW] = new TRWebTextureLoader();

            spriteLoader[TRResourceType.LocalStatic] = new TRDefaultSpriteLoader();

            textLoader[TRResourceType.LocalStatic] = new TRDefaultTextLoader();
            textLoader[TRResourceType.WWW] = new TRWebTextLoader();

            assetBundleLoader[TRResourceType.LocalStatic] = new TRDefaultAssetBundleLoader();
            assetBundleLoader[TRResourceType.WWW] = new TRWebAssetBundleLoader();

            audioLoader[TRResourceType.LocalStreaming] = new TRWebAudioLoader();
            textureLoader[TRResourceType.LocalStreaming] = new TRWebTextureLoader();
            textLoader[TRResourceType.LocalStreaming] = new TRStreamTextLoader();
            assetBundleLoader[TRResourceType.LocalStreaming] = new TRStreamAssetBundleLoader();
        }

        public async TRTaskAudio LoadAudio(string storage, TRResourceType type = defaultResourceType)
        {
            await audioLoader[type].Load(storage);
            return audioLoader[type].instance;
        }

        public async TRTaskString LoadText(string storage, TRResourceType type = defaultResourceType)
        {
            await textLoader[type].Load(storage);
            return textLoader[type].instance;
        }

        public async TRTaskTexture LoadTexture(string storage, TRResourceType type = defaultResourceType)
        {
            await textureLoader[type].Load(storage); ;
            return textureLoader[type].instance;
        }

        public async TRTaskSprite LoadSprite(string storage, TRResourceType type = defaultResourceType)
        {
            Sprite instance = null;
            if (type == TRResourceType.LocalStatic)
            {
                instance = await Resources.LoadAsync<Sprite>(storage) as Sprite;
            }
            else
            {
                Texture2D texture =  await textureLoader[type].Load(storage);
                instance = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }

            return instance;
        }

        public async TRTaskAssetBundle LoadAssetBundle(string storage, TRResourceType type = defaultResourceType)
        {
            await assetBundleLoader[type].Load(storage);
            return assetBundleLoader[type].instance;
        }

        public async TRTaskAudio LoadAudioFromBundle(string storage, string bundle)
        {
            AssetBundleRequest request = assetBundleList[bundle].LoadAssetAsync<AudioClip>(storage);
            return (AudioClip)request.asset;
        }

        public IEnumerator LoadTextFromBundle(string storage, string bundle)
        {
            yield return assetBundleList[bundle].LoadAsset<TextAsset>(storage).text;
        }

        public IEnumerator LoadTextureFromBundle(string storage, string bundle)
        {
            yield return assetBundleList[bundle].LoadAsset<Texture2D>(storage);
        }

        new protected void Awake()
        {
            base.Awake();
            Initialize();
        }
/*
		private void Start()
        {
            Initialize();
        }
*/
    }
}
