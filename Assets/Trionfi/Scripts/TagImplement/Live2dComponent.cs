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
			string name = tagParam.Identifier("name");
			string group = tagParam.Identifier("group");
			string face = tagParam.Identifier("face");	
//			int motion = int.Parse(tagParam."motion"]);
//			int priority = int.Parse(tagParam["priority"]);
//			string tag = this.tagParam["tag"];
//			string storage = this.tagParam["storage"];
//			string idle = this.tagParam["idel"];

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
