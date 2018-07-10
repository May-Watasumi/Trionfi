using System;
using System.Collections;
using System.Collections.Generic;
//using ExpressionParser;
using UnityEngine;
using UnityEngine.UI;

namespace Trionfi {
	public class Uiimage_newComponent : AbstractComponent {
		public Uiimage_newComponent() : base() {
			//必須項目
			essentialParams = new List<string> {
				"name" 
			};
/*
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
*/
		}
		protected override void TagFunction()
		{
			expressionedParams["className"] = "UIImage";
			expressionedParams["scale_x"] = expressionedParams["scale"];
			expressionedParams["scale_y"] = expressionedParams["scale"];
			expressionedParams["scale_z"] = expressionedParams["scale"];
            TRLayerObjectBehaviour g = TRLayerObjectManager.Instance.Create(expressionedParams["name"], TRDataType.BG);
            g.Load(expressionedParams);
        }
    }
}
