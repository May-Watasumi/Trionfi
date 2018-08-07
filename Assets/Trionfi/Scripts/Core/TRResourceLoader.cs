using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public enum TRDataProtocol
    {
        File,
        LocalResource,
        Network,
        Null
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
            public Texture2D texture;
            public AudioClip audio;
            public string text;
            public MovieTexture movie;
            public AssetBundle assetBundole;
        }

        protected const TRDataProtocol defaultDataType = TRDataProtocol.LocalResource;

        TRDataProtocol lastDataType = TRDataProtocol.Null;

        bool isLoading = false;

        protected LoadedResource resourceInstance = new LoadedResource();

        public UnityWebRequest request;

        public Texture2D texture
        {
            get
            {
                return lastDataType == TRDataProtocol.LocalResource ? resourceInstance.texture : DownloadHandlerTexture.GetContent(request);
            }
        }
        public AudioClip audio
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

        public MovieTexture movie
        {
            get
            {
                return lastDataType == TRDataProtocol.LocalResource ? resourceInstance.movie : DownloadHandlerMovieTexture.GetContent(request);
            }
        }

        public AssetBundle assetBundole
        {
            get
            {
                return lastDataType == TRDataProtocol.LocalResource ? resourceInstance.assetBundole : DownloadHandlerAssetBundle.GetContent(request);
            }
        }

        public IEnumerator Load(string url, TRResourceType type, TRDataProtocol protocol = defaultDataType)
        {
            isLoading = true;

            string fullpath = protocol == TRDataProtocol.Network ? url : "file://" + Application.persistentDataPath + url;
            if (protocol == TRDataProtocol.LocalResource)
            {
                switch (type)
                {
                    case TRResourceType.Texture:
                        resourceInstance.texture = Resources.Load<Texture2D>(url);
                        break;
                    case TRResourceType.AssetBundle:
                        //たぶんそんなものはない。
                        resourceInstance.assetBundole = Resources.Load<AssetBundle>(url);
                        break;
                    case TRResourceType.Audio:
                        resourceInstance.audio = Resources.Load<AudioClip>(url);
                        break;
                    case TRResourceType.Movie:
                        //たぶんそんなものはない。
                        resourceInstance.movie = Resources.Load<MovieTexture>(url);
                        break;
                    default:
                        resourceInstance.text = Resources.Load<TextAsset>(url).text;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case TRResourceType.Texture:
                        request = UnityWebRequestTexture.GetTexture(fullpath);
                        break;
                    case TRResourceType.AssetBundle:
                        request = UnityWebRequest.GetAssetBundle(fullpath);
                        break;
                    case TRResourceType.Audio:
                        AudioType _type = audioType[(System.IO.Path.GetExtension(url)).ToLower()];
                        request = UnityWebRequestMultimedia.GetAudioClip(fullpath, _type);
                        break;
                    case TRResourceType.Movie:
                        request = UnityWebRequestMultimedia.GetMovieTexture(fullpath);
                        break;
                    default:
                        request = UnityWebRequest.Get(url);
                        break;
                }
            }

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                ErrorLogger.Log(request.error);
            }

            isLoading = false;
        }
    }
}
