using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
	public class Image_newComponent : AbstractComponent {
		protected string imagePath = "";

		public Image_newComponent() {
			//必須項目
			essentialParams = new List<string> {
//				"name",
				"storage",
                "type"
			};
/*
			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "storage",""},
				{ "tag",""},
				{ "layer","Default"},
				{ "sort","3"},
//				{ "imagePath", StorageManager.Instance.PATH_IMAGE},
				{ "x","0"},
				{ "y","0"},
				{ "z","0"},
//				{ "scale",""},
				{ "scale_x","1"},
				{ "scale_y","1"},
				{ "scale_z","1"},
				{ "strech", "false"},
//				{ "path","false"}, //trueにすると、pathを補完しない
			};
*/
		}

		protected override void TagFunction()
        {
			TRLayerObjectBehaviour g = TRLayerObjectManager.Instance.Create(expressionedParams["name"], TRDataType.BG);
            g.Load(expressionedParams);
        }
    }

	public class Image_posComponent : AbstractComponent {
		public Image_posComponent() {

			//必須項目
			essentialParams = new List<string> {
				"name" 
			};
/*
			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "x",""},
				{ "y",""},
				{ "z",""},
				{ "scale_x",""},
				{ "scale_y",""},
				{ "scale_z",""},
				{ "scale",""},
			};
*/
		}

		protected override void TagFunction() {
			string name = expressionedParams["name"];
            TRLayerObjectBehaviour image = TRLayerObjectManager.Instance.Find(expressionedParams["name"]);
            image.param = expressionedParams;
        }
    }

	public class Image_showComponent : AbstractComponent {
		private List<string> images;
		private bool isWait = false;

		public Image_showComponent() {
			//必須項目
			essentialParams = new List<string> { }; //"name" 
		}

		protected override void TagFunction() {
			string name = expressionedParams ["name"];
			string tag = expressionedParams ["tag"];
			string type = expressionedParams["type"];
			float time = float.Parse(expressionedParams["time"]);
			bool flag_delegate = true;

			List<TRLayerObjectBehaviour> images;

            if (tag != "")
                images = TRLayerObjectManager.Instance.GetImageByTag(tag);
            else
            {
                images = new List<TRLayerObjectBehaviour>();
                images.Add(TRLayerObjectManager.Instance.Find(name));
            }

            if (StatusManager.Instance.onSkip)
				time = 0.0f;

			//アニメーション中にクリックして次に進めるかどうか。
			if(time > 0.0f && expressionedParams["wait"] != "false")
            {
				StatusManager.Instance.Wait();
				isWait = true;
			}

			foreach(AbstractObject image in images)
            {
			//ToDo:
//				image.SetPosition (x, y, z);

				if(isWait) {
					//設定するのは一つだけ
					if (flag_delegate == true) {
						flag_delegate = false;
					}
				}
				image.Show(time, type);	
			}
        }
	}

	public class Image_hideComponent : AbstractComponent {
		bool isWait = false;

		public Image_hideComponent() {
			//必須項目
			essentialParams = new List<string> { }; //	"name",
		}

		protected override void TagFunction() {
			string name = expressionedParams["name"];
			string type = expressionedParams["type"];
			string tag = expressionedParams ["tag"];
			float time = float.Parse(expressionedParams["time"]);

			bool flag_delegate = true;

            List<TRLayerObjectBehaviour> images;

            if (tag != "")
                images = TRLayerObjectManager.Instance.GetImageByTag(tag);
            else
            {
                images = new List<TRLayerObjectBehaviour>();
                images.Add(TRLayerObjectManager.Instance.Find(name));
            }

            if (StatusManager.Instance.onSkip)
				time = 0.0f;

			//アニメーション中にクリックして次に進めるかどうか。
			if (time > 0.0f && expressionedParams["wait"] != "false")
			{
//				nextOrder = false;
				StatusManager.Instance.Wait();
				isWait = true;
			}

			foreach(TRLayerObjectBehaviour image in images) {
				if(isWait) {
					//設定するのは一つだけ
					if (flag_delegate == true) {
						flag_delegate = false;
//						image.SetFinishAnimationDelegate(finishAnimationDeletgate);
					}
				}
				image.Hide(time, type);
			}
        }
	}
/*
    [image_show name=logo]
    [image_face face = logo2 storage = "other_logo"]
    [image_mod name = logo face = logo2]
    [image_mod name = logo face = default]
*/
    //キャラの表情登録用
    public class Image_faceComponent : AbstractComponent {
		protected string imagePath = "";

		public Image_faceComponent()
        {
//			this.imagePath = StorageManager.Instance.PATH_IMAGE;

			//必須項目
			essentialParams = new List<string> {
				"name",
				"face",
				"storage"
			};
		}

		protected override void TagFunction() {
			string name = expressionedParams ["name"];
			string face = expressionedParams ["face"];
			string storage = expressionedParams["storage"];

            TRLayerObjectBehaviour image = TRLayerObjectManager.Instance.Find(name);

			TRLayerObjectManager.Instance.Find(name);
        }
	}   

	public class Image_modComponent : AbstractComponent { 
		public Image_modComponent() {

			//必須項目
			essentialParams = new List<string> {
				"name"
			};

		}

		protected override void TagFunction() {
			StatusManager.Instance.Wait();

			string name = expressionedParams ["name"];
			string face = expressionedParams ["face"];
			string storage = expressionedParams ["storage"];

			float time = float.Parse (expressionedParams["time"]);
			string type = expressionedParams ["type"];

            TRLayerObjectBehaviour image = TRLayerObjectManager.Instance.Find(name);

            //storage指定が優先される
            if (storage != "")
                image.param = expressionedParams;
            else
                image.Load(expressionedParams);

			if (StatusManager.Instance.onSkip || time <= 0.02f)
			{
//				image.OnAnimationFinished();
			}


			//処理を待たないなら
			if (expressionedParams ["wait"] == "false") {
				StatusManager.Instance.NextOrder();
//				StatusManager.Instance.enableNextOrder = true;
//				this.gameManager.nextOrder();
			}
//			else
//				image.SetFinishAnimationDelegate(this.finishAnimationDeletgate);

        }
	}

	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class Image_removeComponent : AbstractComponent {
		public Image_removeComponent() {
			//必須項目
			essentialParams = new List<string> { };	//"name"
		}

		protected override void TagFunction() {
			string tag = expressionedParams ["tag"];
			string name = expressionedParams ["name"];

            List<TRLayerObjectBehaviour> images;
            if (tag != "")
                images = TRLayerObjectManager.Instance.GetImageByTag(tag);
            else
            {
                images = new List<TRLayerObjectBehaviour>();
                images.Add(TRLayerObjectManager.Instance.Find(name));
            }

            foreach (TRLayerObjectBehaviour image in images)
            {
				//Image image = this.gameManager.imageManager.getImage (image_name);
//				TRLayerObjectManager.Instance.Remove(image.GetParam("name"));
			}
        }
    }

	public class ShowComponent : Image_showComponent
    {
		public ShowComponent() : base() { }
		protected override void TagFunction()
        {
            base.TagFunction();
        }
    }

	public class HideComponent : Image_hideComponent {
		public HideComponent() : base() { }
		protected override void TagFunction()
        {
            base.TagFunction();
        }
    }

	public class RemoveComponent : Image_removeComponent {
		public RemoveComponent() : base() { }
		protected override void TagFunction()
        {
            base.TagFunction();
        }
    }
}
