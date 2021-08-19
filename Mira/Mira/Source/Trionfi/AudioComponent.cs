﻿#if !TR_PARSEONLY
 using UnityEngine;
 using UnityEngine.Networking;
 using DG.Tweening;
#endif

using System.Collections;
using System.Collections.Generic;
using System.IO;

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

#if !TR_PARSEONLY
		public override IEnumerator TagSyncFunction()
        {
            TRAudioID id = (TRAudioID)tagParam["buf", 0];
            string storage = tagParam["storage"].Literal();
            int playDelaymsec = tagParam["delay", 0];
            int fadeTimemsec = tagParam["time", 0];

            float playDelay = (float)playDelaymsec / 1000.0f;
            float fadeTime = (float)fadeTimemsec / 1000.0f;
            bool loop = tagParam["loop", false];

            yield return TRResourceLoader.Instance.LoadAudio(storage);

            //            while (TRResourceLoader.Instance.isLoading)
            //                yield return new WaitForSeconds(1.0f);

            //            if (TRResourceLoader.Instance.isSuceeded)

            if (!string.IsNullOrEmpty(storage))
            {
                TRResourceType type = GetResourceType();

                var coroutine = TRResourceLoader.Instance.LoadAudio(storage, type);

                yield return TRResourceLoader.Instance.StartCoroutine(coroutine);

                AudioClip _clip = (AudioClip)coroutine.Current;

                if (_clip != null)
                {
                    Trionfi.Instance.audioInstance[id].path = storage;
                    AudioSource _source = Trionfi.Instance.audioInstance[id].instance;

                    _source.clip = _clip;

                    _source.volume = 0.0f;

                    if (playDelay > 0.0f)
                        yield return new WaitForSeconds(playDelay);

                    if (fadeTime > 0.09f)
                    {
                        _source.Play();
                        float _vol = TRGameConfig.configData.mastervolume * TRGameConfig.configData.bgmvolume;
                        _source.DOFade(_vol, fadeTime);
                    }
                    else
                    {
                        _source.volume = TRGameConfig.configData.mastervolume * TRGameConfig.configData.bgmvolume;
                        _source.Play();
                    }
                }
            }

            yield return null;
        }
#endif
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
#if !TR_PARSEONLY
			TRAudioID id = (TRAudioID)tagParam["buf", 0];

            int fadeTimemsec = tagParam["time", 0];

            float fadeTime = (float)fadeTimemsec / 1000.0f;

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            _source.Stop();
#endif
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
#if !TR_PARSEONLY
			TRAudioID id = (TRAudioID)tagParam["buf", 0];

            int fadeTimemsec = tagParam["time", 0];

            float fadeTime = (float)fadeTimemsec / 1000.0f;

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            _source.Pause();
#endif
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
#if !TR_PARSEONLY
			TRAudioID id = (TRAudioID)tagParam["buf", 0];

            //            float delay = tagParam.Float("delay");
            //            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.audioInstance[id].instance;
            _source.UnPause();
#endif
		}
    }

#if TR_USE_CRI
    public class AdxloadacfComponent : AbstractComponent
    {
        public AdxloadacfComponent()
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
#if !TR_PARSEONLY
            TRAdx.LoadAcf( Path.Combine(TRAdx.basePath, tagParam["storage"].Literal()));
#endif
		}
    }

    public class AdxloadacbComponent : AbstractComponent
    {
        CriAtomExAcbLoader acbLoader;

        public AdxloadacbComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "storage",
                "buf"
            };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
            hasSync = true;

            string _path = tagParam["storage", string.Empty];
            acbLoader = CriAtomExAcbLoader.LoadAcbFileAsync(null, Path.Combine(TRAdx.basePath, _path + ".acb"), Path.Combine(_path, ".awb"), false);
#endif
        }

#if !TR_PARSEONLY
        public override IEnumerator TagSyncFunction()
        {
            if (acbLoader != null)
            {
                CriAtomExAcbLoader.Status status;

                do
                {
                    yield return new WaitForEndOfFrame();

                    status = acbLoader.GetStatus();
                    if (status == CriAtomExAcbLoader.Status.Complete)
                    {
                        int buf = tagParam["buf", 0];
                        Trionfi.Instance.adxInstance[buf].acb = acbLoader.MoveAcb();
                        break;
                    }
                    else if (status == CriAtomExAcbLoader.Status.Error)
                    {
                        break;
                    }

                } while (true);
            }
            yield return null;
        }
#endif
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

#if !TR_PARSEONLY
        public override IEnumerator TagSyncFunction()
        {
            int id = tagParam["buf", 0];
            string storage = tagParam["storage"].Literal();
//            string cueSheet = tagParam["cuesheet"].Literal();
            int playDelaymsec = tagParam["delay", 0];
            int fadeTimemsec = tagParam["time", 0];

            float playDelay = (float)playDelaymsec / 1000.0f;
            float fadeTime = (float)fadeTimemsec / 1000.0f;

            bool loop = tagParam["loop", false];

            if (!tagParam.ContainsKey("buf") && !tagParam.ContainsKey("buf"))
                loop = true;

            {
                CriAtomExPlayer _atom = Trionfi.Instance.adxInstance[id].instance;
                if (_atom != null)
                {
                    Trionfi.Instance.audioInstance[id].path = storage;
                    _atom.Loop(loop);
                    _atom.SetCue(Trionfi.Instance.adxInstance[id].acb, storage);
                    _atom.SetVolume(0.0f);

                    if (playDelay > 0.0f)
                    {
                        _atom.Prepare();
                        yield return new WaitForSeconds(playDelay);
                        _atom.Resume(CriAtomEx.ResumeMode.PreparedPlayback);
                    }

                    //if (fadeTime > 0.09f)
                    //{
                    //    _atom.Play();
                    //    float _vol = TRGameConfig.Instance.configData.mastervolume * TRGameConfig.Instance.configData.bgmvolume;
                    //    _source.DOFade(_vol, fadeTime);
                    //}
                    else
                    {
                        _atom.SetVolume(TRGameConfig.Instance.configData.mastervolume * TRGameConfig.Instance.configData.bgmvolume);
                        _atom.Start();
                    }
                }
                yield return null;
            }
        }
#endif
    }
    
    public class AdxstopComponent : AbstractComponent
    {
        public AdxstopComponent()
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
#if !TR_PARSEONLY
            int id = tagParam["buf", 0];

            int fadeTimemsec = tagParam["time", 0];

            float fadeTime = (float)fadeTimemsec / 1000.0f;

            CriAtomExPlayer _atom = Trionfi.Instance.adxInstance[id].instance;
            if (_atom != null)
                _atom.Stop();
#endif
		}
    }

    public class AdxpauseComponent : AbstractComponent
    {
        public AdxpauseComponent()
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
#if !TR_PARSEONLY
            int id = tagParam["buf", 0];

            int fadeTimemsec = tagParam["time", 0];

            float fadeTime = (float)fadeTimemsec / 1000.0f;

            CriAtomExPlayer _atom = Trionfi.Instance.adxInstance[id].instance;
            _atom.Pause();
#endif
        }
    }

    //[audiostop type=bgm delay=0]
    public class AdxresumeComponent : AbstractComponent
    {
        public AdxresumeComponent()
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
#if !TR_PARSEONLY
            int id = tagParam["buf", 0];

            //            float delay = tagParam.Float("delay");
            //            float fadeTime = tagParam.Float("time");

            CriAtomExPlayer _atom = Trionfi.Instance.adxInstance[id].instance;
            _atom.Resume(CriAtomEx.ResumeMode.PausedPlayback);
#endif
		}
    }
#endif
}
