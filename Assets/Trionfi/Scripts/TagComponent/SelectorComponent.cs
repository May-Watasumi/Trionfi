using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {
	public class Selector_initComponent : AbstractComponent {
		public Selector_initComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				"name",
			};

			originalParamDic = new Dictionary<string, string>() {
				{ "name","" }
			};
		}

		public override IEnumerator Start() {
			GameObject g = StorageManager.Instance.loadPrefab("CanvasSelector") as GameObject;
			GameObject _selector = (GameObject)Object.Instantiate(g, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

			if(!string.IsNullOrEmpty(paramDic["name"]) )
				_selector.name = paramDic["name"];

            //ToDo:CanvasごとPrefab化
            //			JOKEREX.Instance.SelectorManager = _selector.GetComponent<SelectorManager>();
            //			JOKEREX.Instance.selectorManager.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            yield return null;
        }
    }

	public class Selector_addComponent : AbstractComponent {
		public Selector_addComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				"result",
				"text",
				"storage",
			};

			originalParamDic = new Dictionary<string,string>() {
				{"text",""},
//				{"storage",""}
			};
		}

		public override IEnumerator Start() {
			GameObject g = StorageManager.Instance.loadPrefab("SelectorContent") as GameObject;
			GameObject _selector = (GameObject)Object.Instantiate(g, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

				if (!string.IsNullOrEmpty(paramDic["result"]))
					_selector.name = paramDic["result"];
                //ToDo?
//				_selector.GetComponent<SelectorEx>().resultString = paramDic["result"];

//				SelectorManager.Add(_selector);

				if(paramDic["storage"] != "") {
					string filename = StorageManager.Instance.PATH_SYSTEM_IMAGE + paramDic["storage"];
					Sprite imageSprite = StorageManager.Instance.LoadSprite(filename);

					_selector.GetComponent<UnityEngine.UI.Button>().image.sprite = imageSprite;
					_selector.GetComponent<UnityEngine.UI.Button>().image.SetNativeSize();
				}
				if(paramDic["text"] != "") {
					_selector.GetComponentInChildren<UnityEngine.UI.Text>().text = paramDic["text"];
				}

				Vector3 pos = new Vector3(float.Parse(paramDic["x"]), float.Parse(paramDic["y"]), float.Parse(paramDic["z"]));
				_selector.transform.localPosition = pos;
            yield return null;
        }
    }

	public class Selector_startComponent : AbstractComponent
	{
		public Selector_startComponent() { }

		public override IEnumerator Start()
		{
			StatusManager.Instance.UIClicked = true;
            //			JOKEREX.Instance.MainMessage.Hide();
            //ToDo:
//            Trionfi.Instance.currentSelectWindow.SendMessage();
			this.nextOrder = false;
            yield return null;
        }
    }


}


