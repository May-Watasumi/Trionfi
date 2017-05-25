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
	public class TRLayerObjectManager : FlipObject<TRLayerObjectBehaviour, CanvasManager>
    {
        [SerializeField]
        int characterBaseDescent = 0;

        //        public static Dictionary<string, AbstractObject> dicObject = new Dictionary<string, AbstractObject>();
        /*
                //セーブ用のパラメータなど、全てココに入れておく必要がある
                public ParamDictionary dicSave = new ParamDictionary();

                //face 情報はココに格納
                public ParamDictionary dicFace = new ParamDictionary();
        */

        public override TRLayerObjectBehaviour Create(string name, TRDataType type)
        {
            return null;
        }

#if false
        public override TRLayerObjectBehaviour Create(string name, ObjectType type)
        {
            //ToDo:
            string className = param["className"];

            switch(className) {
			case "Live2d":
				imageObject = g.AddComponent<Live2dObject>();
				break;
            case "Text":
                imageObject = new TextObject();
                break;
            case "Sd":
                imageObject = new SdObject();
                break;
            default:
                    //ToDo?
                imageObject = new ImageObject();//StorageManager.GetCustomObject(className);
            break;
			}
            if (!dicObject.ContainsKey(param["name"]))
                g = GameObject.Instantiate(objectPrefab);
            else
                g = dicObject[param["name"]].gameObject;

            g.  UpdateParam(param);
		}

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

			var tmpParam = new ParamDictionary()
            {
				{ "storage",storage }
			};

			imageObject.SetParam(tmpParam);
			imageObject.Show(time,type);
		}

		public static void SetImage(ParamDictionaryparam)
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
