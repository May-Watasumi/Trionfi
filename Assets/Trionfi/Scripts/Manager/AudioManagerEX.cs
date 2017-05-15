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
    public class AudioManager
	{
	    public static Dictionary<string, AudioObject> dicBgm = new Dictionary<string,AudioObject>();
		public static Dictionary<string, AudioObject> dicSound = new Dictionary<string,AudioObject>();
		public static Dictionary<string, AudioObject> dicVoice = new Dictionary<string, AudioObject>();

		public static void AddAudio(string file, AudioType audioType)
		{
			GameObject g = new GameObject();

			AudioObject audioObject = g.AddComponent<AudioObject>();
			audioObject.Load(file);

			GetDic(audioType)[file] = audioObject;
		}

		private static Dictionary<string,AudioObject> GetDic(AudioType audioType)
        {		
			switch (audioType) {
			case AudioType.Bgm:
				return dicBgm;
			case AudioType.Sound:
				return dicSound;
			}

			return null;		
		}

        public static AudioObject GetAudio(string file, AudioType audioType)
        {
			Dictionary<string,AudioObject> dic = GetDic (audioType);

			if(!dic.ContainsKey (file))
            {
				AddAudio (file, audioType);
				return GetAudio (file, audioType);
			}
            else
            {
				return dic[file];
			}
		}

		public static void StopAudio(string file,AudioType audioType,float time, CompleteDelegate completeDelegate = null)
        {
			//全部停止する
			if (file == "")
            {
				Dictionary<string,AudioObject> dic = GetDic(audioType);
				foreach (KeyValuePair<string, AudioObject> kvp in dic)
                {
					string key = kvp.Key;

//					dic[key].time = time;
					dic[key].completeDelegate = completeDelegate;
					dic[key].Stop();
				}
			}
            else
            {
				AudioObject audioObject = GetAudio(file,audioType);
//				audioObject.time = time;
				audioObject.completeDelegate = completeDelegate;
				audioObject.Stop();
			}
		}
	}
}
