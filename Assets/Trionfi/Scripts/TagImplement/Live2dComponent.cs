using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi {
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
