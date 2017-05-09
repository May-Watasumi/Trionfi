using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Text;
//using MiniJSON;

namespace NovelEx {
	[Serializable]
	public class SaveObject {
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

		public Variable variable;

		//スタック管理
		public ScenarioManager scenarioManager;
	}
}