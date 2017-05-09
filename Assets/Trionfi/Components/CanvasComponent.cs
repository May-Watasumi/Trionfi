using System;
using System.Collections;
using System.Collections.Generic;
//using ExpressionParser;
using UnityEngine;
using UnityEngine.UI;

namespace NovelEx {
	public class Canvas_newComponent : AbstractComponent
	{
		public Canvas_newComponent()
			: base()
		{
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
			this.param["className"] = "Canvas";
			this.param["storage"] = "";
			this.param["scale_x"] = this.param["scale"];
			this.param["scale_y"] = this.param["scale"];
			this.param["scale_z"] = this.param["scale"];
			Image image = new Image(this.param);
			JOKEREX.Instance.ImageManager.addImage(image);
		}
	}

	//キャラのポジションを変更する
	public class Canvas_posComponent : Image_posComponent
	{
		public Canvas_posComponent() : base() { }

		public override void start()
		{
			base.start();
		}
	}

	public class Canvas_showComponent : Image_showComponent
	{
		public Canvas_showComponent() : base() { }
		public override void start()
		{
			//ToDo:
			//			this.param["wait"] = "false";
			base.start();
			//			this.gameManager.nextOrder();
		}
	}

	public class Canvas_hideComponent : Image_hideComponent
	{
		public Canvas_hideComponent() : base() { }
		public override void start()
		{
			base.start();
		}
	}

}
