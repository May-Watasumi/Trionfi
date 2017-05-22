using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
    public class SelectWindow : MonoBehaviour
    {
        public enum SelectState { None, Wait };
        public SelectState state = SelectState.None;

        [SerializeField]
        GameObject selectorPrefab;

        [SerializeField]
        private string returnKey = "f.Selectoresult";
        private string _resultString = "result";

        [SerializeField]
        float AreaHeight = 560.0f;

        List<Selector> selectorList = new List<Selector>();
        int activeSelector = 0;

        public string resultString
        {
            get
            {
                return _resultString;
            }
            set
            {
                _resultString = value;
            }
        }

        // Use this for initialization
        void Start()
        {
//            Init(10);
        }

        public void Init(int selectorCount)
        {
            Selector.currentWindow = this;

            for (int a = 0; a < selectorCount; a++)
            {
                GameObject _sel = GameObject.Instantiate(selectorPrefab);
                _sel.GetComponent<Selector>().answer = a;
                _sel.transform.SetParent(gameObject.transform);
                _sel.transform.localPosition = Vector3.zero;
                _sel.SetActive(false);
                selectorList.Add(_sel.GetComponent<Selector>());
            }
        }

        public void Add(string text)
        {
            selectorList[activeSelector].Set(text, null);
            selectorList[activeSelector].gameObject.SetActive(true);
            activeSelector++;
        }

        public void Begin()
        {
            StartCoroutine(BeginSub());
        }

        public IEnumerator BeginSub()
        {
            state = SelectState.Wait;

            for (int a = 0; a < activeSelector; a++)
            {
                //ToDo:選択肢が１つのときはゼロ除算になる。
                selectorList[a].gameObject.transform.localPosition = new Vector3(0, (AreaHeight / activeSelector * a), 0);
            }

            while (state == SelectState.Wait)
                yield return null;

            ResetSelect();

            yield return null;
        }

        public void ResetSelect()
        {
            activeSelector = 0;

            for(int a = 0; a < selectorList.Count; a++)
            {
                selectorList[a].gameObject.SetActive(false);
            }
        }
    }
}