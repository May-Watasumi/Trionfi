using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

//Audio活動を管理する
namespace NovelEx
{
    [Serializable]
    public class TRSoundObjectManager : FlipObject<TRSoundObjectBehaviour, TRSoundObjectBehaviour>
    {
        [NonSerialized]
        public GameObject seRoot;
        [NonSerialized]
        public GameObject voiceRoot;
        [NonSerialized]
        public GameObject BGMRoot;

//        public static Dictionary<string, TRSoundObjectBehaviour> dicBgm = new Dictionary<string, TRSoundObjectBehaviour>();
//        public static Dictionary<string, TRSoundObjectBehaviour> dicSound = new Dictionary<string, TRSoundObjectBehaviour>();
		public static Dictionary<string, TRSoundObjectBehaviour> dicVoice = new Dictionary<string, TRSoundObjectBehaviour>();

		public override TRSoundObjectBehaviour Create(string name, TRDataType type)
		{
            TRSoundObjectBehaviour g = null;
            switch(type)
            {
            case TRDataType.BGM:
                g = currentInstance;
                currentInstance.Load(name);
                break;
            case TRDataType.SE:
            case TRDataType.Voice:
                g = GameObject.Instantiate(objectPrefab).GetComponent<TRSoundObjectBehaviour>();
                g.GetComponent<TRSoundObjectBehaviour>().Load(name);
                g.transform.parent = seRoot.transform;
                dicObject[name] = (g.GetComponent<TRSoundObjectBehaviour>());
                break;
            }
               
            return g;
		}
        /*
		private Dictionary<string, TRSoundObjectBehaviour> GetDic(ObjectType type)
        {		
			switch (type) {
			case ObjectType.Bgm:
//				return currentInstance;
			case ObjectType.Sound:
				return dicSound;
			}

			return null;		
		}
        */
        public TRSoundObjectBehaviour Find(string name, TRDataType type)
        {
            switch (type)
            {
                case TRDataType.BGM:
                    if( currentInstance.gameObject.name != name)
                        currentInstance.Load(name);
                    return currentInstance;

                case TRDataType.SE:
                case TRDataType.Voice:
                    if (!dicObject.ContainsKey(name))
                        Create(name, type);
                    return dicObject[name];
            }
            return null;           
		}

		public void Stop(string file, TRDataType type,float time, CompleteDelegate completeDelegate = null)
        {
			//全部停止する
			if(string.IsNullOrEmpty(file))
            {
                currentInstance.Stop(time);

				foreach (KeyValuePair<string, TRSoundObjectBehaviour> kvp in dicObject)
                {
					kvp.Value.Stop(time);
				}
                foreach (KeyValuePair<string, TRSoundObjectBehaviour> kvp in dicVoice)
                {
                    kvp.Value.Stop(time);
                }
            }
            else
            {
                TRSoundObjectBehaviour audioObject = Find(file, type);
//				audioObject.time = time;
//				audioObject.completeDelegate = completeDelegate;
                if(audioObject != null)
    				audioObject.Stop(time);
			}
		}
    }
}
