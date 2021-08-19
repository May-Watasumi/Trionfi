﻿#if !TR_PARSEONLY
 using UnityEngine;
 using UnityEngine.UI;
 using UnityEngine.Networking;
 using DG.Tweening;
#endif

using System.Collections;
using System.Collections.Generic;

//VideoPlayerのローカルのURLに'files:///'は2017.3から不要になったらしい。ややこしい。

namespace Trionfi
{
    public class VideoplayComponent : AbstractComponent
    {
        public VideoplayComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "url",
            };
#endif
        }

        protected override void TagFunction()
        {
//            hasSync = true;
        }

#if !TR_PARSEONLY
        public override IEnumerator TagSyncFunction()
        {
			string storage = tagParam["url"].Literal();
            //            float playDelay = tagParam.Float("delay");
            string url = Application.dataPath;

            if (storage.IndexOf("https://") >= 0 || storage.IndexOf("http://") >= 0)
                url = storage;
            else
#if UNITY_2017_3_OR_NEWER
                url += "/" + storage;
#else
                url = "files:///" + Application.dataPath + " / " + storage;
#endif
            bool loop = tagParam["loop", false];

            if (!loop)
                hasSync = tagParam["wait", true];
            else
                hasSync = false;

            RawImage _image = Trionfi.Instance.layerInstance[TRLayerID.MOVIE].instance;
            Trionfi.Instance.videoPlayer.url = url;
            Trionfi.Instance.videoPlayer.isLooping = loop;
            Trionfi.Instance.videoPlayer.Play();
            _image.enabled = true;

            if (hasSync)
            {
                Trionfi.Instance.CloseAllUI();
                yield return new WaitWhile(() => Trionfi.Instance.videoPlayer.isPlaying);
                _image.enabled = false;
                Trionfi.Instance.OpenAllUI();
            }

            yield return null;
        }
#endif
	}

	//[audiostop type=bgm delay=0]
	public class VideostopComponent : AbstractComponent
    {
        public VideostopComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> { };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			//            float delay = tagParam.Float("delay");
			Trionfi.Instance.videoPlayer.Stop();
            RawImage _image = Trionfi.Instance.layerInstance[TRLayerID.MOVIE].instance;
            _image.enabled = false;
#endif
		}
    }

    //[audiostop type=bgm delay=0]
    public class VideopauseComponent : AbstractComponent
    {
        public VideopauseComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> { };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			//            float delay = tagParam.Float("delay");

			if (Trionfi.Instance.videoPlayer.isPlaying)
                Trionfi.Instance.videoPlayer.Pause();
#endif
		}
    }

    //[audiostop type=bgm delay=0]
    public class VideoresumeComponent : AbstractComponent
    {
        public VideoresumeComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
            };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			//            float delay = tagParam.Float("delay");

			if (Trionfi.Instance.videoPlayer.isPrepared && !Trionfi.Instance.videoPlayer.isPlaying)
                Trionfi.Instance.videoPlayer.Play();
#endif
		}
    }
}
