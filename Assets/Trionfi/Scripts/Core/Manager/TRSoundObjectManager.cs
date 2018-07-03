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
		public Dictionary<string, TRSoundObjectBehaviour> dicVoice = new Dictionary<string, TRSoundObjectBehaviour>();

		public override TRSoundObjectBehaviour Create(string name, TRDataType type)
		{
            TRSoundObjectBehaviour g = null;
            switch(type)
            {
            case TRDataType.BGM:
                g = currentInstance;
                g.gameObject.name = name;
                break;
            case TRDataType.SE:
            case TRDataType.Voice:
                if(dicObject.ContainsKey(name))
                     return null;

                g = GameObject.Instantiate(objectPrefab).GetComponent<TRSoundObjectBehaviour>();
                g.transform.parent = seRoot.transform;
                dicObject[name] = (g.GetComponent<TRSoundObjectBehaviour>());
                break;
            }               
            return g;
		}

        public override TRSoundObjectBehaviour Find(string name, TRDataType type)
        {
            switch (type)
            {
                case TRDataType.BGM:
                    return currentInstance;

                case TRDataType.SE:
                case TRDataType.Voice:
                    return dicObject.ContainsKey(name) ? dicObject[name] : null;
            }
            return null;           
		}

		public void Stop(string file, TRDataType type, float time)
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

                if (audioObject != null)
    				audioObject.Stop(time);
			}
		}
    }
}
