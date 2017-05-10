using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NovelEx {
//	[Serializable]
	public class BackLogWindow : MonoBehaviour
	{
        public struct LogData
        {
            public string name;
            public string message;
        }

        [SerializeField]
        public GameObject LogContentPrefab;

        public event Action OnApplyLog;

		public List<LogData> LogDataList = new List<LogData>();
		public List<GameObject> LogObjectList = new List<GameObject>();

        [SerializeField]
		public int dataSize = 30;

		public bool isAdd = true;

        LogData _tempLogData = new LogData();

		/// <summary>
		/// ログを一時的に貯める機構
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="name_color">Name color.</param>
		/// <param name="text">Text.</param>
		public void SetLogData(string name, string name_color, string text)
		{
			if(!isAdd)
			{
				return;
			}
			//キャラクター名が変化した！ログも残ってる
			if(_tempLogData.name != name && string.IsNullOrEmpty(_tempLogData.message))
			{
				SaveLogData();
			}

			string str = "";
//ToDo:名前を別のTextに入れるので、クラス化する
			//str += "<color=#"+name_color+">"+name+"</color>\n"+text + "";
			str += text + "";

			str = text;
			str = str.Replace("\r", "").Replace("\n", "");

            _tempLogData.message += str;
            _tempLogData.name = name;
		}

		/// <summary>
		/// ためたログを保存する
		/// </summary>
		public void SaveLogData()
		{
			if(!isAdd || string.IsNullOrEmpty(_tempLogData.message))
				return;

            LogDataList.Add(_tempLogData);

			//上限を超えていたら指定分の配列を削除する
			if( (dataSize + 1) < LogDataList.Count)
                LogDataList.RemoveRange(0, 1);

            _tempLogData.message = "";

            if (OnApplyLog != null)
			{
				OnApplyLog();
			}
		}

		public void ClearLog()
		{
            LogDataList.Clear();
        }

		//ログ配列データ取得
		public List<LogData> GetLogList()
		{
			return LogDataList;
		}

		public string GetLogText()
		{
			string logtext = "";

			LogDataList.Reverse();

			foreach (var item in LogDataList) {
				logtext += item.message + "\n\n";
			}

            LogDataList.Reverse();

			return logtext;
		}

		public void Open()
		{
			isAdd = false;
			gameObject.GetComponent<Canvas>().enabled = true;

//			GameObject _prefab = JOKEREX.Instance.StorageManager.loadPrefab("LogContent") as GameObject;

			//ToDo:
			GameObject content = gameObject.GetComponentInChildren<UnityEngine.UI.VerticalLayoutGroup>().gameObject;
			foreach (var item in LogDataList)
            {
				GameObject logcontent = GameObject.Instantiate(LogContentPrefab) as GameObject;
				logcontent.GetComponentInChildren<UnityEngine.UI.Text>().text = item.message;
				logcontent.transform.SetParent(content.transform);
//何故か引き延ばされる
				logcontent.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				LogObjectList.Add(logcontent);
			}
		}

		public void Close()
        {
			gameObject.GetComponent<Canvas>().enabled = false;

			for (int num = 0; num < LogObjectList.Count; num++)
            {
				GameObject.Destroy(LogObjectList[num]);
			}

            LogObjectList.Clear();

//			JOKEREX.Instance.StatusManager.NextOrder();
			isAdd = true;
		}
	}
}
