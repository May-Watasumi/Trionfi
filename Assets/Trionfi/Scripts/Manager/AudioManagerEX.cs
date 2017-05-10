using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NovelEx;

//Audio活動を管理する
namespace NovelEx
{
	public class AudioManager
	{
		public Dictionary<string,AudioObject> dicBgm = new Dictionary<string,AudioObject>();
		public Dictionary<string,AudioObject> dicSound = new Dictionary<string,AudioObject>();
		public Dictionary<string, AudioObject> dicVoice = new Dictionary<string, AudioObject>();

		public void addAudio(string file,AudioType audioType)
		{
			GameObject g = new GameObject();

			AudioObject audioObject = g.AddComponent<AudioObject>();
			audioObject.Load(file);

			this.getDic (audioType)[file] = audioObject;

		}

		private Dictionary<string,AudioObject> getDic(AudioType audioType)
        {		
			switch (audioType) {
			case AudioType.Bgm:
				return dicBgm;
			case AudioType.Sound:
				return dicSound;
			}

			return null;
		
		}

		public AudioObject getAudio(string file, AudioType audioType)
        {
			Dictionary<string,AudioObject> dic = this.getDic (audioType);

			if (!dic.ContainsKey (file)) {
				this.addAudio (file, audioType);
				return this.getAudio (file, audioType);
			}
            else
            {
				return dic [file];
			}
		}

		public void stopAudio(string file,AudioType audioType,float time,CompleteDelegate completeDelegate)
        {
			//全部停止する
			if (file == "")
            {
				Dictionary<string,AudioObject> dic = this.getDic(audioType);
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
				AudioObject audioObject = this.getAudio (file,audioType);
//				audioObject.time = time;
				audioObject.completeDelegate = completeDelegate;
				audioObject.Stop();
			}
		}
	}
}
