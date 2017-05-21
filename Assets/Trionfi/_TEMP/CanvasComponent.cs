using System;
using System.Collections;
using System.Collections.Generic;
//using ExpressionParser;
using UnityEngine;
using UnityEngine.UI;

namespace NovelEx {
	public class Canvas_newComponent : AbstractComponent
	{
		public Canvas_newComponent() : base()
		{
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
			paramDic["className"] = "Canvas";
			paramDic["storage"] = "";
			paramDic["scale_x"] = paramDic["scale"];
			paramDic["scale_y"] = paramDic["scale"];
			paramDic["scale_z"] = paramDic["scale"];
//			ImageObject image = new ImageObject(paramDic);
            ImageObjectManager.Instance.Create(paramDic["name"]);
            yield return null;

        }
    }

	//キャラのポジションを変更する
	public class Canvas_posComponent : Image_posComponent
	{
		public Canvas_posComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }

	public class Canvas_showComponent : Image_showComponent
	{
		public Canvas_showComponent() : base() { }
		public override IEnumerator Start()
		{
			//ToDo:
			//			paramDic["wait"] = "false";
			base.Start();
            //			this.gameManager.nextOrder();
            yield return null;

        }
    }

	public class Canvas_hideComponent : Image_hideComponent
	{
		public Canvas_hideComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }
}
