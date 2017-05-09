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
	public class Image{
		//セーブ用のパラメータなど、全てココに入れておく必要がある
		public Dictionary<string,string> dicSave = new Dictionary<string,string>();
	    
		//face 情報はココに格納
		public Dictionary<string,string> dicFace = new Dictionary<string,string>();

		[NonSerialized]
		private AbstractObject imageObject;

		public string getParam(string key)
		{
			return this.dicSave [key];
		}

		public void setParam(string key,string value)
		{
			this.dicSave [key] = value;
		}

		public Image(Dictionary<string,string> param)
		{

			this.dicSave ["name"] = param ["name"];
			this.dicSave ["tag"] = param["tag"];
			this.dicSave ["storage"] = param["storage"];
			this.dicSave ["isShow"] ="false";
			this.dicSave ["imagePath"] ="";
			this.dicSave ["className"] ="";
			this.dicSave ["event"] ="false";

			foreach (KeyValuePair<string, string> kvp in param)
			{
				//paramの内容は上書きしていく
				string key = kvp.Key;
//				this.dicSave [key] = param [key];
				this.dicSave[key] = kvp.Value;
			}

			//デフォルトの表情として登録
			this.addFace ("default", this.getParam("storage"));
		}

		public void compile() {
			GameObject g = new GameObject ("gameobject");

			AbstractObject imageObject;
			string className = this.dicSave ["className"];

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
				imageObject = JOKEREX.Instance.StorageManager.GetCustomObject(className, g);
				break;
			}

			imageObject.name = this.getParam ("name");

			//画像なりをセット
			imageObject.imagePath = this.dicSave["imagePath"];
			imageObject.set (this.dicSave);

			this.imageObject = imageObject;

			//このオブジェクトが表示対象の場合は即表示

			this.setPosition(float.Parse(this.dicSave["x"]),float.Parse(this.dicSave["y"]),float.Parse(this.dicSave["z"]));

			//scale の設定

			this.setScale (float.Parse(this.dicSave["scale_x"]),float.Parse(this.dicSave["scale_y"]),float.Parse(this.dicSave["scale_z"]));

			//イベントが登録されている場合はcolider 登録
			if (this.dicSave ["event"] == "true")
				this.setColider();

			if (dicSave ["isShow"] == "true")
				this.show (0, "linear");

		}

		public void setColider() {
			this.dicSave ["event"] = "true";
			this.getObject().setColider();
		}

		public void addFace(string face,string storage) {
			this.dicFace [face] = storage;
		}

		public void setFace(string face,float time,string type) {

			if (!this.dicFace.ContainsKey (face)) {
//ToDo:
//				JOKEREX.Instance.GameManager.showError ("表情「" + face + "」は存在しません。");
			}

			string storage = this.dicFace [face];

			var tmpParam = new Dictionary<string,string>() {
				{ "storage",storage }
			};

			this.imageObject.set (tmpParam);
			this.imageObject.show (time,type);

		}

		public void setImage(Dictionary<string,string>param) {
			foreach (KeyValuePair<string, string> kvp in param) {
//				this.dicSave [kvp.Key] = param [kvp.Key];
				this.dicSave[kvp.Key] = kvp.Value;
			}

			this.imageObject.set(param);
			this.imageObject.show(float.Parse(param["time"]),param["type"]);
		}

		public void remove() {
			this.imageObject.remove();
			this.imageObject = null;
		}

		public void setScale(float scale_x, float scale_y, float scale_z) {
			this.dicSave["scale_x"] = ""+scale_x;
			this.dicSave["scale_y"] = ""+scale_y;
			this.dicSave["scale_z"] = ""+scale_z;
			this.imageObject.setScale (scale_x,scale_y,scale_z);
		}

		public void setPosition(float x,float y,float z) {
			this.dicSave["x"] = ""+x;
			this.dicSave["y"] = ""+y;
			this.dicSave["z"] = ""+z;

			this.imageObject.setPosition (x, y, z);	
		}

		public void animPosition(Vector3 position, float scale,float time,string type) {	
			this.dicSave["x"] = ""+position.x;
			this.dicSave["y"] = ""+position.y;
			this.dicSave["z"] = "" + position.z;
			this.dicSave["scale"] = ""+scale;

			this.getObject().animPosition (position, scale, time, type);
		}

		public void show(float time, string type) {
			this.dicSave["isShow"] ="true";
			this.imageObject.show (time, type);
		}

		public void hide(float time, string type) {
			this.dicSave["isShow"] ="false";
			this.imageObject.hide (time, type);
		}

		public AbstractObject getObject(){
			return this.imageObject;
		}
	}
}
