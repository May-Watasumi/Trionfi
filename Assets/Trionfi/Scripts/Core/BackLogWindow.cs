using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NovelEx {
//	[Serializable]
	public class BackLogWindow : MonoBehaviour
	{
		public event Action OnApplyLog;

		public struct LogData
		{
			public string name;
			public string message;
		}

		public List<LogData> arrLog = new List<LogData>();
		public List<GameObject> arrLogContent = new List<GameObject>();

		public int logCount = -1;

		public bool isAdd = true;

		private string temporaryLog  = "";
		private string characterName = "";

		/// <summary>
		/// ログを一時的に貯める機構
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="name_color">Name color.</param>
		/// <param name="text">Text.</param>
		public void AddLog(string name, string name_color, string text)
		{
			if(!isAdd)
			{
				return;
			}
			//キャラクター名が変化した！ログも残ってる
			if (characterName != name && string.IsNullOrEmpty(temporaryLog))
			{
				ApplyLog();
			}

			string str = "";
//ToDo:名前を別のTextに入れるので、クラス化する
			//str += "<color=#"+name_color+">"+name+"</color>\n"+text + "";
			str += text + "";

			str = text;
			str = str.Replace("\r", "").Replace("\n", "");

			if(this.logCount == -1) {
				this.logCount = JOKEREX.Instance.SystemConfig.backLogCount;
			}

			temporaryLog += str;
			characterName = name;
		}

		/// <summary>
		/// ためたログを保存する
		/// </summary>
		public void ApplyLog()
		{
			if(!isAdd)
			{
				return;
			}

			if (string.IsNullOrEmpty(temporaryLog))
			{
				return;
			}

			this.arrLog.Add(new LogData(){
				name    = characterName,
				message = temporaryLog,
			});

			//上限を超えていたら指定分の配列を削除する
			if( (this.logCount + 1) < this.arrLog.Count) {
				this.arrLog.RemoveRange(0, 1);
			}
			temporaryLog = "";

			if (OnApplyLog != null)
			{
				OnApplyLog();
			}
		}

		public void ClearList()
		{
			arrLog.Clear();
		}

		//ログ配列データ取得
		public List<LogData> getLogList()
		{
			return this.arrLog;
		}

		public string getLogText()
		{
			string logtext = "";

			this.arrLog.Reverse();

			foreach (var item in this.arrLog) {
				logtext += item.message + "\n\n";
			}

			this.arrLog.Reverse();

			return logtext;
		}

		public void Open()
		{
			isAdd = false;
			this.gameObject.GetComponent<Canvas>().enabled = true;

			GameObject _prefab = JOKEREX.Instance.StorageManager.loadPrefab("LogContent") as GameObject;
			//ToDo:
			GameObject content = this.gameObject.GetComponentInChildren<UnityEngine.UI.VerticalLayoutGroup>().gameObject;
			foreach (var item in arrLog) {
				GameObject logcontent = GameObject.Instantiate(_prefab) as GameObject;
				logcontent.GetComponentInChildren<UnityEngine.UI.Text>().text = item.message;
				logcontent.transform.SetParent(content.transform);
//何故か引き延ばされる
				logcontent.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				arrLogContent.Add(logcontent);
			}
		}

		public void Close() {
			this.gameObject.GetComponent<Canvas>().enabled = false;

			for (int num = 0; num < arrLogContent.Count; num++) {
				GameObject.Destroy(arrLogContent[num]);
			}
			arrLogContent.Clear();

			JOKEREX.Instance.StatusManager.NextOrder();
			isAdd = true;
		}
/*
		void Awake()
		{
		}
*/
	}
}
