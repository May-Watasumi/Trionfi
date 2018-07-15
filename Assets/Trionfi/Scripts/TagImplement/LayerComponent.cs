using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    //オブジェクト生成廃棄はシステム管轄から外れる（可搬性）

    public class LayerComponent : AbstractComponent {
		public LayerComponent() {

			//必須項目
			essentialParams = new List<string> {
				"storage",
                "id"
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
			string storage = tagParam["storage"];
            TRDataType _type = tagParam.Type();

            Image _dest;

            int ch = -1;
            if(int.TryParse(tagParam["id"], out ch))
                _dest = Trionfi.Instance.GetLayer(TRDataType.Character, ch);
            else
                _dest = Trionfi.Instance.GetLayer(_type);

            _dest.sprite = StorageManager.Instance.LoadObject(storage, _type) as Sprite;
        }
    }
    
	public class LayerfreeComponent : AbstractComponent {
		public LayerfreeComponent() {
			//必須項目
			essentialParams = new List<string>
            {
                "id"
            };	//"name"
		}

		protected override void TagFunction() {
            TRDataType _type = tagParam.Type();

            Image _dest;

            int ch = -1;
            if (int.TryParse(tagParam["layer"], out ch))
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
                "id"
            };  //"name"
        }

        protected override void TagFunction()
        {
            TRDataType _type = tagParam.Type();

            Image _dest;

            int ch = -1;
            if (int.TryParse(tagParam["id"], out ch))
                _dest = Trionfi.Instance.GetLayer(TRDataType.Character, ch);
            else
                _dest = Trionfi.Instance.GetLayer(_type);

            //ToDo:
        }
    }

    public class LaytransComponent : AbstractComponent
    {
        public LaytransComponent()
        {
            //必須項目
            essentialParams = new List<string>
            {
                "id"
            };
        }

        protected override void TagFunction()
        {
            TRDataType _type = tagParam.Type();

            Image _dest;

            int ch = -1;
            if (int.TryParse(tagParam["id"], out ch))
                _dest = Trionfi.Instance.GetLayer(TRDataType.Character, ch);
            else
                _dest = Trionfi.Instance.GetLayer(_type);

            float time = tagParam.Float("time", 1.0f);
            Vector3 pos = new Vector3(tagParam.Float("pos_x"), tagParam.Float("pos_y"), tagParam.Float("pos_z"));
            Vector3 scale  = new Vector3(tagParam.Float("scale_x", 1.0f), tagParam.Float("scale_y", 1.0f), tagParam.Float("scale_z", 1.0f));
            Vector3 rotate = new Vector3(tagParam.Float("rot_x"), tagParam.Float("rot_y"), tagParam.Float("rot_z"));

            Sequence seq = DOTween.Sequence();
            seq.Append(_dest.rectTransform.DOLocalMove(pos, time));
            seq.Join (_dest.rectTransform.DOScale(scale, time));
            seq.Join(_dest.rectTransform.DORotate(rotate, time));
            seq.Play();
        }
    }

}
