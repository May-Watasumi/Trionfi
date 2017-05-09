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
			this.arrayVitalParam = new List<string> {
				"name" 
			};

			this.originalParam = new Dictionary<string, string>() {
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
		public override void start()
		{
			this.param["className"] = "UIImage";
			this.param["scale_x"] = this.param["scale"];
			this.param["scale_y"] = this.param["scale"];
			this.param["scale_z"] = this.param["scale"];
			Image image = new Image(this.param);
			JOKEREX.Instance.ImageManager.addImage(image);
		}
	}
}
