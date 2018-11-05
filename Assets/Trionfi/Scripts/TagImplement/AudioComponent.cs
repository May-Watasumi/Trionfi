using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

//ToDo:フェード、再生しているストレージ情保存
//クリック待ち、同期

namespace Trionfi
{
    public class AudioComponent : AbstractComponent
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
            int id = tagParam["buf", 0];
            string storage = tagParam["storage"].Literal();
            int playDelaymsec = tagParam["delay", 0];
            int fadeTimemsec = tagParam["time", 0];

            float playDelay = (float)playDelaymsec / 1000.0f;
            float fadeTime = (float)fadeTimemsec / 1000.0f;
            bool loop = tagParam["loop", false];

            TRResourceLoader.Instance.Load(storage, TRResourceType.Audio);

            while (TRResourceLoader.Instance.isLoading)
                yield return new WaitForSeconds(1.0f);

            if (TRResourceLoader.Instance.isSuceeded)
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
            essentialParams = new List<string>
            {
                //                "buf"
            };
#endif
        }

        protected override void TagFunction()
        {
            int id = tagParam["buf", 0];

            int fadeTimemsec = tagParam["time", 0];

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
            int id = tagParam["buf", 0];

            int fadeTimemsec = tagParam["time", 0];

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
            int id = tagParam["buf", 0];

            //            float delay = tagParam.Float("delay");
            //            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            _source.UnPause();
        }
    }

#if TR_USE_CRI
    public class AdxcuesheetComponent : AbstractComponent
    {
        public AdxcuesheetComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "storage"
            };
#endif
        }

        protected override void TagFunction()
        {
            TRAudio.curSheetName = tagParam["stprage", string.Empty];
        }
    }

    public class AdxComponent : AbstractComponent
    {
        public AdxComponent()
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
            int id = tagParam["buf", 0];
            string storage = tagParam["storage"].Literal();
            string cueSheet = tagParam["cuesheet", TRAudio.curSheetName];
            int playDelaymsec = tagParam["delay", 0];
            int fadeTimemsec = tagParam["time", 0];

            float playDelay = (float)playDelaymsec / 1000.0f;
            float fadeTime = (float)fadeTimemsec / 1000.0f;
            bool loop = tagParam["loop", false];

            //            TRResourceLoader.Instance.Load(storage, TRResourceType.Audio);
            //            while (TRResourceLoader.Instance.isLoading)
            //                yield return new WaitForSeconds(1.0f);

            //            if (TRResourceLoader.Instance.isSuceeded)
            {
                AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
                CriAtomSource _atom = _source.gameObject.GetComponent<CriAtomSource>();
                if (_atom != null)
                {
                    Trionfi.Instance.audioInstance[id].path = storage;
                    //                AudioClip _clip = TRResourceLoader.Instance.audio;
                    //_source.clip = _clip;
                    //_source.volume = 0.0f;
                    _atom.cueName = storage;
                    _atom.cueSheet = cueSheet;
                    _atom.loop = loop;
                    _atom.volume = 0.0f;

                    if (playDelay > 0.0f)
                        yield return new WaitForSeconds(playDelay);

                    //if (fadeTime > 0.09f)
                    //{
                    //    _atom.Play();
                    //    float _vol = TRGameConfig.Instance.configData.mastervolume * TRGameConfig.Instance.configData.bgmvolume;
                    //    _source.DOFade(_vol, fadeTime);
                    //}
                    //else
                    {
                        _source.volume = TRGameConfig.Instance.configData.mastervolume * TRGameConfig.Instance.configData.bgmvolume;
                        _source.Play();
                    }
                }
                yield return null;
            }
        }
    }

    public class adxstopComponent : AbstractComponent
    {
        public adxstopComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string>
            {
                //                "buf"
            };
#endif
        }

        protected override void TagFunction()
        {
            int id = tagParam["buf", 0];

            int fadeTimemsec = tagParam["time", 0];

            float fadeTime = (float)fadeTimemsec / 1000.0f;

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            CriAtomSource _atom = _source.gameObject.GetComponent<CriAtomSource>();
            if (_atom != null)
                _atom.Stop();
        }
    }

    public class adxpauseComponent : AbstractComponent
    {
        public adxpauseComponent()
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
            int id = tagParam["buf", 0];

            int fadeTimemsec = tagParam["time", 0];

            float fadeTime = (float)fadeTimemsec / 1000.0f;

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            CriAtomSource _atom = _source.gameObject.GetComponent<CriAtomSource>();
            _atom.Pause(true);
        }
    }

    //[audiostop type=bgm delay=0]
    public class adxresumeComponent : AbstractComponent
    {
        public adxresumeComponent()
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
            int id = tagParam["buf", 0];

            //            float delay = tagParam.Float("delay");
            //            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            CriAtomSource _atom = _source.gameObject.GetComponent<CriAtomSource>();
            _atom.Pause(false);
//            _source.UnPause();
        }
    }
#endif
}
