using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NovelEx;

namespace NovelEx
{
    public class TRAudioObjectBehaviour : MonoBehaviour
	{
        public float volume1 = 1.0f;   //本来の音量
        public float volume2 = 1.0f;   //フェーダー用。普段は1.0

        public float volume
        {
            get { return volume1 * volume2; }
        }

        [SerializeField]
        public  AudioSource audioSource;
        private AudioClip audioClip;

//        public bool 
//		public CompleteDelegate completeDelegate ;

        public void Load(string storage)
		{
			AudioClip audioClip =  StorageManager.Instance.LoadAudioAsset(storage);
            gameObject.name = storage;
			audioSource.clip = audioClip;
		}

        public void Play(float time = 0.0f)
        {
            audioSource.volume = volume1;
            StartCoroutine(FadeIn(time));
		}

        public void Stop(float time = 0.0f)
        {
            StartCoroutine(FadeOut(time));
		}

        public IEnumerator FadeIn(float time)
        {
            if (time <= 0.0f)
                audioSource.Play();
            else
            {
                audioSource.volume = 0.0f;
                volume2 = 1.0f;

                audioSource.Play();

                float count = 0.0f;
                while (count < time)
                {
                    count += Time.deltaTime;
                    volume2 = count / time;
                    audioSource.volume = volume;
                    yield return new WaitForEndOfFrame();
                }
            }

            audioSource.volume = volume1;
            yield return null;
        }

        public IEnumerator FadeOut(float time)
        {
            if (time <= 0.0f)
                audioSource.Stop();
            else
            {
                //            audioSource.volume = volume1;
                volume2 = 1.0f;

                float count = 0.0f;
                while (count < time)
                {
                    count += Time.deltaTime;
                    volume2 = count / time;
                    audioSource.volume = volume;
                    yield return new WaitForEndOfFrame();
                }
                audioSource.Stop();
                yield return null;
            }
        }
    }
}
