using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {
    //[chara_new name="yuko" tag="scene1" jname="優子" ]
    public class Chara_newComponent : Image_newComponent
    {
		public Chara_newComponent() : base() {
			//画像のルートパスが異なってくる
//			base.imagePath = StorageManager.Instance.PATH_CHARA_IMAGE;
		}

		protected override void TagFunction() {
			expressionedParams["className"] = "Chara";

			expressionedParams ["layer"] ="character";
			//expressionedParams["imagePath"] = StorageManager.Instance.PATH_CHARA_IMAGE;

			//jname jcolor  名前表示のときに色と名前を指定できます
			if (expressionedParams.ContainsKey ("jname"))
				ScriptDecoder.Instance.variable.Set("_chara_jname." + expressionedParams["name"], expressionedParams["jname"]);

			if (expressionedParams.ContainsKey ("jcolor"))
				ScriptDecoder.Instance.variable.Set("_chara_jcolor." + expressionedParams["name"], expressionedParams["jcolor"]);

			base.TagFunction();
        }
    }

	public class Chara_posComponent : Image_posComponent
    {
		public Chara_posComponent() : base() { }
		protected override void TagFunction()
        {
            base.TagFunction();
        }
    }

	public class Chara_showComponent:Image_showComponent
    {
		public Chara_showComponent() : base() { }
		protected override void TagFunction()
        {
            base.TagFunction();
        }
    }

	public class Chara_hideComponent : Image_hideComponent
    {
		public Chara_hideComponent() : base() {  }
        protected override void TagFunction()
        {
            base.TagFunction();
        }
    }

/*
    [chara_face name="hiro" face="no2" storage=sad]
    [chara_show name = "hiro"]
    [chara_mod name = "hiro" face = "no1"]
    [chara_mod name = "hiro" face = "default"]
 */
    //キャラの表情登録用
    public class Chara_faceComponent : Image_faceComponent
    {
//		public Chara_faceComponent():base() { base.imagePath = StorageManager.Instance.PATH_CHARA_IMAGE; }
		protected override void TagFunction()
        {
            base.TagFunction();
        }
    }

	public class Chara_modComponent:Image_modComponent
    {
		public Chara_modComponent() : base() { }
		protected override void TagFunction()
        {
            base.TagFunction();
        }
    }

	public class Chara_removeComponent : Image_removeComponent
    {
		public Chara_removeComponent() : base() { }
		protected override void TagFunction()
        {
            base.TagFunction();
        }
    }
}
