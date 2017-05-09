using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NovelEx;

namespace NovelEx
{
    public class AudioObjectBase:MonoBehaviour
    {
        float volume1;
        float volume2;

        public virtual void Load(string resourceName) { }
        public virtual void Play(float time = 0.0f) { }
        public virtual void Stop(float time = 0.0f) { }
        public virtual void FadeIn(float time) { }
        public virtual void FadeOUt(float time) { }
    }

    public class AudioObject:AudioObjectBase
	{
		private AudioSource audioSource;
        private AudioClip audioClip;
		public bool isWait;
		public CompleteDelegate completeDelegate ;

        public override void Load(string resourceName)
		{
//			AudioClip audioClip = Resources.Load(resourceName, typeof(AudioClip)) as AudioClip;
//			AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
			audioSource.clip = audioClip;
//			audioSource = audioSource;
		}

        public override void Play(float time = 0.0f)
        {
            audioSource.Play();

            if (time > 0.0f)
                FadeIn(time);
		}

        public override void Stop(float time = 0.0f)
        {
            if (time > 0.0f)
                FadeOUt(time);
            else
                audioSource.Stop();
		}
	}
}
