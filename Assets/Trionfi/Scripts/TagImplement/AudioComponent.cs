using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

//ToDo:フェード、再生しているストレージ情保存
//クリック待ち、同期

namespace Trionfi
{    public class AudioComponent : AbstractComponent
    {
        public AudioComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "storage",
//                "buf"
            };
#endif
        }

        protected override void TagFunction()
        {
            hasSync = true;
        }

        public override IEnumerator TagSyncFunction()
        {
            int id = tagParam.Int("buf");

            string storage = tagParam.Identifier("storage");
            int playDelaymsec = tagParam.Int("delay");
            int fadeTimemsec = tagParam.Int("time");

            float playDelay = (float)playDelaymsec / 1000.0f;
            float fadeTime = (float)fadeTimemsec / 1000.0f;
            bool loop = tagParam.Bool("loop");

            TRResourceLoader.Instance.Load(storage, TRResourceType.Audio);

            while (TRResourceLoader.Instance.isLoading)
                yield return new WaitForSeconds(1.0f);

            if(TRResourceLoader.Instance.isSuceeded)
            {
                Trionfi.Instance.audioInstance[id].path = storage;
                AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
                AudioClip _clip = TRResourceLoader.Instance.audio;
                _source.clip = _clip;

                _source.volume = 0.0f;

                if (playDelay > 0.0f)
                    yield return new WaitForSeconds(playDelay);

                if (fadeTime > 0.09f)
                {
                    _source.Play();
                    float _vol = TRGameConfig.Instance.configData.mastervolume * TRGameConfig.Instance.configData.bgmvolume;
                    _source.DOFade(_vol, fadeTime);
                }
                else
                {
                    _source.volume = TRGameConfig.Instance.configData.mastervolume * TRGameConfig.Instance.configData.bgmvolume;
                    _source.Play();
                }
            }

            yield return null;
        }
    }

    public class AudiostopComponent : AbstractComponent
    {
        public AudiostopComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "buf"
            };
#endif
        }

        protected override void TagFunction()
        {
            int id = tagParam.Int("buf");

            int fadeTimemsec = tagParam.Int("time");

            float fadeTime = (float)fadeTimemsec / 1000.0f;

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            _source.Stop();
        }
    }

    public class AudiopauseComponent : AbstractComponent
    {
        public AudiopauseComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "buf"
            };
#endif
        }

        protected override void TagFunction()
        {
            int id = tagParam.Int("buf");

            int fadeTimemsec = tagParam.Int("time");

            float fadeTime = (float)fadeTimemsec / 1000.0f;

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            _source.Pause();
        }
    }

    //[audiostop type=bgm delay=0]
    public class AudioresumeComponent : AbstractComponent
    {
        public AudioresumeComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "buf"
            };
#endif
        }

        protected override void TagFunction()
        {
            int id = tagParam.Int("buf");

//            float delay = tagParam.Float("delay");
//            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            _source.UnPause();
        }
    }
}
