using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {
    public class Live2d_newComponent:AbstractComponent {
		public Live2d_newComponent() {
			//必須項目
			essentialParams = new List<string> {
				"name",
				"storage" 
			};
		}

		protected override void TagFunction()
		{
			//			string name = expressionedParams ["name"];
			//			string tag = expressionedParams ["tag"];
			expressionedParams ["className"] = "Live2d";
			expressionedParams ["scale_x"] = expressionedParams ["scale"];
			expressionedParams ["scale_y"] = expressionedParams ["scale"];
			expressionedParams ["scale_z"] = expressionedParams ["scale"];
//		
			string storage = expressionedParams["storage"];
			expressionedParams["storage"] = storage;

            TRLayerObjectBehaviour g = TRLayerObjectManager.Instance.Create(expressionedParams["name"], TRDataType.BG);
            g.Load(expressionedParams);
        }
    }

	public class Live2d_motionComponent : AbstractComponent {
		public Live2d_motionComponent() {
			//必須項目
			essentialParams = new List<string> {
				"name",
				"motion",
				"face",
//				"storage"
			};
		}

		protected override void TagFunction() {
			string name = expressionedParams["name"];
			string group = expressionedParams["group"];
			string face = expressionedParams["face"];	
			int motion = int.Parse(expressionedParams["motion"]);
			int priority = int.Parse(expressionedParams["priority"]);
//			string tag = this.expressionedParams["tag"];
//			string storage = this.expressionedParams["storage"];
//			string idle = this.expressionedParams["idel"];

			List<string> images = new List<string>();
     		images.Add(name);
		
			foreach (string image_name in images) {
                //ToDo:
/*
                Live2dObject image = TRLayerObjectManager.GetObject(image_name);
				obj.SetMotion(group, motion, face, priority);
*/
			}
        }
    }
}
