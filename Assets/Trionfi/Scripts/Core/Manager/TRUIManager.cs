using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trionfi
{
    public class TRUIManager : SingletonMonoBehaviour<TRUIManager>
    {
        [SerializeField]
        private GameObject defaultMessageWindowPrefab;
        [SerializeField]
        private GameObject defaultMessageLogWindowPrefab;
        [SerializeField]
        private GameObject defaultSelectWindowPrefab;

        public TRMessageWindow currentMessageWindow;
        public TRMessageLogWindow currentLogWindow;
        public TRSelectWindow currentSelectWindow;

        public delegate void ClickEvent();

        public ClickEvent OnClick;

        // Use this for initialization
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init()
        {
/*
            GameObject _message = GameObject.Instantiate(defaultMessageWindowPrefab);
            GameObject _log = GameObject.Instantiate(defaultLogWindowPrefab);
            GameObject _select = GameObject.Instantiate(defaultSelectWindowPrefab);
*/
            currentMessageWindow = defaultMessageWindowPrefab.GetComponent<TRMessageWindow>();
            currentLogWindow = defaultMessageLogWindowPrefab.GetComponent<TRMessageLogWindow>();
            currentSelectWindow = defaultSelectWindowPrefab.GetComponent<TRSelectWindow>();
/**
            _message.gameObject.transform.SetParent(this.gameObject.transform);
            _log.gameObject.transform.SetParent(this.gameObject.transform);
            _select.gameObject.transform.SetParent(this.gameObject.transform);

            _message.transform.localPosition = Vector3.zero;
            _log.transform.position = Vector3.zero;
            _select.transform.position = Vector3.zero;
*/
            defaultMessageLogWindowPrefab.SetActive(false);
            defaultSelectWindowPrefab.SetActive(false);
        }
        /*
                private void OnDestroy()
                {
                    currentMessageWindow.gameObject.transform.SetParent(null);
                    currentLogWindow.gameObject.transform.SetParent(null);
                    currentSelectWindow.gameObject.transform.SetParent(null);

                    GameObject.Destroy(currentMessageWindow.gameObject);
                    GameObject.Destroy(currentLogWindow.gameObject);
                    GameObject.Destroy(currentSelectWindow.gameObject);
                }
                */

        public void OnAutoButton()
        {
            currentMessageWindow.onAuto = true;
        }

        public void OnSkipButton()
        {
            currentMessageWindow.onSkip = true;
        }

        public void OnMessageLogButton()
        {
            currentMessageWindow.gameObject.SetActive(false);
        }

        public void OnWindowCloseButton()
        {
            currentMessageWindow.gameObject.SetActive(false);
            currentSelectWindow.gameObject.SetActive(false);
            currentLogWindow.gameObject.SetActive(false);
        }
    }
}