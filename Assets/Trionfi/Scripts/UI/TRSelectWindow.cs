using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public class TRSelectWindow : SingletonMonoBehaviour<TRSelectWindow>
    {
        public string result;
        public int resutNum;

        int activeSelector = 0;

        public bool onWait = false;

        [SerializeField]
        List<TRSelectButton> selectorList = new List<TRSelectButton>();

//        [Range(1, 10)]
//        readonly int maxSelectorCount = 5;

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
    }
}
