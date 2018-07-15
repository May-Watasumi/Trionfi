using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//ToDo:フェード、再生しているストレージ情保存
//クリック待ち、同期

namespace Trionfi
{
    //[audioplay type=bgm storage=ggg name=ggg delay=0]
    public class AudioplayComponent : AbstractComponent
    {
        bool isWait;

        public AudioplayComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "storage",
                "type"
            };
        }

        protected override void TagFunction()
        {
            TRDataType _type = tagParam.Type();

            string storage = tagParam["storage"];
            float playDelay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            bool loop = false;

            if(!tagParam.IsValid(ref loop, "loop"))
            {
                loop = _type == TRDataType.BGM ? true : false;
            }

            AudioSource _source = Trionfi.Instance.GetAudio(_type);
            AudioClip _clip = StorageManager.Instance.LoadObject(storage, _type) as AudioClip;
            _source.clip = _clip;

            if (playDelay > 0.1f)
                _source.PlayDelayed(playDelay);
            else
                _source.Play();
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
            float delay = tagParam.Float("delay");
            TRDataType _type = tagParam.Type();
            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.GetAudio(_type);
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
            float delay = tagParam.Float("delay");
            TRDataType _type = tagParam.Type();
            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.GetAudio(_type);
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
            float delay = tagParam.Float("delay");
            TRDataType _type = tagParam.Type();
            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.GetAudio(_type);
            _source.UnPause();
        }
    }
}
