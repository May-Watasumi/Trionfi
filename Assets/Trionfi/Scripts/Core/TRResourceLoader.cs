using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public enum TRAssetType
    {
        None,
        TextAsset,
        JsonText,
        BinaryData,
        Character,
        BG,
        Event,
        UI,
        Live2D,
        FBX,
        Emote,
        BGM,
        SE,
        Voice,
        Terminate,
    }

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
        readonly Dictionary<string, AudioType> type = new Dictionary<string, AudioType>()
        {
            { "wav", AudioType.WAV },
            { "ogg", AudioType.OGGVORBIS },
        };


        bool isLoading = false;

        UnityWebRequest request;

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
                    //ToDo:
                    request = UnityWebRequestMultimedia.GetAudioClip(fullpath, AudioType.OGGVORBIS);
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
