using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

//ToDo:フェード、再生しているストレージ情保存
//クリック待ち、同期

namespace Trionfi
{    public class AudioplayComponent : AbstractComponent
    {
        bool isWait;

        public AudioplayComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "storage",
                "id"
            };
        }

        protected override void TagFunction()
        {
            hasSync = true;
        }

        protected override IEnumerator TagSyncFunction()
        {
            int id = tagParam.Int("id");

            string storage = tagParam.Identifier("storage");
            float playDelay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            bool loop = false;

            if (!tagParam.IsValid(ref loop, "loop"))
            {
                loop = id == 0 ? true : false;
            }

            yield return TRResourceLoader.Instance.Load(storage, TRResourceType.Audio);

            if (!TRResourceLoader.Instance.request.isHttpError && !TRResourceLoader.Instance.request.isNetworkError)
            {
                AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
                AudioClip _clip = DownloadHandlerAudioClip.GetContent(TRResourceLoader.Instance.request);
                _source.clip = _clip;

                if (playDelay > 0.1f)
                    _source.PlayDelayed(playDelay);
                else
                    _source.Play();

                if (fadeTime > 0.1f)
                {
                    //ToDo:
                    float _vol = TRGameConfig.Instance.configData.mastervolume * TRGameConfig.Instance.configData.bgmvolume;
                    _source.DOFade(_vol, fadeTime);
                }
            }

            yield return null;
        }
    }

    //[audiostop type=bgm delay=0]
    public class AudiostopComponent : AbstractComponent
    {
        public AudiostopComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "type",
            };
        }

        protected override void TagFunction()
        {
            int id = tagParam.Int("id");

            float delay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            _source.Stop();
        }
    }

    //[audiostop type=bgm delay=0]
    public class AudiopauseComponent : AbstractComponent
    {
        public AudiopauseComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "type",
            };
        }

        protected override void TagFunction()
        {
            int id = tagParam.Int("id");

            float delay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            _source.Pause();
        }
    }

    //[audiostop type=bgm delay=0]
    public class AudioresumeComponent : AbstractComponent
    {
        public AudioresumeComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "type",
            };
        }

        protected override void TagFunction()
        {
            int id = tagParam.Int("id");

            float delay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            _source.UnPause();
        }
    }
}
