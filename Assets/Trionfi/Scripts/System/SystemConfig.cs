using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace NovelEx
{
    public enum MessageType { Error, Warning, Info };
    public enum ObjectType { None, BG, Stand, Event, Bgm, Sound, Voice }
    public enum FirstAction { Start, Load }

    [ExecuteInEditMode]
	public class SystemConfig : SingletonMonoBehaviour<SystemConfig>
    {
        [SerializeField]
        public TextAsset initialScriptFile;

        [SerializeField]
        public bool debugMode = false;

        [SerializeField]
        private bool showFPS = false;

        [SerializeField]
        public bool showTag = true;

        //[SerializeField]
        //private Vector2 canvasSize = new Vector2(1136.0f, 640.0f);

        [SerializeField]
        [Range(10, 50)]
        private int _backlogCount = 30;
        private int backlogCount
        {
            get { return _backlogCount; }
        }

        [SerializeField]
        [Range(4, 40)]
        private int _saveSlotCount = 5;
        public int saveSlotCount
        {
            get { return _saveSlotCount; }
        }


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