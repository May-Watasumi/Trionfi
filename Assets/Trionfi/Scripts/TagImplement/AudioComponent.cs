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
                "buf"
            };
        }

        protected override void TagFunction()
        {
            hasSync = true;
        }

        protected override IEnumerator TagSyncFunction()
        {
            int id = tagParam.Int("buf");

            string storage = tagParam.Identifier("storage");
            float playDelay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            bool loop = false;

            if (!tagParam.IsValid(ref loop, "loop"))
            {
                loop = id == 0 ? true : false;
            }

            TRResourceLoader.Instance.Load(storage, TRResourceType.Audio);

            while (TRResourceLoader.Instance.isLoading)
                yield return new WaitForSeconds(1.0f);

            if(TRResourceLoader.Instance.isSuceeded)
            {
                Trionfi.Instance.audioInstance[id].path = storage;
                AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
                AudioClip _clip = TRResourceLoader.Instance.audio;
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
                "buf"
            };
        }

        protected override void TagFunction()
        {
            int id = tagParam.Int("buf");

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
                "buf"
            };
        }

        protected override void TagFunction()
        {
            int id = tagParam.Int("buf");

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
                "buf"
            };
        }

        protected override void TagFunction()
        {
            int id = tagParam.Int("buf");

            float delay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            _source.UnPause();
        }
    }
}
