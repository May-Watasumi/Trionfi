using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public class TRSelectWindow : SingletonMonoBehaviour<TRSelectWindow>
    {
        [NonSerialized]
        public string result;
        [NonSerialized]
        public int resutNum;
        [NonSerialized]
        int activeSelector = 0;
        [NonSerialized]
        public bool onWait = false;

        [SerializeField]
        public GameObject selectorPrefab;
        [SerializeField] [Range(1, 10)]
        private int selectorCount = 5;
        [SerializeField]
        List<TRSelectButtonBase> selectorList = new List<TRSelectButtonBase>();
        [SerializeField]
        public AudioSource decisionSound;

        [SerializeField]
        float AreaHeight = 560.0f;

        public void Add(string content, string label)
        {
            selectorList[activeSelector].Set(content, label, activeSelector);
            activeSelector++;
        }

        public void Begin()
        {
            Trionfi.Instance.selectWindow.onWait = true;

            Trionfi.Instance.currentMessageWindow.Pause();
            Trionfi.Instance.HideObject(Trionfi.Instance.globalTap);

            if(Trionfi.Instance.configWindow != null)
                Trionfi.Instance.HideObject(Trionfi.Instance.configWindow.gameObject);

            Trionfi.Instance.HideObject(Trionfi.Instance.currentMessageWindow.gameObject);
            Trionfi.Instance.HideObject(Trionfi.Instance.systemMenuWindow.gameObject);
            Trionfi.Instance.HideObject(Trionfi.Instance.messageLogwindow.gameObject);

            for (int a = 0; a < selectorList.Count; a++)
            {
                selectorList[a].gameObject.SetActive(false);
            }

            for (int a = 0; a < activeSelector; a++)
            {
                selectorList[a].gameObject.SetActive(true);
                selectorList[a].gameObject.transform.localPosition = new Vector3(0, (AreaHeight / activeSelector * -a + (AreaHeight / 2)) , 0);
            }

            activeSelector = 0;

            Trionfi.Instance.OpenUI(Trionfi.Instance.selectWindow.gameObject);
        }

        public void Start()
        {
            for(int a = 0;a < selectorCount; a++)
            {
                GameObject obj = Instantiate(selectorPrefab, transform);
                TRSelectButtonBase button = obj.GetComponent<TRSelectButtonBase>();
                button.resultNum = a;
                button.gameObject.SetActive(false);
                selectorList.Add(button);
            }
        }
    }
}
