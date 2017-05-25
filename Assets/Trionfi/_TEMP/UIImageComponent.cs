using System;
using System.Collections;
using System.Collections.Generic;
//using ExpressionParser;
using UnityEngine;
using UnityEngine.UI;

namespace NovelEx {
	public class Uiimage_newComponent : AbstractComponent {
		public Uiimage_newComponent() : base() {
			//必須項目
			arrayVitalParam = new List<string> {
				"name" 
			};

			originalParamDic = new ParamDictionary() {
				{ "name","" },
				{ "val","" },
				{ "tag","" },
				{ "layer","Default" },
				{ "sort","0" },
				{ "storage", ""},
				{ "x","0" },
				{ "y","0" },
				{ "z","-3.2" },
				{ "scale","1" },
				{ "anchor","MiddleCenter" },
				{ "cut","" },
			};
		}
		public override IEnumerator Start()
		{
			paramDic["className"] = "UIImage";
			paramDic["scale_x"] = paramDic["scale"];
			paramDic["scale_y"] = paramDic["scale"];
			paramDic["scale_z"] = paramDic["scale"];
            TRLayerObjectBehaviour g = TRLayerObjectManager.Instance.Create(paramDic["name"], TRDataType.BG);
            g.Load(paramDic);
            yield return null;
        }
    }
}
