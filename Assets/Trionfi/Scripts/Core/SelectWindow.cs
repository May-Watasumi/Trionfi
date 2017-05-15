using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
	public class SelectWindow : MonoBehaviour
	{
        [SerializeField]
        GameObject selectorPrefab;

        [SerializeField]
		private string returnKey = "f.Selectoresult";
		private string _resultString = "result";

        List<GameObject> selectorList = new List<GameObject>();

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

		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}