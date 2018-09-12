using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Trionfi
{
    public class TRSelectWindow : SingletonMonoBehaviour<TRSelectWindow>
    {
        public string result;
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

        // Use this for initialization
        void Start()
        {    }

        public void Add(string content, string result)
        {
            selectorList[activeSelector++].Set(content, result);
        }

        public void Begin()
        {
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

            gameObject.SetActive(true);
        }
    }
}
