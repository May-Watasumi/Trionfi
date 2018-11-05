using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;
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

        public override IEnumerator TagSyncFunction()
        {
            string storage = tagParam["url"].Literal();
            //            float playDelay = tagParam.Float("delay");
            string url = Application.dataPath;

            if (storage.IndexOf("https://") >= 0 || storage.IndexOf("http://") >= 0)
                url = storage;
            else
                url += "/" + storage;

            bool loop = tagParam["loop", false];

            if (!loop)
                hasSync = tagParam["wait", true];
            else
                hasSync = false;

            RawImage _image = Trionfi.Instance.layerInstance[99].instance;
            Trionfi.Instance.videoPlayer.url = url;
            Trionfi.Instance.videoPlayer.isLooping = loop;
            Trionfi.Instance.videoPlayer.Play();
            _image.enabled = true;

            if (hasSync)
            {
                Trionfi.Instance.CloseWindow();
                yield return new WaitWhile(() => Trionfi.Instance.videoPlayer.isPlaying);
                _image.enabled = false;
                Trionfi.Instance.ReactiveWindow();
            }

            yield return null;
        }
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
//            float delay = tagParam.Float("delay");
            Trionfi.Instance.videoPlayer.Stop();
            RawImage _image = Trionfi.Instance.layerInstance[99].instance;
            _image.enabled = false;
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
//            float delay = tagParam.Float("delay");

            if(Trionfi.Instance.videoPlayer.isPlaying)
                Trionfi.Instance.videoPlayer.Pause();
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
//            float delay = tagParam.Float("delay");

            if (Trionfi.Instance.videoPlayer.isPrepared && !Trionfi.Instance.videoPlayer.isPlaying)
                Trionfi.Instance.videoPlayer.Play();
        }
    }
}
