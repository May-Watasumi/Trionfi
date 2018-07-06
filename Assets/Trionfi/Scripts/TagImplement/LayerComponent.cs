using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
    //オブジェクト生成廃棄はシステム管轄から外れる（可搬性）

    public class ImageComponent : AbstractComponent {
		public ImageComponent() {

			//必須項目
			essentialParams = new List<string> {
				"storage",
                "layer"
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
			string storage = expressionedParams["storage"];
            TRDataType _type = expressionedParams.Type();

            Image _dest;

            int ch = -1;
            if(int.TryParse(expressionedParams["layer"], out ch))
                _dest = Trionfi.Instance.GetLayer(TRDataType.Character, ch);
            else
                _dest = Trionfi.Instance.GetLayer(_type);

            _dest.sprite = StorageManager.Instance.LoadObject(storage, _type) as Sprite;
        }
    }
    
	public class ImagefreeComponent : AbstractComponent {
		public ImagefreeComponent() {
			//必須項目
			essentialParams = new List<string>
            {
                "layer"
            };	//"name"
		}

		protected override void TagFunction() {
            TRDataType _type = expressionedParams.Type();

            Image _dest;

            int ch = -1;
            if (int.TryParse(expressionedParams["layer"], out ch))
                _dest = Trionfi.Instance.GetLayer(TRDataType.Character, ch);
            else
                _dest = Trionfi.Instance.GetLayer(_type);

            _dest.sprite = null;
        }
    }

    public class LayoptComponent : AbstractComponent
    {
        public LayoptComponent()
        {
            //必須項目
            essentialParams = new List<string>
            {
                "layer"
            };  //"name"
        }

        protected override void TagFunction()
        {
            TRDataType _type = expressionedParams.Type();

            Image _dest;

            int ch = -1;
            if (int.TryParse(expressionedParams["layer"], out ch))
                _dest = Trionfi.Instance.GetLayer(TRDataType.Character, ch);
            else
                _dest = Trionfi.Instance.GetLayer(_type);

            //ToDo:
        }
    }

}
