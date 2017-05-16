using System;
using System.Collections.Generic;

namespace NovelEx {
	[Serializable]
	public class UserSaveData {
		public string currentFile ="";
		public int  currentIndex =-1;

		public string name ="";
		public string title ="";
		public string description ="";
		public string date = "";
		public string currentMessage ="";

		public bool visibleMessageFrame = true;
		public bool enableNextOrder =true;
		public bool enableEventClick = true;
		public bool enableClickOrder = true;
		public string currentPlayBgm = "";

		public bool isEventStop = false;

		//画面のキャプチャ情報
		public string cap_img_file ="";

        //スタック管理
        public Variable variable;
        public Dictionary<string, AbstractObject> dicObject;

        public ScriptDecoder scriptManager;
	}
}
