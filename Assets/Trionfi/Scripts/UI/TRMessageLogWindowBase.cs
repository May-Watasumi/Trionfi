using UnityEngine;
using System.Collections.Generic;
using System;

namespace Trionfi {
    //	[Serializable]
    public class TRMessageLogWindowBase : SingletonMonoBehaviour<TRMessageLogWindowBase> 
    {
        [SerializeField]
        public GameObject logContentPrefab;

        [SerializeField]
        public GameObject logDataRoot;

        [SerializeField]
        [Range(-1, 50)]
        public int backlogCount = -1;

        public event Action OnApplyLog;

        [NonSerialized]
        public List<TRMessageLogDataBase> logDataList = new List<TRMessageLogDataBase>();
		//public List<GameObject> logObjectList = new List<GameObject>();

		public void AddLogData(string message, string name, AudioClip voice = null)
		{
            GameObject _temp = GameObject.Instantiate(logContentPrefab);

			//_temp.hideFlags = HideFlags.HideInHierarchy;

			TRMessageLogDataBase logData = _temp.GetComponent<TRMessageLogDataBase>();

            logData.SetLogData(message, name,voice);

            //ToDo:共通キャラアイコン
            /*
            if (TRStageEnviroment.Instance.actorInfoes != null &&
                TRStageEnviroment.Instance.actorInfoes.ContainsKey(TRLayer.currentSpeaker) &&
                TRStageEnviroment.Instance.actorInfoes[TRLayer.currentSpeaker].logIcon != null)
                logData.characterIcon.sprite = TRStageEnviroment.Instance.actorInfoes[TRLayer.currentSpeaker].logIcon;
            else
            */
                logData.characterIcon.gameObject.SetActive(false);

            //ToDo:
            //str += "<color=#"+name_color+">"+name+"</color>\n"+text + "";
            logDataList.Add(logData);
            logData.gameObject.transform.SetParent(logDataRoot.transform);

            logData.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;

            //上限を超えていたら指定分の配列を削除する
            if (backlogCount > 0 && (backlogCount + 1) < logDataList.Count)
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
            foreach (TRMessageLogDataBase logData in logDataList)
            {
                GameObject.Destroy(logData);
            }

            logDataList.Clear();
        }
        
        public void Exit()
        {
            gameObject.SetActive(false);
            Trionfi.Instance.currentMessageWindow.gameObject.SetActive(true);
            Trionfi.Instance.systemMenuWindow.gameObject.SetActive(true);
            Trionfi.Instance.globalTap.SetActive(true);
            Trionfi.Instance.currentMessageWindow.Restart();
        }

        public void OnDestroy()
        {
            DeleteLogObject();
        }

		public void Start()
		{
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape))
            {
                Exit();
            }
        }
    }
}
