#if false
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NovelEx;

namespace NovelEx
{
	public class SelectorManager : MonoBehaviour
	{
//		public GameObject _parentCanvas = null;
//		public GameObject selectorObject = null;
		public List<GameObject> selectorList = new List<GameObject>();

		public void Init()
		{
//			selectorList.Clear();
//			selectorObject = new GameObject();
//			selectorObject.name = name;
//			selectorObject.SetActive(false);
//			selectorObject.transform.parent = gameObject.transform;
		}

		public void Add(GameObject instanse)
		{
			selectorList.Add(instanse);
			instanse.transform.SetParent(gameObject.transform);
		}

		public void Begin()
		{
			StatusManager.Instance.Wait();
//			selectorObject.SetActive(true);
		}

		public void End() {
			gameObject.SetActive(false);
			StatusManager.Instance.UIClicked = false;
		}
	}
}
#endif