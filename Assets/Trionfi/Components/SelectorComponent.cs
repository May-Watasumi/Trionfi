using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {
	public class Selector_initComponent : AbstractComponent {
		public Selector_initComponent() {
			//必須項目
			this.arrayVitalParam = new List<string> {
				"name",
			};

			this.originalParam = new Dictionary<string, string>() {
				{ "name","" }
			};
		}

		public override void start() {
			GameObject g = JOKEREX.Instance.StorageManager.loadPrefab("CanvasSelector") as GameObject;
			GameObject _selector = (GameObject)Object.Instantiate(g, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

			if(!string.IsNullOrEmpty(param["name"]) )
				_selector.name = param["name"];

//ToDo:CanvasごとPrefab化
//			JOKEREX.Instance.SelectorManager = _selector.GetComponent<SelectorManager>();
//			JOKEREX.Instance.selectorManager.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		}
	}

	public class Selector_addComponent : AbstractComponent {
		public Selector_addComponent() {
			//必須項目
			this.arrayVitalParam = new List<string> {
				"result",
				"text",
				"storage",
			};

			this.originalParam = new Dictionary<string,string>() {
				{"text",""},
//				{"storage",""}
			};
		}

		public override void start() {
			GameObject g = JOKEREX.Instance.StorageManager.loadPrefab("SelectorContent") as GameObject;
			GameObject _selector = (GameObject)Object.Instantiate(g, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

				if (!string.IsNullOrEmpty(param["result"]))
					_selector.name = param["result"];

				_selector.GetComponent<SelectorEx>().resultString = param["result"];

				JOKEREX.Instance.SelectorManager.Add(_selector);

				if(param ["storage"] != "") {
					string filename = JOKEREX.Instance.StorageManager.PATH_SYSTEM_IMAGE + param ["storage"];
					Sprite imageSprite = JOKEREX.Instance.StorageManager.loadSprite(filename);

					_selector.GetComponent<UnityEngine.UI.Button>().image.sprite = imageSprite;
					_selector.GetComponent<UnityEngine.UI.Button>().image.SetNativeSize();
				}
				if(param ["text"] != "") {
					_selector.GetComponentInChildren<UnityEngine.UI.Text>().text = param["text"];
				}

				Vector3 pos = new Vector3(float.Parse(param["x"]), float.Parse(param["y"]), float.Parse(param["z"]));
				_selector.transform.localPosition = pos;
		}
	}

	public class Selector_startComponent : AbstractComponent
	{
		public Selector_startComponent()
		{
		}

		public override void start()
		{
			JOKEREX.Instance.StatusManager.UIClicked = true;
//			JOKEREX.Instance.MainMessage.Hide();
			JOKEREX.Instance.SelectorManager.Begin();
			this.nextOrder = false;
		}
	}


}


