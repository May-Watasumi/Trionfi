using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Trionfi;

namespace Trionfi
{
    public class TRSoundProperty
    {
        public float volume1 = 1.0f;   //本来の音量
        public float volume2 = 1.0f;   //フェーダー用。普段は1.0

        public float volume
        {
            get { return volume1 * volume2; }
            set { volume1 = value; }
        }

        string storageName;
    }

    public class TRSoundObjectBehaviour : MonoBehaviour
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

        public void Load(string storage, TRDataType type, string name = null)
		{
			AudioClip audioClip =  StorageManager.Instance.LoadObject(storage, type) as AudioClip;
            gameObject.name = storage;
			audioSource.clip = audioClip;
		}

        public IEnumerator Play(float time = 0.0f)
        {
            audioSource.volume = volume1;
            yield return FadeIn(time);
		}

        public IEnumerator Stop(float time = 0.0f)
        {
            yield return FadeOut(time);
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
