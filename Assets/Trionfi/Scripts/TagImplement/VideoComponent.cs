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
        bool isWait;

        public VideoplayComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "storage",
            };
        }

        protected override void TagFunction()
        {
            hasSync = true;
        }

        protected override IEnumerator TagSyncFunction()
        {
            string storage = tagParam.Identifier("storage");
            float playDelay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            bool loop = tagParam.Bool("loop");

            Trionfi.Instance.videoPlayer.url = storage;
            Trionfi.Instance.videoPlayer.Play();

            yield return null;
        }
    }

    //[audiostop type=bgm delay=0]
    public class VideostopComponent : AbstractComponent
    {
        public VideostopComponent()
        {
            //必須項目
            essentialParams = new List<string> {
            };
        }

        protected override void TagFunction()
        {
            float delay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");
            Trionfi.Instance.videoPlayer.Stop();
        }
    }

    //[audiostop type=bgm delay=0]
    public class VideopauseComponent : AbstractComponent
    {
        public VideopauseComponent()
        {
            //必須項目
            essentialParams = new List<string> {
            };
        }

        protected override void TagFunction()
        {
            float delay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            if(Trionfi.Instance.videoPlayer.isPlaying)
                Trionfi.Instance.videoPlayer.Pause();
        }
    }

    //[audiostop type=bgm delay=0]
    public class VideoresumeComponent : AbstractComponent
    {
        public VideoresumeComponent()
        {
            //必須項目
            essentialParams = new List<string> {
            };
        }

        protected override void TagFunction()
        {
            float delay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            if (Trionfi.Instance.videoPlayer.isPrepared && !Trionfi.Instance.videoPlayer.isPlaying)
                Trionfi.Instance.videoPlayer.Play();
        }
    }
}
