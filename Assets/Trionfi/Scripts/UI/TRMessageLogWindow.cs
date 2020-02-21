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
		//public List<GameObject> logObjectList = new List<GameObject>();

		public void AddLogData(string message, string name, AudioClip voice = null)
		{
            GameObject _temp = GameObject.Instantiate(logContentPrefab);

            //_temp.hideFlags = HideFlags.HideInHierarchy;

            TRMessageLogData logData = _temp.GetComponent<TRMessageLogData>();

            logData.logText.text = message;
            logData.logName = name;
            logData.SetLogData(name, voice);
            logData.voice = voice;
            if (TRStageEnviroment.Instance.actorInfoes != null &&
                TRStageEnviroment.Instance.actorInfoes.ContainsKey(TRLayer.speaker) &&
                TRStageEnviroment.Instance.actorInfoes[TRLayer.speaker].logIcon != null)
                logData.characterIcon.sprite = TRStageEnviroment.Instance.actorInfoes[TRLayer.speaker].logIcon;
            else
                logData.characterIcon.gameObject.SetActive(false);

            //ToDo:
            //str += "<color=#"+name_color+">"+name+"</color>\n"+text + "";
            logDataList.Add(logData);
            logData.gameObject.transform.SetParent(logDataRoot.transform);

            //上限を超えていたら指定分の配列を削除する
            if (TRSystemConfig.Instance.backlogCount > 0 && (TRSystemConfig.Instance.backlogCount + 1) < logDataList.Count)
                logDataList.RemoveRange(0, 1);
        }

        public void SaveLogData()
		{
            if (OnApplyLog != null)
			{
				OnApplyLog();
			}
		}

		public void DeleteLogObject()
		{
            foreach (TRMessageLogData logData in logDataList)
            {
                GameObject.Destroy(logData);
            }

            logDataList.Clear();
        }
        
        public void Exit()
        {
            Trionfi.Instance.PopWindow();
        }

        public void OnDestroy()
        {
            DeleteLogObject();

        }
    }
}
