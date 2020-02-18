using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Trionfi
{
    [ExecuteInEditMode]
	public class TRSystemConfig : SingletonMonoBehaviour<TRSystemConfig>
    {
        [System.Serializable]
        public class LayerPos : SerializableDictionary<string, float> { };


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
        public Vector2 screenSize = new Vector2(1280, 720);

        [SerializeField]
        public bool useUnityAudioMixer = false;

        [SerializeField]
        public LayerPos layerPos = new LayerPos()
        {
            { "left", -0.5f },//-360 },
            { "center", 0.0f },
            { "right", 0.5f },//360 },
            { "left_center", -0.6f },// -420 },
            { "right_center", 0.6f },//420 },
            { "l", -0.5f },//-360 },
            { "c", 0.0f },
            { "r", 0.5f },//360 },
            { "lc", -0.6f },// -420 },
            { "rc", 0.6f },//420 },
        };

        [SerializeField]
        public int characterPosdescent = 30;

        [SerializeField]
        [Range(-1, 50)]
        public int backlogCount = -1;

        [SerializeField]
        [Range(5, 30)]
        public int saveSlotCount = 5;

        [SerializeField]
        [Range(3, 10)]
        public int standObjectCount = 3;

        [SerializeField]
        Font defaultFont = null;

        [SerializeField]
        public Color fontColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        [SerializeField]
        public int fontSize = 26;

        [SerializeField]
        public Color imageDefaultColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        [SerializeField]
        public float defaultEffectTime = 1.0f;

        [SerializeField]
        public bool KAGCompatibility = true;
        public bool useCRI = false;
        public bool useLive2D = false;
        public bool useEmote = false;

        private float oldTime;
        private int frame = 0;
        private float frameRate = 0f;
        private const float INTERVAL = 0.5f;

        private void Start()
        {
            defaultFont = defaultFont ?? Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        }

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

		public float GetFrameRate()
        {
			return frameRate;
		}

		void OnGUI() {
			if(showFPS) 
				GUI.Label(new Rect(25, 25, 160, 20), "FPS : " + frameRate.ToString());
		}

	}

}