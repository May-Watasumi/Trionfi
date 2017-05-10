using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace NovelEx {
    public enum MessageType { Error, Warning, Info };
    public enum AudioType { Bgm, Sound, Voice }
    public enum FirstAction { Start, Load }

    [ExecuteInEditMode]
	public class SystemConfig : SingletonMonoBehaviour<SystemConfig> {
        public static string PATH_PREFAB = "novel/data/prefab/";
        public static string PATH_IMAGE = "novel/data/images/image/";
        public static string PATH_CHARA_IMAGE = "novel/data/images/character/";
        public static string PATH_BG_IMAGE = "novel/data/images/background/";
        public static string PATH_SYSTEM_IMAGE = "novel/data/images/system/";
        public static string PATH_SD_OBJECT = "novel/data/sd/";
        public static string PATH_AUDIO_BGM = "novel/data/bgm/";
        public static string PATH_AUDIO_SE = "novel/data/sound/";
        public static string PATH_ANIM_FILE = "novel/data/anim/";

        [SerializeField]
		public bool ignoreCR = true;

		[SerializeField]
        public string actorMarker = "【】";

		[SerializeField]
        public string atcorTag = "talk_name";

        [SerializeField]
        public bool debugMode = false;

        [SerializeField]
        private bool showFPS = false;

        [SerializeField]
        public bool showTag = true;

        [SerializeField]
        private Vector2 canvasSize = new Vector2(1136.0f, 640.0f);

        //        [SerializeField]
        //        [Range(10, 50)]
        //        private int _backlogCount = 30;

        [SerializeField]
        [Range(4, 40)]
        private int _saveSlotCount = 5;
        public int saveSlotCount
        {
            get
            {
                return _saveSlotCount;
            }
        }

        [SerializeField]
        public bool autoBoot = false;
        public TextAsset bootScript;

        public bool useSerializer = true;

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