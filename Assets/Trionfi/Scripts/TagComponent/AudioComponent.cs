using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
    //    [sound type=bgm storage=ggg name=ggg delay=0]
    public class SoundComponent : AbstractComponent
    {
        bool isWait;

        public SoundComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "storage",
                "type"
            };
/*
            originalParamDic = new ParamDictionary() {
                { "storage",""},
                { "time","0"},
                { "vol","1"}, //ボリューム 0〜1
				{ "wait","true"},
                { "next","true"},
            };
*/
        }

        protected override void TagFunction()
        {
            string storage = expressionedParams["storage"];
            string name = expressionedParams.String("name", null);
            float playDelay = expressionedParams.Float("delay");
            TRDataType _type = expressionedParams.Type();

            bool loop = false;

            if(!expressionedParams.IsValid(ref loop, "loop"))
            {
                loop = _type == TRDataType.BGM ? true : false;
            }

            TRSoundObjectBehaviour audioObject = TRSoundObjectManager.Instance.Create(expressionedParams["name"], _type).GetComponent<TRSoundObjectBehaviour>();

            if (audioObject != null)
            {
                audioObject.Load(storage, _type, name);
                //ToDo:同期待ちするときある？
                //yield return audioObject.Play(playDelay);
            }
            else
            {
                ErrorLogger.Log("Failed:SoundComponent");
            }
        }
    }

    //    [soundstop type=bgm name="" delay=0 delete=true]
    public class SoundstopComponent : AbstractComponent
    {
        public SoundstopComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "name",
                "type",
//                "delay",
            };
            /*
                        originalParamDic = new ParamDictionary() {
                            { "storage",""},
                            { "time","0"},
                            { "vol","1"}, //ボリューム 0〜1
                            { "wait","true"},
                            { "next","true"},
                        };
            */
        }

        protected override void TagFunction()
        {
            string name = expressionedParams.String("name", null);
            float delay = expressionedParams.Float("delay");
            TRDataType _type = expressionedParams.Type();

            TRSoundObjectBehaviour audioObject = TRSoundObjectManager.Instance.Find(name, _type);

//            if (audioObject != null)
//                yield return audioObject.Stop(delay);
//            else
//                ErrorLogger.Log("Failed:SoundstopComponent");
        }
    }

    //    [sounddelete type=bgm name=""]
    public class SounddeleteComponent : AbstractComponent
    {
        public SounddeleteComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "name",
                "type",
            };
            /*
                        originalParamDic = new ParamDictionary() {
                            { "storage",""},
                            { "time","0"},
                            { "vol","1"}, //ボリューム 0〜1
                            { "wait","true"},
                            { "next","true"},
                        };
            */
        }

        protected override void TagFunction()
        {
            string name = expressionedParams.String("name", null);
            TRDataType _type = expressionedParams.Type();

            TRSoundObjectBehaviour audioObject = TRSoundObjectManager.Instance.Find(name, _type);
            if (audioObject != null)
            {
//                yield return audioObject.Stop();
                GameObject.Destroy(audioObject.gameObject);
                TRSoundObjectManager.Instance.dicObject.Remove(name);
            }
            else
                ErrorLogger.Log("Failed:SounddeleteComponent");
        }
    }
}
