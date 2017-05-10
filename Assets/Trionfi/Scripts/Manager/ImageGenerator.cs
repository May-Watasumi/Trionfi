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
	public class Image
    {
		//セーブ用のパラメータなど、全てココに入れておく必要がある
		public Dictionary<string,string> dicSave = new Dictionary<string,string>();
	    
		//face 情報はココに格納
		public Dictionary<string,string> dicFace = new Dictionary<string,string>();

		[NonSerialized]
		private AbstractObject imageObject;

		public string getParam(string key)
		{
			return dicSave[key];
		}

		public void setParam(string key,string value)
		{
			dicSave[key] = value;
		}

		public Image(Dictionary<string,string> param)
		{
			dicSave["name"] = param ["name"];
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

			//デフォルトの表情として登録
			addFace ("default", getParam("storage"));
		}

		public void Compile() {
			GameObject g = new GameObject ("gameobject");

			AbstractObject imageObject;
			string className = dicSave ["className"];

			switch(className) {
			case "Canvas":
				imageObject = g.AddComponent<CanvasObject>();
				break;
			case "Message":
				imageObject = g.AddComponent<MessageObject>();
				break;
			case "UIImage":
				imageObject = g.AddComponent<UIImageObject>();
				break;
			case "Text":
				imageObject = g.AddComponent<TextObject>();
				break;
			case "Clickable":
				imageObject = g.AddComponent<ClickableObject>();
				break;
			case "Sd":
				imageObject = g.AddComponent<SdObject>();
				break;
			case "Button":
				imageObject = g.AddComponent<ButtonObject>();
				break;
			case "Live2d":
				imageObject = g.AddComponent<Live2dObject>();
				break;
			default:
				imageObject = StorageManager.Instance.GetCustomObject(className, g);
				break;
			}

			imageObject.name = getParam ("name");

			//画像なりをセット
			imageObject.imagePath = dicSave["imagePath"];
			imageObject.set(dicSave);

//			imageObject = imageObject;

			//このオブジェクトが表示対象の場合は即表示
			SetPosition(float.Parse(dicSave["x"]), float.Parse(dicSave["y"]), float.Parse(dicSave["z"]));

			//scale の設定
			SetScale (float.Parse(dicSave["scale_x"]), float.Parse(dicSave["scale_y"]), float.Parse(dicSave["scale_z"]));

			//イベントが登録されている場合はcolider 登録
			if (dicSave ["event"] == "true")
				SetColider();

			if (dicSave ["isShow"] == "true")
				Show(0, "linear");

		}

		public void SetColider()
        {
			dicSave ["event"] = "true";
			imageObject.setColider();
		}

		public void AddFace(string face, string storage)
        {
			dicFace[face] = storage;
		}

		public void SetFace(string face, float time, string type)
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

			imageObject.set (tmpParam);
			imageObject.show (time,type);
		}

		public void SetImage(Dictionary<string,string>param)
        {
			foreach (KeyValuePair<string, string> kvp in param) {
//				dicSave [kvp.Key] = param [kvp.Key];
				dicSave[kvp.Key] = kvp.Value;
			}

			imageObject.set(param);
			imageObject.show(float.Parse(param["time"]),param["type"]);
		}

		public void Remove()
        {
			imageObject.remove();
			imageObject = null;
		}

		public void SetScale(float scale_x, float scale_y, float scale_z)
        {
			dicSave["scale_x"] = ""+scale_x;
			dicSave["scale_y"] = ""+scale_y;
			dicSave["scale_z"] = ""+scale_z;
			imageObject.setScale (scale_x,scale_y,scale_z);
		}

		public void SetPosition(float x,float y,float z)
        {
			dicSave["x"] = ""+x;
			dicSave["y"] = ""+y;
			dicSave["z"] = ""+z;

			imageObject.setPosition (x, y, z);	
		}

		public void AnimationPosition(Vector3 position, float scale,float time,string type)
        {	
			dicSave["x"] = ""+position.x;
			dicSave["y"] = ""+position.y;
			dicSave["z"] = "" + position.z;
			dicSave["scale"] = ""+scale;

			imageObject.animPosition(position, scale, time, type);
		}

		public void Show(float time, string type)
        {
			dicSave["isShow"] = "true";
			imageObject.show(time, type);
		}

		public void Hide(float time, string type)
        {
			dicSave["isShow"] = "false";
			imageObject.hide(time, type);
		}

		public AbstractObject GetObject()
        {
			return imageObject;
		}
	}
}
