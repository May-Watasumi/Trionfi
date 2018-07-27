using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Trionfi {

    public struct TRMessageLog
    {
        public string name;
        public string message;
        public string voice;
    }

    //	[Serializable]
    public class TRMessageLogWindow : SingletonMonoBehaviour<TRMessageLogWindow>
    {
        [SerializeField]
        public GameObject logContentPrefab;

        public event Action OnApplyLog;

		public List<TRMessageLog> logDataList = new List<TRMessageLog>();
		public List<GameObject> logObjectList = new List<GameObject>();

        [SerializeField]
		public int logLimit = 30;

        TRMessageLog tempLogData = new TRMessageLog();

		public void AddLogData(TRMessageLog logData)
		{
            //ToDo:
            //str += "<color=#"+name_color+">"+name+"</color>\n"+text + "";
            logDataList.Add(logData);

            //上限を超えていたら指定分の配列を削除する
            if ((logLimit + 1) < logDataList.Count)
                logDataList.RemoveRange(0, 1);
        }

        public void AddLogData(string name, string message, string voice = "")
        {
            tempLogData.name = name;
            tempLogData.message = message;
            tempLogData.voice = voice;
            logDataList.Add(tempLogData);
        }

        public void SaveLogData()
		{
            if (OnApplyLog != null)
			{
				OnApplyLog();
			}
		}

		public void ClearLog()
		{
            logDataList.Clear();
        }

		public string GetLogText()
		{
			string logtext = "";

			logDataList.Reverse();

			foreach (var item in logDataList) {
				logtext += item.message + "\n\n";
			}

            logDataList.Reverse();

			return logtext;
		}

		public void Show()
		{
			//ToDo:
			GameObject content = gameObject.GetComponentInChildren<UnityEngine.UI.VerticalLayoutGroup>().gameObject;
			foreach (var item in logDataList)
            {
				GameObject logcontent = GameObject.Instantiate(logContentPrefab) as GameObject;
				logcontent.GetComponentInChildren<UnityEngine.UI.Text>().text = item.message;
				logcontent.transform.SetParent(content.transform);
//何故か引き延ばされる
				logcontent.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				logObjectList.Add(logcontent);
			}
		}

		public void Close()
        {
			for (int num=0; num < logObjectList.Count; num++)
            {
                GameObject.Destroy(logObjectList[num]);
			}

            logObjectList.Clear();
		}
	}
}
