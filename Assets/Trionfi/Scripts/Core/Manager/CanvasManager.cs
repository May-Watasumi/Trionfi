using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Trionfi
{
    public enum TRlayerOrder { BG, Stand, Event, UI };

    public class CanvasManager : MonoBehaviour
    {
        [SerializeField]
        GameObject BGRoot;
        [SerializeField]
        GameObject StandRoot;
        [SerializeField]
        GameObject EventRoot;

//        private Canvas canvas;

        private void Start()
        {
  //          canvas = gameObject.GetComponent<Canvas>();
        }       

		public void CrossFade(float val) {
			//var test = this.gameObject.GetComponent<Image>();
			CanvasGroup group = GetComponent<CanvasGroup>();
			group.alpha = val;
		}

/*
		void Awake() {
			canvas = GetComponent<Canvas>();
//ToDo:
			CanvasScaler scaler = GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(SystemConfig.Instance.CanvasSize.x, SystemConfig.Instance.CanvasSize.y);
			RectTransform rect = GetComponent<RectTransform>();
			rect.sizeDelta = new Vector2(SystemConfig.Instance.CanvasSize.x, SystemConfig.Instance.CanvasSize.y)
            ;
		}

		// Use this for initialization
		void Start() {
		}

		// Update is called once per frame
		void Update() {
		}
        */
	}
}
