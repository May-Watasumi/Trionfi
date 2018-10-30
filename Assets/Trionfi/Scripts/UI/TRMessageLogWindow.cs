using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Trionfi {
    //	[Serializable]
    public class TRMessageLogWindow : SingletonMonoBehaviour<TRMessageLogWindow>
    {
        [SerializeField]
        public GameObject logContentPrefab;

        [SerializeField]
        public GameObject logDataRoot;

        public event Action OnApplyLog;

		public List<TRMessageLogData> logDataList = new List<TRMessageLogData>();
		public List<GameObject> logObjectList = new List<GameObject>();

        [SerializeField]
		public int logLimit = 100;

		public void AddLogData(string message, string name, AudioClip voice = null)
		{
            GameObject _temp = GameObject.Instantiate(logContentPrefab);
            TRMessageLogData logData = _temp.GetComponent<TRMessageLogData>();

            logData.logText.text = message;
            logData.SetIcon(name);
            logData.voice = voice;

            //ToDo:
            //str += "<color=#"+name_color+">"+name+"</color>\n"+text + "";
            logDataList.Add(logData);

            logData.gameObject.transform.SetParent(logDataRoot.transform);

            //上限を超えていたら指定分の配列を削除する
            if ((logLimit + 1) < logDataList.Count)
                logDataList.RemoveRange(0, 1);
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
            foreach (TRMessageLogData logData in logDataList)
            {
                GameObject.Destroy(logData);
            }

            logDataList.Clear();
        }

		public void Show()
		{
/*
			//ToDo:
			GameObject content = gameObject.GetComponentInChildren<UnityEngine.UI.VerticalLayoutGroup>().gameObject;
			foreach (var item in logDataList)
            {
				GameObject logcontent = GameObject.Instantiate(logContentPrefab) as GameObject;
//				logcontent.GetComponentInChildren<UnityEngine.UI.Text>().text = item.message;
				logcontent.transform.SetParent(content.transform);

                //何故か引き延ばされる
				logcontent.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				logObjectList.Add(logcontent);
			}
*/
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
