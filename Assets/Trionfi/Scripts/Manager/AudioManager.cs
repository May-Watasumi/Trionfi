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
    public class AudioManager : FlipObject<TRAudioObjectBehaviour, TRAudioObjectBehaviour>
    {
        [NonSerialized]
        public GameObject seRoot;
        [NonSerialized]
        public GameObject voiceRoot;
        [NonSerialized]
        public GameObject BGMRoot;

//        public static Dictionary<string, TRAudioObjectBehaviour> dicBgm = new Dictionary<string, TRAudioObjectBehaviour>();
//        public static Dictionary<string, TRAudioObjectBehaviour> dicSound = new Dictionary<string, TRAudioObjectBehaviour>();
		public static Dictionary<string, TRAudioObjectBehaviour> dicVoice = new Dictionary<string, TRAudioObjectBehaviour>();

		public override TRAudioObjectBehaviour Create(string name, TRObjectType type)
		{
            TRAudioObjectBehaviour g = null;
            switch(type)
            {
            case TRObjectType.BGM:
                g = currentInstance;
                currentInstance.Load(name);
                break;
            case TRObjectType.SE:
            case TRObjectType.Voice:
                g = GameObject.Instantiate(objectPrefab).GetComponent<TRAudioObjectBehaviour>();
                g.GetComponent<TRAudioObjectBehaviour>().Load(name);
                g.transform.parent = seRoot.transform;
                dicObject[name] = (g.GetComponent<TRAudioObjectBehaviour>());
                break;
            }
               
            return g;
		}
        /*
		private Dictionary<string, TRAudioObjectBehaviour> GetDic(ObjectType type)
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
        public TRAudioObjectBehaviour Find(string name, TRObjectType type)
        {
            switch (type)
            {
                case TRObjectType.BGM:
                    if( currentInstance.gameObject.name != name)
                        currentInstance.Load(name);
                    return currentInstance;

                case TRObjectType.SE:
                case TRObjectType.Voice:
                    if (!dicObject.ContainsKey(name))
                        Create(name, type);
                    return dicObject[name];
            }
            return null;           
		}

		public void Stop(string file, TRObjectType type,float time, CompleteDelegate completeDelegate = null)
        {
			//全部停止する
			if(string.IsNullOrEmpty(file))
            {
                currentInstance.Stop(time);

				foreach (KeyValuePair<string, TRAudioObjectBehaviour> kvp in dicObject)
                {
					kvp.Value.Stop(time);
				}
                foreach (KeyValuePair<string, TRAudioObjectBehaviour> kvp in dicVoice)
                {
                    kvp.Value.Stop(time);
                }
            }
            else
            {
                TRAudioObjectBehaviour audioObject = Find(file, type);
//				audioObject.time = time;
//				audioObject.completeDelegate = completeDelegate;
                if(audioObject != null)
    				audioObject.Stop(time);
			}
		}
    }
}
