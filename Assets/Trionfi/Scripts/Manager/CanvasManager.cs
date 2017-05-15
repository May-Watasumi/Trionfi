using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace NovelEx {
	public class CanvasManager : MonoBehaviour {
		Dictionary<string, GameObject> componentArray = new Dictionary<string, GameObject>();
		public Dictionary<string, GameObject> components {
			get {
				return componentArray;
			}
		}
		private Canvas canvas;

		public Canvas Canvas {
			get	{
				return canvas;
			}
		}

		bool enable {
			get {
				return canvas.enabled;
			}
			set {
				canvas.enabled = value;
			}
		}

		public Text addText(string name) {
			GameObject g = new GameObject(this.name);
			g.transform.parent = this.transform;
			g.name = name;
			Text t = g.AddComponent<Text>();
			componentArray[name] = g;
				return t;
		}

		public UnityEngine.UI.Image addImage() {
			GameObject g = new GameObject(this.name);
			g.transform.parent = this.transform;
			g.name = name;
			UnityEngine.UI.Image t = g.AddComponent<UnityEngine.UI.Image>();
			componentArray[name] = g;		
			return t;
		}

		public void hide(float time, bool nextOrder) {
			if (time > 0.0f) {
				//通常の表示切り替えの場合
				iTween.ValueTo(this.gameObject, iTween.Hash(
					"from", 1,
					"to", 0,
					"time", time,
					"oncomplete", "finishAnimation",
					"oncompletetarget", this.gameObject,
					"easeType", "linear",
					"onupdate", "crossFade"
				));
			}
		}

		public void show(float time, bool nextOrder) {
			string order = nextOrder ? "finishAnimation" : "finishAnimationWithoutNextOrder";

			enable = true;

			if (time > 0.0f) {
				//通常の表示切り替えの場合
				iTween.ValueTo(this.gameObject, iTween.Hash(
					"from", 0,
					"to", 1,
					"time", time,
					"oncomplete", order,
					"oncompletetarget", this.gameObject,
					"easeType", "linear",
					"onupdate", "crossFade"
				));
			}
		}
		public void crossFade(float val) {
			//var test = this.gameObject.GetComponent<Image>();
			CanvasGroup group = GetComponent<CanvasGroup>();
			group.alpha = val;
		}

		public void finishAnimation() {
			StatusManager.Instance.NextOrder();
			//StatusManager.Instance.enableClickOrder = true;
			//			NovelSingleton.GameManager.nextOrder();
		}

		public void finishAnimationWithoutNextOrder() {
			StatusManager.Instance.NextOrder();
//			StatusManager.Instance.enableClickOrder = true;
		}

		void Awake() {
			canvas = GetComponent<Canvas>();
//ToDo:
			CanvasScaler scaler = GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(SystemConfig.Instance.CanvasSize.x, SystemConfig.Instance.CanvasSize.y);
			RectTransform rect = GetComponent<RectTransform>();
			rect.sizeDelta = new Vector2(SystemConfig.Instance.CanvasSize.x, SystemConfig.Instance.CanvasSize.y);
		}

		// Use this for initialization
		void Start() {
		}

		// Update is called once per frame
		void Update() {
		}
	}
}
