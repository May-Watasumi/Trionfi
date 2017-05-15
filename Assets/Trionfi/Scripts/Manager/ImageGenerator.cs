using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NovelEx
{
	[Serializable]
	public class ImageObjectManager
    {
        public static Dictionary<string, AbstractObject> dicObject = new Dictionary<string, AbstractObject>();

        //セーブ用のパラメータなど、全てココに入れておく必要がある
        public static Dictionary<string,string> dicSave = new Dictionary<string,string>();

        //face 情報はココに格納
        public static Dictionary<string,string> dicFace = new Dictionary<string,string>();

		[NonSerialized]
		private static AbstractObject imageObject;
        [NonSerialized]
        public static ImageObjectManager Instance;
/*
        public static string GetParam(string key)
		{
			return dicSave[key];
		}

		public static void SetParam(string key,string value)
		{
			dicSave[key] = value;
		}
*/
		protected static void InitParam(Dictionary<string,string> param)
		{
            dicSave.Clear();
			dicSave["tag"] = param["tag"];
			dicSave["storage"] = param["storage"];
			dicSave["isShow"] ="false";
			dicSave["imagePath"] ="";
			dicSave["className"] ="";
			dicSave["event"] ="false";

			foreach (KeyValuePair<string, string> kvp in param)
			{
				//paramの内容は上書きしていく
				string key = kvp.Key;
//				dicSave [key] = param [key];
				dicSave[key] = kvp.Value;
			}

            //ToDo:
			//デフォルトの表情として登録
			//addFace("default", getParam("storage"));
		}

        public static void AddObject(AbstractObject instance)
        {
            dicObject[instance.name] = instance;
        }

        public static AbstractObject GetObject(string key)
        {
            return dicObject[key];
        }

        public static void RemoveObject(string key)
        {
            GameObject.Destroy(dicObject[key].instanceObject);
            dicObject.Remove(key);
        }

        public static void CreateObject(Dictionary<string, string> param)
        {
            GameObject g = new GameObject(param["name"]);

			AbstractObject imageObject;

            string className = dicSave["className"];

            switch(className) {
//ToDo:
#if false
            case "Canvas":
				imageObject = g.AddComponent<CanvasObject>();
				break;
			case "Message":
				imageObject = g.AddComponent<MessageObject>();
				break;
			case "UIImage":
				imageObject = g.AddComponent<UIImageObject>();
				break;
			case "Clickable":
				imageObject = g.AddComponent<ClickableObject>();
				break;
			case "Button":
				imageObject = g.AddComponent<ButtonObject>();
				break;
			case "Live2d":
				imageObject = g.AddComponent<Live2dObject>();
				break;
#endif
            case "Text":
                imageObject = new TextObject();
                break;
            case "Sd":
                imageObject = new SdObject();
                break;
            default:
				imageObject = StorageManager.GetCustomObject(className);
				break;
			}

            InitParam(param);

            g.tag = dicSave["tag"];
//            imageObject.name = dicSave["name"];
//            imageObject.imagePath = dicSave["imagePath"];
            imageObject.instanceObject = g;
            imageObject.SetParam(dicSave);

			//このオブジェクトが表示対象の場合は即表示
			imageObject.SetPosition(float.Parse(dicSave["x"]), float.Parse(dicSave["y"]), float.Parse(dicSave["z"]));

            //scale の設定
            imageObject.SetScale (float.Parse(dicSave["scale_x"]), float.Parse(dicSave["scale_y"]), float.Parse(dicSave["scale_z"]));

			//イベントが登録されている場合はcolider 登録
			if (dicSave ["event"] == "true")
                imageObject.SetColider();

			if (dicSave ["isShow"] == "true")
                imageObject.Show(0, "linear");
		}
#if false
        public static void SetColider()
        {
			dicSave["event"] = "true";
			imageObject.SetColider();
		}

        public static void AddFace(string face, string storage)
        {
			dicFace[face] = storage;
		}

		public static void SetFace(string face, float time, string type)
        {
			if(!dicFace.ContainsKey(face))
            {
//ToDo:
//				JOKEREX.Instance.GameManager.showError ("表情「" + face + "」は存在しません。");
			}

			string storage = dicFace [face];

			var tmpParam = new Dictionary<string,string>()
            {
				{ "storage",storage }
			};

			imageObject.SetParam(tmpParam);
			imageObject.Show(time,type);
		}

		public static void SetImage(Dictionary<string,string>param)
        {
			foreach (KeyValuePair<string, string> kvp in param) {
//				dicSave [kvp.Key] = param [kvp.Key];
				dicSave[kvp.Key] = kvp.Value;
			}

			imageObject.SetParam(param);
			imageObject.Show(float.Parse(param["time"]),param["type"]);
		}

		public static void Remove()
        {
			imageObject.Remove();
			imageObject = null;
		}

		public static void SetScale(float scale_x, float scale_y, float scale_z)
        {
			dicSave["scale_x"] = ""+scale_x;
			dicSave["scale_y"] = ""+scale_y;
			dicSave["scale_z"] = ""+scale_z;
			imageObject.SetScale (scale_x,scale_y,scale_z);
		}

		public static void SetPosition(float x,float y,float z)
        {
			dicSave["x"] = ""+x;
			dicSave["y"] = ""+y;
			dicSave["z"] = ""+z;

			imageObject.SetPosition (x, y, z);	
		}

		public static void AnimationPosition(Vector3 position, float scale,float time,string type)
        {	
			dicSave["x"] = ""+position.x;
			dicSave["y"] = ""+position.y;
			dicSave["z"] = "" + position.z;
			dicSave["scale"] = ""+scale;

			imageObject.AnimationPosition(position, scale, time, type);
		}

		public static void Show(float time, string type)
        {
			dicSave["isShow"] = "true";
			imageObject.Show(time, type);
		}

		public static void Hide(float time, string type)
        {
			dicSave["isShow"] = "false";
			imageObject.Hide(time, type);
		}
#endif
    }
}
