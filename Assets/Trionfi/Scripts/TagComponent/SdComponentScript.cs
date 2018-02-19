
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {
	public class Sd_newComponent : AbstractComponent {
		public Sd_newComponent() {
			//必須項目

			essentialParams = new List<string> {
				"name",
				"storage"
			};
        }

		protected override void TagFunction() {
			//string name = expressionedParams ["name"];
			//string tag = paramDic ["tag"];
		//c["className"] ="Sd";

            TRLayerObjectBehaviour g = TRLayerObjectManager.Instance.Create(tagParam["name"], TRDataType.BG);
            g.Load(tagParam);
        }
    }

	public class Sd_showComponent:Image_showComponent
    {
		public Sd_showComponent() : base() { }
		protected override void TagFunction()
        {
            base.TagFunction();
        }
    }

	public class Sd_removeComponent : Image_removeComponent
    {
		public Sd_removeComponent() : base() { }
		protected override void TagFunction()
        {
            base.TagFunction();
        }
    }

	public class Sd_hideComponent:Image_hideComponent
    {
		public Sd_hideComponent() : base() { }
		protected override void TagFunction()
        {
            base.TagFunction();
        }
    }

	public class Sd_animComponent : AbstractComponent {
		public Sd_animComponent() {

			essentialParams = new List<string> {
				"name","state"
			};
		}

		protected override void TagFunction() {
			string name = tagParam["name"];
			string tag = tagParam["tag"];
			List<TRLayerObjectBehaviour> images;

            if (tag != "")
                images = TRLayerObjectManager.Instance.GetImageByTag(tag);
            else
            {
                images = new List<TRLayerObjectBehaviour>();
                images.Add(TRLayerObjectManager.Instance.Find(name));
            }

            foreach(TRLayerObjectBehaviour image in images)
            {
                /*
                if (paramDic ["condition"] == "true")
					image.PlayAnimation(paramDic["state"]);
				else
					image.StopAnimation(paramDic["state"]);
                */
			}
        }
    }
}
