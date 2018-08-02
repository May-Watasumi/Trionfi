using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public enum TRDataProtocol
    {
        File,
        Network
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
    
    //	[System.Serializable]
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
            { "ogg", AudioType.OGGVORBIS },
        };

        bool isLoading = false;

        public UnityWebRequest request;

        public IEnumerator Load(string url, TRResourceType type, TRDataProtocol protocol = TRDataProtocol.File)
        {
            isLoading = true;

            string fullpath = protocol == TRDataProtocol.Network ? url : "file://" + Application.persistentDataPath + url;

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

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                ErrorLogger.Log(request.error);
            }

            isLoading = false;
        }
    }
}
