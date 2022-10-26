#if !TR_PARSEONLY
 using UnityEngine;
 using UnityEngine.UI;
 using UnityEngine.Networking;
 using DG.Tweening;
 using Cysharp.Threading.Tasks;
using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;

#else
using TRTask = System.Threading.Tasks.Task;
using TRTaskString = System.Threading.Tasks.Task<string>;
#endif

using System;
using System.Collections.Generic;

namespace Trionfi
{
    [Serializable]
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            //            hasSync = true;

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

            RawImage _image = Trionfi.Instance.movieTexture;
            Trionfi.Instance.videoPlayer.url = url;
            Trionfi.Instance.videoPlayer.isLooping = loop;

            if (Trionfi.Instance.nowLoading != null)
                Trionfi.Instance.nowLoading.gameObject.SetActive(true);

            Trionfi.Instance.videoPlayer.Prepare();

            await UniTask.WaitWhile(() => !Trionfi.Instance.videoPlayer.isPrepared);

            if (Trionfi.Instance.nowLoading != null)
                Trionfi.Instance.nowLoading.gameObject.SetActive(false);

            Trionfi.Instance.videoPlayer.Play();

            // ２回目以後の動画を再生する時、実際再生開始するまでは直前の最後のフレームが表示されるので、
            // 実際動画が再生するのを確認したら動画表示を開始する
            await UniTask.WaitWhile(() => Trionfi.Instance.videoPlayer.frame < 1);

            _image.enabled = true;

            if (tagParam["skip"].Bool())
            {
                hasSync = true;
                Trionfi.Instance.ClickEvent += StopFunc;
            }

            if (hasSync)
            {
                //Trionfi.Instance.CloseAllUI();
                await UniTask.WaitWhile(() => Trionfi.Instance.videoPlayer.isPlaying);
                _image.enabled = false;
                //Trionfi.Instance.OpenAllUI();
            }
#endif
            return string.Empty;

        }

        static public void StopFunc()
		{
#if !TR_PARSEONLY
            Trionfi.Instance.ClickEvent -= StopFunc;

            if (Trionfi.Instance.videoPlayer.isPlaying || Trionfi.Instance.videoPlayer.isPaused)
                Trionfi.Instance.videoPlayer.Stop();

            RawImage _image = Trionfi.Instance.movieTexture;
            _image.enabled = false;
#endif
        }
    }

    //[audiostop type=bgm delay=0]
    [Serializable]
    public class VideostopComponent : AbstractComponent
    {
        public VideostopComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> { };
#endif
        }

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            VideoplayComponent.StopFunc();
#endif
            return string.Empty;
        }
    }

    //[audiostop type=bgm delay=0]
    [Serializable]
    public class VideopauseComponent : AbstractComponent
    {
        public VideopauseComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> { };
#endif
        }

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            //            float delay = tagParam.Float("delay");

            if (Trionfi.Instance.videoPlayer.isPlaying)
                Trionfi.Instance.videoPlayer.Pause();

#endif
            return string.Empty;
        }
    }

    //[audiostop type=bgm delay=0]
    [Serializable]
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            //            float delay = tagParam.Float("delay");

            if (Trionfi.Instance.videoPlayer.isPrepared && !Trionfi.Instance.videoPlayer.isPlaying)
                Trionfi.Instance.videoPlayer.Play();
#endif
            return string.Empty;
		}
    }
}
