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
                "id"
            };
        }

        protected override void TagFunction()
        {
            int id = tagParam.Int("id");

            string storage = tagParam.Identifier("storage");
            float playDelay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            bool loop = false;

            if(!tagParam.IsValid(ref loop, "loop"))
            {
                loop = id == 0 ? true : false;
            }

            AudioSource _source = Trionfi.Instance.GetAudio(TRAssetType.BGM, id);
            AudioClip _clip = TRResourceLoader.Instance.LoadObject(storage, TRAssetType.BGM) as AudioClip;
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
            TRAssetType _type = TRAssetType.BGM;

            float delay = tagParam.Float("delay");
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
            TRAssetType _type = TRAssetType.BGM;

            float delay = tagParam.Float("delay");
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
            TRAssetType _type = TRAssetType.BGM;

            float delay = tagParam.Float("delay");
            float fadeTime = tagParam.Float("time");

            AudioSource _source = Trionfi.Instance.GetAudio(_type);
            _source.UnPause();
        }
    }
}
