using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Trionfi
{
    [ExecuteInEditMode]
	public class TRSystemConfig : SingletonMonoBehaviour<TRSystemConfig>
    {
        [SerializeField]
        public bool debugMode = false;

        [SerializeField]
        private bool showFPS = false;

        [SerializeField]
        public bool showTag = true;

        //trueの時は自動でクリック待ち（メッセージクリアは手動）falseの時は自動でPタグ（クリック待ち＋メッセージクリア）
        [SerializeField]
        public bool isNovelMode = false;

        [SerializeField]
        public int characterPosdescent = 30;

        [SerializeField]
        [Range(10, 50)]
        public int backlogCount = 30;

        [SerializeField]
        [Range(5, 30)]
        public int saveSlotCount = 5;

        [SerializeField]
        public Color fontColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        [SerializeField]
        public int fontSize = 26;

        [SerializeField]
        public bool useCRI = false;
        public bool useLive2D = false;
        public bool useEmote = false;

        private float oldTime;
        private int frame = 0;
        private float frameRate = 0f;
        private const float INTERVAL = 0.5f;

        void Update() {
			if(showFPS)
            {
				// FPS
				frame++;
				float time = Time.realtimeSinceStartup - oldTime;
				if (time >= INTERVAL)
                {
					frameRate = frame / time;
					oldTime = Time.realtimeSinceStartup;
					frame = 0;
				}
			}
		}

		public float getFrameRate()
        {
			return frameRate;
		}

		void OnGUI() {
			if(showFPS) 
				GUI.Label(new Rect(25, 25, 160, 20), "FPS : " + frameRate.ToString());
		}

	}

}