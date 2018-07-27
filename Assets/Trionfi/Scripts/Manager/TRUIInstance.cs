using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Trionfi
{
    public class TRUIInstance : SingletonMonoBehaviour<TRUIInstance>
    {
        public TRMessageWindow messageWindow;
        public TRMessageLogWindow logWindow;
        public TRSelectWindow selectWindow;
        public TRCustomDialog dialogWindow;
        public TRSystemMenuWindow systemWindow;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if(!EventSystem.current.IsPointerOverGameObject())
            {
//                OnTouch();
            }
#else 
    if(!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                OnTouch();               
    }
#endif
        }

        void OnTouch()
        {
            if (!messageWindow.gameObject.activeSelf)
            {
                instance.logWindow.gameObject.SetActive(false);
                messageWindow.gameObject.SetActive(true);
                systemWindow.gameObject.SetActive(true);
            }
            else
                messageWindow.OnClick();
        }
    }
}